using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using Flare.Syntax;
using Flare.Tree.LowLevel;

namespace Flare.Tree.HighLevel
{
    sealed class TreeBlockNode : TreeNode
    {
        public ImmutableArray<TreeReference> Expressions { get; }

        public TreeBlockNode(TreeContext context, SourceLocation location, ImmutableArray<TreeReference> expressions)
            : base(context, location)
        {
            Expressions = expressions;
        }

        public override IEnumerable<TreeReference> Children()
        {
            foreach (var elem in Expressions)
                yield return elem;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override TreeNode Rewrite()
        {
            // A block with a single expression can simply be rewritten to that expression, unless
            // it's a labeled expression.
            if (Expressions.Length == 1)
            {
                var expr = Expressions[0].Value;

                if (!(expr is TreeLabelNode))
                    return expr;
            }

            // If the block directly contains other blocks, the expressions in those blocks can be
            // folded into the current block.

            var unchanged = true;

            foreach (var expr in Expressions)
            {
                if (expr.Value is TreeBlockNode)
                {
                    unchanged = false;
                    break;
                }
            }

            if (unchanged)
                return this;

            var exprs = ImmutableArray<TreeReference>.Empty;

            for (var i = 0; i < Expressions.Length; i++)
            {
                var expr = Expressions[i];

                exprs = expr.Value is TreeBlockNode b ? exprs.AddRange(b.Expressions) : exprs.Add(expr);
            }

            return new TreeBlockNode(Context, Location, exprs);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.WriteLine("{");
            writer.Indent++;

            foreach (var expr in Expressions)
            {
                expr.ToString(writer);
                writer.WriteLine(";");
            }

            writer.Indent--;
            writer.Write("}");
        }
    }
}
