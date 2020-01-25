using System;
using System.Collections.Generic;
using App.Data.Conventions;
using AutoFixture;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using Moq;
using Xunit;

namespace App.Data.Tests.Conventions
{
    public class IdConventionTests
    {
        protected class TestIdConvention : IdConvention
        {
            private readonly string _tableName;
            private readonly Type _expectedType;

            public TestIdConvention(int increment, string tableName, Type expectedType) : base(increment)
            {
                _tableName = tableName;
                _expectedType = expectedType;
            }

            protected override string GetTableName(Type entityType)
            {
                Assert.Equal(_expectedType, entityType);
                return _tableName;
            }
        }

        protected class TestEntityDto { }

        protected class DifferentTestEntity { }

        private const string TableName = "NhHiLo";
        private const string NextHiColumn = "NextHi";

        public Fixture AutoFixture { get; set; }

        public IdConventionTests()
        {
            AutoFixture = new Fixture();
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(long))]
        [InlineData(typeof(ulong))]
        public void WhenSettingUpIdConventionWithoutOverridingTableNameGettingWithEntityTypeEndingDto(Type type)
        {
            // Arrange
            var increment = AutoFixture.Create<int>();
            IIdConvention subject = new IdConvention(increment);

            var generatorInstanceMock = new Mock<IGeneratorInstance>();
            generatorInstanceMock.Setup(
                gen => gen.HiLo(TableName, NextHiColumn, increment.ToString(), It.Is<Action<ParamBuilder>>(action => VerifyHiLoAction(action, "TestEntity"))));

            var identityInstanceMock = new Mock<IIdentityInstance>();
            identityInstanceMock.SetupGet(id => id.EntityType).Returns(typeof(TestEntityDto));
            identityInstanceMock.SetupGet(id => id.GeneratedBy).Returns(generatorInstanceMock.Object);
            identityInstanceMock.SetupGet(id => id.Type).Returns(new TypeReference(type));

            // Act
            subject.Apply(identityInstanceMock.Object);

            // Assert
            generatorInstanceMock.Verify();
            identityInstanceMock.Verify();
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(long))]
        [InlineData(typeof(ulong))]
        public void WhenSettingUpIdConventionWithoutOverridingTableNameGettingWithEntityTypeNotEndingDto(Type type)
        {
            // Arrange
            var increment = AutoFixture.Create<int>();
            IIdConvention subject = new IdConvention(increment);

            var generatorInstanceMock = new Mock<IGeneratorInstance>();
            generatorInstanceMock.Setup(
                gen => gen.HiLo(TableName, NextHiColumn, increment.ToString(), It.Is<Action<ParamBuilder>>(action => VerifyHiLoAction(action, "DifferentTestEntity"))));

            var identityInstanceMock = new Mock<IIdentityInstance>();
            identityInstanceMock.SetupGet(id => id.EntityType).Returns(typeof(DifferentTestEntity));
            identityInstanceMock.SetupGet(id => id.GeneratedBy).Returns(generatorInstanceMock.Object);
            identityInstanceMock.SetupGet(id => id.Type).Returns(new TypeReference(type));

            // Act
            subject.Apply(identityInstanceMock.Object);

            // Assert
            generatorInstanceMock.Verify();
            identityInstanceMock.Verify();
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(long))]
        [InlineData(typeof(ulong))]
        public void WhenSettingUpIdConventionOverridingTableNameGetting(Type type)
        {
            // Arrange
            var increment = AutoFixture.Create<int>();
            var tableName = AutoFixture.Create<string>();
            IIdConvention subject = new TestIdConvention(increment, tableName, typeof(DifferentTestEntity));

            var generatorInstanceMock = new Mock<IGeneratorInstance>();
            generatorInstanceMock.Setup(
                gen => gen.HiLo(TableName, NextHiColumn, increment.ToString(), It.Is<Action<ParamBuilder>>(action => VerifyHiLoAction(action, tableName))));

            var identityInstanceMock = new Mock<IIdentityInstance>();
            identityInstanceMock.SetupGet(id => id.EntityType).Returns(typeof(DifferentTestEntity));
            identityInstanceMock.SetupGet(id => id.GeneratedBy).Returns(generatorInstanceMock.Object);
            identityInstanceMock.SetupGet(id => id.Type).Returns(new TypeReference(type));

            // Act
            subject.Apply(identityInstanceMock.Object);

            // Assert
            generatorInstanceMock.Verify();
            identityInstanceMock.Verify();
        }

        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(Guid))]
        public void WhenSettingUpIdConventionWhenTypeIsNotIntegral(Type type)
        {
            // Arrange
            var increment = AutoFixture.Create<int>();
            var tableName = AutoFixture.Create<string>();
            IIdConvention subject = new TestIdConvention(increment, tableName, typeof(DifferentTestEntity));

            var generatorInstanceMock = new Mock<IGeneratorInstance>();

            var identityInstanceMock = new Mock<IIdentityInstance>();
            identityInstanceMock.SetupGet(id => id.EntityType).Returns(typeof(DifferentTestEntity));
            identityInstanceMock.SetupGet(id => id.GeneratedBy).Returns(generatorInstanceMock.Object);
            identityInstanceMock.SetupGet(id => id.Type).Returns(new TypeReference(type));

            // Act
            subject.Apply(identityInstanceMock.Object);

            // Assert
            generatorInstanceMock.Verify(
                mock => mock.HiLo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<Action<ParamBuilder>>()), Times.Never);
            identityInstanceMock.Verify();
        }

        private bool VerifyHiLoAction(Action<ParamBuilder> action, string entityName)
        {
            var dictionary = new Dictionary<string, string>();
            var paramBuilder = new ParamBuilder(dictionary);

            action(paramBuilder);

            Assert.True(dictionary.ContainsKey("where"));
            Assert.Equal($"[TableKey] LIKE '{entityName}'", dictionary["where"]);

            return true;
        }
    }
}