using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using Flare.Syntax;
using Flare.Tree;
using Flare.Tree.HighLevel;
using Flare.Tree.HighLevel.Patterns;

namespace Flare.Metadata
{
    sealed class SyntaxTreeLowerer : SyntaxVisitor<TreeReference>
    {
        sealed class PatternLowerer : SyntaxVisitor<TreePattern>
        {
            readonly SyntaxTreeLowerer _parent;

            public PatternLowerer(SyntaxTreeLowerer parent)
            {
                _parent = parent;
            }

            public TreePattern Visit(PatternNode node)
            {
                return Visit(node, null!);
            }

            TreeLocal? CreateAlias(PatternNode node)
            {
                if (!(node.Alias is PatternAliasNode alias))
                    return null;

                var sym = alias.GetAnnotation<SyntaxSymbol>("Symbol");
                var local = _parent._context.CreateLocal(_parent._freezes.Contains(sym), sym.Name);

                _parent.AddVariable(sym, local);

                return local;
            }

            public override TreePattern Visit(IdentifierPatternNode node, TreePattern state)
            {
                var sym = node.GetAnnotation<SyntaxSymbol>("Symbol");
                var local = _parent._context.CreateLocal(_parent._freezes.Contains(sym), sym.Name);

                _parent.AddVariable(sym, local);

                return new TreeIdentifierPattern(CreateAlias(node), local);
            }

            public override TreePattern Visit(LiteralPatternNode node, TreePattern state)
            {
                var value = node.ValueToken.Value;

                if (node.MinusToken != null)
                    value = -(BigInteger)value!;

                return new TreeLiteralPattern(CreateAlias(node), value);
            }

            public override TreePattern Visit(ModulePatternNode node, TreePattern state)
            {
                return new TreeModulePattern(CreateAlias(node), _parent._loader.GetModule(
                    node.GetAnnotation<ModulePath>("Path"))!);
            }

            public override TreePattern Visit(TuplePatternNode node, TreePattern state)
            {
                var comps = ImmutableArray<TreePattern>.Empty;

                foreach (var comp in node.Components.Nodes)
                    comps = comps.Add(Visit(comp));

                return new TreeTuplePattern(CreateAlias(node), comps);
            }

            public override TreePattern Visit(RecordPatternNode node, TreePattern state)
            {
                var fields = ImmutableArray<TreeRecordPatternField>.Empty;

                foreach (var field in node.Fields.Nodes)
                    fields = fields.Add(new TreeRecordPatternField(field.NameToken.Text, Visit(field.Pattern)));

                return new TreeRecordPattern(CreateAlias(node), node.NameToken?.Text, fields);
            }

            public override TreePattern Visit(ExceptionPatternNode node, TreePattern state)
            {
                var fields = ImmutableArray<TreeRecordPatternField>.Empty;

                foreach (var field in node.Fields.Nodes)
                    fields = fields.Add(new TreeRecordPatternField(field.NameToken.Text, Visit(field.Pattern)));

                return new TreeExceptionPattern(CreateAlias(node), node.NameToken.Text, fields);
            }

            public override TreePattern Visit(ArrayPatternNode node, TreePattern state)
            {
                var elems = ImmutableArray<TreePattern>.Empty;

                foreach (var elem in node.Elements.Nodes)
                    elems = elems.Add(Visit(elem));

                return new TreeArrayPattern(CreateAlias(node), elems,
                    node.Remainder is ArrayPatternRemainderNode r ? Visit(r.Pattern) : null);
            }

            public override TreePattern Visit(SetPatternNode node, TreePattern state)
            {
                var elems = ImmutableArray<TreeReference>.Empty;

                foreach (var elem in node.Elements.Nodes)
                    elems = elems.Add(_parent.Visit(elem));

                return new TreeSetPattern(CreateAlias(node), elems);
            }

            public override TreePattern Visit(MapPatternNode node, TreePattern state)
            {
                var pairs = ImmutableArray<TreeMapPatternPair>.Empty;

                foreach (var pair in node.Pairs.Nodes)
                    pairs = pairs.Add(new TreeMapPatternPair(_parent.Visit(pair.Key), Visit(pair.Value)));

                return new TreeMapPattern(CreateAlias(node), pairs);
            }

            protected override TreePattern DefaultVisit(SyntaxNode node, TreePattern state)
            {
                throw DebugAssert.Unreachable();
            }
        }

        readonly ModuleLoader _loader;

        readonly TreeContext _context;

        readonly ExpressionNode _node;

        readonly ImmutableHashSet<SyntaxSymbol> _freezes;

        readonly PatternLowerer _patterns;

        readonly Dictionary<SyntaxSymbol, TreeVariable> _variables = new Dictionary<SyntaxSymbol, TreeVariable>();

        readonly Dictionary<UseStatementNode, TreeLocal> _uses = new Dictionary<UseStatementNode, TreeLocal>();

        readonly Dictionary<PrimaryExpressionNode, TreeReference> _loops =
            new Dictionary<PrimaryExpressionNode, TreeReference>();

        public SyntaxTreeLowerer(ModuleLoader loader, TreeContext context, ExpressionNode node,
            ImmutableHashSet<SyntaxSymbol> freezes)
        {
            _loader = loader;
            _context = context;
            _node = node;
            _freezes = freezes;
            _patterns = new PatternLowerer(this);
        }

        public void AddVariable(SyntaxSymbol symbol, TreeVariable variable)
        {
            _variables.Add(symbol, variable);
        }

        public TreeReference Lower()
        {
            return Visit(_node);
        }

        TreeReference Visit(ExpressionNode node)
        {
            return Visit(node, default);
        }

        public override TreeReference Visit(UnaryExpressionNode node, TreeReference state)
        {
            return new TreeUnaryNode(_context, node.OperatorOrKeywordToken.Location, node.OperatorOrKeywordToken.Text,
                Visit(node.Operand));
        }

        public override TreeReference Visit(AssertExpressionNode node, TreeReference state)
        {
            // TODO: Stringify the operand expression and pass it through here.
            return new TreeAssertNode(_context, node.OperatorOrKeywordToken.Location, Visit(node.Operand),
                string.Empty);
        }

        public override TreeReference Visit(SendExpressionNode node, TreeReference state)
        {
            return new TreeSendNode(_context, node.OperatorToken.Location, Visit(node.LeftOperand),
                Visit(node.RightOperand));
        }

        public override TreeReference Visit(AssignExpressionNode node, TreeReference state)
        {
            return new TreeAssignNode(_context, node.OperatorToken.Location, Visit(node.LeftOperand),
                Visit(node.RightOperand));
        }

        public override TreeReference Visit(LogicalExpressionNode node, TreeReference state)
        {
            var loc = node.OperatorToken.Location;
            var left = Visit(node.LeftOperand);
            var right = Visit(node.RightOperand);

            return node.OperatorToken.Kind switch
            {
                SyntaxTokenKind.AndKeyword => new TreeLogicalAndNode(_context, loc, left, right),
                SyntaxTokenKind.OrKeyword => new TreeLogicalOrNode(_context, loc, left, right),
                _ => throw DebugAssert.Unreachable(),
            };
        }

        public override TreeReference Visit(RelationalExpressionNode node, TreeReference state)
        {
            return new TreeRelationalNode(_context, node.OperatorToken.Location, Visit(node.LeftOperand),
                node.OperatorToken.Kind switch
                {
                    SyntaxTokenKind.EqualsEquals => TreeRelationalOperator.Equal,
                    SyntaxTokenKind.ExclamationEquals => TreeRelationalOperator.NotEqual,
                    SyntaxTokenKind.OpenAngle => TreeRelationalOperator.LessThan,
                    SyntaxTokenKind.OpenAngleEquals => TreeRelationalOperator.LessThanOrEqual,
                    SyntaxTokenKind.CloseAngle => TreeRelationalOperator.GreaterThan,
                    SyntaxTokenKind.CloseAngleEquals => TreeRelationalOperator.GreaterThanOrEqual,
                    _ => throw DebugAssert.Unreachable(),
                }, Visit(node.RightOperand));
        }

        public override TreeReference Visit(BitwiseExpressionNode node, TreeReference state)
        {
            return new TreeBinaryNode(_context, node.OperatorToken.Location, Visit(node.LeftOperand),
                node.OperatorToken.Text, Visit(node.RightOperand));
        }

        public override TreeReference Visit(ShiftExpressionNode node, TreeReference state)
        {
            return new TreeBinaryNode(_context, node.OperatorToken.Location, Visit(node.LeftOperand),
                node.OperatorToken.Text, Visit(node.RightOperand));
        }

        public override TreeReference Visit(AdditiveExpressionNode node, TreeReference state)
        {
            return new TreeBinaryNode(_context, node.OperatorToken.Location, Visit(node.LeftOperand),
                node.OperatorToken.Text, Visit(node.RightOperand));
        }

        public override TreeReference Visit(MultiplicativeExpressionNode node, TreeReference state)
        {
            return new TreeBinaryNode(_context, node.OperatorToken.Location, Visit(node.LeftOperand),
                node.OperatorToken.Text, Visit(node.RightOperand));
        }

        public override TreeReference Visit(CallExpressionNode node, TreeReference state)
        {
            var args = node.ArgumentList;
            var nodes = ImmutableArray<TreeReference>.Empty;
            var variadic = (TreeReference?)null;

            foreach (var arg in args.Arguments.Nodes)
            {
                if (arg.DotDotToken != null)
                    variadic = Visit(arg.Value);
                else
                    nodes = nodes.Add(Visit(arg.Value));
            }

            var catches = ImmutableArray<TreePatternArm>.Empty;
            var loc = args.OpenParenToken.Location;

            if (node.Try is CallTryNode @try)
            {
                if (!(@try.Catch is CallTryCatchNode @catch))
                {
                    var uses = @try.GetAnnotation<ImmutableArray<UseStatementNode>>("Uses");
                    var local = _context.CreateLocal();
                    var ret = new TreeRaiseNode(_context, loc,
                        new TreeVariableNode(_context, loc, local));

                    TreeReference body;

                    if (!uses.IsEmpty)
                    {
                        var stmts = ImmutableArray<TreeReference>.Empty;

                        foreach (var use in uses)
                            stmts = stmts.Add(new TreeMethodCallNode(_context, loc,
                                new TreeVariableNode(_context, loc, _uses[use]), "__drop__",
                                ImmutableArray<TreeReference>.Empty, null, ImmutableArray<TreePatternArm>.Empty));

                        body = new TreeBlockNode(_context, loc, stmts.Add(ret));
                    }
                    else
                        body = ret;

                    catches = catches.Add(new TreePatternArm(new TreeIdentifierPattern(null, local), null, body));
                }
                else
                    foreach (var arm in @catch.Arms)
                        catches = catches.Add(new TreePatternArm(_patterns.Visit(arm.Pattern),
                            arm.Guard is PatternArmGuardNode g ? Visit(g.Condition) : (TreeReference?)null,
                            Visit(arm.Body)));
            }

            return new TreeCallNode(_context, loc, Visit(node.Subject), nodes, variadic, catches);
        }

        public override TreeReference Visit(MethodCallExpressionNode node, TreeReference state)
        {
            var args = node.ArgumentList;
            var nodes = ImmutableArray<TreeReference>.Empty;
            var variadic = (TreeReference?)null;

            foreach (var arg in args.Arguments.Nodes)
            {
                if (arg.DotDotToken == null)
                    nodes = nodes.Add(Visit(arg.Value));
                else
                    variadic = Visit(arg.Value);
            }

            var catches = ImmutableArray<TreePatternArm>.Empty;
            var loc = args.OpenParenToken.Location;

            if (node.Try is CallTryNode @try)
            {
                if (!(@try.Catch is CallTryCatchNode @catch))
                {
                    var uses = @try.GetAnnotation<ImmutableArray<UseStatementNode>>("Uses");
                    var local = _context.CreateLocal();
                    var ret = new TreeRaiseNode(_context, loc,
                        new TreeVariableNode(_context, loc, local));

                    TreeReference body;

                    if (!uses.IsEmpty)
                    {
                        var stmts = ImmutableArray<TreeReference>.Empty;

                        foreach (var use in uses)
                            stmts = stmts.Add(new TreeMethodCallNode(_context, loc,
                                new TreeVariableNode(_context, loc, _uses[use]), "__drop__",
                                ImmutableArray<TreeReference>.Empty, null, ImmutableArray<TreePatternArm>.Empty));

                        body = new TreeBlockNode(_context, loc, stmts.Add(ret));
                    }
                    else
                        body = ret;

                    catches = catches.Add(new TreePatternArm(new TreeIdentifierPattern(null, local), null, body));
                }
                else
                    foreach (var arm in @catch.Arms)
                        catches = catches.Add(new TreePatternArm(_patterns.Visit(arm.Pattern),
                            arm.Guard is PatternArmGuardNode g ? Visit(g.Condition) : (TreeReference?)null,
                            Visit(arm.Body)));
            }

            return new TreeMethodCallNode(_context, loc, Visit(node.Subject), node.NameToken.Text, nodes, variadic,
                catches);
        }

        public override TreeReference Visit(IndexExpressionNode node, TreeReference state)
        {
            var idxs = node.IndexList;
            var nodes = ImmutableArray<TreeReference>.Empty;
            var variadic = (TreeReference?)null;

            foreach (var idx in idxs.Indices.Nodes)
            {
                var value = Visit(idx.Value);

                if (idx.DotDotToken == null)
                    nodes = nodes.Add(value);
                else
                    variadic = value;
            }

            return new TreeIndexNode(_context, idxs.OpenBracketToken.Location, Visit(node.Subject), nodes, variadic);
        }

        public override TreeReference Visit(FieldAccessExpressionNode node, TreeReference state)
        {
            return new TreeFieldAccessNode(_context, node.DotToken.Location, Visit(node.Subject), node.NameToken.Text);
        }

        public override TreeReference Visit(ParenthesizedExpressionNode node, TreeReference state)
        {
            return new TreeParenthesizedNode(_context, node.OpenParenToken.Location, Visit(node.Expression));
        }

        public override TreeReference Visit(IdentifierExpressionNode node, TreeReference state)
        {
            var sym = node.GetAnnotation<SyntaxSymbol>("Symbol");
            var loc = node.IdentifierToken.Location;

            switch (sym.Kind)
            {
                case SyntaxSymbolKind.Immutable:
                case SyntaxSymbolKind.Mutable:
                    return new TreeVariableNode(_context, loc, _variables[sym]);
            }

            var decl = _loader.GetModule(sym.Module!)!.Declarations.Single(x => x.Name == sym.Name);

            return sym.Kind switch
            {
                SyntaxSymbolKind.Constant => new TreeConstantNode(_context, loc, (Constant)decl),
                SyntaxSymbolKind.Function => new TreeFunctionNode(_context, loc, (Function)decl),
                SyntaxSymbolKind.External => new TreeExternalNode(_context, loc, (External)decl),
                _ => throw DebugAssert.Unreachable(),
            };
        }

        public override TreeReference Visit(LiteralExpressionNode node, TreeReference state)
        {
            return new TreeLiteralNode(_context, node.ValueToken.Location, node.ValueToken.Value);
        }

        public override TreeReference Visit(LambdaExpressionNode node, TreeReference state)
        {
            var parms = ImmutableArray<TreeParameter>.Empty;
            var variadic = (TreeVariadicParameter?)null;
            var syms = ImmutableArray<(SyntaxSymbol, TreeVariable)>.Empty;

            foreach (var param in node.ParameterList.Parameters.Nodes)
            {
                TreeVariable variable;

                if (param.DotDotToken == null)
                {
                    var p = new TreeParameter(param.NameToken.Text);

                    variable = p;
                    parms = parms.Add(p);
                }
                else
                    variable = variadic = new TreeVariadicParameter(param.NameToken.Text);

                syms = syms.Add((param.GetAnnotation<SyntaxSymbol>("Symbol"), variable));
            }

            var upvalues = ImmutableArray<TreeUpvalue>.Empty;

            foreach (var upvalue in node.GetAnnotation<ImmutableArray<SyntaxUpvalueSymbol>>("Upvalues"))
            {
                var variable = new TreeUpvalue(_variables[upvalue.Symbol], upvalue.Slot);

                upvalues = upvalues.Add(variable);
                syms = syms.Add((upvalue, variable));
            }

            var lambda = _context.CreateLambda(parms, variadic, upvalues);
            var lowerer = new SyntaxTreeLowerer(_loader, lambda, node.Body,
                node.GetAnnotation<ImmutableHashSet<SyntaxSymbol>>("Freezes"));

            foreach (var (sym, variable) in syms)
                lowerer.AddVariable(sym, variable);

            lambda.Body = lowerer.Visit(node.Body);

            return new TreeLambdaNode(_context, node.FnKeywordToken.Location, lambda);
        }

        public override TreeReference Visit(ModuleExpressionNode node, TreeReference state)
        {
            return new TreeModuleNode(_context, node.Path.ComponentTokens.Tokens[0].Location,
                _loader.GetModule(node.GetAnnotation<ModulePath>("Path"))!);
        }

        public override TreeReference Visit(RecordExpressionNode node, TreeReference state)
        {
            var fields = ImmutableArray<TreeRecordField>.Empty;

            foreach (var field in node.Fields.Nodes)
                fields = fields.Add(new TreeRecordField(field.NameToken.Text, Visit(field.Value),
                    field.MutKeywordToken != null));

            return new TreeRecordNode(_context, node.RecKeywordToken.Location, node.NameToken?.Text, fields);
        }

        public override TreeReference Visit(ExceptionExpressionNode node, TreeReference state)
        {
            var fields = ImmutableArray<TreeRecordField>.Empty;

            foreach (var field in node.Fields.Nodes)
                fields = fields.Add(new TreeRecordField(field.NameToken.Text, Visit(field.Value),
                    field.MutKeywordToken != null));

            return new TreeExceptionNode(_context, node.ExcKeywordToken.Location, node.NameToken.Text, fields);
        }

        public override TreeReference Visit(TupleExpressionNode node, TreeReference state)
        {
            var comps = ImmutableArray<TreeReference>.Empty;

            foreach (var comp in node.Components.Nodes)
                comps = comps.Add(Visit(comp));

            return new TreeTupleNode(_context, node.OpenParenToken.Location, comps);
        }

        public override TreeReference Visit(ArrayExpressionNode node, TreeReference state)
        {
            var elems = ImmutableArray<TreeReference>.Empty;

            foreach (var elem in node.Elements.Nodes)
                elems = elems.Add(Visit(elem));

            var mut = node.MutKeywordToken;

            return new TreeArrayNode(_context, (mut ?? node.OpenBracketToken).Location, elems, mut != null);
        }

        public override TreeReference Visit(SetExpressionNode node, TreeReference state)
        {
            var elems = ImmutableArray<TreeReference>.Empty;

            foreach (var elem in node.Elements.Nodes)
                elems = elems.Add(Visit(elem));

            var mut = node.MutKeywordToken;

            return new TreeSetNode(_context, (mut ?? node.HashToken).Location, elems, mut != null);
        }

        public override TreeReference Visit(MapExpressionNode node, TreeReference state)
        {
            var pairs = ImmutableArray<TreeMapPair>.Empty;

            foreach (var pair in node.Pairs.Nodes)
                pairs = pairs.Add(new TreeMapPair(Visit(pair.Key), Visit(pair.Value)));

            var mut = node.MutKeywordToken;

            return new TreeMapNode(_context, (mut ?? node.HashToken).Location, pairs, mut != null);
        }

        public override TreeReference Visit(BlockExpressionNode node, TreeReference state)
        {
            var stmts = ImmutableArray<TreeReference>.Empty;
            var uses = ImmutableStack<TreeLocal>.Empty; // Use a stack for free reverse iteration.

            foreach (var stmt in node.Statements)
            {
                TreeReference result;

                switch (stmt)
                {
                    case LetStatementNode let:
                        result = new TreeLetNode(_context, let.LetKeywordToken.Location, _patterns.Visit(let.Pattern),
                            Visit(let.Initializer));
                        break;
                    case UseStatementNode use:
                        var local = _context.CreateLocal();

                        uses = uses.Push(local);
                        _uses.Add(use, local);

                        var loc = use.UseKeywordToken.Location;

                        result = new TreeAssignNode(_context, loc,
                            new TreeVariableNode(_context, loc, local),
                            new TreeLetNode(_context, loc, _patterns.Visit(use.Pattern), Visit(use.Initializer)));
                        break;
                    case ExpressionStatementNode expr:
                        result = Visit(expr.Expression);
                        break;
                    default:
                        throw DebugAssert.Unreachable();
                }

                stmts = stmts.Add(result);
            }

            if (!uses.IsEmpty)
            {
                var result = _context.CreateLocal();
                var last = stmts[stmts.Length - 1].Value;
                var loc = last.Location;

                stmts = stmts.SetItem(stmts.Length - 1, new TreeAssignNode(_context, loc,
                    new TreeVariableNode(_context, loc, result), last));

                foreach (var use in uses)
                    stmts = stmts.Add(new TreeMethodCallNode(_context, loc,
                        new TreeVariableNode(_context, loc, use), "__drop__",
                        ImmutableArray<TreeReference>.Empty, null, ImmutableArray<TreePatternArm>.Empty));

                stmts = stmts.Add(new TreeVariableNode(_context, loc, result));
            }

            return new TreeBlockNode(_context, node.OpenBraceToken.Location, stmts);
        }

        public override TreeReference Visit(IfExpressionNode node, TreeReference state)
        {
            var loc = node.IfKeywordToken.Location;

            return new TreeIfNode(_context, loc, Visit(node.Condition), Visit(node.ThenBody),
                node.Else is IfExpressionElseNode e ? Visit(e.ElseBody) : new TreeLiteralNode(_context, loc, null));
        }

        public override TreeReference Visit(ConditionExpressionNode node, TreeReference state)
        {
            var arms = ImmutableArray<TreeConditionArm>.Empty;

            foreach (var arm in node.Arms)
                arms = arms.Add(new TreeConditionArm(Visit(arm.Condition), Visit(arm.Body)));

            return new TreeConditionNode(_context, node.CondKeywordToken.Location, arms);
        }

        public override TreeReference Visit(MatchExpressionNode node, TreeReference state)
        {
            var arms = ImmutableArray<TreePatternArm>.Empty;

            foreach (var arm in node.Arms)
                arms = arms.Add(new TreePatternArm(_patterns.Visit(arm.Pattern),
                    arm.Guard is PatternArmGuardNode g ? Visit(g.Condition) : (TreeReference?)null, Visit(arm.Body)));

            return new TreeMatchNode(_context, node.MatchKeywordToken.Location, Visit(node.Operand), arms);
        }

        public override TreeReference Visit(ForExpressionNode node, TreeReference state)
        {
            var loop = new TreeForNode(_context, node.ForKeywordToken.Location, _patterns.Visit(node.Pattern),
                Visit(node.Collection), Visit(node.Body));

            _loops.Add(node, loop);

            return loop;
        }

        public override TreeReference Visit(WhileExpressionNode node, TreeReference state)
        {
            var loop = new TreeWhileNode(_context, node.WhileKeywordToken.Location, Visit(node.Condition),
                Visit(node.Body));

            _loops.Add(node, loop);

            return loop;
        }

        public override TreeReference Visit(LoopExpressionNode node, TreeReference state)
        {
            var uses = node.GetAnnotation<ImmutableArray<UseStatementNode>>("Uses");
            var loc = node.LoopKeywordToken.Location;
            var expr = new TreeLoopNode(_context, loc, _loops[node.GetAnnotation<PrimaryExpressionNode>("Target")]);

            if (!uses.IsEmpty)
            {
                var stmts = ImmutableArray<TreeReference>.Empty;

                foreach (var use in uses)
                    stmts = stmts.Add(new TreeMethodCallNode(_context, loc,
                        new TreeVariableNode(_context, loc, _uses[use]), "__drop__",
                        ImmutableArray<TreeReference>.Empty, null, ImmutableArray<TreePatternArm>.Empty));

                return new TreeBlockNode(_context, loc, stmts.Add(expr));
            }

            return expr;
        }

        public override TreeReference Visit(BreakExpressionNode node, TreeReference state)
        {
            var uses = node.GetAnnotation<ImmutableArray<UseStatementNode>>("Uses");
            var loc = node.BreakKeywordToken.Location;
            var expr = new TreeBreakNode(_context, loc, _loops[node.GetAnnotation<PrimaryExpressionNode>("Target")]);

            if (!uses.IsEmpty)
            {
                var stmts = ImmutableArray<TreeReference>.Empty;

                foreach (var use in uses)
                    stmts = stmts.Add(new TreeMethodCallNode(_context, loc,
                        new TreeVariableNode(_context, loc, _uses[use]), "__drop__",
                        ImmutableArray<TreeReference>.Empty, null, ImmutableArray<TreePatternArm>.Empty));

                return new TreeBlockNode(_context, loc, stmts.Add(expr));
            }

            return expr;
        }

        public override TreeReference Visit(ReceiveExpressionNode node, TreeReference state)
        {
            var arms = ImmutableArray<TreePatternArm>.Empty;

            foreach (var arm in node.Arms)
                arms = arms.Add(new TreePatternArm(_patterns.Visit(arm.Pattern),
                    arm.Guard is PatternArmGuardNode g ? Visit(g.Condition) : (TreeReference?)null, Visit(arm.Body)));

            return new TreeReceiveNode(_context, node.RecvKeywordToken.Location, arms,
                node.Else is ReceiveExpressionElseNode e ? Visit(e.ElseBody) : (TreeReference?)null);
        }

        public override TreeReference Visit(RaiseExpressionNode node, TreeReference state)
        {
            var uses = node.GetAnnotation<ImmutableArray<UseStatementNode>>("Uses");
            var loc = node.RaiseKeywordToken.Location;
            var expr = new TreeRaiseNode(_context, loc, Visit(node.Operand));

            if (!uses.IsEmpty)
            {
                var stmts = ImmutableArray<TreeReference>.Empty;

                foreach (var use in uses)
                    stmts = stmts.Add(new TreeMethodCallNode(_context, loc,
                        new TreeVariableNode(_context, loc, _uses[use]), "__drop__",
                        ImmutableArray<TreeReference>.Empty, null, ImmutableArray<TreePatternArm>.Empty));

                return new TreeBlockNode(_context, loc, stmts.Add(expr));
            }

            return expr;
        }

        public override TreeReference Visit(FreezeExpressionNode node, TreeReference state)
        {
            return new TreeFreezeNode(_context, node.FreezeKeywordToken.Location, Visit(node.Operand),
                node.InKeywordToken != null);
        }

        public override TreeReference Visit(ReturnExpressionNode node, TreeReference state)
        {
            var uses = node.GetAnnotation<ImmutableArray<UseStatementNode>>("Uses");
            var loc = node.ReturnKeywordToken.Location;
            var expr = new TreeReturnNode(_context, loc, Visit(node.Operand));

            if (!uses.IsEmpty)
            {
                var stmts = ImmutableArray<TreeReference>.Empty;

                foreach (var use in uses)
                    stmts = stmts.Add(new TreeMethodCallNode(_context, loc,
                        new TreeVariableNode(_context, loc, _uses[use]), "__drop__",
                        ImmutableArray<TreeReference>.Empty, null, ImmutableArray<TreePatternArm>.Empty));

                return new TreeBlockNode(_context, loc, stmts.Add(expr));
            }

            return expr;
        }

        protected override TreeReference DefaultVisit(SyntaxNode node, TreeReference state)
        {
            throw DebugAssert.Unreachable();
        }
    }
}
