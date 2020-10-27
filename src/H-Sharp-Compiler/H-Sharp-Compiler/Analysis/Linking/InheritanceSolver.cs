using System.Linq;
using HSharp.Analysis.TypeData;
using HSharp.Generator;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;

namespace HSharp.Analysis.Linking {
    
    public static class InheritanceSolver {

        public static CompileResult Solve(AST[] trees, Domain globalDomain) {

            foreach (AST tree in trees) {
                foreach (ASTNode node in tree.Root) {
                    var res = SolveInNode(node, globalDomain);
                    if (!res) {
                        return res;
                    }
                }
            }

            return new CompileResult(true);

        }

        private static CompileResult SolveInNode(ASTNode node, Domain currentDomain) {

            return node switch
            {
                ClassDeclNode classDeclNode => SolveClassDecl(classDeclNode, currentDomain),
                _ => new CompileResult(true)
            };

        }

        public static CompileResult SolveClassDecl(ClassDeclNode node, Domain domain) {

            // Find first instance of the class in the domain
            ClassType classType = domain.First<ClassType>(node.LocalClassName);

            // If anything in the inheritance node - handle it
            if ((node.Inheritance?.InheritanceNodes.Count ?? 0) > 0) {

                foreach (ITypeIdentifier inheritFrom in node.Inheritance.InheritanceNodes) {

                    if (domain.First<ClassType>(inheritFrom.Content) is ClassType baseClassType) {
                        classType.SetBase(baseClassType);
                    }

                }

            }

            // Add a constructor if none
            if (classType.Constructors.Count == 0) {
                var ctor = ConstructorGenerator.GenerateBasicConstructor(classType);
                node.Methods.Add(ctor);
                classType.Constructors.Add(StaticTypeDefiner.DefineFunction(ctor, classType, domain));
            } else { // otherwise - loop through and instantiate base classes though their constructors
                foreach (ClassCtorDecl ctor in classType.Constructors.Select(x => x.Origin)) {
                    ConstructorGenerator.GenerateInheritanceCode(ctor, classType);
                }
            }

            // Handle sub-cases of class
            foreach (ClassDeclNode subClass in node.Classes) {
                var result = SolveClassDecl(subClass, classType);
                if (!result) {
                    return result;
                }
            }

            return new CompileResult(true);

        }

    }

}
