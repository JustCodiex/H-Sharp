using System.Collections.Generic;
using HSharp.Analysis.TypeData;
using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Analysis.Linking {

    public class StaticTypeDetector {

        private AST[] m_workAsts;

        public StaticTypeDetector(AST[] asts) {
            this.m_workAsts = asts;
        }

        public CompileResult Detect(Domain globalDomain) {

            // 1st pass -> detect everything
            foreach (AST ast in this.m_workAsts) {
                foreach (ASTNode node in ast.Root) {
                    var res = this.DetectInNode(node, globalDomain);
                    if (!res) {
                        return res;
                    }
                }
            }

            // 2nd pass -> handle inheritance etc. (May eventually need its own class !!! )
            foreach (AST ast in this.m_workAsts) {
                foreach (ASTNode node in ast.Root) {
                    var res = this.FixInNode(node, globalDomain);
                    if (!res) {
                        return res;
                    }
                }
            }

            return new CompileResult(true);
        
        }

        private CompileResult DetectInNode(ASTNode node, Domain currentDomain) {

            return node switch
            {
                ClassDeclNode classDeclNode => DetectClassDecl(classDeclNode, currentDomain),
                _ => new CompileResult(true)
            };

        }

        private CompileResult DetectClassDecl(ClassDeclNode node, Domain domain) {

            if (domain.HasDomain(node.LocalClassName)) {
                return new CompileResult(false, $"Identifier '{node.LocalClassName}' already exists in scope.");
            }

            ClassType classType = new ClassType(node.LocalClassName);
            domain.AddSubdomain(classType);

            foreach (ClassDeclNode subClass in node.Classes) {
                var result = this.DetectClassDecl(subClass, classType);
                if (!result) {
                    return result;
                }
            }

            return new CompileResult(true);

        }

        public CompileResult DefineAllTypes(Domain domain) {

            foreach (AST ast in this.m_workAsts) {
                foreach (ASTNode node in ast.Root) {
                    var res = this.DefineType(node, domain);
                    if (!res) {
                        return res;
                    }
                }
            }

            return new CompileResult(true);

        }

        private CompileResult DefineType(ASTNode node, Domain domain) {
            return node switch
            {
                ClassDeclNode classDecl => this.DefineClass(classDecl, domain),
                FuncDeclNode funcDecl => this.DefineFunc(funcDecl, domain),
                _ => new CompileResult(true)
            };
        }

        private CompileResult DefineClass(ClassDeclNode classDeclNode, Domain domain) {
            if (domain.HasDomain(classDeclNode.LocalClassName)) {

                ClassType klass = domain.Get<ClassType>(classDeclNode.LocalClassName);

                foreach (ClassDeclNode subClass in classDeclNode.Classes) {
                    var res = this.DefineClass(subClass, klass);
                    if (!res) {
                        return res;
                    }
                }

                foreach (VarDeclNode field in classDeclNode.Fields) {
                    if (!klass.Fields.ContainsKey(field.VarName)) {
                        string typeName = field.TypeExpr.ToString();
                        if(domain.First<HSharpType>(typeName) is HSharpType type) {
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
                    var res = this.DefineFunction(method, klass, domain, out FunctionType methodSignature);
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
                return new CompileResult(false, "Uhm...");
            }
        }

        private CompileResult DefineFunc(FuncDeclNode func, Domain domain) {

            CompileResult res = this.DefineFunction(func, null, domain, out FunctionType type);
            if (!res) {
                return res;
            } else {

                // Add to subdomain
                domain.AddSubdomain(type);

                return new CompileResult(true);
            }

        }

        private CompileResult DefineFunction(FuncDeclNode funcDeclNode, ClassType methodOwner, Domain domain, out FunctionType signature) {

            signature = null;

            HSharpType returnType = domain.First<HSharpType>(funcDeclNode.Return.ToString());
            if (returnType is null) {
                return new CompileResult(false, $"Unknown method return type '{funcDeclNode.Return}'").SetOrigin(funcDeclNode.Return.Pos);
            }

            List<HSharpType> parameters = new List<HSharpType>();

            // add the "this" parameter
            SourcePosition pos = funcDeclNode.Pos; // Stor position for convenience
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

            signature = new FunctionType(funcDeclNode.Name, methodOwner, returnType, parameters);
            return new CompileResult(true);

        }

        private CompileResult FixInNode(ASTNode node, Domain currentDomain) {

            return node switch
            {
                ClassDeclNode classDeclNode => FixClassDecl(classDeclNode, currentDomain),
                _ => new CompileResult(true)
            };

        }


        private CompileResult FixClassDecl(ClassDeclNode node, Domain domain) {

            ClassType classType = domain.First<ClassType>(node.LocalClassName);

            if ((node.Inheritance?.InheritanceNodes.Count ?? 0) > 0) {

                foreach (ITypeIdentifier inheritFrom in node.Inheritance.InheritanceNodes) {

                    if (domain.First<ClassType>(inheritFrom.Content) is ClassType baseClassType) {
                        classType.SetBase(baseClassType);
                    }

                }

            }

            foreach (ClassDeclNode subClass in node.Classes) {
                var result = this.FixClassDecl(subClass, classType);
                if (!result) {
                    return result;
                }
            }

            return new CompileResult(true);

        }


    }

}
