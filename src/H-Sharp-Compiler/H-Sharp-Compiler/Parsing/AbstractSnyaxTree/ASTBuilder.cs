using System;
using System.Collections.Generic;
using System.Linq;
using HSharp.IO;
using HSharp.Language;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;
using HSharp.Parsing.AbstractSnyaxTree.Directive;
using HSharp.Parsing.AbstractSnyaxTree.Expression;
using HSharp.Parsing.AbstractSnyaxTree.Literal;
using HSharp.Parsing.AbstractSnyaxTree.Statement;
using HSharp.Util;
using HSharp.Util.Functional;

namespace HSharp.Parsing.AbstractSnyaxTree {

    public class ASTBuilder {

        delegate bool GrammarRule(List<ASTNode> nodes, ref int from);

        LexToken[] m_tokens;
        List<ASTNode> m_topNodes;

        public ASTBuilder(LexToken[] tokens) {
            this.m_tokens = tokens;
        }

        public ParsingResult Parse() {

            // Parse all top-level nodes (As best as possible)
            this.m_topNodes = this.ParseTopLevel();

            // Apply groupings
            this.Safe(false, () => this.ApplyGroupings(this.m_topNodes), e => { });

            // Apply order of operations
            this.Safe(false, () => this.ApplyOrderOfOperations(this.m_topNodes), e => { });

            // Apply grammar rules
            this.Safe(false, () => this.ApplyGrammar(this.m_topNodes), e => { });

            // Return true parse result
            return new ParsingResult(true);

        }

        private void ApplyGroupings(List<ASTNode> nodes) {

            /*
             * 1. { }
             * 2. [ ]
             * 3. ( )
             */

            int i = 0;

            // Apply block/scope groupings
            this.ApplyGroup<ScopeNode>(nodes, i, LexTokenType.BlockStart, LexTokenType.BlockEnd);
            i = 0;

            // Apply Indexer groupings
            this.ApplyGroup<IndexerNode>(nodes, i, LexTokenType.IndexerStart, LexTokenType.IndexerEnd);
            i = 0;

            // Apply expression groupings
            this.ApplyGroup<ExpressionNode>(nodes, i, LexTokenType.ExpressionStart, LexTokenType.ExpressionEnd);

        }

        private void ApplyGroup<T>(List<ASTNode> nodes, int i, LexTokenType open, LexTokenType close) where T : IGroupedASTNode {

            while (i < nodes.Count) {

                if (nodes[i].LexicalType == open) {

                    List<ASTNode> sub = new List<ASTNode>();
                    int j = i + 1;
                    while(nodes[j].LexicalType != close) {
                        if (nodes[j].LexicalType == open) {
                            int k = j;
                            this.ApplyGroup<T>(nodes, k, open, close);
                            sub.Add(nodes[j]);
                            j++;
                        } else {
                            sub.Add(nodes[j]);
                            j++;
                        }
                    }

                    T groupNode = (T)Activator.CreateInstance(typeof(T), nodes[i].Pos);
                    groupNode.SetNodes(sub);

                    nodes.RemoveRange(i + 1, sub.Count + 1);
                    nodes[i] = groupNode as ASTNode;

                } else if (nodes[i] is IGroupedASTNode group) {
                    int j = 0;
                    this.ApplyGroup<T>(group.Nodes, j, open, close);
                }

                i++;

            }

        }

        private void ApplyOrderOfOperations(List<ASTNode> nodes) { // AKA operator presedence

            Type binOp = typeof(BinOpNode);
            Type unOp = typeof(UnaryOpNode);
            Type memAccessOp = typeof(MemberAccessNode);
            (Type, string)[][] opTypes = new (Type, string)[][] {
                new (Type, string)[] { (memAccessOp, ".") },
                new (Type, string)[] { (binOp, "*"), (binOp, "/") },
                new (Type, string)[] { (binOp, "&") },
                new (Type, string)[] { (unOp, "++"), (unOp, "--") },
                new (Type, string)[] { (binOp, "+"), (binOp, "-") },
                new (Type, string)[] { (binOp, "=") }
            }; // TODO: Add custom OperatorBehaviour type to better handle syntax errors etc...

            for (int i = 0; i < opTypes.Length; i++) {

                var ops = opTypes[i];

                int j = 0;
                while (j < nodes.Count) {
                    if (nodes[j].LexicalType == LexTokenType.Operator && ops.Any(x => x.Item2 == nodes[j].Content)) {
                        var m = ops.First(x => x.Item2 == nodes[j].Content);
                        bool pre = j - 1 >= 0;
                        bool post = j + 1 < nodes.Count;
                        if (m.Item1 == binOp && pre && post) {
                            BinOpNode binaryOperation = new BinOpNode(nodes[j].Pos, nodes[j - 1], nodes[j].Content, nodes[j + 1]);
                            this.ApplyOrderOfOperationsBinary(binaryOperation);
                            nodes[j - 1] = binaryOperation;
                            nodes.RemoveAt(j + 1);
                            nodes.RemoveAt(j);
                            continue;
                        } else if (m.Item1 == unOp) {
                            throw new NotImplementedException();
                            //continue;
                        } else if (m.Item1 == memAccessOp && pre && post && nodes[j - 1].Is<IdentifierNode>().Is<ThisNode>().Or<MemberAccessNode>() && nodes[j+1] is IdentifierNode accessId) {
                            IExpr accessExpr = nodes[j - 1] as IExpr;
                            nodes[j - 1] = new MemberAccessNode(accessExpr, accessId, m.Item2, nodes[j].Pos);
                            nodes.RemoveAt(j + 1);
                            nodes.RemoveAt(j);
                            continue;
                        } else {
                            throw new NotImplementedException();
                        }
                    }
                    j++;
                }

            }

            // Run recursively
            for (int i = 0; i < nodes.Count; i++) {
                if (nodes[i] is IGroupedASTNode groupedNode) {
                    this.ApplyOrderOfOperations(groupedNode.Nodes);
                }
            }

        }

        private void ApplyOrderOfOperationsBinary(BinOpNode binOpNode) {
            if (binOpNode.Left is IGroupedASTNode leftGroup) {
                this.ApplyOrderOfOperations(leftGroup.Nodes);
            }
            if (binOpNode.Right is IGroupedASTNode rightGroup) {
                this.ApplyOrderOfOperations(rightGroup.Nodes);
            }
        }

        private void ApplyGrammar(List<ASTNode> nodes, int from = 0, params GrammarRule[] additionalPatterns) {

            int i = from;
            while (i < nodes.Count) {

                if (additionalPatterns != null) {
                    bool skipStandard = false;
                    for (int j = 0; j < additionalPatterns.Length; j++) {
                        if (additionalPatterns[j](nodes, ref i)) {
                            skipStandard = true;
                            break;
                        }
                    }
                    if (skipStandard) {
                        i++;
                        continue;
                    }
                }

                if (nodes[i].LexicalType == LexTokenType.Keyword) {
                    switch (nodes[i].Content) {
                        case "class":
                            this.ApplyClassGrammar(nodes, ref i);
                            break;
                        case "return":
                            this.ApplyReturnStatementGrammar(nodes, i);
                            break;
                        case "public":
                        case "private":
                        case "protected":
                        case "external":
                        case "internal":
                            nodes[i] = new AccessModifierNode(nodes[i].Content, nodes[i].Pos);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                } else if (TypeSequence<ASTNode, IdentifierNode, IdentifierNode, SeperatorNode>.Match(nodes, i)) { // int x;
                    
                    this.ApplyModifierGrammar(nodes, ref i, out AccessModifierNode accessModifier, out HashSet<StorageModifierNode> storageModifiers);
                    
                    nodes[i] = new VarDeclNode(nodes[i].Pos, nodes[i], nodes[i+1].Content);
                    this.ApplyModifiers(nodes[i] as VarDeclNode, accessModifier, storageModifiers);
                    this.RemoveNode(nodes, i + 1, 2);

                } else if (TypeSequence<ASTNode, IdentifierNode, BinOpNode, SeperatorNode>.Match(nodes, i)) { // int x = <expr>;
                    
                    this.ApplyModifierGrammar(nodes, ref i, out AccessModifierNode accessModifier, out HashSet<StorageModifierNode> storageModifiers);
                    
                    BinOpNode assignOp = nodes[i + 1] as BinOpNode;
                    if (assignOp.Op.CompareTo("=") == 0) {
                        this.ApplyGrammarBinary(assignOp);
                        nodes[i] = new VarDeclNode(nodes[i].Pos, nodes[i], assignOp);
                        this.ApplyModifiers(nodes[i] as VarDeclNode, accessModifier, storageModifiers);
                        this.RemoveNode(nodes, i + 1, 2);
                    }

                } else if (TypeSequence<ASTNode, IdentifierNode, BinOpNode, IdentifierNode, ExpressionNode, SeperatorNode>.Match(nodes, i)) {
                    
                    this.ApplyModifierGrammar(nodes, ref i, out AccessModifierNode accessModifier, out HashSet<StorageModifierNode> storageModifiers);
                    
                    BinOpNode assignOp = nodes[i + 1] as BinOpNode;
                    if (assignOp.Right.LexicalType == LexTokenType.Keyword && assignOp.Right.Is("new")) { // Klass k = new Klass();
                        NewObjectNode newObjectNode = new NewObjectNode(new TypeIdentifierNode(nodes[i + 2] as IdentifierNode), nodes[i + 3] as ExpressionNode, assignOp.Right.Pos);
                        nodes[i] = new VarDeclNode(nodes[i].Pos, nodes[i], new BinOpNode(assignOp.Pos, assignOp.Left, "=", newObjectNode));
                        this.ApplyModifiers(nodes[i] as VarDeclNode, accessModifier, storageModifiers);
                        this.RemoveNode(nodes, i + 1, 4);
                    } // else error?

                } else if (TypeSequence<ASTNode, IdentifierNode, ExpressionNode, ASTNode, IdentifierNode, ScopeNode>.Match(nodes, i)) {
                    if (nodes[i + 2].Content.CompareTo(":") == 0) {
                        this.ApplyFunctionGrammar(nodes, ref i);
                    }
                } else if (TypeSequence<ASTNode, IExpr, ExpressionNode>.Match(nodes, i)) {
                    ExpressionNode groupNode = nodes[i + 1] as ExpressionNode;
                    this.ApplyGrammar(groupNode.Nodes); // apply grammar on arguments ==> should lead to a nice arg1,arg2,arg3 setup (otherwise error)
                    CallNode callNode = new CallNode(nodes[i], nodes[i].Pos) {
                        Arguments = new ArgumentsNode(groupNode) // Note: This constructor will apply the grammar rule on its own
                    };
                    if (!callNode.Arguments.IsValid) {
                        throw new Exception();
                    }
                    this.RemoveNode(nodes, i + 1);
                    nodes[i] = callNode;
                } else if (nodes[i] is IGroupedASTNode groupNode) {
                    this.ApplyGrammar(groupNode.Nodes);
                }

                if (nodes[i] is IExpr && i + 1 < nodes.Count && nodes[i+1] is SeperatorNode sepNode && sepNode.Content.CompareTo(";") == 0) {
                    this.RemoveNode(nodes, i + 1);
                } else if (nodes[i] is IExpr && i + 1 >= nodes.Count) {
                    throw new SyntaxError(100, nodes[i].Pos, $"Expected ';' found <EOL>");
                }

                i++;

            }

        }

        private void ApplyGrammarBinary(BinOpNode binop) {
            if (binop.Left is IGroupedASTNode leftGroup) {
                this.ApplyGrammar(leftGroup.Nodes);
            }
            if (binop.Right is IGroupedASTNode rightGroup) {
                this.ApplyGrammar(rightGroup.Nodes);
            }
        }

        private void ApplyModifiers<T>(T node, AccessModifierNode accessModifier, HashSet<StorageModifierNode> storageModifiers) where T : IStorageModifiable, IAccessModifiable {
            node.SetAccessModifier(accessModifier.Modifier);
            storageModifiers.Select(x => x.Modifier).ToList().ForEach(node.AddStorageModifier);
        }

        private void ApplyModifierGrammar(List<ASTNode> nodes, ref int from, out AccessModifierNode accessModifier, out HashSet<StorageModifierNode> storageModifiers) {

            accessModifier = new AccessModifierNode("default", nodes[from].Pos);
            storageModifiers = new HashSet<StorageModifierNode>();
            int fromNeg = from - 1;
            while (fromNeg >= 0) {
                if (nodes[fromNeg] is AccessModifierNode accMod) {
                    if (accessModifier.Modifier != AccessModifier.Default) {
                        throw new SyntaxError(-1, accMod.Pos, "Unexpected access modifier"); // Semantic error might be more proper?
                    } else {
                        accessModifier = accMod;
                    }
                    this.RemoveNode(nodes, fromNeg);
                    from--;
                    continue;
                } else if (nodes[fromNeg] is StorageModifierNode stoMod) {
                    if (!storageModifiers.Add(stoMod)) {
                        throw new SyntaxError(-1, stoMod.Pos, "Unexpected storage modifier, already exists"); // Semantic error might be more proper?
                    }
                    this.RemoveNode(nodes, fromNeg);
                    from--;
                    continue;
                } else {
                    break;
                }
            }

        }

        private bool IsInheritanceSequence(List<ASTNode> nodes, int from, out List<ASTNode> inheritance) {
            if (TypeSequenceUntil<ASTNode, ASTNode, IdentifierNode, ASTNode, ScopeNode>.Match(nodes, from, out inheritance, new Predicate<ASTNode>[] {
                null,
                null,
                x => x.LexicalType == LexTokenType.Operator,
            })) {
                return true;
            } else if (TypeSequenceUntil<ASTNode, ASTNode, IdentifierNode, ASTNode, SeperatorNode>.Match(nodes, from, out inheritance, new Predicate<ASTNode>[] {
                null,
                null,
                x => x.LexicalType == LexTokenType.Operator,
            })) {
                return true;
            } else if (TypeSequenceUntil<ASTNode, ASTNode, IdentifierNode, ExpressionNode, ASTNode, SeperatorNode>.Match(nodes, from, out inheritance, new Predicate<ASTNode>[] {
                null,
                null,
                null,
                x => x.LexicalType == LexTokenType.Operator,
            })) {
                return true;
            } else {
                return false;
            }
        }

        private ClassInheritanceDeclNode ApplyInheritanceGrammar(SourcePosition pos, List<ASTNode> nodes) {
            ClassInheritanceDeclNode inheritNode = new ClassInheritanceDeclNode(pos);
            for (int i = 0; i < nodes.Count; i++) {
                if (nodes[i] is IdentifierNode baseIdentifier && i + 1 == nodes.Count) {
                    inheritNode.AddType(baseIdentifier);
                } else if (TypeSequence<ASTNode, IdentifierNode, ExpressionNode>.Match(nodes, i) && i +2 == nodes.Count) {
                    ArgumentsNode typeCtorArgs = new ArgumentsNode(nodes[i + 1] as ExpressionNode);
                    if (!typeCtorArgs.IsValid) {
                        throw new SyntaxError(-1, nodes[i + 1].Pos, string.Empty);
                    }
                    inheritNode.AddType(new TypeCtorNode(new TypeIdentifierNode(nodes[i] as IdentifierNode), typeCtorArgs));
                    i++;
                } else if (TypeSequence<ASTNode, IdentifierNode, ExpressionNode, ASTNode>.Match(nodes, i)) {
                    throw new NotImplementedException();
                } /* other cases*/ else {
                    throw new SyntaxError(-1, pos, string.Empty);
                }
            }
            return inheritNode;
        }

        private ClassInheritanceDeclNode ApplyInheritanceGrammar(List<ASTNode> nodes, int from) {

            // Create inheritance
            ClassInheritanceDeclNode inheritanceDeclaration = null;

            // Is inheritance sequence?
            if (this.IsInheritanceSequence(nodes, from, out List<ASTNode> inheritance)) { // TODO: Consider generics

                // Remove offset
                int offset = 2;

                // Increase offset if there's an expression node at from + 2 (and not the ':' char)
                if (nodes[from + 2] is ExpressionNode) {
                    offset++;
                }

                // Apply inheritance grammar
                inheritanceDeclaration = this.ApplyInheritanceGrammar(nodes[from + offset].Pos, inheritance);

                // Remove inheritance nodes from array
                this.RemoveNode(nodes, from + offset, inheritance.Count + 1);

            }

            // Return declaration
            return inheritanceDeclaration;

        }

        private ClassDeclNode CreateStandardClassDecl(List<ASTNode> nodes, int from, 
            out string identifierName,
            AccessModifierNode accessModifier, 
            HashSet<StorageModifierNode> storageModifiers, 
            ClassInheritanceDeclNode inheritanceDeclaration) {

            // Create class decl
            ClassDeclNode classDecl = new ClassDeclNode(nodes[from + 1].Content, nodes[from].Pos);
            identifierName = classDecl.LocalClassName;

            // Apply modifiers
            this.ApplyModifiers(classDecl, accessModifier, storageModifiers);

            // Add inheritance if any
            if (inheritanceDeclaration is not null) {
                classDecl.Inheritance = inheritanceDeclaration;
            }

            // Return it
            return classDecl;

        }

        private void ApplyClassGrammar(List<ASTNode> nodes, ref int from) {

            string identifierName = string.Empty;

            bool ClassCtorPattern(List<ASTNode> nodes, ref int from) {
                if (TypeSequence<ASTNode, IdentifierNode, ExpressionNode, ScopeNode>.Match(nodes, from)) {
                    if (nodes[from].Content.CompareTo(identifierName) == 0) {
                        this.ApplyModifierGrammar(nodes, ref from, out AccessModifierNode accessModifier, out HashSet<StorageModifierNode> storageModifiers);
                        ClassCtorDecl ctor = new ClassCtorDecl(identifierName, nodes[from].Pos) {
                            Params = new ParamsNode(nodes[from + 1] as ExpressionNode),
                            Body = nodes[from + 2] as ScopeNode
                        };
                        if (!ctor.Params.IsValid) {
                            throw new Exception();
                        }
                        
                        // Apply modifiers
                        this.ApplyModifiers(ctor, accessModifier, storageModifiers);

                        this.ApplyGrammar(ctor.Body.Nodes);
                        
                        nodes[from] = ctor;
                        
                        this.RemoveNode(nodes, from + 1, 2);

                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return false;
                }
            }

            this.ApplyModifierGrammar(nodes, ref from, out AccessModifierNode accessModifier, out HashSet<StorageModifierNode> storageModifiers);

            // TODO: Parse generics here

            // Create inheritance
            ClassInheritanceDeclNode inheritanceDeclaration = this.ApplyInheritanceGrammar(nodes, from);

            ClassDeclNode classDecl = null;
            int removeStart = from + 1;
            int removeEnd = 2;

            if (TypeSequence<ASTNode, ASTNode, IdentifierNode, ScopeNode>.Match(nodes, from)) {

                classDecl = this.CreateStandardClassDecl(nodes, from, out identifierName, accessModifier, storageModifiers, inheritanceDeclaration);

                ScopeNode classBody = nodes[from + 2] as ScopeNode;

                this.ApplyGrammar(classBody.Nodes, 0, ClassCtorPattern);

                for (int i = 0; i < classBody.Size; i++) {

                    if (classBody[i] is VarDeclNode field) {
                        classDecl.Fields.Add(field);
                    } else if (classBody[i] is ClassDeclNode subclass) {
                        classDecl.Classes.Add(subclass);
                    } else if (classBody[i] is FuncDeclNode func) {
                        classDecl.Methods.Add(func);
                    } else {
                        throw new SyntaxError(-1, nodes[from].Pos, string.Empty);
                    }

                }
                
                removeStart = from + 1;
                removeEnd = 2;

            } else if (TypeSequence<ASTNode, ASTNode, IdentifierNode, SeperatorNode>.Match(nodes, from)) {

                classDecl = this.CreateStandardClassDecl(nodes, from, out identifierName, accessModifier, storageModifiers, inheritanceDeclaration);

                removeStart = from + 1;
                removeEnd = 3;

            } else if (TypeSequence<ASTNode, ASTNode, IdentifierNode, ExpressionNode, SeperatorNode>.Match(nodes, from)) {

                // Create class declaration node and parse ParamsNode
                classDecl = this.CreateStandardClassDecl(nodes, from, out identifierName, accessModifier, storageModifiers, inheritanceDeclaration);
                ParamsNode classParams = new ParamsNode(nodes[from + 2] as ExpressionNode);

                // Throw error if params were invalid
                if (!classParams.IsValid) {
                    throw new SyntaxError(-1, classParams.Pos, string.Empty);
                }

                // Loop through paremeters and register them as fields
                foreach (ParamsNode.ParameterNode p in classParams.Parameters) {
                    // do check if property and such...
                    classDecl.Fields.Add(new VarDeclNode(p.Pos, p.Type, p.Identifier.Content));
                }

                // Set remove parameters
                removeStart = from + 1;
                removeEnd = 3;

            } else {

                throw new SyntaxError(-1, nodes[from].Pos, "");

            }

            nodes[from] = classDecl;
            this.RemoveNode(nodes, removeStart, removeEnd);

        }

        private void ApplyFunctionGrammar(List<ASTNode> nodes, ref int from) {

            // Read modifiers
            this.ApplyModifierGrammar(nodes, ref from, out AccessModifierNode accessModifier, out HashSet<StorageModifierNode> storageModifiers);

            // Create function declaration
            FuncDeclNode funcDecl = new FuncDeclNode(nodes[from].Content, nodes[from].Pos) {
                Body = nodes[from + 4] as ScopeNode,
                Return = nodes[from + 3],
                Params = new ParamsNode(nodes[from + 1] as ExpressionNode) // Note: This constructor will apply the grammar rule on its own
            };

            // Apply function modifiers
            this.ApplyModifiers(funcDecl, accessModifier, storageModifiers);

            if (!funcDecl.Params.IsValid) {
                throw new Exception();
            }

            this.ApplyGrammar(funcDecl.Body.Nodes);
            this.RemoveNode(nodes, from + 1, 4);
            nodes[from] = funcDecl;

        }

        private void ApplyReturnStatementGrammar(List<ASTNode> nodes, int from) {

            // List of elements "sandwiched" between return keyword and ';'
            List<ASTNode> sandwichedElements = new List<ASTNode>();

            // Loop until we find a seperator ';'
            int i = from + 1;
            while(!(nodes[i] is SeperatorNode && nodes[i].Is(";"))) {
                sandwichedElements.Add(nodes[i]);
                i++;
            }

            // Add seperator (Needed so we don't get syntax error)
            sandwichedElements.Add(nodes[i]);

            // Remove sandwiched elements
            this.RemoveNode(nodes, from + 1, sandwichedElements.Count);

            // Apply grammar on expression
            this.ApplyGrammar(sandwichedElements);

            // Set return statement (if possible - otherwise error)
            if (sandwichedElements.Count == 1 && sandwichedElements[0] is IExpr expr) {
                nodes[from] = new ReturnStatement(expr, nodes[0].Pos);
            } else {
                throw new Exception();
            }

        }

        private List<ASTNode> ParseTopLevel() {

            List<ASTNode> nodes = new List<ASTNode>();

            for (int i = 0; i < this.m_tokens.Length; i++) {

                switch (this.m_tokens[i].Type) {
                    case LexTokenType.Identifier:
                        nodes.Add(new IdentifierNode(this.m_tokens[i].Content, this.m_tokens[i].Position));
                        break;
                    case LexTokenType.IntLiteral:
                        nodes.Add(new IntLitNode(this.m_tokens[i].Content, this.m_tokens[i].Position));
                        break;
                    case LexTokenType.Separator:
                        nodes.Add(new SeperatorNode(this.m_tokens[i].Content, this.m_tokens[i].Position));
                        break;
                    case LexTokenType.Keyword:
                        switch (this.m_tokens[i].Content) {
                            case "this":
                                nodes.Add(new ThisNode(this.m_tokens[i].Position));
                                break;
                            default:
                                nodes.Add(new ASTNode(this.m_tokens[i].Position, this.m_tokens[i].Content, this.m_tokens[i].Type));
                                break;
                        }
                        break;
                    // More easy-to-solve cases here
                    default:
                        nodes.Add(new ASTNode(this.m_tokens[i].Position, this.m_tokens[i].Content, this.m_tokens[i].Type));
                        break;
                }

            }

            return nodes;

        }

        private void RemoveNode(List<ASTNode> nodes, int from, int count = 1) {
#if DEBUG
            List<ASTNode> cpy = new List<ASTNode>();
            cpy.AddRange(nodes);
#endif
            nodes.RemoveRange(from, count);
            return;
        }

        private void Safe(bool enable, Action doAction, Action<Exception> handler) {
            if (enable) {
                try { doAction.Invoke(); }
                catch (Exception e) { handler.Invoke(e); }
            } else {
                doAction.Invoke();
            }
        }

        public AST Build() => new AST(new CompileUnitNode(this.m_topNodes));

    }

}
