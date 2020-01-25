using System;
using System.Linq;
using App.Data.Conventions;
using AutoFixture;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using Moq;
using Xunit;

namespace App.Data.Tests.Conventions
{
    public class SchemaTableConventionTests
    {
        [Entity(Schema = "testSchema")] protected class SchemaDecoratedTestEntityDto { }
        [Entity] protected class SchemalessDecoratedTestEntity { }
        protected class UndecoratedTestEntity { }

        protected class TestSchemaTableConvention : SchemaTableConvention
        {
            private readonly string _schema;
            private readonly string _table;
            private readonly Type _expectedType;

            public TestSchemaTableConvention(string schema, string table, Type expectedType)
            {
                _schema = schema;
                _table = table;
                _expectedType = expectedType;
            }

            protected override string GetSchemaName(Type entityType, EntityAttribute entityAttribute)
            {
                Assert.Equal(_expectedType, entityType);
                Assert.Equal(_expectedType.GetCustomAttributes(typeof(EntityAttribute), true).FirstOrDefault(), entityAttribute);
                return _schema;
            }

            protected override string GetTableName(Type entityType)
            {
                Assert.Equal(_expectedType, entityType);
                return _table;
            }
        }

        public Fixture AutoFixture { get; set; }

        public SchemaTableConventionTests()
        {
            AutoFixture = new Fixture();
        }

        [Fact]
        public void WhenDeterminingIfTheSchemaConventionShouldBeAppliedToACorrectlyDecoratedEntity()
        {
            // Arrange
            IConventionAcceptance<IClassInspector> subject = new SchemaTableConvention();
            var acceptanceCriteriaMock = new Mock<IAcceptanceCriteria<IClassInspector>>();
            var entityType = typeof(SchemalessDecoratedTestEntity);

            acceptanceCriteriaMock.Setup(criteria => criteria.Expect(It.Is<Func<IClassInspector, bool>>(func => VerifyCriteriaExpectation(func, entityType, true))));

            // Act
            subject.Accept(acceptanceCriteriaMock.Object);

            // Assert
            acceptanceCriteriaMock.Verify();
        }

        [Fact]
        public void WhenDeterminingIfTheSchemaConventionShouldBeAppliedToAnUndecoratedEntity()
        {
            // Arrange
            IConventionAcceptance<IClassInspector> subject = new SchemaTableConvention();
            var acceptanceCriteriaMock = new Mock<IAcceptanceCriteria<IClassInspector>>();
            var entityType = typeof(UndecoratedTestEntity);

            acceptanceCriteriaMock.Setup(criteria => criteria.Expect(It.Is<Func<IClassInspector, bool>>(func => VerifyCriteriaExpectation(func, entityType, false))));

            // Act
            subject.Accept(acceptanceCriteriaMock.Object);

            // Assert
            acceptanceCriteriaMock.Verify();
        }

        private bool VerifyCriteriaExpectation(Func<IClassInspector, bool> func, Type entityType, bool expectedResult)
        {
            var classInspectorMock = new Mock<IClassInspector>();
            classInspectorMock.SetupGet(inspector => inspector.EntityType).Returns(entityType);

            Assert.Equal(expectedResult, func(classInspectorMock.Object));

            return true;
        }

        [Fact]
        public void WhenApplyingTheSchemaTableConventionToASchemalessEntity()
        {
            // Arrange
            IClassConvention subject = new SchemaTableConvention();

            var classInstanceMock = new Mock<IClassInstance>();
            classInstanceMock.SetupGet(instance => instance.EntityType).Returns(typeof(SchemalessDecoratedTestEntity));

            // Act
            subject.Apply(classInstanceMock.Object);

            // Assert
            classInstanceMock.Verify(instance => instance.Table("[SchemalessDecoratedTestEntity]"), Times.Once());
            classInstanceMock.Verify(instance => instance.Schema(It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void WhenApplyingTheSchemaTableConventionToASchemaEntity()
        {
            // Arrange
            IClassConvention subject = new SchemaTableConvention();

            var classInstanceMock = new Mock<IClassInstance>();
            classInstanceMock.SetupGet(instance => instance.EntityType).Returns(typeof(SchemaDecoratedTestEntityDto));

            // Act
            subject.Apply(classInstanceMock.Object);

            // Assert
            classInstanceMock.Verify(instance => instance.Table("[SchemaDecoratedTestEntity]"), Times.Once());
            classInstanceMock.Verify(instance => instance.Schema("testSchema"), Times.Once());
        }

        [Fact]
        public void WhenApplyingTheSchemaTableConventionWithOverriddenSchemaTableFunctions()
        {
            // Arrange
            var schema = AutoFixture.Create<string>();
            var table = AutoFixture.Create<string>();
            IClassConvention subject = new TestSchemaTableConvention(schema, table, typeof(SchemaDecoratedTestEntityDto));

            var classInstanceMock = new Mock<IClassInstance>();
            classInstanceMock.SetupGet(instance => instance.EntityType).Returns(typeof(SchemaDecoratedTestEntityDto));

            // Act
            subject.Apply(classInstanceMock.Object);

            // Assert
            classInstanceMock.Verify(instance => instance.Table($"[{table}]"), Times.Once());
            classInstanceMock.Verify(instance => instance.Schema(schema), Times.Once());
        }
    }
}