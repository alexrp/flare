using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.LowLevel
{
    sealed class TreeTypeTestNode : TreeNode
    {
        public TreeReference Operand { get; }

        public TreeType Type { get; }

        public TreeTypeTestNode(TreeContext context, SourceLocation location, TreeReference operand, TreeType type)
            : base(context, location)
        {
            Operand = operand;
            Type = type;
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
            writer.Write("(");
            Operand.ToString(writer);
            writer.Write(" is {0})", Type);
        }
    }
}
