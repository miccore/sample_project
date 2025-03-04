using System.Linq.Expressions;
using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Miccore.Clean.Sample.Infrastructure.Tests.Persistances;

public class MockSampleApplicationDbContext
{
    public static  Mock<SampleApplicationDbContext> GetDbContext(){
        var options = new DbContextOptionsBuilder<SampleApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        var configurationMock = new Mock<IConfiguration>();
        configurationMock.SetupGet(c => c["Server"]).Returns("localhost");
        configurationMock.SetupGet(c => c["Port"]).Returns("3306");
        configurationMock.SetupGet(c => c["Database"]).Returns("TestDatabase");
        configurationMock.SetupGet(c => c["User"]).Returns("root");
        configurationMock.SetupGet(c => c["Password"]).Returns("password");

        var dbContextMock = new Mock<SampleApplicationDbContext>(options, configurationMock.Object);

        var sampleEntities = new List<SampleEntity>().AsQueryable();
        var sampleEntitiesDbSetMock = CreateMockDbSet(sampleEntities);

        dbContextMock.Setup(c => c.Set<SampleEntity>()).Returns(sampleEntitiesDbSetMock.Object);

        return dbContextMock;
    }

    private static Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        mockSet.As<IAsyncEnumerable<T>>()
               .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
               .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

        mockSet.As<IQueryable<T>>()
               .Setup(m => m.Provider)
               .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
               var list = queryable.ToList();

        // Setup Add, AddAsync, Remove, and Update methods
        mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(list.Add);
        mockSet.Setup(m => m.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>())).Callback<T, CancellationToken>((entity, token) => list.Add(entity));
        mockSet.Setup(m => m.Remove(It.IsAny<T>())).Callback<T>(entity => list.Remove(entity));
        mockSet.Setup(m => m.Update(It.IsAny<T>())).Callback<T>(entity =>
        {
            var index = list.FindIndex(e => e.Equals(entity));
            if (index != -1)
            {
                list[index] = entity;
            }
        });
        return mockSet;
    }

    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public T Current => _inner.Current;
    }

    private class TestAsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<T>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression) ?? throw new InvalidOperationException("Execution result is null.");
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executeMethod = typeof(IQueryProvider)
                .GetMethod(
                    name: nameof(IQueryProvider.Execute),
                    genericParameterCount: 1,
                    types: [typeof(Expression)]) ?? throw new InvalidOperationException("Execute method not found.");
            var executionResult = executeMethod
                .MakeGenericMethod(expectedResultType)
                .Invoke(this, [expression]);

            var fromResultMethod = typeof(Task).GetMethod(nameof(Task.FromResult)) ?? throw new InvalidOperationException("Task.FromResult method not found.");
            var genericFromResultMethod = fromResultMethod.MakeGenericMethod(expectedResultType);
            var result = genericFromResultMethod.Invoke(null, new[] { executionResult }) ?? throw new InvalidOperationException("Result of Task.FromResult is null.");
            return (TResult)result;
        }
    }

    private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }
}
