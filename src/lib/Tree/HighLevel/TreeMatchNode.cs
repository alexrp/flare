using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeMatchNode : TreeNode
    {
        public TreeReference Operand { get; }

        public ImmutableArray<TreePatternArm> Arms { get; }

        public TreeMatchNode(TreeContext context, SourceLocation location, TreeReference operand,
            ImmutableArray<TreePatternArm> arms)
            : base(context, location)
        {
            Operand = operand;
            Arms = arms;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Operand;

            foreach (var arm in Arms)
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
            return this; // TODO
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("match ");
            Operand.ToString(writer);
            writer.WriteLine(" {");
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
