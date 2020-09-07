using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeMethodCallNode : TreeNode
    {
        public TreeReference Subject { get; }

        public string Name { get; }

        public ImmutableArray<TreeReference> Arguments { get; }

        public TreeReference? VariadicArgument { get; }

        public ImmutableArray<TreePatternArm> CatchArms { get; }

        public TreeMethodCallNode(TreeContext context, SourceLocation location, TreeReference subject, string name,
            ImmutableArray<TreeReference> arguments, TreeReference? variadicArgument,
            ImmutableArray<TreePatternArm> catchArms)
            : base(context, location)
        {
            Subject = subject;
            Name = name;
            Arguments = arguments;
            VariadicArgument = variadicArgument;
            CatchArms = catchArms;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Subject;

            foreach (var arg in Arguments)
                yield return arg;

            if (VariadicArgument is TreeReference variadic)
                yield return variadic;

            foreach (var arm in CatchArms)
            {
                if (arm.Guard is TreeReference guard)
                    yield return guard;

                yield return arm.Body;
            }
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override TreeNode Rewrite()
        {
            // Rewrite 'obj->method()` to:
            //
            // ```
            // {
            //     $loc = obj;
            //     $loc.method($loc);
            // }
            // ```

            var local = Context.CreateLocal();

            return new TreeBlockNode(Context, Location, ImmutableArray.Create<TreeReference>(
                new TreeAssignNode(Context, Location,
                    new TreeVariableNode(Context, Location, local), Subject),
                new TreeCallNode(Context, Location,
                    new TreeFieldAccessNode(Context, Location,
                        new TreeVariableNode(Context, Location, local), Name),
                    Arguments.Insert(0,
                        new TreeVariableNode(Context, Location, local)), VariadicArgument, CatchArms)));
        }

        public override void ToString(IndentedTextWriter writer)
        {
            Subject.ToString(writer);
            writer.Write("->{0}(", Name);

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

            writer.Write(")");

            if (CatchArms.IsEmpty)
                return;

            writer.WriteLine(" catch {");
            writer.Indent++;

            foreach (var arm in CatchArms)
            {
                arm.ToString(writer);
                writer.WriteLine();
            }

            writer.Indent--;
            writer.Write("}");
        }
    }
}
