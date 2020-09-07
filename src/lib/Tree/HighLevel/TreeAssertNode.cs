using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeAssertNode : TreeNode
    {
        public TreeReference Operand { get; }

        public string Message { get; }

        public TreeAssertNode(TreeContext context, SourceLocation location, TreeReference operand, string message)
            : base(context, location)
        {
            Operand = operand;
            Message = message;
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
            writer.Write("assert ");
            Operand.ToString(writer);
        }
    }
}
