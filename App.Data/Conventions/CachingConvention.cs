using System;
using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace App.Data.Conventions
{
    public class CachingConvention: IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            if (IsCacheable(instance.EntityType))
            {
                instance.Cache.ReadWrite();
            }
        }

        private static bool IsCacheable(Type entityType)
        {
            var entityAttribute = entityType.GetCustomAttributes(typeof(EntityAttribute), true).FirstOrDefault() as EntityAttribute;
            return entityAttribute != null && entityAttribute.Cache;
        }
    }
}
