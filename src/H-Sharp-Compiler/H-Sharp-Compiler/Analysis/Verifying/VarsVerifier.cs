using System;
using System.Collections.Generic;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Util;

namespace HSharp.Analysis.Verifying {
    
    public class VarsVerifier {
    
        public VarsVerifier() {



        }

        public CompileResult Vars(AST ast) {

            VarScope vScope = new VarScope();

            foreach (ASTNode node in ast.Root) {

                // check node type etc... for declarations and other scopes...

                VarsNode(node, vScope);

            }

            return new CompileResult(true);

        }

        private CompileResult VarsNode(ASTNode node, VarScope scope) {

            switch (node) {
                case ScopeNode scopeNode:
                    VarScope subScope = new VarScope(scope);
                    foreach(ASTNode subNode in scopeNode.Nodes) {
                        this.VarsNode(subNode, subScope);
                    }
                    scopeNode.VarIndices = this.Exit(scope, subScope);
                    scope.top = subScope.top;
                    break;
                case IGroupedASTNode groupExprNode:
                    foreach (ASTNode subNode in groupExprNode.Nodes) {
                        this.VarsNode(subNode, scope);
                    }
                    break;
                case VarDeclNode vDeclNode:
                    this.VarsNode(vDeclNode.AssignToExpr, scope);
                    vDeclNode.EnterIndex = scope.Enter(vDeclNode.VarName);
                    break;
                case BinOpNode binopNode:
                    this.VarsNode(binopNode.Left, scope);
                    this.VarsNode(binopNode.Right, scope);
                    break;
                case IdentifierNode iDNode:
                    iDNode.Index = scope.Lookup(iDNode.Content);
                    break;
                case IntLitNode:
                    break;
                default:
                    Console.WriteLine($"Vars not implemented for type: {node.GetType().Name}");
                    break;
            }

            return new CompileResult(true);

        }

        private ushort[] Exit(VarScope current, VarScope sub) {

            (string key, PrimitiveStack<ushort> stack)[] subAsArray = sub.stack.ToArray();
            (string key, PrimitiveStack<ushort> stack)[] curAsArray = current.stack.ToArray();

            List<ushort> exits = new List<ushort>();

            for (int i = 0; i < subAsArray.Length; i++) {

                bool any = false;

                for (int j = 0; j < curAsArray.Length; j++) {
                    if (curAsArray[j].key == subAsArray[i].key) {
                        while(subAsArray[i].stack.Count > curAsArray[j].stack.Count) {
                            exits.Add(subAsArray[i].stack.Pop());
                        }
                        any = true;
                        break;
                    }
                }

                if (!any) {
                    exits.AddRange(subAsArray[i].stack);
                }

            }

            return exits.ToArray();

        }

    }

}
