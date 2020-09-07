using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeFreezeNode : TreeNode
    {
        public TreeReference Operand { get; }

        public bool IsCollection { get; }

        public TreeFreezeNode(TreeContext context, SourceLocation location, TreeReference operand, bool collection)
            : base(context, location)
        {
            Operand = operand;
            IsCollection = collection;
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
            writer.Write("freeze ");

            if (IsCollection)
                writer.Write("in ");

            Operand.ToString(writer);
        }
    }
}
