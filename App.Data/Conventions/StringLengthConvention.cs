using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace App.Data.Conventions
{
    public class StringLengthConvention : IPropertyConventionAcceptance, IPropertyConvention
    {
        public const int DefaultStringLength = 50;

        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(
                inspector =>
                    inspector.Property.PropertyType == typeof(string) &&
                    inspector.Property.MemberInfo.GetCustomAttributes(typeof(StringLengthAttribute), true).Length > 0);
        }

        public void Apply(IPropertyInstance instance)
        {
            var stringLengthAttribute =
                instance.Property.MemberInfo.GetCustomAttributes(typeof(StringLengthAttribute), true).FirstOrDefault()
                    as StringLengthAttribute;
            instance.Length(stringLengthAttribute?.Length ?? DefaultStringLength);
        }
    }
}