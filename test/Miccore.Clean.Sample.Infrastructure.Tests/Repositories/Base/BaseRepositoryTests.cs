using Miccore.Clean.Sample.Core.Exceptions;
using Miccore.Clean.Sample.Infrastructure.Repositories.Base;
using Miccore.Pagination.Model;
using Miccore.Clean.Sample.Infrastructure.Persistance;
using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Infrastructure.Tests.Persistances;
using Moq;
using FluentAssertions;

namespace Miccore.Clean.Sample.Infrastructure.Tests.Repositories.Base
{
    public class BaseRepositoryTests
    {
        private readonly BaseRepository<SampleEntity> _repository;
        private readonly Mock<SampleApplicationDbContext> _context;

        public BaseRepositoryTests()
        {   
            _context = MockSampleApplicationDbContext.GetDbContext();
            _repository = new BaseRepository<SampleEntity>(_context.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntity()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Test" };

            // Act
            var _ = await _repository.AddAsync(entity);

            // Assert
            var result = await _context.Object.Set<SampleEntity>().FindAsync(entity.Id);
            result.Should().NotBeNull();
            result.Name.Should().Be("Test");
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDeleteEntity()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Test" };
            _context.Object.Set<SampleEntity>().Add(entity);
            await _context.Object.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(entity.Id);

            // Assert
            result.Should().NotBeNull();
            result.DeletedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteHardAsync_ShouldHardDeleteEntities()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Test" };
            _context.Object.Set<SampleEntity>().Add(entity);
            await _context.Object.SaveChangesAsync();

            // Act
            await _repository.DeleteHardAsync(e => e.Name == "Test");

            // Assert
            var result = _context.Object.Set<SampleEntity>().FirstOrDefault(x => x.Name == "Test");
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedEntities()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Test" };
            _context.Object.Set<SampleEntity>().Add(entity);
            await _context.Object.SaveChangesAsync();
            var query = new PaginationQuery { page = 1, limit = 10 };

            // Act
            var result = await _repository.GetAllAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Test" };
            _context.Object.Set<SampleEntity>().Add(entity);
            await _context.Object.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(entity.Id);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Test");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenEntityDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _repository.GetByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetAllByParametersPaginatedAsync_ShouldReturnPaginatedEntities()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Test" };
            _context.Object.Set<SampleEntity>().Add(entity);
            await _context.Object.SaveChangesAsync();
            var query = new PaginationQuery { page = 1, limit = 10 };

            // Act
            var result = await _repository.GetAllByParametersPaginatedAsync(query, e => e.Name == "Test");

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAllByParametersAsync_ShouldReturnEntities()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Test" };
            _context.Object.Set<SampleEntity>().Add(entity);
            await _context.Object.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllByParametersAsync(e => e.Name == "Test");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetByParametersAsync_ShouldReturnEntity()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Test" };
            _context.Object.Set<SampleEntity>().Add(entity);
            await _context.Object.SaveChangesAsync();

            // Act
            var result = await _repository.GetByParametersAsync(e => e.Name == "Test");

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Test");
        }

        [Fact]
        public async Task GetByParametersAsync_ShouldThrowNotFoundException_WhenEntityDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _repository.GetByParametersAsync(e => e.Name == "NonExistent"));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Test" };
            _context.Object.Set<SampleEntity>().Add(entity);
            await _context.Object.SaveChangesAsync();

            entity.Name = "UpdatedTest";

            // Act
            var result = await _repository.UpdateAsync(entity);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("UpdatedTest");
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenEntityDoesNotExist()
        {
            // Arrange
            var entity = new SampleEntity { Id = Guid.NewGuid(), Name = "NonExistent" };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _repository.UpdateAsync(entity));
        }
    }
}