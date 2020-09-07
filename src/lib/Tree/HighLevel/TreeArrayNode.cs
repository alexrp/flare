using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeArrayNode : TreeNode
    {
        public ImmutableArray<TreeReference> Elements { get; }

        public bool IsMutable { get; }

        public TreeArrayNode(TreeContext context, SourceLocation location, ImmutableArray<TreeReference> elements,
            bool mutable)
            : base(context, location)
        {
            Elements = elements;
            IsMutable = mutable;
        }

        public override IEnumerable<TreeReference> Children()
        {
            foreach (var elem in Elements)
                yield return elem;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            if (IsMutable)
                writer.Write("mut ");

            writer.Write("[");

            var first = true;

            foreach (var elem in Elements)
            {
                if (!first)
                    writer.Write(", ");

                elem.ToString(writer);

                first = false;
            }

            writer.Write("]");
        }
    }
}
