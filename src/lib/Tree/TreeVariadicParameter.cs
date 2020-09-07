namespace Flare.Tree
{
    sealed class TreeVariadicParameter : TreeVariable
    {
        public override string Name { get; }

        public TreeVariadicParameter(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"@{Name}";
        }
    }
}
