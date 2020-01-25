using App.Data.Conventions;
using AutoFixture;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using Moq;
using Xunit;

namespace App.Data.Tests.Conventions
{
    public class CascadeConventionTests
    {
        public class TestCascadeConvention : CascadeConvention
        {
            private readonly string _name;
            private readonly IManyToOneInstance _expectedManyToOneInstance;

            public TestCascadeConvention(string name, IManyToOneInstance expectedManyToOneInstance)
            {
                _name = name;
                _expectedManyToOneInstance = expectedManyToOneInstance;
            }

            protected override string GetColumnName(IManyToOneInstance instance)
            {
                Assert.Equal(_expectedManyToOneInstance, instance);
                return _name;
            }
        }

        public Fixture AutoFixture { get; set; }

        public CascadeConventionTests()
        {
            AutoFixture = new Fixture();
        }

        [Fact]
        public void WhenSettingUpTheReferenceConventionWithoutOverridingColumnNameGetting()
        {
            // Arrange
            IReferenceConvention subject = new CascadeConvention();

            var name = AutoFixture.Create<string>();
            var cascadeInstanceMock = new Mock<ICascadeInstance>();
            var manyToOneInstanceMock = new Mock<IManyToOneInstance>();
            manyToOneInstanceMock.SetupGet(mto => mto.Cascade).Returns(cascadeInstanceMock.Object);
            manyToOneInstanceMock.SetupGet(mto => mto.Name).Returns(name);

            // Act
            subject.Apply(manyToOneInstanceMock.Object);

            // Assert
            manyToOneInstanceMock.Verify(mto => mto.Column($"{name}Id"), Times.Once());
            cascadeInstanceMock.Verify(cascade => cascade.All(), Times.Once());
        }

        [Fact]
        public void WhenSettingUpTheReferenceConventionOverridingColumnNameGetting()
        {
            // Arrange
            var manyToOneInstanceMock = new Mock<IManyToOneInstance>();
            var name = AutoFixture.Create<string>();
            IReferenceConvention subject = new TestCascadeConvention(name, manyToOneInstanceMock.Object);

            var cascadeInstanceMock = new Mock<ICascadeInstance>();
            manyToOneInstanceMock.SetupGet(mto => mto.Cascade).Returns(cascadeInstanceMock.Object);
            manyToOneInstanceMock.SetupGet(mto => mto.Name).Returns(name);

            // Act
            subject.Apply(manyToOneInstanceMock.Object);

            // Assert
            manyToOneInstanceMock.Verify(mto => mto.Column(name), Times.Once());
            cascadeInstanceMock.Verify(cascade => cascade.All(), Times.Once());
        }

        [Fact]
        public void WhenSettingUpTheHasManyConvention()
        {
            // Arrange
            IHasManyConvention subject = new CascadeConvention();

            var collectionCascadeInstanceMock = new Mock<ICollectionCascadeInstance>();
            var oneToManyCollectionInstanceMock = new Mock<IOneToManyCollectionInstance>();
            oneToManyCollectionInstanceMock.SetupGet(otm => otm.Cascade).Returns(collectionCascadeInstanceMock.Object);

            // Act
            subject.Apply(oneToManyCollectionInstanceMock.Object);

            // Assert
            oneToManyCollectionInstanceMock.Verify(otm => otm.Inverse());
            collectionCascadeInstanceMock.Verify(cc => cc.All());
        }
    }
}