using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Tree.HighLevel.Patterns
{
    sealed class TreeRecordPattern : TreePattern
    {
        public string? Name { get; }

        public ImmutableArray<TreeRecordPatternField> Fields { get; }

        public TreeRecordPattern(TreeLocal? alias, string? name, ImmutableArray<TreeRecordPatternField> fields)
            : base(alias)
        {
            Name = name;
            Fields = fields;
        }

        public override TreeReference Compile(TreeContext context, SourceLocation location, TreeLocal operand)
        {
            throw new System.NotImplementedException(); // TODO
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("rec ");

            if (Name != null)
                writer.Write("{0} ", Name);

            writer.Write("{ ");

            var first = true;

            foreach (var field in Fields)
            {
                if (!first)
                    writer.Write(", ");

                field.ToString(writer);

                first = false;
            }

            writer.Write("}");

            if (Alias != null)
                writer.Write(" as {0}", Alias);
        }
    }
}
