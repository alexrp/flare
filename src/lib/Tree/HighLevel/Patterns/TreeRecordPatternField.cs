using System.CodeDom.Compiler;

namespace Flare.Tree.HighLevel.Patterns
{
    sealed class TreeRecordPatternField
    {
        public string Name { get; }

        public TreePattern Pattern { get; }

        public TreeRecordPatternField(string name, TreePattern pattern)
        {
            Name = name;
            Pattern = pattern;
        }

        public void ToString(IndentedTextWriter writer)
        {
            writer.Write("{0} = ", Name);
            Pattern.ToString(writer);
        }
    }
}
