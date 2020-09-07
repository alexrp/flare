namespace Flare.Tree
{
    sealed class TreeLocal : TreeVariable
    {
        public int Id { get; }

        public override string? Name { get; }

        public bool IsChecked { get; }

        public TreeLocal(int id, string? name, bool @checked)
        {
            Id = id;
            Name = name;
            IsChecked = @checked;
        }

        public override string ToString()
        {
            return $"${Id}";
        }
    }
}
