using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Tree.HighLevel.Patterns
{
    sealed class TreeSetPattern : TreePattern
    {
        public ImmutableArray<TreeReference> Elements { get; }

        public TreeSetPattern(TreeLocal? alias, ImmutableArray<TreeReference> elements)
            : base(alias)
        {
            Elements = elements;
        }

        public override TreeReference Compile(TreeContext context, SourceLocation location, TreeLocal operand)
        {
            throw new System.NotImplementedException(); // TODO
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("%{");

            var first = true;

            foreach (var elem in Elements)
            {
                if (!first)
                    writer.Write(", ");

                elem.ToString(writer);

                first = false;
            }

            writer.Write("}");

            if (Alias != null)
                writer.Write(" as {0}", Alias);
        }
    }
}
