using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ExpressionsPower.Tests")]

namespace ExpressionsPower
{
    internal static class LambdaExpressionsCombiner
    {
        /// <summary>
        /// Combine two lambda expressions which have only one parameter. 
        /// Example: (x => x.y) + (y => y.z) ---> (x => x.y.z)
        /// </summary>
        /// <param name="left">Left lambda expression.</param>
        /// <param name="right">Right lambda expression.</param>
        /// <returns></returns>
        internal static LambdaExpression Combine(LambdaExpression left, LambdaExpression right)
        {
            var newBody = new ReplaceVisitor(right.Parameters.First(), left.Body)
                .Visit(right.Body);

            var lambda = Expression.Lambda(newBody, left.Parameters.First());
            return lambda;
        }
    }
}
