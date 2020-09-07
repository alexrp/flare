using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeTupleNode : TreeNode
    {
        public ImmutableArray<TreeReference> Components { get; }

        public TreeTupleNode(TreeContext context, SourceLocation location, ImmutableArray<TreeReference> components)
            : base(context, location)
        {
            Components = components;
        }

        public override IEnumerable<TreeReference> Children()
        {
            foreach (var comp in Components)
                yield return comp;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("(");

            var first = true;

            foreach (var comp in Components)
            {
                if (!first)
                    writer.Write(", ");

                comp.ToString(writer);

                first = false;
            }

            writer.Write(")");
        }
    }
}
