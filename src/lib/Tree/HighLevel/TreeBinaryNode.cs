using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeBinaryNode : TreeNode
    {
        public TreeReference Left { get; }

        public string Operator { get; }

        public TreeReference Right { get; }

        public TreeBinaryNode(TreeContext context, SourceLocation location, TreeReference left, string @operator,
            TreeReference right)
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
            writer.Write(" {0} ", Operator);
            Right.ToString(writer);
            writer.Write(")");
        }
    }
}
