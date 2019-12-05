using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ExpressionsPower.Tests")]

namespace ExpressionsPower
{
    internal sealed class ReplaceVisitor : ExpressionVisitor
    {
        private readonly Expression from, to;

        public ReplaceVisitor(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }

        public override Expression Visit(Expression node)
        {
            return node == from ? to : base.Visit(node);
        }
    }
}
