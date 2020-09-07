using System.CodeDom.Compiler;
using Flare.Metadata;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeConstantNode : TreeNode
    {
        public Constant Constant { get; }

        public TreeConstantNode(TreeContext context, SourceLocation location, Constant constant)
            : base(context, location)
        {
            Constant = constant;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("const {0}", Constant);
        }
    }
}
