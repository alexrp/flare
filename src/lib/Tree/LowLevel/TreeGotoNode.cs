using System.CodeDom.Compiler;
using Flare.Syntax;

namespace Flare.Tree.LowLevel
{
    sealed class TreeGotoNode : TreeNode
    {
        public TreeReference Target { get; }

        public TreeGotoNode(TreeContext context, SourceLocation location, TreeReference target)
            : base(context, location)
        {
            Target = target;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("goto {0}", ((TreeLabelNode)Target).Name);
        }
    }
}
