using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;
using Flare.Tree.HighLevel;

namespace Flare.Tree.LowLevel
{
    sealed class TreeTryReceiveNode : TreeNode
    {
        public TreeLocal Result { get; }

        public TreeReference Then { get; }

        public TreeReference Else { get; }

        public TreeTryReceiveNode(TreeContext context, SourceLocation location, TreeLocal result, TreeReference then,
            TreeReference @else)
            : base(context, location)
        {
            Result = result;
            Then = then;
            Else = @else;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Then;
            yield return Else;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("recv? {0} ", Result);

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
