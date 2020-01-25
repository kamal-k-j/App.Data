using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace App.Data.Conventions
{
    public class VarBinaryLengthConvention : IPropertyConventionAcceptance, IPropertyConvention
    {
        public const int DefaultLength = 8192; // 8Kb

        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(
                inspector =>
                    inspector.Property.PropertyType == typeof(byte[]) &&
                    inspector.Property.MemberInfo.GetCustomAttributes(typeof(VarBinaryLengthAttribute), true).Length > 0);
        }

        public void Apply(IPropertyInstance instance)
        {
            var stringLengthAttribute =
                instance.Property.MemberInfo.GetCustomAttributes(typeof(VarBinaryLengthAttribute), true).FirstOrDefault()
                    as VarBinaryLengthAttribute;
            instance.Length(stringLengthAttribute?.Length ?? DefaultLength);
        }
    }
}