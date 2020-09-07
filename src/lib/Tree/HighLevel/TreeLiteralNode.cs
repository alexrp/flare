using System;
using System.CodeDom.Compiler;
using System.Numerics;
using System.Text;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeLiteralNode : TreeNode
    {
        public object? Value { get; }

        public TreeLiteralNode(TreeContext context, SourceLocation location, object? value)
            : base(context, location)
        {
            Value = value;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
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
        }
    }
}
