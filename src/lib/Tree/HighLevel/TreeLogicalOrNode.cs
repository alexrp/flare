using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeLogicalOrNode : TreeNode
    {
        public TreeReference Left { get; }

        public TreeReference Right { get; }

        public TreeLogicalOrNode(TreeContext context, SourceLocation location, TreeReference left, TreeReference right)
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
            // Rewrite `lhs or rhs` to `!(!lhs and !rhs)`, i.e. we implement `or` in terms of `and`.

            return new TreeLogicalNotNode(Context, Location,
                new TreeLogicalAndNode(Context, Location,
                    new TreeLogicalNotNode(Context, Location, Left),
                    new TreeLogicalNotNode(Context, Location, Right)));
        }

        public override void ToString(IndentedTextWriter writer)
        {
            Left.ToString(writer);
            writer.Write(" or ");
            Right.ToString(writer);
        }
    }
}
