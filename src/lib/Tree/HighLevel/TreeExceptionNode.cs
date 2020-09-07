using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeExceptionNode : TreeNode
    {
        public string Name { get; }

        public ImmutableArray<TreeRecordField> Fields { get; }

        public TreeExceptionNode(TreeContext context, SourceLocation location, string name,
            ImmutableArray<TreeRecordField> fields)
            : base(context, location)
        {
            Name = name;
            Fields = fields;
        }

        public override IEnumerable<TreeReference> Children()
        {
            foreach (var field in Fields)
                yield return field.Value;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            writer.Write("exc {0} { ", Name);

            var first = true;

            foreach (var field in Fields)
            {
                if (!first)
                    writer.Write(", ");

                field.ToString(writer);

                first = false;
            }

            writer.Write("}");
        }
    }
}
