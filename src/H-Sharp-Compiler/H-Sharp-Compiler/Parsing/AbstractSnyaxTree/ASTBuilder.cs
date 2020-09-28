using System;
using System.Collections.Generic;
using System.Linq;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;
using HSharp.Parsing.AbstractSnyaxTree.Expression;
using HSharp.Parsing.AbstractSnyaxTree.Literal;
using HSharp.Util;

namespace HSharp.Parsing.AbstractSnyaxTree {

    public class ASTBuilder {

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

        private void ApplyOrderOfOperations(List<ASTNode> nodes) {

            Type binOp = typeof(BinOpNode);
            Type unOp = typeof(UnaryOpNode);
            (Type, string)[][] opTypes = new (Type, string)[][] {
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
                        if (m.Item1 == binOp && j - 1 >= 0 && j + 1 < nodes.Count) {
                            BinOpNode binaryOperation = new BinOpNode(nodes[j].Pos, nodes[j - 1], nodes[j].Content, nodes[j + 1]);
                            this.ApplyOrderOfOperationsBinary(binaryOperation);
                            nodes[j - 1] = binaryOperation;
                            nodes.RemoveAt(j + 1);
                            nodes.RemoveAt(j);
                            continue;
                        } else if (m.Item1 == unOp) {
                            throw new NotImplementedException();
                            //continue;
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

        private void ApplyGrammar(List<ASTNode> nodes) {

            int i = 0;
            while (i < nodes.Count) {

                if (nodes[i].LexicalType == LexTokenType.Keyword) {
                    if (nodes[i].Content.CompareTo("class") == 0) {
                        this.ApplyClassGrammar(nodes, i);
                    }
                } else if (TypeSequence<ASTNode, IdentifierNode, IdentifierNode, SeperatorNode>.Match(nodes, i)) {
                    nodes[i] = new VarDeclNode(nodes[i].Pos, nodes[i], nodes[i+1].Content);
                    nodes.RemoveRange(i + 1, 2);
                } else if (TypeSequence<ASTNode, IdentifierNode, BinOpNode, SeperatorNode>.Match(nodes, i)) {
                    BinOpNode assignOp = nodes[i + 1] as BinOpNode;
                    if (assignOp.Op.CompareTo("=") == 0) {
                        this.ApplyGrammarBinary(assignOp);
                        nodes[i] = new VarDeclNode(nodes[i].Pos, nodes[i], assignOp);
                        nodes.RemoveRange(i + 1, 2);
                    }
                } else if (TypeSequence<ASTNode, IdentifierNode, ExpressionNode, ASTNode, IdentifierNode, ScopeNode>.Match(nodes, i)) {
                    if (nodes[i + 2].Content.CompareTo(":") == 0) {
                        FuncDeclNode funcDecl = new FuncDeclNode(nodes[i].Content, nodes[i].Pos) {
                            Body = nodes[i + 4] as ScopeNode,
                            Return = nodes[i + 3],
                            Params = new ParamsNode(nodes[i + 1] as ExpressionNode) // Note: This constructor will apply the grammar rule on its own
                            // Possibly not the best design ^^
                        };
                        if (!funcDecl.Params.IsValid) {
                            throw new Exception();
                        }
                        this.ApplyGrammar(funcDecl.Body.Nodes);
                        nodes.RemoveRange(i + 1, 4);
                        nodes[i] = funcDecl;
                    }
                } else if (TypeSequence<ASTNode, IdentifierNode, ExpressionNode>.Match(nodes, i)) {
                    ExpressionNode groupNode = nodes[i + 1] as ExpressionNode;
                    this.ApplyGrammar(groupNode.Nodes);
                    CallNode callNode = new CallNode(nodes[i], nodes[i].Pos) {
                        Arguments = new ArgumentsNode(groupNode) // Note: This constructor will apply the grammar rule on its own
                        // Possibly not the best design ^^
                    };
                    if (!callNode.Arguments.IsValid) {
                        throw new Exception();
                    }
                    nodes.RemoveAt(i + 1);
                    nodes[i] = callNode;
                } else if (nodes[i] is IGroupedASTNode groupNode) {
                    this.ApplyGrammar(groupNode.Nodes);
                }

                if (nodes[i] is IExpr && i + 1 < nodes.Count && nodes[i+1] is SeperatorNode sepNode && sepNode.Content.CompareTo(";") == 0) {
                    nodes.RemoveAt(i + 1);
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

        private void ApplyClassGrammar(List<ASTNode> nodes, int from) {

            if (TypeSequence<ASTNode, ASTNode, IdentifierNode, ScopeNode>.Match(nodes, from)) {

                ClassDeclNode classDecl = new ClassDeclNode(nodes[from + 1].Content, nodes[from].Pos);

                ScopeNode classBody = nodes[from + 2] as ScopeNode;
                this.ApplyGrammar(classBody.Nodes);

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

                nodes[from] = classDecl;
                nodes.RemoveRange(from + 1, 2);

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
                        // More easy-to-solve cases here
                    default:
                        nodes.Add(new ASTNode(this.m_tokens[i].Position, this.m_tokens[i].Content, this.m_tokens[i].Type));
                        break;
                }

            }

            return nodes;

        }

        private void Safe(bool enable, Action doAction, Action<Exception> handler) {
            if (enable) {
                try { doAction.Invoke(); }
                catch (Exception e) { handler.Invoke(e); }
            } else {
                doAction.Invoke();
            }
        }

        private bool Sequence(List<ASTNode> nodes, int from, params Type[] types) {
            for (int i = from, j = 0; i < nodes.Count && j < types.Length; i++, j++) {
                if (nodes[i].GetType() != types[j]) {
                    return false;
                }
            }
            return from + types.Length <= nodes.Count;
        }

        public AST Build() => new AST(new CompileUnitNode(this.m_topNodes));

    }

}
