using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace App.Data.Conventions
{
    public class CascadeConvention : IReferenceConvention, IHasManyConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.Column(GetColumnName(instance));
            instance.Cascade.All();
        }

        protected virtual string GetColumnName(IManyToOneInstance instance)
        {
            return $"{instance.Name}Id";
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.Inverse();
            instance.Cascade.All();
        }
    }
}