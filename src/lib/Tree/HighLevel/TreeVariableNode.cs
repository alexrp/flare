using System.CodeDom.Compiler;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeVariableNode : TreeNode
    {
        public TreeVariable Variable { get; }

        public TreeVariableNode(TreeContext context, SourceLocation location, TreeVariable variable)
            : base(context, location)
        {
            Variable = variable;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write(Variable);
        }
    }
}
