using App.Data.Conventions;
using AutoFixture;
using FluentNHibernate.Conventions.Instances;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace App.Data.Tests.Conventions
{
    public class CachingConventionTests
    {
        public Fixture AutoFixture { get; set; }
        public AutoMocker Mocker { get; set; }

        [Entity(Cache = true)]
        private class TestEntityWithCacheSetToTrue
        {
        }

        [Entity(Cache = false)]
        private class TestEntityWithCacheSetToFalse
        {
        }

        [Entity()]
        private class TestEntityWithCacheNotSet
        {
        }

        public CachingConventionTests()
        {
            AutoFixture = new Fixture();
            Mocker = new AutoMocker();
        }


        [Fact]
        public void WhenApplyingTheCachingConventionAndCacheSetToTrue()
        {
            // Arrange
            var subject = new CachingConvention();

            var classInstanceMock = new Mock<IClassInstance>();
            var cacheMock = new Mock<ICacheInstance>();
            classInstanceMock.SetupGet(instance => instance.Cache).Returns(cacheMock.Object);
            classInstanceMock.SetupGet(instance => instance.EntityType).Returns(typeof(TestEntityWithCacheSetToTrue));

            // Act
            subject.Apply(classInstanceMock.Object);

            // Assert
            cacheMock.Verify(cache => cache.ReadWrite(), Times.Once);
        }

        [Fact]
        public void WhenApplyingTheCachingConventionAndCacheSetToFalse()
        {
            // Arrange
            var subject = new CachingConvention();

            var classInstanceMock = new Mock<IClassInstance>();
            var cacheMock = new Mock<ICacheInstance>();
            classInstanceMock.SetupGet(instance => instance.Cache).Returns(cacheMock.Object);
            classInstanceMock.SetupGet(instance => instance.EntityType).Returns(typeof(TestEntityWithCacheSetToFalse));

            // Act
            subject.Apply(classInstanceMock.Object);

            // Assert
            cacheMock.Verify(cache => cache.ReadWrite(), Times.Never);
        }

        [Fact]
        public void WhenApplyingTheCachingConventionAndCacheNotSet()
        {
            // Arrange
            var subject = new CachingConvention();

            var classInstanceMock = new Mock<IClassInstance>();
            var cacheMock = new Mock<ICacheInstance>();
            classInstanceMock.SetupGet(instance => instance.Cache).Returns(cacheMock.Object);
            classInstanceMock.SetupGet(instance => instance.EntityType).Returns(typeof(TestEntityWithCacheNotSet));

            // Act
            subject.Apply(classInstanceMock.Object);

            // Assert
            cacheMock.Verify(cache => cache.ReadWrite(), Times.Never);
        }

        [Fact]
        public void WhenApplyingTheCachingConventionAndNotAnEntity()
        {
            // Arrange
            var subject = new CachingConvention();

            var classInstanceMock = new Mock<IClassInstance>();
            var cacheMock = new Mock<ICacheInstance>();
            classInstanceMock.SetupGet(instance => instance.Cache).Returns(cacheMock.Object);
            classInstanceMock.SetupGet(instance => instance.EntityType).Returns(typeof(object));

            // Act
            subject.Apply(classInstanceMock.Object);

            // Assert
            cacheMock.Verify(cache => cache.ReadWrite(), Times.Never);
        }
    }
}
