using System;
using System.Collections.Generic;
using System.Linq;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;
using HSharp.Parsing.AbstractSnyaxTree.Expression;
using HSharp.Parsing.AbstractSnyaxTree.Literal;
using HSharp.Util;

namespace HSharp.Analysis.Verifying {
    
    public class VarsVerifier {
    
        public VarsVerifier() {



        }

        public CompileResult Vars(AST ast) {

            VarScope vScope = new VarScope();

            foreach (ASTNode node in ast.Root) {

                // check node type etc... for declarations and other scopes...

                CompileResult result = VarsNode(node, vScope);
                if (!result) {
                    return result;
                }

            }

            return new CompileResult(true);

        }

        private CompileResult VarsNode(ASTNode node, VarScope scope) {

            switch (node) {
                case FuncDeclNode funcDeclNode:
                    VarScope funcScope = new VarScope();
                    foreach (ParamsNode.ParameterNode param in funcDeclNode.Params.Parameters) {
                        param.Identifier.Index = funcScope.Enter(param.Identifier.Content);
                    }
                    this.VarsNode(funcDeclNode.Body, funcScope);
                    funcDeclNode.Body.VarIndices = funcDeclNode.Body.VarIndices.Union(this.Exit(new VarScope(), funcScope)).ToArray();
                    scope.Enter(funcDeclNode.Name);
                    break;
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
                case VarDeclNode vDeclNode: // TODO: Make it an error such that two variables of the same name may not be declared in the same scope
                    this.VarsNode(vDeclNode.AssignToExpr, scope);
                    vDeclNode.EnterIndex = scope.Enter(vDeclNode.VarName);
                    break;
                case BinOpNode binopNode:
                    this.VarsNode(binopNode.Left, scope);
                    this.VarsNode(binopNode.Right, scope);
                    break;
                case IdentifierNode iDNode:
                    if (scope.Lookup(iDNode.Content, out ushort idIndex)) {
                        iDNode.Index = idIndex;
                    } else {
                        return new CompileResult(false, $"Variable '{iDNode.Content}' does not exist").SetOrigin(iDNode.Pos);
                    }
                    break;
                case CallNode callNode:
                    this.VarsNode(callNode.IdentifierNode, scope);
                    foreach(ASTNode arg in callNode.Arguments.Arguments) {
                        this.VarsNode(arg, scope);
                    }
                    if (callNode.IdentifierNode is IdentifierNode callIdNode) {
                        callIdNode.IsFuncIdentifier = true;
                    } // else ...
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
