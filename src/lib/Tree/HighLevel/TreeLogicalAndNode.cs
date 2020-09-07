using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;
using Flare.Tree.LowLevel;

namespace Flare.Tree.HighLevel
{
    sealed class TreeLogicalAndNode : TreeNode
    {
        public TreeReference Left { get; }

        public TreeReference Right { get; }

        public TreeLogicalAndNode(TreeContext context, SourceLocation location, TreeReference left, TreeReference right)
            : base(context, location)
        {
            Left = left;
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

        public override TreeNode Rewrite()
        {
            // Rewrite `lhs and rhs` to:
            //
            // ```
            // if (!lhs) {
            //     false;
            // } else {
            //     test rhs;
            // }
            // ```

            return new TreeIfNode(Context, Location,
                new TreeLogicalNotNode(Context, Location, Left),
                new TreeLiteralNode(Context, Location, false),
                new TreeTestNode(Context, Location, Right));
        }

        public override void ToString(IndentedTextWriter writer)
        {
            Left.ToString(writer);
            writer.Write(" and ");
            Right.ToString(writer);
        }
    }
}
