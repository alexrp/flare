using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using Flare.Syntax;
using Flare.Tree.HighLevel.Patterns;
using Flare.Tree.LowLevel;

namespace Flare.Tree.HighLevel
{
    sealed class TreeForNode : TreeNode
    {
        public TreePattern Pattern { get; }

        public TreeReference Collection { get; }

        public TreeReference Body { get; }

        public TreeForNode(TreeContext context, SourceLocation location, TreePattern pattern, TreeReference collection,
            TreeReference body)
            : base(context, location)
        {
            Pattern = pattern;
            Collection = collection;
            Body = body;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Collection;
            yield return Body;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override TreeNode Rewrite()
        {
            // Rewrite `for pat in col ...` to:
            //
            // ```
            // {
            //     $loc = iter col;
            //     while $loc->__next__() {
            //         let pat = $loc->__current__();
            //         ...;
            //     };
            // }
            // ```

            var local = Context.CreateLocal();

            return new TreeBlockNode(Context, Location, ImmutableArray.Create<TreeReference>(
                new TreeAssignNode(Context, Location,
                    new TreeVariableNode(Context, Location, local),
                    new TreeIteratorNode(Context, Location, Collection)),
                new TreeWhileNode(Context, Location,
                    new TreeMethodCallNode(Context, Location,
                        new TreeVariableNode(Context, Location, local), "__next__",
                        ImmutableArray<TreeReference>.Empty, null, ImmutableArray<TreePatternArm>.Empty),
                    new TreeBlockNode(Context, Location, ImmutableArray.Create(
                        new TreeLetNode(Context, Location, Pattern,
                            new TreeMethodCallNode(Context, Location,
                                new TreeVariableNode(Context, Location, local), "__current__",
                                ImmutableArray<TreeReference>.Empty, null, ImmutableArray<TreePatternArm>.Empty)),
                        Body)))));
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("for ");
            Pattern.ToString(writer);
            writer.Write(" in ");
            Collection.ToString(writer);
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
