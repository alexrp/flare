using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Flare.Syntax;
using Flare.Tree.HighLevel.Patterns;
using Flare.Tree.LowLevel;

namespace Flare.Tree.HighLevel
{
    sealed class TreeLetNode : TreeNode
    {
        public TreePattern Pattern { get; }

        public TreeReference Initializer { get; }

        public TreeLetNode(TreeContext context, SourceLocation location, TreePattern pattern, TreeReference initializer)
            : base(context, location)
        {
            Pattern = pattern;
            Initializer = initializer;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Initializer;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override TreeNode Rewrite()
        {
            // Rewrite `let pat = expr` to:
            //
            // ```
            // {
            //     $loc = expr;
            //     if pat {
            //         $loc;
            //     } else {
            //         panic "Let pattern did not match.";
            //     };
            // }
            // ```

            var local = Context.CreateLocal();

            return new TreeBlockNode(Context, Location, ImmutableArray.Create<TreeReference>(
                new TreeAssignNode(Context, Location,
                    new TreeVariableNode(Context, Location, local), Initializer),
                new TreeIfNode(Context, Location, Pattern.Compile(Context, Location, local),
                    new TreeVariableNode(Context, Location, local),
                    new TreePanicNode(Context, Location, "Let pattern did not match."))));
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("let ");
            Pattern.ToString(writer);
            writer.Write(" = ");
            Initializer.ToString(writer);
        }
    }
}
