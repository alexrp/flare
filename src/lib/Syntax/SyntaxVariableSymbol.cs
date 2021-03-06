using Flare.Metadata;

namespace Flare.Syntax
{
    sealed class SyntaxVariableSymbol : SyntaxSymbol
    {
        public override SyntaxSymbolKind Kind { get; }

        public override ModulePath? Module { get; }

        public override SyntaxNode? Definition { get; }

        public override string Name { get; }

        public SyntaxVariableSymbol(SyntaxSymbolKind kind, ModulePath? module, SyntaxNode? definition, string name)
        {
            Kind = kind;
            Module = module;
            Definition = definition;
            Name = name;
        }

        public override string ToString()
        {
            var name = Name;

            if (Module != null)
                name = $"{Module}.{name}";

            return $"Variable({Kind}, {name})";
        }
    }
}
