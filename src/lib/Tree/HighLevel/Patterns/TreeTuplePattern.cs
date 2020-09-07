using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Tree.HighLevel.Patterns
{
    sealed class TreeTuplePattern : TreePattern
    {
        public ImmutableArray<TreePattern> Components { get; }

        public TreeTuplePattern(TreeLocal? alias, ImmutableArray<TreePattern> components)
            : base(alias)
        {
            Components = components;
        }

        public override TreeReference Compile(TreeContext context, SourceLocation location, TreeLocal operand)
        {
            throw new System.NotImplementedException(); // TODO
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

            if (Alias != null)
                writer.Write(" as {0}", Alias);
        }
    }
}
