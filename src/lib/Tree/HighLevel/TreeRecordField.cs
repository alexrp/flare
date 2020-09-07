using System.CodeDom.Compiler;

namespace Flare.Tree.HighLevel
{
    sealed class TreeRecordField
    {
        public string Name { get; }

        public TreeReference Value { get; }

        public bool IsMutable { get; }

        public TreeRecordField(string name, TreeReference value, bool mutable)
        {
            Name = name;
            Value = value;
            IsMutable = mutable;
        }

        public void ToString(IndentedTextWriter writer)
        {
            if (IsMutable)
                writer.Write("mut ");

            writer.Write("{0} = ", Name);
            Value.ToString(writer);
        }
    }
}
