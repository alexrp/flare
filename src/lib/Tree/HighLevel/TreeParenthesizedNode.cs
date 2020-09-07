using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeParenthesizedNode : TreeNode
    {
        public TreeReference Expression { get; }

        public TreeParenthesizedNode(TreeContext context, SourceLocation location, TreeReference expression)
            : base(context, location)
        {
            Expression = expression;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Expression;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override TreeNode Rewrite()
        {
            // Just remove the parentheses as precedence is already encoded in the tree.

            return Expression;
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("(");
            Expression.ToString(writer);
            writer.Write(")");
        }
    }
}
