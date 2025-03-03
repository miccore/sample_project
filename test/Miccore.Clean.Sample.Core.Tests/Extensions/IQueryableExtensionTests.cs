using Microsoft.EntityFrameworkCore;
using Moq;
using Miccore.Clean.Sample.Core.Extensions;
using Miccore.Clean.Sample.Core.Entities;
using FluentAssertions;

namespace Miccore.Clean.Sample.Core.Tests.Extensions
{
    public class IQueryableExtensionTests
    {
        [Fact]
        public void ApplyIncludes_ShouldApplyIncludesToQuery()
        {
            // Arrange
            var data = new List<SampleEntity>
            {
                new() { Id = Guid.NewGuid(), Name = "Test1" },
                new() { Id = Guid.NewGuid(), Name = "Test2" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<SampleEntity>>();
            mockSet.As<IQueryable<SampleEntity>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<SampleEntity>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<SampleEntity>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<SampleEntity>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        
            var includes = new[] { "RelatedEntity1", "RelatedEntity2" };

            // Act
            var result = mockSet.Object.ApplyIncludes(includes);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void ApplyIncludes_ShouldReturnQuery_WhenIncludesIsNull()
        {
            // Arrange
            var data = new List<SampleEntity>
            {
                new() { Id = Guid.NewGuid(), Name = "Test1" },
                new() { Id = Guid.NewGuid(), Name = "Test2" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<SampleEntity>>();
            mockSet.As<IQueryable<SampleEntity>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<SampleEntity>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<SampleEntity>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<SampleEntity>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            // Act
            var result = mockSet.Object.ApplyIncludes(null!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(data);
        }

        [Fact]
        public void ApplyIncludes_ShouldReturnQuery_WhenIncludesIsEmpty()
        {
            // Arrange
            var data = new List<SampleEntity>
            {
                new() { Id = Guid.NewGuid(), Name = "Test1" },
                new() { Id = Guid.NewGuid(), Name = "Test2" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<SampleEntity>>();
            mockSet.As<IQueryable<SampleEntity>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<SampleEntity>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<SampleEntity>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<SampleEntity>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var includes = Array.Empty<string>();

            // Act
            var result = mockSet.Object.ApplyIncludes(includes);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(data);
        }
    }
}