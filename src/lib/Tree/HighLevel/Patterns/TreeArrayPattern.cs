using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Tree.HighLevel.Patterns
{
    sealed class TreeArrayPattern : TreePattern
    {
        public ImmutableArray<TreePattern> Elements { get; }

        public TreePattern? Remainder { get; }

        public TreeArrayPattern(TreeLocal? alias, ImmutableArray<TreePattern> elements, TreePattern? remainder)
            : base(alias)
        {
            Elements = elements;
            Remainder = remainder;
        }

        public override TreeReference Compile(TreeContext context, SourceLocation location, TreeLocal operand)
        {
            throw new System.NotImplementedException(); // TODO
        }

        public override void ToString(IndentedTextWriter writer)
        {
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

            if (Remainder != null)
            {
                writer.Write(" :: ");
                Remainder.ToString(writer);
            }

            if (Alias != null)
                writer.Write(" as {0}", Alias);
        }
    }
}
