using System.Linq.Expressions;
using FluentAssertions;
using Miccore.Clean.Sample.Core.Helpers.Builders;

namespace Miccore.Clean.Sample.Core.Tests.Helpers.Builders
{
    public class ExpressionBuilderTests
    {
        private class SampleEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        [Fact]
        public void Build_ShouldCreateEqualityExpression()
        {
            // Arrange
            var propertyName = "Name";
            var value = "Test";

            // Act
            var expression = ExpressionBuilder.Build<SampleEntity>(propertyName, value);
            var compiledExpression = expression.Compile();
            var entity = new SampleEntity { Name = "Test" };

            // Assert
            compiledExpression(entity).Should().BeTrue();
        }

        [Fact]
        public void And_ShouldCombineExpressionsWithAndOperator()
        {
            // Arrange
            Expression<Func<SampleEntity, bool>> expr1 = e => e.Id > 0;
            Expression<Func<SampleEntity, bool>> expr2 = e => e.Name == "Test";

            // Act
            var combinedExpression = expr1.And(expr2);
            var compiledExpression = combinedExpression.Compile();
            var entity = new SampleEntity { Id = 1, Name = "Test" };

            // Assert
            compiledExpression(entity).Should().BeTrue();
        }

        [Fact]
        public void Or_ShouldCombineExpressionsWithOrOperator()
        {
            // Arrange
            Expression<Func<SampleEntity, bool>> expr1 = e => e.Id > 0;
            Expression<Func<SampleEntity, bool>> expr2 = e => e.Name == "Test";

            // Act
            var combinedExpression = expr1.Or(expr2);
            var compiledExpression = combinedExpression.Compile();
            var entity = new SampleEntity { Id = 0, Name = "Test" };

            // Assert
            compiledExpression(entity).Should().BeTrue();
        }

        [Fact]
        public void BuildSortExpression_ShouldCreateSortExpression()
        {
            // Arrange
            var propertyName = "Name";

            // Act
            var sortExpression = ExpressionBuilder.BuildSortExpression<SampleEntity>(propertyName);
            var compiledExpression = sortExpression.Compile();
            var entity = new SampleEntity { Name = "Test" };

            // Assert
            compiledExpression(entity).Should().Be("Test");
        }

        [Fact]
        public void ReplaceParameterVisitor_ShouldReplaceParameters()
        {
            // Arrange
            var parameter1 = Expression.Parameter(typeof(SampleEntity), "e1");
            var parameter2 = Expression.Parameter(typeof(SampleEntity), "e2");
            var visitor = new ExpressionBuilder.ReplaceParameterVisitor();
            visitor.Add(parameter1, parameter2);

            // Act
            var visitedExpression = visitor.Visit(parameter1);

            // Assert
            visitedExpression.Should().Be(parameter2);
        }
    }
}