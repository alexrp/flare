using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeMapNode : TreeNode
    {
        public ImmutableArray<TreeMapPair> Pairs { get; }

        public bool IsMutable { get; }

        public TreeMapNode(TreeContext context, SourceLocation location, ImmutableArray<TreeMapPair> pairs,
            bool mutable)
            : base(context, location)
        {
            Pairs = pairs;
            IsMutable = mutable;
        }

        public override IEnumerable<TreeReference> Children()
        {
            foreach (var pair in Pairs)
            {
                yield return pair.Key;
                yield return pair.Value;
            }
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            if (IsMutable)
                writer.Write("mut ");

            writer.Write("#[");

            var first = true;

            foreach (var pair in Pairs)
            {
                if (!first)
                    writer.Write(", ");

                pair.ToString(writer);

                first = false;
            }

            writer.Write("]");
        }
    }
}
