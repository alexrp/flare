using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Flare.Metadata;
using Flare.Syntax;
using Flare.Tree.HighLevel;

namespace Flare.Tree
{
    sealed class TreeContext
    {
        public ModuleLoader Loader { get; }

        public Declaration? Declaration { get; }

        public ImmutableArray<TreeParameter> Parameters { get; }

        public TreeVariadicParameter? VariadicParameter { get; }

        public ImmutableArray<TreeUpvalue> Upvalues { get; }

        public TreeReference? Body { get; set; }

        public IReadOnlyList<TreeLocal> Locals => _locals;

        public IReadOnlyList<TreeContext> Lambdas => _lambdas;

        readonly List<TreeLocal> _locals = new List<TreeLocal>();

        int _nextLocal;

        readonly List<TreeContext> _lambdas = new List<TreeContext>();

        readonly Dictionary<(string?, string), External> _intrinsics = new Dictionary<(string?, string), External>();

        TreeNode[] _nodes = new TreeNode[1024];

        int _nextNode;

        public TreeContext(ModuleLoader loader, Declaration? declaration, ImmutableArray<TreeParameter> parameters,
            TreeVariadicParameter? variadicParameter, ImmutableArray<TreeUpvalue> upvalues)
        {
            Loader = loader;
            Declaration = declaration;
            Parameters = parameters;
            VariadicParameter = variadicParameter;
            Upvalues = upvalues;
        }

        public TreeLocal CreateLocal(bool @checked = false, string? name = null)
        {
            var local = new TreeLocal(_nextLocal++, name, @checked);

            _locals.Add(local);

            return local;
        }

        public TreeContext CreateLambda(ImmutableArray<TreeParameter> parameters,
            TreeVariadicParameter? variadicParameter, ImmutableArray<TreeUpvalue> upvalues)
        {
            var lambda = new TreeContext(Loader, null, parameters, variadicParameter, upvalues);

            _lambdas.Add(lambda);

            return lambda;
        }

        public TreeCallNode CreateIntrinsicCall(SourceLocation location, string? module, string name,
            params TreeReference[] arguments)
        {
            var key = (module, name);

            if (!_intrinsics.TryGetValue(key, out var ext))
            {
                ext = Loader.GetModule(new ModulePath(module != null ? new[] { ModulePath.CoreModuleName, module } :
                    new[] { ModulePath.CoreModuleName }))!.Declarations.OfType<External>().Single(x => x.Name == name);

                _intrinsics.Add(key, ext);
            }

            return new TreeCallNode(this, location,
                new TreeExternalNode(this, location, ext), arguments.ToImmutableArray(), null,
                ImmutableArray<TreePatternArm>.Empty);
        }

        public TreeReference RegisterNode(TreeNode node)
        {
            var id = _nextNode++;
            var r = new TreeReference(this, id);

            if (id >= _nodes.Length)
                Array.Resize(ref _nodes, _nodes.Length * 2);

            ReplaceNode(id, node);

            return r;
        }

        public TreeNode ResolveNode(int id)
        {
            return _nodes[id];
        }

        public void ReplaceNode(int id, TreeNode node)
        {
            _nodes[id] = node;
        }

        public void ToString(IndentedTextWriter writer)
        {
            writer.Write(Declaration switch
            {
                Constant _ => "const",
                Function _ => "fn",
                Test _ => "test",
                null => "lambda",
                _ => throw DebugAssert.Unreachable(),
            });

            if (Declaration != null)
                writer.Write(" {0}", Declaration);

            writer.WriteLine(":");
            writer.Indent++;

            writer.WriteLine("Parameters:");
            writer.Indent++;

            foreach (var param in Parameters)
                writer.WriteLine(param);

            if (VariadicParameter != null)
                writer.WriteLine("{0} (Variadic)", VariadicParameter);

            writer.Indent--;
            writer.WriteLine();

            writer.WriteLine("Locals:");
            writer.Indent++;

            foreach (var local in _locals)
            {
                writer.Write("{0}:", local);

                if (local.Name != null)
                    writer.Write(" {0}", local.Name);

                if (local.IsChecked)
                    writer.Write(" (Checked)");

                writer.WriteLine();
            }

            writer.Indent--;
            writer.WriteLine();

            writer.WriteLine("Code:");
            writer.Indent++;

            if (Body is TreeReference body)
                body.ToString(writer);

            writer.Indent--;
            writer.WriteLine();

            writer.Indent--;
        }

        public override string ToString()
        {
            using var sw = new StringWriter();
            using var itw = new IndentedTextWriter(sw);

            ToString(itw);

            return sw.ToString();
        }
    }
}
