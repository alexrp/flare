using System.CodeDom.Compiler;
using Flare.Metadata;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeFunctionNode : TreeNode
    {
        public Function Function { get; }

        public TreeFunctionNode(TreeContext context, SourceLocation location, Function function)
            : base(context, location)
        {
            Function = function;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("fn {0}", Function);
        }
    }
}
