using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace App.Data.Conventions
{
    public class IdConvention : IIdConvention
    {
        private const string TableName = "NhHiLo";
        private const string NextHiColumn = "NextHi";

        private readonly int _increment;

        public IdConvention(int increment)
        {
            _increment = increment;
        }

        public virtual void Apply(IIdentityInstance instance)
        {
            if (IsIntegral(instance))
            {
                var table = GetTableName(instance.EntityType);
                instance.GeneratedBy.HiLo(TableName, NextHiColumn, _increment.ToString(),
                    builder => builder.AddParam("where", $"[TableKey] LIKE '{table}'"));
            }
        }

        protected virtual string GetTableName(Type entityType)
        {
            return entityType.Name.EndsWith("dto", StringComparison.InvariantCultureIgnoreCase)
                ? entityType.Name.Substring(0, entityType.Name.Length - 3)
                : entityType.Name;
        }

        private bool IsIntegral(IIdentityInstance instance)
        {
            return instance.Type == typeof(int) 
                   || instance.Type == typeof(long) 
                   || instance.Type == typeof(uint) 
                   || instance.Type == typeof(ulong);
        }
    }
}