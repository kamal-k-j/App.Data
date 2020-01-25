using System;
using System.Reflection;
using App.Data.Conventions;
using FluentNHibernate;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using Moq;
using Xunit;

namespace App.Data.Tests.Conventions
{
    public class VarBinaryLengthConventionTests
    {
        protected const int ExpectedStringLength = 100;

        protected class DecoratedEntity
        {
            [VarBinaryLength(ExpectedStringLength)]
            public virtual byte[] TestProperty { get; set; }

            public virtual byte[] UndecoratedProperty { get; set; }

            [VarBinaryLength(ExpectedStringLength)]
            public virtual int DecoratedIntProperty { get; set; }

            public virtual int UndecoratedIntProperty { get; set; }
        }

        protected class TestMember : Member
        {
            public TestMember(PropertyInfo propertyInfo)
            {
                PropertyType = propertyInfo.PropertyType;
                MemberInfo = propertyInfo;
            }

            public override void SetValue(object target, object value)
            {
                throw new NotImplementedException();
            }

            public override object GetValue(object target)
            {
                throw new NotImplementedException();
            }

            public override bool TryGetBackingField(out Member backingField)
            {
                throw new NotImplementedException();
            }

            public override string Name { get; }
            public override Type PropertyType { get; }
            public override bool CanWrite { get; }
            public override MemberInfo MemberInfo { get; }
            public override Type DeclaringType { get; }
            public override bool HasIndexParameters { get; }
            public override bool IsMethod { get; }
            public override bool IsField { get; }
            public override bool IsProperty { get; }
            public override bool IsAutoProperty { get; }
            public override bool IsPrivate { get; }
            public override bool IsProtected { get; }
            public override bool IsPublic { get; }
            public override bool IsInternal { get; }
        }

        [Fact]
        public void WhenDeterminingIfTheConventionShouldBeAppliedToACorrectlyDecoratedProperty()
        {
            // Arrange
            IPropertyConventionAcceptance subject = new VarBinaryLengthConvention();
            var acceptanceCriteriaMock = new Mock<IAcceptanceCriteria<IPropertyInspector>>();
            var testMember = new TestMember(typeof(DecoratedEntity).GetProperty("TestProperty"));

            acceptanceCriteriaMock.Setup(criteria => criteria.Expect(It.Is<Func<IPropertyInspector, bool>>(func => VerifyCriteriaExpectation(func, testMember, true))));

            // Act
            subject.Accept(acceptanceCriteriaMock.Object);

            // Assert
            acceptanceCriteriaMock.Verify();
        }

        [Fact]
        public void WhenDeterminingIfTheConventionShouldBeAppliedToAUndecoratedByteArrayProperty()
        {
            // Arrange
            IPropertyConventionAcceptance subject = new VarBinaryLengthConvention();
            var acceptanceCriteriaMock = new Mock<IAcceptanceCriteria<IPropertyInspector>>();
            var testMember = new TestMember(typeof(DecoratedEntity).GetProperty("UndecoratedProperty"));

            acceptanceCriteriaMock.Setup(criteria => criteria.Expect(It.Is<Func<IPropertyInspector, bool>>(func => VerifyCriteriaExpectation(func, testMember, false))));

            // Act
            subject.Accept(acceptanceCriteriaMock.Object);

            // Assert
            acceptanceCriteriaMock.Verify();
        }

        [Fact]
        public void WhenDeterminingIfTheConventionShouldBeAppliedToADecoratedNonByteArrayProperty()
        {
            // Arrange
            IPropertyConventionAcceptance subject = new VarBinaryLengthConvention();
            var acceptanceCriteriaMock = new Mock<IAcceptanceCriteria<IPropertyInspector>>();
            var testMember = new TestMember(typeof(DecoratedEntity).GetProperty("DecoratedIntProperty"));

            acceptanceCriteriaMock.Setup(criteria => criteria.Expect(It.Is<Func<IPropertyInspector, bool>>(func => VerifyCriteriaExpectation(func, testMember, false))));

            // Act
            subject.Accept(acceptanceCriteriaMock.Object);

            // Assert
            acceptanceCriteriaMock.Verify();
        }

        [Fact]
        public void WhenDeterminingIfTheConventionShouldBeAppliedToAnUndecoratedNonByteArrayProperty()
        {
            // Arrange
            IPropertyConventionAcceptance subject = new VarBinaryLengthConvention();
            var acceptanceCriteriaMock = new Mock<IAcceptanceCriteria<IPropertyInspector>>();
            var testMember = new TestMember(typeof(DecoratedEntity).GetProperty("UndecoratedIntProperty"));

            acceptanceCriteriaMock.Setup(criteria => criteria.Expect(It.Is<Func<IPropertyInspector, bool>>(func => VerifyCriteriaExpectation(func, testMember, false))));

            // Act
            subject.Accept(acceptanceCriteriaMock.Object);

            // Assert
            acceptanceCriteriaMock.Verify();
        }

        private bool VerifyCriteriaExpectation(Func<IPropertyInspector, bool> func, Member member, bool expectedValue)
        {
            var propertyInspectorMock = new Mock<IPropertyInspector>();
            propertyInspectorMock.SetupGet(inspector => inspector.Property).Returns(member);

            Assert.Equal(expectedValue, func(propertyInspectorMock.Object));

            return true;
        }

        [Fact]
        public void WhenApplyingTheConventionToADecoratedByteArrayProperty()
        {
            // Arrange
            IPropertyConvention subject = new VarBinaryLengthConvention();
            var member = new TestMember(typeof(DecoratedEntity).GetProperty("TestProperty"));
            var propertyInstanceMock = new Mock<IPropertyInstance>();
            propertyInstanceMock.SetupGet(instance => instance.Property).Returns(member);

            // Act
            subject.Apply(propertyInstanceMock.Object);

            // Assert
            propertyInstanceMock.Verify(instance => instance.Length(ExpectedStringLength));
        }

        [Fact]
        public void WhenApplyingTheConventionToAnUndecoratedProperty()
        {
            // Arrange
            IPropertyConvention subject = new VarBinaryLengthConvention();
            var member = new TestMember(typeof(DecoratedEntity).GetProperty("UndecoratedProperty"));
            var propertyInstanceMock = new Mock<IPropertyInstance>();
            propertyInstanceMock.SetupGet(instance => instance.Property).Returns(member);

            // Act
            subject.Apply(propertyInstanceMock.Object);

            // Assert
            propertyInstanceMock.Verify(instance => instance.Length(VarBinaryLengthConvention.DefaultLength));
        }
    }
}