using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Flare.Syntax;
using Flare.Tree.LowLevel;

namespace Flare.Tree.HighLevel
{
    sealed class TreeConditionNode : TreeNode
    {
        public ImmutableArray<TreeConditionArm> Arms { get; }

        public TreeConditionNode(TreeContext context, SourceLocation location, ImmutableArray<TreeConditionArm> arms)
            : base(context, location)
        {
            Arms = arms;
        }

        public override IEnumerable<TreeReference> Children()
        {
            foreach (var arm in Arms)
            {
                yield return arm.Condition;
                yield return arm.Body;
            }
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
            // cond {
            //     a => b;
            //     c => d;
            //     e => f;
            // }
            // ```
            //
            // To:
            //
            // ```
            // if a {
            //     b;
            // } else {
            //     if c {
            //         d;
            //     } else {
            //         if e {
            //             f;
            //         } else {
            //             panic "No condition arm matched.";
            //         };
            //     };
            // }
            // ```

            var ifs = ImmutableArray<TreeIfNode>.Empty;

            // We set up the `if`/`else` chain in this roundabout way with placeholder nodes in
            // order to avoid recursion.

            foreach (var arm in Arms)
                ifs = ifs.Add(new TreeIfNode(Context, Location, arm.Condition, arm.Body,
                    new TreeLiteralNode(Context, Location, null)));

            var enumerator = ifs.GetEnumerator();

            _ = enumerator.MoveNext();

            foreach (var @if in ifs)
                @if.Else.Replace(enumerator.MoveNext() ? (TreeNode)enumerator.Current :
                    new TreePanicNode(Context, Location, "No condition arm matched."));

            return ifs[0];
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.WriteLine("cond {");
            writer.Indent++;

            foreach (var arm in Arms)
            {
                arm.ToString(writer);
                writer.WriteLine();
            }

            writer.Indent--;
            writer.Write("}");
        }
    }
}
