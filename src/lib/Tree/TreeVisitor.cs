using Flare.Tree.HighLevel;
using Flare.Tree.LowLevel;

namespace Flare.Tree
{
    abstract class TreeVisitor<T>
    {
        public T Visit(TreeNode node, T state)
        {
            return node.Accept(this, state);
        }

        protected virtual T DefaultVisit(TreeNode node, T state)
        {
            return state;
        }

        public virtual T Visit(TreeArrayNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeAssertNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeAssignNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeBinaryNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeBlockNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeBreakNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeCallNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeConditionNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeConstantNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeExceptionNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeExternalNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeFieldAccessNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeForNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeFreezeNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeFunctionNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeIfNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeIndexNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeLambdaNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeLetNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeLiteralNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeLogicalAndNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeLogicalNotNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeLogicalOrNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeLoopNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeMapNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeMatchNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeMethodCallNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeModuleNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeParenthesizedNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeRaiseNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeReceiveNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeRecordNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeRelationalNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeReturnNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeSendNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeSetNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeTupleNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeUnaryNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeVariableNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeWhileNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeArraySliceNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeBlockingReceiveNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeCheckFieldAccessNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeGotoNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeIteratorNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeLabelNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreePanicNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeRecordNameNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeTestNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeTryCallNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeTryReceiveNode node, T state)
        {
            return DefaultVisit(node, state);
        }

        public virtual T Visit(TreeTypeTestNode node, T state)
        {
            return DefaultVisit(node, state);
        }
    }
}
