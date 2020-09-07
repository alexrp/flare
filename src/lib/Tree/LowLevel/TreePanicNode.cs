using System.CodeDom.Compiler;
using Flare.Syntax;

namespace Flare.Tree.LowLevel
{
    sealed class TreePanicNode : TreeNode
    {
        public string Message { get; }

        public TreePanicNode(TreeContext context, SourceLocation location, string message)
            : base(context, location)
        {
            Message = message;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("panic \"{0}\"", Message);
        }
    }
}
