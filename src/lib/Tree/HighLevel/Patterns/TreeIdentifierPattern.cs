using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Tree.HighLevel.Patterns
{
    sealed class TreeIdentifierPattern : TreePattern
    {
        public TreeLocal Identifier { get; }

        public TreeIdentifierPattern(TreeLocal? alias, TreeLocal identifier)
            : base(alias)
        {
            Identifier = identifier;
        }

        public override TreeReference Compile(TreeContext context, SourceLocation location, TreeLocal operand)
        {
            var exprs = ImmutableArray.Create<TreeReference>(
                new TreeAssignNode(context, location,
                    new TreeVariableNode(context, location, Identifier),
                    new TreeVariableNode(context, location, operand)));

            if (Alias is TreeLocal alias)
                exprs = exprs.Add(
                    new TreeAssignNode(context, location,
                        new TreeVariableNode(context, location, alias),
                        new TreeVariableNode(context, location, operand)));

            return new TreeBlockNode(context, location, exprs.Add(
                new TreeLiteralNode(context, location, true)));
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write(Identifier);

            if (Alias != null)
                writer.Write(" as {0}", Alias);
        }
    }
}
