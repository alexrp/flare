using System.CodeDom.Compiler;
using Flare.Syntax;

namespace Flare.Tree.LowLevel
{
    sealed class TreeBlockingReceiveNode : TreeNode
    {
        public TreeBlockingReceiveNode(TreeContext context, SourceLocation location)
            : base(context, location)
        {
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("recv!");
        }
    }
}
