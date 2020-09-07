namespace Flare.Tree
{
    sealed class TreeParameter : TreeVariable
    {
        public override string Name { get; }

        public TreeParameter(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"@{Name}";
        }
    }
}
