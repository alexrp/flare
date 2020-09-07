using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Flare.Syntax;

namespace Flare.Tree
{
    abstract class TreeNode
    {
        public TreeReference Reference { get; }

        public TreeContext Context => Reference.Context;

        public SourceLocation Location { get; }

        protected TreeNode(TreeContext context, SourceLocation location)
        {
            Reference = context.RegisterNode(this);
            Location = location;
        }

        public virtual IEnumerable<TreeReference> Children()
        {
            yield break;
        }

        public abstract T Accept<T>(TreeVisitor<T> visitor, T state);

        public virtual TreeNode Rewrite()
        {
            return this;
        }

        public abstract void ToString(IndentedTextWriter writer);

        public override string ToString()
        {
            using var sw = new StringWriter();
            using var itw = new IndentedTextWriter(sw);

            ToString(itw);

            return sw.ToString();
        }
    }
}
