using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeUnaryNode : TreeNode
    {
        public string Operator { get; }

        public TreeReference Operand { get; }

        public TreeUnaryNode(TreeContext context, SourceLocation location, string @operator, TreeReference operand)
            : base(context, location)
        {
            Operator = @operator;
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
            writer.Write("{0}(", Operator);
            Operand.ToString(writer);
            writer.Write(")");
        }
    }
}
