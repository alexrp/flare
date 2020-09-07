using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeRaiseNode : TreeNode
    {
        public TreeReference Operand { get; }

        public TreeRaiseNode(TreeContext context, SourceLocation location, TreeReference operand)
            : base(context, location)
        {
            Operand = operand;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Operand;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("raise ");
            Operand.ToString(writer);
        }
    }
}
