using System.Collections.Generic;
using HSharp.Analysis.TypeData;
using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;
using HSharp.Parsing.AbstractSnyaxTree.Directive;
using HSharp.Parsing.AbstractSnyaxTree.Expression;
using HSharp.Parsing.AbstractSnyaxTree.Type;

namespace HSharp.Analysis.Linking {
    
    public static class StaticTypeDefiner {

        public static CompileResult DefineAllTypes(AST[] trees, Domain domain) {

            foreach (AST ast in trees) {
                foreach (ASTNode node in ast.Root) {
                    var res = DefineType(node, domain);
                    if (!res) {
                        return res;
                    }
                }
            }

            return new CompileResult(true);

        }

        private static CompileResult DefineType(ASTNode node, Domain domain) {
            return node switch
            {
                NamespaceDirectiveNode namespaceDirective => DefineNamespaceElements(namespaceDirective, domain),
                ClassDeclNode classDecl => DefineClass(classDecl, domain),
                FuncDeclNode funcDecl => DefineFunc(funcDecl, domain),
                _ => new CompileResult(true)
            };
        }

        private static CompileResult DefineNamespaceElements(NamespaceDirectiveNode namespaceDirective, Domain domain) {
            NamespaceDomain subDomain = domain.First<NamespaceDomain>(namespaceDirective.Name.FullName);
            foreach (var node in namespaceDirective.Body.Nodes) {
                var result = DefineType(node, subDomain);
                if (!result) {
                    return result;
                }
            }
            return new CompileResult(true);
        }

        private static CompileResult DefineClass(ClassDeclNode classDeclNode, Domain domain) {
            if (domain.HasDomain(classDeclNode.LocalClassName)) {

                ClassType klass = domain.Get<ClassType>(classDeclNode.LocalClassName);

                foreach (ClassDeclNode subClass in classDeclNode.Classes) {
                    var res = DefineClass(subClass, klass);
                    if (!res) {
                        return res;
                    }
                }

                foreach (VarDeclNode field in classDeclNode.Fields) {
                    if (!klass.Fields.ContainsKey(field.VarName)) {
                        string typeName = field.TypeExpr.ToString();
                        if (domain.First<HSharpType>(typeName) is HSharpType type) {
                            klass.Fields.Add(field.VarName, type);
                        } else {
                            return new CompileResult(false, $"Unkown type '{typeName}' in field declaration.").SetOrigin(field.Pos);
                        }
                    } else {
                        return new CompileResult(false, $"Field '{field.VarName}' already exists in the class.").SetOrigin(field.Pos);
                    }
                }

                foreach (FuncDeclNode method in classDeclNode.Methods) {
                    bool isCtor = method is ClassCtorDecl;
                    var res = DefineFunction(method, klass, domain, out FunctionType methodSignature);
                    if (!res) {
                        return res;
                    } else if (isCtor) {
                        klass.Constructors.Add(methodSignature);
                    } else {
                        klass.Methods.Add(method.Name, methodSignature);
                    }
                }

                return new CompileResult(true);

            } else {
                return new CompileResult(false, $"Unknown class type '{classDeclNode.LocalClassName}' inside domain '{domain}'").SetOrigin(classDeclNode);
            }
        }

        public static CompileResult DefineFunc(FuncDeclNode func, Domain domain) {

            CompileResult res = DefineFunction(func, null, domain, out FunctionType type);
            if (!res) {
                return res;
            } else {

                // Add to subdomain
                domain.AddSubdomain(type);

                return new CompileResult(true);
            }

        }

        public static FunctionType DefineFunction(FuncDeclNode funcDeclNode, ClassType owner, Domain domain) {
            if (DefineFunction(funcDeclNode, owner, domain, out FunctionType t)) {
                return t;
            } else {
                return null;
            }
        }

        private static CompileResult DefineFunction(FuncDeclNode funcDeclNode, ClassType methodOwner, Domain domain, out FunctionType signature) {

            signature = null;

            HSharpType returnType = domain.First<HSharpType>(funcDeclNode.Return.ToString());
            if (returnType is null) {
                return new CompileResult(false, $"Unknown method return type '{funcDeclNode.Return}'").SetOrigin(funcDeclNode.Return.Pos);
            }

            List<HSharpType> parameters = new List<HSharpType>();

            // add the "this" parameter
            SourcePosition pos = funcDeclNode.Pos; // Store position for convenience
            if (methodOwner is not null) {
                funcDeclNode.Params.InsertParam(0, new ParamsNode.ParameterNode(new TypeIdentifierNode(methodOwner.Name, pos), new IdentifierNode("this", pos), pos));
            }

            foreach (ParamsNode.ParameterNode node in funcDeclNode.Params.Parameters) {
                if (domain.First<HSharpType>(node.Type.ToString()) is HSharpType paramType) {
                    parameters.Add(paramType);
                } else {
                    return new CompileResult(false, $"Unknown parameter type '{node.Type}'").SetOrigin(node.Pos);
                }
            }

            signature = new FunctionType(funcDeclNode.Name, methodOwner, funcDeclNode, returnType, parameters);
            return new CompileResult(true);

        }


    }

}
