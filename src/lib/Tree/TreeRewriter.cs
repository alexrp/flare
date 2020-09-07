using Flare.Tree.HighLevel;

namespace Flare.Tree
{
    sealed class TreeRewriter : TreeWalker<object?>
    {
        public static TreeRewriter Instance { get; } = new TreeRewriter();

        TreeNode Rewrite(TreeNode node)
        {
            var current = node;

            while (true)
            {
                var rewritten = current.Rewrite();

                if (rewritten == current)
                    break;

                current = rewritten;
            }

            if (current != node)
                node.Reference.Replace(current);

            return current;
        }

        protected override object? DefaultVisit(TreeNode node, object? state)
        {
            // For most nodes, we want to rewrite the current node before we head into child nodes,
            // since the rewrite may produce additional child nodes that need to be rewritten.

            node = Rewrite(node);

            return base.DefaultVisit(node, state);
        }

        public override object? Visit(TreeBlockNode node, object? state)
        {
            // For block nodes, we want to rewrite the children first since doing so often exposes
            // opportunities to fold child block nodes into the current block node. We know that
            // blocks themselves never rewrite to anything other than a block or the single node
            // contained within.

            state = base.DefaultVisit(node, state);

            _ = Rewrite(node);

            return state;
        }
    }
}
