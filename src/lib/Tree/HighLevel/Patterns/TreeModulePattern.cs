using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Flare.Metadata;
using Flare.Syntax;

namespace Flare.Tree.HighLevel.Patterns
{
    sealed class TreeModulePattern : TreePattern
    {
        public Module Module { get; }

        public TreeModulePattern(TreeLocal? alias, Module module)
            : base(alias)
        {
            Module = module;
        }

        public override TreeReference Compile(TreeContext context, SourceLocation location, TreeLocal operand)
        {
            var local = context.CreateLocal();
            var exprs = ImmutableArray.Create<TreeReference>(
                new TreeAssignNode(context, location,
                    new TreeVariableNode(context, location, local),
                    new TreeRelationalNode(context, location,
                        new TreeVariableNode(context, location, operand), TreeRelationalOperator.Equal,
                        new TreeModuleNode(context, location, Module))));

            if (Alias is TreeLocal alias)
                exprs = exprs.Add(
                    new TreeAssignNode(context, location,
                        new TreeVariableNode(context, location, alias),
                        new TreeVariableNode(context, location, operand)));

            return new TreeBlockNode(context, location, exprs.Add(
                new TreeVariableNode(context, location, local)));
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write(Module);

            if (Alias != null)
                writer.Write(" as {0}", Alias);
        }
    }
}
