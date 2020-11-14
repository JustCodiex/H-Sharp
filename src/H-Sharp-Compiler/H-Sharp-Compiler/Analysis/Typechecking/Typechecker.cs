using System;
using System.Collections.Generic;
using System.Linq;
using HSharp.Compiling.Hint;
using HSharp.Analysis.TypeData;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;
using HSharp.Parsing.AbstractSnyaxTree.Expression;
using HSharp.Parsing.AbstractSnyaxTree.Initializer;
using HSharp.Parsing.AbstractSnyaxTree.Literal;
using HSharp.Parsing.AbstractSnyaxTree.Statement;
using HSharp.Parsing.AbstractSnyaxTree.Type;
using HSharp.Util.Functional;
using ValueType = HSharp.Analysis.TypeData.ValueType;

namespace HSharp.Analysis.Typechecking {
    
    public class Typechecker {
    
        public Typechecker() {

        }

        public CompileResult Typecheck(AST ast, Domain globDomain) {

            TypeEnvironment tenv = new TypeEnvironment();
            foreach (ASTNode node in ast.Root) {
                var res = this.TypecheckNode(node, tenv, globDomain).Item1;
                if (!res) {
                    return res;
                }
            }

            return new CompileResult(true);

        }

        private (CompileResult, IValType) TypecheckNode(ASTNode node, TypeEnvironment tenv, Domain domain) => node switch
        {
            VarDeclNode varDecl => this.TypecheckVardeclOperation(varDecl, tenv, domain),
            FuncDeclNode funcDecl => this.TypecheckFuncDeclOperation(funcDecl, tenv, domain),
            ClassDeclNode classDecl => this.TypecheckClassDeclOperation(classDecl, tenv, domain),
            BinOpNode binop => this.TypecheckBinaryOperation(binop, tenv, domain),
            MemberAccessNode memberAccess => this.TypecheckMemberAccessOperation(memberAccess, tenv, domain),
            IdentifierNode id => this.TypecheckIdentifierOperation(id, tenv, domain),
            ThisNode thisNode => this.TypecheckIdentifierOperation(thisNode, tenv, domain),
            BaseNode baseNode => this.TypecheckBaseOperation(baseNode, tenv, domain),
            ReturnStatement returnStatement => this.TypecheckNode(returnStatement.Expression as ASTNode, tenv, domain),
            NewObjectNode newObject => this.TypecheckNewOperation(newObject, tenv, domain),
            NewArrayNode newArray => this.TypecheckNewOperation(newArray, tenv, domain),
            CallNode callNode => this.TypecheckCallOperation(callNode, tenv, domain),
            ScopeNode scope => this.TypecheckScope(scope, tenv, domain).Select(x => x, y => y[^1]),
            ExpressionNode expr => this.TypecheckExpressionOperation(expr, tenv, domain),
            ValueListInitializerNode vListNode => this.TypecheckValListInitOperation(vListNode, tenv, domain),
            LookupNode lookupNode => this.TypecheckLookupOperation(lookupNode, tenv, domain),
            IndexerNode indexerNode => this.TypecheckIndexerOperation(indexerNode, tenv, domain),
            ILiteral lit => this.TypecheckConstOperation(lit, domain),
            _ => (new CompileResult(false, $"Unsupported type-checkable element '{node.NodeType}'").SetOrigin(node), null),
        };

        private (CompileResult, IValType) TypecheckClassDeclOperation(ClassDeclNode classDecl, TypeEnvironment tenv, Domain domain) {

            TypeEnvironment newEnv = new TypeEnvironment(tenv);
            Domain klassDom = domain.First<ClassType>(classDecl.LocalClassName);

            for (int i = 0; i < classDecl.Fields.Count; i++) {
                var res = this.TypecheckNode(classDecl.Fields[i], newEnv, klassDom);
                if (!res.Item1) {
                    return res;
                }
            }

            for (int i = 0; i < classDecl.Methods.Count; i++) {
                var res = this.TypecheckFuncDeclOperation(classDecl.Methods[i], newEnv, klassDom);
                if (!res.Item1) {
                    return res;
                }
            }

            for (int i = 0; i < classDecl.Classes.Count; i++) {
                var res = this.TypecheckClassDeclOperation(classDecl.Classes[i], tenv, domain.First<ClassType>(classDecl.LocalClassName));
                if (!res.Item1) {
                    return res;
                }
            }

            return (new CompileResult(true), null);

        }

        private (CompileResult, IValType) TypecheckFuncDeclOperation(FuncDeclNode funcDecl, TypeEnvironment tenv, Domain domain) {

            TypeEnvironment funTypeEnv = new TypeEnvironment(tenv);

            foreach (ParamsNode.ParameterNode param in funcDecl.Params.Parameters) {
                funTypeEnv.MapsTo(param.Identifier.Content, this.TypeOf(param.Type.ToString(), domain));
            }

            // Check scope
            var bodyResult = this.TypecheckScope(funcDecl.Body, funTypeEnv, domain);
            if (!bodyResult.Item1) {
                return (bodyResult.Item1, null);
            }

            IValType outType = this.TypeOf(funcDecl.Return.ToString(), domain);
            FunctionType funcType;
            if (domain is ClassType klass) {
                if (!klass.Methods.TryGetValue(funcDecl.Name, out funcType)) {
                    // TODO: Something here
                }
            } else {
                funcType = domain.Get<FunctionType>(funcDecl.Name);
            }

            if (funcDecl is ClassCtorDecl) { // or is void method

                return (new CompileResult(true), funcType);

            } else {

                if (!this.IsAllSubtypeOf(outType, bodyResult.Item2, out string err) && funcDecl is not ClassCtorDecl) {
                    return (new CompileResult(false, err).SetOrigin(funcDecl), null);
                } else {
                    if (!funcType.IsMethod) {
                        tenv.MapsTo(funcDecl.Name, funcType);
                    }
                    return (new CompileResult(true), funcType);
                }

            }

        }

        private (CompileResult, IValType) TypecheckVardeclOperation(VarDeclNode varDecl, TypeEnvironment tenv, Domain domain) {

            IValType declType = this.TypeOf(varDecl.TypeExpr, domain);

            (CompileResult result, IValType exprType) = varDecl.AssignToExpr is null ? (new CompileResult(true), declType) : this.TypecheckNode(varDecl.AssignToExpr, tenv, domain);
            if (!result) {
                return (result, null);
            }

            if (!IsSubtype(exprType, declType, out string subTypeError)) {
                return (new CompileResult(false, subTypeError).SetOrigin(varDecl), null);
            } else {
                tenv.MapsTo(varDecl.VarName, declType);
            }

            return (new CompileResult(true), declType);
        
        }

        private (CompileResult, IValType) TypecheckBinaryOperation(BinOpNode binop, TypeEnvironment tenv, Domain domain) {

            (CompileResult, IValType) expr2 = this.TypecheckNode(binop.Right, tenv, domain);
            if (!expr2.Item1) { return expr2; }

            (CompileResult, IValType) expr1 = this.TypecheckNode(binop.Left, tenv, domain);
            if (!expr1.Item1) { return expr1; }

            if (expr1.Item2 is ValueType && expr2.Item2 is ValueType) {
                switch (binop.Op) {
                    case "=":
                        if (!this.IsSubtype(expr2.Item2, expr1.Item2, out _)) {
                            return (new CompileResult(false, $"Invalid assignemt, type '{expr2.Item2}' cannot be assigned to type '{expr1.Item2}'.").SetOrigin(binop.Pos), null);
                        } else {
                            return (new CompileResult(true), expr1.Item2);
                        }
                    case "+":
                        if (expr1.Item2 == expr2.Item2) {
                            return (new CompileResult(true), expr1.Item2);
                        } else {
                            return (new CompileResult(false).SetOrigin(binop), null);
                        }
                    case "-":
                        if (expr1.Item2 == expr2.Item2) {
                            return (new CompileResult(true), expr1.Item2);
                        } else {
                            return (new CompileResult(false).SetOrigin(binop), null);
                        }
                    case "*":
                        if (expr1.Item2 == expr2.Item2) {
                            return (new CompileResult(true), expr1.Item2);
                        } else {
                            return (new CompileResult(false).SetOrigin(binop), null);
                        }
                    case "/":
                        if (expr1.Item2 == expr2.Item2) {
                            return (new CompileResult(true), expr1.Item2);
                        } else {
                            return (new CompileResult(false).SetOrigin(binop), null);
                        }
                    default:
                        return (new CompileResult(false, $"Unknown operator '{binop.Op}'.").SetOrigin(binop.Pos), null);
                }
            }

            return (new CompileResult(true), null);

        }

        private (CompileResult, IValType) TypecheckIdentifierOperation(ASTNode idOp, TypeEnvironment tenv, Domain domain) {

            if (tenv.IsDefined(idOp.Content)) {
                return (new CompileResult(true), tenv.Lookup(idOp.Content));
            } else {
                if (domain.First<FunctionType>(idOp.Content) is FunctionType func) {
                    return (new CompileResult(true), func);
                } else {
                    return (new CompileResult(false, $"Unknown identifier").SetOrigin(idOp), null);
                }
            }

        }

        private (CompileResult, IValType) TypecheckConstOperation(ILiteral lit, Domain domain) => lit switch
        {
            IntLitNode => (new CompileResult(true), domain.First<ValueType>("int")),
            _ => (new CompileResult(false).SetOrigin((lit as ASTNode).Pos), null),
        };

        private (CompileResult, IValType) TypecheckMemberAccessOperation(MemberAccessNode memberAccess, TypeEnvironment tenv, Domain domain) {

            (CompileResult, IValType) left = this.TypecheckNode(memberAccess.Left as ASTNode, tenv, domain);
            if (!left.Item1) { 
                return left; 
            }

            if (left.Item2 is ReferenceType refType) {
                if (refType.ReferencedType is ClassType refClassType) {
                    if (memberAccess.AccessMethodType.CompareTo(".") == 0) {
                        if (refClassType.FindMember(memberAccess.Right.Content) is HSharpType refClassMember) {
                            return (new CompileResult(true), refClassMember is IRefType ? new ReferenceType(refClassMember) : refClassMember as IValType);
                        } else {
                            return (new CompileResult(false, $"Class '{refClassType.Name}' has no member by name '{memberAccess.Right.Content}'.").SetOrigin(memberAccess.Pos), null);
                        }
                    } else if (memberAccess.AccessMethodType.CompareTo(MemberAccessNode.CtorAccess) == 0) {
                        if (refClassType.Constructors.Find(x => x.Name.CompareTo(memberAccess.Right.Content) == 0) is FunctionType ctor) {
                            return (new CompileResult(true), VoidType.Void);
                        } else {
                            return (new CompileResult(false, $"'base' has no constructor with specified signature!").SetOrigin(memberAccess.Pos), null);
                        }
                    } else {
                        throw new NotImplementedException();
                    }
                }
            } else {
                throw new NotImplementedException();
            }

            return (new CompileResult(false).SetOrigin(memberAccess.Pos), null);

        }

        private (CompileResult, IValType) TypecheckNewOperation(NewObjectNode newObject, TypeEnvironment tenv, Domain domain) {

            IValType type = this.TypeOf(newObject.Type.ToString(), domain);
            if (type is null) {
                return (new CompileResult(false, $"Undefined type '{newObject.Type}'").SetOrigin(newObject), null);
            }

            foreach (ASTNode arg in newObject.CtorArguments.Arguments) {
                var res = this.TypecheckNode(arg, tenv, domain);
                if (!res.Item1) {
                    return res;
                }
            }

            return (new CompileResult(true), type);

        }

        private (CompileResult, IValType) TypecheckNewOperation(NewArrayNode newArray, TypeEnvironment tenv, Domain domain) {

            IValType type = this.TypeOf(new TypeArrayIdentifierNode(newArray.Type, newArray.Pos), domain);
            if (type is null) {
                return (new CompileResult(false, $"Undefined type '{newArray.Type}'").SetOrigin(newArray), null);
            }

            foreach (ASTNode arg in newArray.Indexer.Nodes) {
                var res = this.TypecheckNode(arg, tenv, domain);
                if (!res.Item1) {
                    return res;
                }
            }

            // Add compiler type hint
            newArray.AddCompilerHint(CompileHintType.TypeHint, type);

            return (new CompileResult(true), type);

        }

        private (CompileResult, IValType) TypecheckCallOperation(CallNode callNode, TypeEnvironment tenv, Domain domain) {

            (CompileResult result, IValType callType) = this.TypecheckNode(callNode.IdentifierNode, tenv, domain);
            if (!result) {
                return (result, null);
            }

            if (callType is not null) {

                foreach (ASTNode arg in callNode.Arguments.Arguments) {
                    var res = this.TypecheckNode(arg, tenv, domain);
                    if (!res.Item1) {
                        return res;
                    }
                }

                if (callType is FunctionType func) { // normal method/function
                    return (new CompileResult(true), func.ReturnType is IValType ? func.ReturnType as IValType : new ReferenceType(func.ReturnType));
                } else if (callType is VoidType) { // ctor
                    return (new CompileResult(true), callType);
                }

            }

            return (new CompileResult(false, $"Cannot invoke class member '{callNode.IdentifierNode}'").SetOrigin(callNode), null);

        }

        private (CompileResult, IValType) TypecheckBaseOperation(BaseNode baseNode, TypeEnvironment tenv, Domain domain) {

            IValType thisNode = tenv.Lookup("this");

            if (thisNode is ReferenceType refType) {
                if (refType.ReferencedType is IExtendableType extendable && this.TypeOf(extendable.Base) is IValType extAsVal) {
                    return (new CompileResult(true), extAsVal);
                }
            }

            return (new CompileResult(false).SetOrigin(baseNode), null);

        }

        private (CompileResult, List<IValType>) TypecheckScope(ScopeNode scopeNode, TypeEnvironment tenv, Domain domain) {

            List<IValType> results = new List<IValType>();
            TypeEnvironment scopeEnv = new TypeEnvironment(tenv);

            IValType last = null;
            foreach (ASTNode node in scopeNode.Nodes) {
                var res = this.TypecheckNode(node, scopeEnv, domain);
                if (res.Item1) {
                    if (node is ReturnStatement) {
                        results.Add(res.Item2);
                    } // TODO: Add more statements here that may yield retrn types
                    last = res.Item2;
                    if (last is null) {
                        break;
                    }
                } else {
                    return (res.Item1, null);
                }
            }

            if ((results.Count > 0 && last != results[^1]) || results.Count == 0) {
                results.Add(last);
            }

            return (new CompileResult(true), results);

        }

        private (CompileResult, IValType) TypecheckExpressionOperation(ExpressionNode scopeNode, TypeEnvironment tenv, Domain domain) {

            IValType last = null;
            foreach (ASTNode node in scopeNode.Nodes) {
                var res = this.TypecheckNode(node, tenv, domain);
                if (!res.Item1) {
                    return (res.Item1, null);
                } else {
                    last = res.Item2;
                }
            }

            return (new CompileResult(true), last);

        }

        private (CompileResult, IValType) TypecheckValListInitOperation(ValueListInitializerNode valListInitializerNode, TypeEnvironment tenv, Domain domain) {

            IValType defType = null;
            foreach (IExpr expr in valListInitializerNode) {
                var result = this.TypecheckNode(expr as ASTNode, tenv, domain);
                if (!result.Item1) {
                    return result;
                } else {
                    if (defType == null) {
                        defType = result.Item2;
                    } else if (!this.IsSubtype(result.Item2, defType, out string s)) {
                        return (new CompileResult(false, s).SetOrigin(expr as ASTNode), null);
                    }
                }
            }

            // Adding a small compiler hint
            valListInitializerNode.AddCompilerHint(CompileHintType.TypeHint, defType);

            return (new CompileResult(true), new ArrayType(defType as HSharpType));

        }

        private (CompileResult, IValType) TypecheckLookupOperation(LookupNode lookupNode, TypeEnvironment tenv, Domain domain) {

            var indexedResult = this.TypecheckNode(lookupNode.Index, tenv, domain);
            var result = this.TypecheckNode(lookupNode.Left as ASTNode, tenv, domain);
            if (result.Item2 is ArrayType array) {
                if (indexedResult.Item2 == domain.Get<ValueType>("int")) {
                    return (new CompileResult(true), this.TypeOf(array.ReferencedType));
                } else {
                    return (new CompileResult(false, "Invalid Indexing Type").SetOrigin(lookupNode), null);
                }
            } else {
                return (new CompileResult(false, "Invalid Indexing Type").SetOrigin(lookupNode), null);
            }

        }

        private (CompileResult, IValType) TypecheckIndexerOperation(IndexerNode indexerNode, TypeEnvironment tenv, Domain domain)
            => this.TypecheckNode(indexerNode.Nodes[0], tenv, domain); // TODO: Make this more generic...

        private bool IsSubtype(IValType subType, IValType baseType, out string err) {
            if (subType is null || baseType is null) {
                err = "Fatal compiler error !!!!";
                return false;
            }
            err = string.Empty;
            if (subType == baseType) {
                return true;
            } else if (subType is ReferenceType subRefType && baseType is ReferenceType baseRefType) {
                if (subRefType.Equals(baseRefType) || subRefType.IsSubTypeOf(baseRefType)) {
                    return true;
                } else {
                    return false;
                }
            } else {
                // TODO: Check
                err = $"Invalid type '{subType}' found. Expected  type '{baseType}'";
                return false;
            }
        }

        private bool IsAllSubtypeOf(IValType baseType, List<IValType> valtypes, out string err) {
            err = string.Empty;
            for (int i = 0; i < valtypes.Count; i++) {
                if (!this.IsSubtype(valtypes[i], baseType, out err)) {
                    return false;
                }
            }
            return true;
        }

        private IValType TypeOf(ITypeIdentifier identifier, Domain domain) => identifier switch
        {
            TypeArrayIdentifierNode arrayIdentifierNode => new ArrayType(this.TypeOf(arrayIdentifierNode.EncapsulatedType, domain) as HSharpType),
            _ => this.TypeOf(identifier.Content, domain)
        };
        

        private IValType TypeOf(HSharpType type) {
            if (type is IRefType) {
                return new ReferenceType(type);
            } else {
                return type as IValType;
            }
        }

        private IValType TypeOf(string typeId, Domain domain) => this.TypeOf(domain.First<HSharpType>(typeId));

        private IValType TypeOf(IExtendableType type) {
            if (type is ClassType klass) {
                return new ReferenceType(klass);
            } else if (type is StructType strct) {
                return strct;
            } else {
                return null;
            }
        }

    }

}
