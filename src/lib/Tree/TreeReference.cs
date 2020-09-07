using System.CodeDom.Compiler;
using Flare.Syntax;

namespace Flare.Tree
{
    readonly struct TreeReference
    {
        public TreeContext Context { get; }

        public int Id { get; }

        public TreeNode Value => Context.ResolveNode(Id);

        public SourceLocation Location => Value.Location;

        public TreeReference(TreeContext context, int id)
        {
            Context = context;
            Id = id;
        }

        public void Replace(TreeNode node)
        {
            Context.ReplaceNode(Id, node);
        }

        public void ToString(IndentedTextWriter writer)
        {
            Value.ToString(writer);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator TreeReference(TreeNode node)
        {
            return node.Reference;
        }

        public static implicit operator TreeNode(TreeReference reference)
        {
            return reference.Value;
        }
    }
}
