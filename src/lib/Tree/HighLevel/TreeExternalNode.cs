using System.CodeDom.Compiler;
using Flare.Metadata;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeExternalNode : TreeNode
    {
        public External External { get; }

        public TreeExternalNode(TreeContext context, SourceLocation location, External external)
            : base(context, location)
        {
            External = external;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("extern {0}", External);
        }
    }
}
