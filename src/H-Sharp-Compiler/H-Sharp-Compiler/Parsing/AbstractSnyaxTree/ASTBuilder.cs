using System;
using System.Collections.Generic;
using System.Linq;

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
            this.ApplyGroup<ScopeNode>(nodes, ref i, LexTokenType.BlockStart, LexTokenType.BlockEnd);
            i = 0;

            // Apply Indexer groupings
            this.ApplyGroup<IndexerNode>(nodes, ref i, LexTokenType.IndexerStart, LexTokenType.IndexerEnd);
            i = 0;

            // Apply expression groupings
            this.ApplyGroup<GroupedExpressionNode>(nodes, ref i, LexTokenType.ExpressionStart, LexTokenType.ExpressionEnd);

        }

        private void ApplyGroup<T>(List<ASTNode> nodes, ref int i, LexTokenType open, LexTokenType close) where T : IGroupedASTNode {

            while (i < nodes.Count) {

                if (nodes[i].LexicalType == open) {

                    List<ASTNode> sub = new List<ASTNode>();
                    int j = i + 1;
                    while(nodes[j].LexicalType != close) {
                        if (nodes[j].LexicalType == open) {
                            break;
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
                    this.ApplyGroup<T>(group.Nodes, ref j, open, close);
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

            return;

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

                if (nodes[i] is IdentifierNode && i + 2 < nodes.Count && nodes[i+1] is BinOpNode assignOp && nodes[i+2] is SeperatorNode n) {
                    if (assignOp.Op.CompareTo("=") == 0 && n.Content == ";") {
                        this.ApplyGrammarBinary(assignOp);
                        nodes[i] = new VarDeclNode(nodes[i].Pos, nodes[i], assignOp);
                        nodes.RemoveRange(i + 1, 2);
                        continue;
                    }
                } else if (nodes[i] is IGroupedASTNode groupNode) {
                    this.ApplyGrammar(groupNode.Nodes);
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

        public AST Build() => new AST(new CompileUnitNode(this.m_topNodes));

    }

}
