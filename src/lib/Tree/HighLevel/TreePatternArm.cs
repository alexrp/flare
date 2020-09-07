using System.CodeDom.Compiler;
using Flare.Tree.HighLevel.Patterns;

namespace Flare.Tree.HighLevel
{
    sealed class TreePatternArm
    {
        public TreePattern Pattern { get; }

        public TreeReference? Guard { get; }

        public TreeReference Body { get; }

        public TreePatternArm(TreePattern pattern, TreeReference? guard, TreeReference body)
        {
            Pattern = pattern;
            Guard = guard;
            Body = body;
        }

        public void ToString(IndentedTextWriter writer)
        {
            Pattern.ToString(writer);

            if (Guard is TreeReference guard)
            {
                writer.Write(" if ");
                guard.ToString(writer);
            }

            writer.Write(" => ");
            Body.ToString(writer);
            writer.Write(";");
        }
    }
}
