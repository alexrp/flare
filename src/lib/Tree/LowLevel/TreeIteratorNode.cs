using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.LowLevel
{
    sealed class TreeIteratorNode : TreeNode
    {
        public TreeReference Operand { get; }

        public TreeIteratorNode(TreeContext context, SourceLocation location, TreeReference operand)
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
            writer.Write("iter ");
            Operand.ToString(writer);
        }
    }
}
