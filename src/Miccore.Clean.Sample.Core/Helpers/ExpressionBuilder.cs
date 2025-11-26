namespace Miccore.Clean.Sample.Core.Helpers
{
    /// <summary>
    /// Expression builder
    /// </summary>
    public static class ExpressionBuilder
    {
        /// <summary>
        /// Builds an expression for filtering based on a property name and value.
        /// creates an equality expression.
        /// </summary>
        /// <param name="propertyName">The name of the property to filter by.</param>
        /// <param name="value">The value to filter by.</param>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <returns>An expression for filtering.</returns>
        public static Expression<Func<T, bool>> Build<T>(string propertyName, object value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var member = Expression.Property(parameter, propertyName);
            var constant = Expression.Constant(value);
            var body = Expression.Equal(member, constant);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        /// <summary>
        /// Combines two expressions using the logical AND operator.
        /// </summary>
        /// <param name="expr1">The first expression.</param>
        /// <param name="expr2">The second expression.</param>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <returns>A combined expression using the logical AND operator.</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var combined = new ReplaceParameterVisitor
            {
                { expr1.Parameters[0], parameter },
                { expr2.Parameters[0], parameter }
            }.Visit(Expression.AndAlso(expr1.Body, expr2.Body));

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        /// <summary>
        /// Combines two expressions using the logical OR operator.
        /// </summary>
        /// <param name="expr1">The first expression.</param>
        /// <param name="expr2">The second expression.</param>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <returns>A combined expression using the logical OR operator.</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var combined = new ReplaceParameterVisitor
            {
                { expr1.Parameters[0], parameter },
                { expr2.Parameters[0], parameter }
            }.Visit(Expression.OrElse(expr1.Body, expr2.Body));

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        /// <summary>
        /// Builds an expression for sorting based on a property name.
        /// </summary>
        /// <param name="propertyName">The name of the property to sort by.</param>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <returns>An expression for sorting.</returns>
        public static Expression<Func<T, object>> BuildSortExpression<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var member = Expression.Property(parameter, propertyName);
            var converted = Expression.Convert(member, typeof(object));
            return Expression.Lambda<Func<T, object>>(converted, parameter);
        }

        /// <summary>
        /// Visitor for replacing parameters in expressions.
        /// </summary>
        public class ReplaceParameterVisitor : ExpressionVisitor, IEnumerable<KeyValuePair<ParameterExpression, ParameterExpression>>
        {
            private readonly Dictionary<ParameterExpression, ParameterExpression> _map = new Dictionary<ParameterExpression, ParameterExpression>();

            /// <summary>
            /// Adds a parameter replacement to the visitor.
            /// </summary>
            /// <param name="from">The parameter to replace.</param>
            /// <param name="to">The replacement parameter.</param>
            public void Add(ParameterExpression from, ParameterExpression to)
            {
                _map[from] = to;
            }

            /// <summary>
            /// Visits a parameter expression and replaces it if a replacement is found.
            /// </summary>
            /// <param name="node">The parameter expression to visit.</param>
            /// <returns>The visited expression.</returns>
            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (_map.TryGetValue(node, out var replacement))
                {
                    node = replacement;
                }
                return base.VisitParameter(node);
            }

            public IEnumerator<KeyValuePair<ParameterExpression, ParameterExpression>> GetEnumerator()
            {
                return _map.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}