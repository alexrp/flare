using System.CodeDom.Compiler;
using System.Collections.Generic;
using Flare.Syntax;

namespace Flare.Tree.HighLevel
{
    sealed class TreeFieldAccessNode : TreeNode
    {
        public TreeReference Subject { get; }

        public string Name { get; }

        public TreeFieldAccessNode(TreeContext context, SourceLocation location, TreeReference subject, string name)
            : base(context, location)
        {
            Subject = subject;
            Name = name;
        }

        public override IEnumerable<TreeReference> Children()
        {
            yield return Subject;
        }

        public override T Accept<T>(TreeVisitor<T> visitor, T state)
        {
            return visitor.Visit(this, state);
        }

        public override void ToString(IndentedTextWriter writer)
        {
            Subject.ToString(writer);
            writer.Write(".{0}", Name);
        }
    }
}
