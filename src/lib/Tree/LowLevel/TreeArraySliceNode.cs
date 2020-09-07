using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.LowLevel
{
    sealed class TreeArraySliceNode : TreeNode
    {
        public TreeReference Operand { get; }

        public int Start { get; }

        public TreeArraySliceNode(TreeContext context, SourceLocation location, TreeReference operand, int start)
            : base(context, location)
        {
            Operand = operand;
            Start = start;
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
            Operand.ToString(writer);
            writer.Write("[{0} ...]", Start);
        }
    }
}
