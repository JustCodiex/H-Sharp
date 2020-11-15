using System.Collections.Generic;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Compiling.Branching {
    
    public static class BranchCompiler {
    
        public static List<ByteInstruction> CompileCondition(ASTCompiler compiler, IExpr condition, CompileContext context) {

            ASTNode node = condition as ASTNode;

            var instructions = compiler.CompileNode(node, context);

            // Check instructions for condition optimization

            return instructions;

        }

    }

}
