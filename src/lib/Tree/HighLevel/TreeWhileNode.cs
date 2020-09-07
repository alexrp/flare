using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeWhileNode : TreeNode
    {
        public TreeReference Condition { get; }

        public TreeReference Body { get; }

        public TreeWhileNode(TreeContext context, SourceLocation location, TreeReference condition, TreeReference body)
            : base(context, location)
        {
            Condition = condition;
            Body = body;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Condition;
            yield return Body;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("while ");
            Condition.ToString(writer);
            writer.Write(" ");

            var body = Body.Value;
            var block = body is TreeBlockNode;

            if (!block)
            {
                writer.WriteLine("{");
                writer.Indent++;
            }

            body.ToString(writer);

            if (!block)
            {
                writer.WriteLine();
                writer.Indent--;
                writer.Write("}");
            }
        }
    }
}
