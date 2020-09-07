namespace Flare.Tree
{
    abstract class TreeWalker<T> : TreeVisitor<T>
    {
        protected override T DefaultVisit(TreeNode node, T state)
        {
            foreach (var child in node.Children())
                state = Visit(child, state);

            return state;
        }
    }
}
