using System;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Numerics;
using System.Text;
using Flare.Syntax;

namespace Flare.Tree.HighLevel.Patterns
{
    sealed class TreeLiteralPattern : TreePattern
    {
        public object? Value { get; }

        public TreeLiteralPattern(TreeLocal? alias, object? value)
            : base(alias)
        {
            Value = value;
        }

        public override TreeReference Compile(TreeContext context, SourceLocation location, TreeLocal operand)
        {
            var local = context.CreateLocal();
            var exprs = ImmutableArray.Create<TreeReference>(
                new TreeAssignNode(context, location,
                    new TreeVariableNode(context, location, local),
                    new TreeRelationalNode(context, location,
                        new TreeVariableNode(context, location, operand), TreeRelationalOperator.Equal,
                        new TreeLiteralNode(context, location, Value))));

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
            writer.Write(Value switch
            {
                null => "nil",
                bool b => b ? "true" : "false",
                string a => $":{a}",
                BigInteger i => i.ToString(),
                double d => d.ToString(),
                ReadOnlyMemory<byte> s => $"\"{Encoding.UTF8.GetString(s.Span)}\"",
                _ => throw DebugAssert.Unreachable(),
            });

            if (Alias != null)
                writer.Write(" as {0}", Alias);
        }
    }
}
