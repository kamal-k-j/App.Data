using App.Data.UoW;
using Autofac;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using NHibernate;
using System.Reflection;
using Module = Autofac.Module;

namespace App.Data.Conventions
{
    public class DatabaseRegistrationModule : Module
    {
        private readonly IConfiguration _configuration;
        private readonly Assembly[] _automapAssemblies;

        public DatabaseRegistrationModule(
            IConfiguration configuration,
            params Assembly[] automapAssemblies)
        {
            _configuration = configuration;
            _automapAssemblies = automapAssemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var connectionString = GetConnectionString();
            var persistenceConfigurer = GetPersistentConfigurer(connectionString);
            var sessionFactory = BuildISessionFactory(persistenceConfigurer);
            builder.RegisterInstance(sessionFactory);
            builder.Register(s => s.Resolve<ISessionFactory>().OpenSession()).InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .OnActivated(e => e.Instance.BeginTransaction())
                .InstancePerLifetimeScope();
        }

        protected virtual string GetConnectionString()
        {
            return _configuration.GetValue<string>("DatabaseConfig:ConnectionString");
        }

        protected static IPersistenceConfigurer GetPersistentConfigurer(string connectionString)
        {
            return MsSqlConfiguration.MsSql2008.ConnectionString(connectionString);
        }

        private ISessionFactory BuildISessionFactory(IPersistenceConfigurer msSqlConfiguration)
        {
            return Fluently.Configure()
                .Database(msSqlConfiguration)
                .Mappings(m => m.AutoMappings.Add(AutomappingConventions))
                .BuildSessionFactory();
        }

        protected virtual int GetIncrementMultiplier()
        {
            return _configuration.GetValue<int>("DatabaseConfig:HiLoIncrement");
        }

        protected virtual AutoPersistenceModel AutomappingConventions()
        {
            return
                AutoMap.Assemblies(new AutomappingConventionConfiguration(), _automapAssemblies)
                    .Conventions.Add<SchemaTableConvention>()
                    .Conventions.Add(new IdConvention(GetIncrementMultiplier()))
                    .Conventions.Add<CascadeConvention>()
                    .Conventions.Add<StringLengthConvention>()
                    .Conventions.Add<VarBinaryLengthConvention>();
        }
    }
}
