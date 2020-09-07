namespace Flare.Tree
{
    sealed class TreeUpvalue : TreeVariable
    {
        public TreeVariable Variable { get; }

        public int Slot { get; }

        public override string Name => Variable.Name!;

        public TreeUpvalue(TreeVariable variable, int slot)
        {
            Variable = variable;
            Slot = slot;
        }

        public override string ToString()
        {
            return $"!{Slot}";
        }
    }
}
