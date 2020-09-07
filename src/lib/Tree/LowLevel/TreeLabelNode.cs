using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.LowLevel
{
    sealed class TreeLabelNode : TreeNode
    {
        public string Name { get; }

        public TreeReference Expression { get; }

        public TreeLabelNode(TreeContext context, SourceLocation location, string name, TreeReference expression)
            : base(context, location)
        {
            Name = name;
            Expression = expression;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Expression;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("{0}: ", Name);
            Expression.ToString(writer);
        }
    }
}
