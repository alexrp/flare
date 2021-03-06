using System.Collections.Immutable;
using Flare.Syntax;

namespace Flare.Metadata
{
    public sealed class External : Declaration
    {
        public bool HasParameters => !Parameters.IsEmpty;

        public ImmutableArray<Parameter> Parameters { get; }

        internal External(Module module, ExternalDeclarationNode node)
            : base(module, node)
        {
            var parms = ImmutableArray<Parameter>.Empty;
            var i = 0;

            foreach (var param in node.ParameterList.Parameters.Nodes)
            {
                parms = parms.Add(new Parameter(this, param.Attributes, param.NameToken.Text, i,
                    param.DotDotToken != null));

                i++;
            }

            Parameters = parms;
        }
    }
}
