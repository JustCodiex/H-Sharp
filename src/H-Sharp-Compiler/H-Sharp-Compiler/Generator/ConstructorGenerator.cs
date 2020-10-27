using System.Collections.Generic;
using System.Linq;
using HSharp.Analysis.TypeData;
using HSharp.Metadata;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Generator {
    
    public static class ConstructorGenerator {
    
        public static FuncDeclNode GenerateBasicConstructor(ClassType klass) {

            // The body
            ScopeNode ctorBody = new ScopeNode(klass.CodeOrigin);

            // Create ctor declaration
            ClassCtorDecl ctorDecl = new ClassCtorDecl(klass.Name, klass.CodeOrigin) {
                Params = new ParamsNode(klass.CodeOrigin),
                Body = ctorBody
            };

            // Write a small TODO-ish error
            Log.WriteLine("Warning: Class default constructor does currently not inherit from base object!");

            if (klass.Base is not null) {
                GenerateInheritanceCode(ctorDecl, klass);
            }

            // Return constructor
            return ctorDecl;

        }

        public static void GenerateInheritanceCode(ClassCtorDecl classCtor, ClassType klass) {

            if (klass.Base is ClassType baseKlass) { // TODO: Do a proper arguments check and all that!

                FunctionType baseCtor = baseKlass.Constructors.First();

                classCtor.Body.InjectNodes(new List<ASTNode>() {
                    new CallNode(new MemberAccessNode(new BaseNode(classCtor.Pos), new IdentifierNode(baseCtor.Name, classCtor.Pos), MemberAccessNode.CtorAccess, classCtor.Pos), classCtor.Pos) {
                        Arguments = new ArgumentsNode(new ExpressionNode(classCtor.Pos, new List<ASTNode>() { 
                            new ThisNode(classCtor.Pos)
                        }))
                    } 
                }, 0);

            }

        }

    }

}
