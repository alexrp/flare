using System.CodeDom.Compiler;
using Flare.Metadata;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeModuleNode : TreeNode
    {
        public Module Module { get; }

        public TreeModuleNode(TreeContext context, SourceLocation location, Module module)
            : base(context, location)
        {
            Module = module;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write(Module);
        }
    }
}
