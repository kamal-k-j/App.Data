using System;
using System.ComponentModel;

namespace App.Data.Conventions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute : Attribute
    {
        [Description("The database schema the entity lives in, only set if the schema is different to dbo")]
        public string Schema { get; set; }

        [Description("Whether the entity should be stored in the second level cache. The default value is false.")]
        public bool Cache { get; set; }
    }
}