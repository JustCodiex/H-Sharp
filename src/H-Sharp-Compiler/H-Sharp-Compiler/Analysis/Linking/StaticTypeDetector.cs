using HSharp.Analysis.TypeData;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;

namespace HSharp.Analysis.Linking {

    public static class StaticTypeDetector {

        public static CompileResult Detect(AST[] trees, Domain globalDomain) {

            // 1st pass -> detect everything
            foreach (AST ast in trees) {
                foreach (ASTNode node in ast.Root) {
                    var res = DetectInNode(node, globalDomain);
                    if (!res) {
                        return res;
                    }
                }
            }

            return new CompileResult(true);
        
        }

        private static CompileResult DetectInNode(ASTNode node, Domain currentDomain) {

            return node switch
            {
                ClassDeclNode classDeclNode => DetectClassDecl(classDeclNode, currentDomain),
                _ => new CompileResult(true)
            };

        }

        private static CompileResult DetectClassDecl(ClassDeclNode node, Domain domain) {

            if (domain.HasDomain(node.LocalClassName)) {
                return new CompileResult(false, $"Identifier '{node.LocalClassName}' already exists in scope.");
            }

            ClassType classType = new ClassType(node.LocalClassName, node.Pos);
            domain.AddSubdomain(classType);

            foreach (ClassDeclNode subClass in node.Classes) {
                var result = DetectClassDecl(subClass, classType);
                if (!result) {
                    return result;
                }
            }

            return new CompileResult(true);

        }

    }

}
