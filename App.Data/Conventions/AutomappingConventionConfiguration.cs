using System;
using FluentNHibernate.Automapping;

namespace App.Data.Conventions
{
    public class AutomappingConventionConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type.GetCustomAttributes(typeof(EntityAttribute), true).Length == 1;
        }
    }
}