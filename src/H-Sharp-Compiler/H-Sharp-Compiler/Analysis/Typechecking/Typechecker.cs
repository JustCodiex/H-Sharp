using System;
using System.Collections.Generic;
using System.Text;
using HSharp.Analysis.TypeData;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;
using HSharp.Parsing.AbstractSnyaxTree.Expression;
using HSharp.Parsing.AbstractSnyaxTree.Literal;

using ValueType = HSharp.Analysis.TypeData.ValueType;

namespace HSharp.Analysis.Typechecking {
    
    public class Typechecker {
    
        public Typechecker() {

        }

        public CompileResult Typecheck(AST ast, Domain globDomain) {

            foreach (ASTNode node in ast.Root) {
                TypeEnvironment tenv = new TypeEnvironment();
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
            ILiteral lit => this.TypecheckConstOperation(lit, domain),
            _ => (new CompileResult(true), null),
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

            IValType finalType = null;
            foreach (ASTNode bodyNode in funcDecl.Body.Nodes) {
                (CompileResult result, IValType exprType) = this.TypecheckNode(bodyNode, funTypeEnv, domain);
                if (!result) {
                    return (result, exprType);
                } else {
                    finalType = exprType;
                }
            }

            // TODO: Second pass going through all control paths

            IValType outType = this.TypeOf(funcDecl.Return.ToString(), domain);

            if (funcDecl is ClassCtorDecl) { // or is void method

                return (new CompileResult(true), outType);

            } else {

                if (!this.IsSubtype(finalType, outType, out string err) && funcDecl is not ClassCtorDecl) {
                    return (new CompileResult(false, err), null);
                } else {
                    return (new CompileResult(true), outType);
                }

            }

        }

        private (CompileResult, IValType) TypecheckVardeclOperation(VarDeclNode varDecl, TypeEnvironment tenv, Domain domain) {

            IValType declType = this.TypeOf(varDecl.TypeExpr.ToString(), domain);

            (CompileResult result, IValType exprType) = varDecl.AssignToExpr is null ? (new CompileResult(true), declType) : this.TypecheckNode(varDecl.AssignToExpr, tenv, domain);
            if (!result) {
                return (result, null);
            }

            if (!IsSubtype(exprType, declType, out string subTypeError)) {
                return (new CompileResult(false, subTypeError), null);
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
                            return (new CompileResult(false), null);
                        }
                    case "-":
                        if (expr1.Item2 == expr2.Item2) {
                            return (new CompileResult(true), expr1.Item2);
                        } else {
                            return (new CompileResult(false), null);
                        }
                    case "*":
                        if (expr1.Item2 == expr2.Item2) {
                            return (new CompileResult(true), expr1.Item2);
                        } else {
                            return (new CompileResult(false), null);
                        }
                    case "/":
                        if (expr1.Item2 == expr2.Item2) {
                            return (new CompileResult(true), expr1.Item2);
                        } else {
                            return (new CompileResult(false), null);
                        }
                    default:
                        return (new CompileResult(false, $"Unknown operator '{binop.Op}'.").SetOrigin(binop.Pos), null);
                }
            }

            return (new CompileResult(true), null);

        }

        private (CompileResult, IValType) TypecheckIdentifierOperation(ASTNode idOp, TypeEnvironment tenv, Domain domain) {

            IValType idType = tenv.Lookup(idOp.Content);

            return (new CompileResult(true), idType);

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
                    if (refClassType.Fields.TryGetValue(memberAccess.Right.Content, out HSharpType classFieldType)) {
                        return (new CompileResult(true), classFieldType is IRefType ? new ReferenceType(classFieldType) : classFieldType as IValType);
                    } else {
                        return (new CompileResult(false, $"Class '{refClassType.Name}' has no member by name '{memberAccess.Right.Content}'.").SetOrigin(memberAccess.Pos), null);
                    }
                }
            } else {

            }

            return (new CompileResult(false).SetOrigin(memberAccess.Pos), null);

        }

        private bool IsSubtype(IValType subType, IValType baseType, out string err) {
            if (subType is null || baseType is null) {
                err = "Fatal compiler error !!!!";
                return false;
            }
            err = string.Empty;
            if (subType == baseType) {
                return true;
            } else {
                // TODO: Check
                err = $"Invalid type operand {subType}. Expected {baseType}";
                return false;
            }
        }

        private IValType TypeOf(string typeId, Domain domain) {
            HSharpType type = domain.First<HSharpType>(typeId);
            if (type is IRefType) {
                return new ReferenceType(type);
            } else {
                return type as IValType;
            }
        }

    }

}
