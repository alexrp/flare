using System.CodeDom.Compiler;
using Flare.Syntax;

namespace Flare.Tree.HighLevel.Patterns
{
    abstract class TreePattern
    {
        public TreeLocal? Alias { get; }

        public TreePattern(TreeLocal? alias)
        {
            Alias = alias;
        }

        public abstract TreeReference Compile(TreeContext context, SourceLocation location, TreeLocal operand);

        public abstract void ToString(IndentedTextWriter writer);
    }
}
