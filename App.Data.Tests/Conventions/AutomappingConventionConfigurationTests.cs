using App.Data.Conventions;
using FluentNHibernate.Automapping;
using Xunit;

namespace App.Data.Tests.Conventions
{
    public class AutomappingConventionConfigurationTests
    {
        public class UndecoratedEntityDto { }
        [Entity] public class DecoratedEntityDto { }

        [Fact]
        public void DeterminingIfAnUndecoratedEntityShouldBeMapped()
        {
            // Arrange
            IAutomappingConfiguration subject = new AutomappingConventionConfiguration();

            // Act
            var result = subject.ShouldMap(typeof(UndecoratedEntityDto));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DeterminingIfADecoratedEntityShouldBeMapped()
        {
            // Arrange
            IAutomappingConfiguration subject = new AutomappingConventionConfiguration();

            // Act
            var result = subject.ShouldMap(typeof(DecoratedEntityDto));

            // Assert
            Assert.True(result);
        }
    }
}