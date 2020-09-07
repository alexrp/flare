using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Tree.HighLevel.Patterns
{
    sealed class TreeMapPattern : TreePattern
    {
        public ImmutableArray<TreeMapPatternPair> Pairs { get; }

        public TreeMapPattern(TreeLocal? alias, ImmutableArray<TreeMapPatternPair> pairs)
            : base(alias)
        {
            Pairs = pairs;
        }

        public override TreeReference Compile(TreeContext context, SourceLocation location, TreeLocal operand)
        {
            throw new System.NotImplementedException(); // TODO
        }

        public override void ToString(IndentedTextWriter writer)
        {
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

            if (Alias != null)
                writer.Write(" as {0}", Alias);
        }
    }
}
