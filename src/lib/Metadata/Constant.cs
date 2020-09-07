using System.Collections.Immutable;
using Flare.Syntax;
using Flare.Tree;

namespace Flare.Metadata
{
    public sealed class Constant : Declaration
    {
        internal TreeContext Tree { get; }

        SyntaxTreeLowerer? _lowerer;

        internal Constant(Module module, ConstantDeclarationNode node)
            : base(module, node)
        {
            Tree = new TreeContext(module.Loader, this, ImmutableArray<TreeParameter>.Empty, null,
                ImmutableArray<TreeUpvalue>.Empty);
            _lowerer = new SyntaxTreeLowerer(module.Loader, Tree, node.Initializer,
                node.GetAnnotation<ImmutableHashSet<SyntaxSymbol>>("Freezes"));
        }

        internal override void Lower()
        {
            if (_lowerer == null)
                return;

            Tree.Body = _lowerer.Lower();
            _ = TreeRewriter.Instance.Visit(Tree.Body, null);

            _lowerer = null; // Release AST references.
        }
    }
}
