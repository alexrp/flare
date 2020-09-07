using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using Flare.Syntax;
using Flare.Tree.HighLevel;

namespace Flare.Tree.LowLevel
{
    sealed class TreeTryCallNode : TreeNode
    {
        public TreeReference Subject { get; }

        public ImmutableArray<TreeReference> Arguments { get; }

        public TreeReference? VariadicArgument { get; }

        public TreeLocal Exception { get; }

        public TreeReference Catch { get; }

        public TreeTryCallNode(TreeContext context, SourceLocation location, TreeReference subject,
            ImmutableArray<TreeReference> arguments, TreeReference? variadicArgument, TreeLocal exception,
            TreeReference @catch)
            : base(context, location)
        {
            Subject = subject;
            Arguments = arguments;
            VariadicArgument = variadicArgument;
            Exception = exception;
            Catch = @catch;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Subject;

            foreach (var arg in Arguments)
                yield return arg;

            if (VariadicArgument is TreeReference variadic)
                yield return variadic;

            yield return Catch;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            Subject.ToString(writer);
            writer.Write("(");

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

            writer.Write(") catch {0} ", Exception);

            var @catch = Catch.Value;
            var catchBlock = @catch is TreeBlockNode;

            if (!catchBlock)
            {
                writer.WriteLine("{");
                writer.Indent++;
            }

            @catch.ToString(writer);

            if (!catchBlock)
            {
                writer.WriteLine();
                writer.Indent--;
                writer.Write("}");
            }
        }
    }
}
