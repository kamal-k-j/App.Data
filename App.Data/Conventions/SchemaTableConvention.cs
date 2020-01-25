using System;
using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace App.Data.Conventions
{
    public class SchemaTableConvention : IConventionAcceptance<IClassInspector>, IClassConvention
    {
        public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
        {
            criteria.Expect(inspector => GetEntityAttribute(inspector.EntityType) != null);
        }

        public void Apply(IClassInstance instance)
        {
            instance.Table($"[{GetTableName(instance.EntityType)}]");
            var schema = GetSchemaName(instance.EntityType, GetEntityAttribute(instance.EntityType));
            if (!string.IsNullOrEmpty(schema))
            {
                instance.Schema(schema);
            }
        }

        protected virtual string GetSchemaName(Type entityType, EntityAttribute entityAttribute)
        {
            return entityAttribute?.Schema;
        }

        private EntityAttribute GetEntityAttribute(Type entityType)
        {
            return entityType.GetCustomAttributes(typeof(EntityAttribute), true).FirstOrDefault() as EntityAttribute;
        }

        protected virtual string GetTableName(Type entityType)
        {
            return entityType.Name.EndsWith("dto", StringComparison.InvariantCultureIgnoreCase)
                ? entityType.Name.Substring(0, entityType.Name.Length - 3)
                : entityType.Name;
        }
    }
}