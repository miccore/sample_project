using Miccore.Clean.Sample.Core.Extensions;
using Miccore.Clean.Sample.Core.Entities.Base;
using FluentAssertions;

namespace Miccore.Clean.Sample.Core.Tests.Extensions
{
    public class DbContextExtensionTests
    {
        private class SampleEntity : BaseEntity
        {
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
        }

        [Fact]
        public void SetUpdatedValues_ShouldUpdateEntityProperties()
        {
            // Arrange
            var originalEntity = new SampleEntity { Id = Guid.NewGuid(), Name = "Original", Age = 25 };
            var updatedEntity = new SampleEntity { Id = originalEntity.Id, Name = "Updated", Age = 30 };

            // Act
            var result = originalEntity.SetUpdatedValues(updatedEntity);

            // Assert
            result.Name.Should().Be(updatedEntity.Name);
            result.Age.Should().Be(updatedEntity.Age);
        }

        [Fact]
        public void SetUpdatedValues_ShouldNotUpdateNullProperties()
        {
            // Arrange
            var originalEntity = new SampleEntity { Id = Guid.NewGuid(), Name = "Original", Age = 25 };
            var updatedEntity = new SampleEntity { Id = originalEntity.Id, Name = null!, Age = 30 };

            // Act
            var result = originalEntity.SetUpdatedValues(updatedEntity);

            // Assert
            result.Name.Should().Be("Original"); // Name should not be updated
            result.Age.Should().Be(updatedEntity.Age); // Age should be updated
        }

        [Fact]
        public void SetUpdatedValues_ShouldNotUpdateIfValuesAreSame()
        {
            // Arrange
            var originalEntity = new SampleEntity { Id = Guid.NewGuid(), Name = "Original", Age = 25 };
            var updatedEntity = new SampleEntity { Id = originalEntity.Id, Name = "Original", Age = 25 };

            // Act
            var result = originalEntity.SetUpdatedValues(updatedEntity);

            // Assert
            result.Name.Should().Be(originalEntity.Name);
            result.Age.Should().Be(originalEntity.Age);
        }

        [Fact]
        public void SetUpdatedValues_ShouldSkipNullPropertiesInContext()
        {
            // Arrange
            var originalEntity = new SampleEntity { Id = Guid.NewGuid(), Name = "Original", Age = 25 };
            var updatedEntity = new SampleEntity { Id = originalEntity.Id, Name = "Updated", Age = 30 };

            // Act
            var result = originalEntity.SetUpdatedValues(updatedEntity);

            // Assert
            result.Name.Should().Be(updatedEntity.Name);
            result.Age.Should().Be(updatedEntity.Age);
        }
    }
}