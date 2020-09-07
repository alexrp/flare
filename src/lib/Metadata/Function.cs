using System.Collections.Immutable;
using Flare.Syntax;
using Flare.Tree;

namespace Flare.Metadata
{
    public sealed class Function : Declaration
    {
        public bool HasParameters => !Parameters.IsEmpty;

        public ImmutableArray<Parameter> Parameters { get; }

        internal TreeContext Tree { get; }

        SyntaxTreeLowerer? _lowerer;

        internal Function(Module module, FunctionDeclarationNode node)
            : base(module, node)
        {
            var parms = ImmutableArray<Parameter>.Empty;
            var treeParams = ImmutableArray<TreeParameter>.Empty;
            var treeVariadic = (TreeVariadicParameter?)null;
            var syms = ImmutableArray<(SyntaxSymbol, TreeVariable)>.Empty;
            var i = 0;

            foreach (var param in node.ParameterList.Parameters.Nodes)
            {
                var name = param.NameToken.Text;
                var variadic = param.DotDotToken != null;

                parms = parms.Add(new Parameter(this, param.Attributes, name, i, variadic));

                TreeVariable variable;

                if (!variadic)
                {
                    var p = new TreeParameter(name);

                    variable = p;
                    treeParams = treeParams.Add(p);
                }
                else
                    variable = treeVariadic = new TreeVariadicParameter(name);

                syms = syms.Add((param.GetAnnotation<SyntaxSymbol>("Symbol"), variable));

                i++;
            }

            Parameters = parms;
            Tree = new TreeContext(module.Loader, this, treeParams, treeVariadic, ImmutableArray<TreeUpvalue>.Empty);
            _lowerer = new SyntaxTreeLowerer(module.Loader, Tree, node.Body,
                node.GetAnnotation<ImmutableHashSet<SyntaxSymbol>>("Freezes"));

            foreach (var (sym, variable) in syms)
                _lowerer.AddVariable(sym, variable);
        }

        public void Test()
        {
            Lower();
            System.Console.WriteLine(Tree);
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
