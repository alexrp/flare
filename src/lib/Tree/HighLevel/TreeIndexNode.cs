using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeIndexNode : TreeNode
    {
        public TreeReference Subject { get; }

        public ImmutableArray<TreeReference> Arguments { get; }

        public TreeReference? VariadicArgument { get; }

        public TreeIndexNode(TreeContext context, SourceLocation location, TreeReference subject,
            ImmutableArray<TreeReference> arguments, TreeReference? variadicArgument)
            : base(context, location)
        {
            Subject = subject;
            Arguments = arguments;
            VariadicArgument = variadicArgument;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Subject;

            foreach (var arg in Arguments)
                yield return arg;

            if (VariadicArgument is TreeReference variadic)
                yield return variadic;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            Subject.ToString(writer);
            writer.Write("[");

            var first = true;

            foreach (var arg in Arguments)
            {
                if (!first)
                    writer.Write(", ");

                arg.ToString(writer);

                first = false;
            }

            if (VariadicArgument is TreeReference variadic)
            {
                if (!first)
                    writer.Write(", ");

                writer.Write(".. ");
                variadic.ToString(writer);
            }

            writer.Write("]");
        }
    }
}
