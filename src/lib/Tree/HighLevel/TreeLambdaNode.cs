using System.CodeDom.Compiler;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeLambdaNode : TreeNode
    {
        public TreeContext Lambda { get; }

        public TreeLambdaNode(TreeContext context, SourceLocation location, TreeContext lambda)
            : base(context, location)
        {
            Lambda = lambda;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("fn[");

            var first = true;

            foreach (var upvalue in Lambda.Upvalues)
            {
                if (!first)
                    writer.Write(", ");

                writer.Write(upvalue.Variable);

                first = false;
            }

            writer.Write("](");

            first = true;

            foreach (var param in Lambda.Parameters)
            {
                if (!first)
                    writer.Write(", ");

                writer.Write(param);

                first = false;
            }

            if (Lambda.VariadicParameter != null)
            {
                if (!first)
                    writer.Write(", ");

                writer.Write(".. {0}", Lambda.VariadicParameter);
            }

            writer.Write(") => ");
            ((TreeReference)Lambda.Body!).ToString(writer);
        }
    }
}
