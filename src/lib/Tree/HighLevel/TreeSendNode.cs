using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeSendNode : TreeNode
    {
        public TreeReference Left { get; }

        public TreeReference Right { get; }

        public TreeSendNode(TreeContext context, SourceLocation location, TreeReference left, TreeReference right)
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

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("(");
            Left.ToString(writer);
            writer.Write(" <- ");
            Right.ToString(writer);
            writer.Write(")");
        }
    }
}
