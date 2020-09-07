using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using Flare.Syntax;
using Flare.Tree.HighLevel.Patterns;
using Flare.Tree.LowLevel;

namespace Flare.Tree.HighLevel
{
    sealed class TreeReceiveNode : TreeNode
    {
        public ImmutableArray<TreePatternArm> Arms { get; }

        public TreeReference? Else { get; }

        public TreeReceiveNode(TreeContext context, SourceLocation location, ImmutableArray<TreePatternArm> arms,
            TreeReference? @else)
            : base(context, location)
        {
            Arms = arms;
            Else = @else;
        }

        public override IEnumerable<TreeReference> Children()
        {
            foreach (var arm in Arms)
            {
                if (arm.Guard is TreeReference guard)
                    yield return guard;

                yield return arm.Body;
            }

            if (Else is TreeReference @else)
                yield return @else;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override TreeNode Rewrite()
        {
            // Rewrite:
            //
            // ```
            // recv {
            //     pat1 => a;
            //     pat2 => b;
            //     pat3 => c;
            // }
            // ```
            //
            // To:
            //
            // ```
            // match recv! {
            //     pat1 => a;
            //     pat2 => b;
            //     pat3 => c;
            //     _ => panic "No receive arm matched.";
            // }
            // ```
            //
            // Rewrite:
            //
            // ```
            // recv {
            //     pat1 => a;
            //     pat2 => b;
            //     pat3 => c;
            // } else {
            //     d;
            // }
            // ```
            //
            // To:
            //
            // ```
            // recv? $loc {
            //     match $loc {
            //         pat1 => a;
            //         pat2 => b;
            //         pat3 => c;
            //         _ => panic "No receive arm matched.";
            //     };
            // } else {
            //     d;
            // }
            // ```

            var arms = Arms.Add(new TreePatternArm(new TreeIdentifierPattern(null, Context.CreateLocal()), null,
                new TreePanicNode(Context, Location, "No receive arm matched.")));

            if (Else is TreeReference @else)
            {
                var local = Context.CreateLocal();

                return new TreeTryReceiveNode(Context, Location, local,
                    new TreeMatchNode(Context, Location,
                        new TreeVariableNode(Context, Location, local), arms), @else);
            }

            return new TreeMatchNode(Context, Location,
                new TreeBlockingReceiveNode(Context, Location), arms);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.WriteLine("recv {");
            writer.Indent++;

            foreach (var arm in Arms)
            {
                arm.ToString(writer);
                writer.WriteLine();
            }

            writer.Indent--;
            writer.Write("}");

            if (!(Else is TreeReference @else))
                return;

            writer.WriteLine(" else ");

            var body = @else.Value;
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
