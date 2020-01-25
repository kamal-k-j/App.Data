using App.Data.UoW;
using Autofac;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using System.Reflection;

namespace App.Data.Conventions
{
    public class CachingDatabaseRegistrationModule : DatabaseRegistrationModule
    {
        private readonly IConfiguration _configuration;
        private readonly string _cacheProvider;

        public CachingDatabaseRegistrationModule(
            IConfiguration configuration,
            string cacheProvider,
            params Assembly[] automapAssemblies) : base(configuration, automapAssemblies)
        {
            _configuration = configuration;
            _cacheProvider = cacheProvider;
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

        private ISessionFactory BuildISessionFactory(IPersistenceConfigurer msSqlConfiguration)
        {
            return Fluently.Configure()
                .Database(msSqlConfiguration)
                .Mappings(m => m.AutoMappings
                    .Add(AutomappingConventions()
                        .Conventions.Add<CachingConvention>()))
                .Cache(csb => csb.UseQueryCache().ProviderClass(_cacheProvider))
                .ExposeConfiguration(config => config.Cache(c => c.DefaultExpiration = GetCacheExpirationValue()))
                .BuildSessionFactory();
        }

        private int GetCacheExpirationValue()
        {
            var configValue = _configuration.GetValue<string>("DatabaseConfig:CacheExpirationInSeconds");
            return configValue != null ? int.Parse(configValue) : _configuration.GetValue<int>("DatabaseConfig:DefaultCacheExpirationInSeconds");
        }
    }
}
