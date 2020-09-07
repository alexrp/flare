using System.CodeDom.Compiler;

namespace Flare.Tree.HighLevel
{
    sealed class TreeConditionArm
    {
        public TreeReference Condition { get; }

        public TreeReference Body { get; }

        public TreeConditionArm(TreeReference condition, TreeReference body)
        {
            Condition = condition;
            Body = body;
        }

        public void ToString(IndentedTextWriter writer)
        {
            Condition.ToString(writer);
            writer.Write(" => ");
            Body.ToString(writer);
            writer.Write(";");
        }
    }
}
