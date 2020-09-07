using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeRelationalNode : TreeNode
    {
        public TreeReference Left { get; }

        public TreeRelationalOperator Operator { get; }

        public TreeReference Right { get; }

        public TreeRelationalNode(TreeContext context, SourceLocation location, TreeReference left,
            TreeRelationalOperator @operator, TreeReference right)
            : base(context, location)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Left;
            yield return Right;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("(");
            Left.ToString(writer);

            writer.Write(" {0} ", Operator switch
            {
                TreeRelationalOperator.Equal => "==",
                TreeRelationalOperator.NotEqual => "!=",
                TreeRelationalOperator.LessThan => "<",
                TreeRelationalOperator.LessThanOrEqual => "<=",
                TreeRelationalOperator.GreaterThan => ">",
                TreeRelationalOperator.GreaterThanOrEqual => ">=",
                _ => throw DebugAssert.Unreachable(),
            });

            Right.ToString(writer);
            writer.Write(")");
        }
    }
}
