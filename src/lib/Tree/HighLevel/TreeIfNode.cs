using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeIfNode : TreeNode
    {
        public TreeReference Condition { get; }

        public TreeReference Then { get; }

        public TreeReference Else { get; }

        public TreeIfNode(TreeContext context, SourceLocation location, TreeReference condition, TreeReference then,
            TreeReference @else)
            : base(context, location)
        {
            Condition = condition;
            Then = then;
            Else = @else;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Condition;
            yield return Then;
            yield return Else;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("if ");
            Condition.ToString(writer);
            writer.Write(" ");

            var then = Then.Value;
            var thenBlock = then is TreeBlockNode;

            if (!thenBlock)
            {
                writer.WriteLine("{");
                writer.Indent++;
            }

            then.ToString(writer);

            if (!thenBlock)
            {
                writer.WriteLine();
                writer.Indent--;
                writer.Write("}");
            }

            writer.Write(" else ");

            var @else = Else.Value;
            var elseBlock = @else is TreeBlockNode;

            if (!elseBlock)
            {
                writer.WriteLine("{");
                writer.Indent++;
            }

            @else.ToString(writer);

            if (!elseBlock)
            {
                writer.WriteLine();
                writer.Indent--;
                writer.Write("}");
            }
        }
    }
}
