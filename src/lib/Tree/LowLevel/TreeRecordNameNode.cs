using System.CodeDom.Compiler;
using Flare.Syntax;

namespace Flare.Tree.LowLevel
{
    sealed class TreeRecordNameNode : TreeNode
    {
        public TreeReference Operand { get; }

        public TreeRecordNameNode(TreeContext context, SourceLocation location, TreeReference operand)
            : base(context, location)
        {
            Operand = operand;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("name ");
            Operand.ToString(writer);
        }
    }
}
