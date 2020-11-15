using System;
using System.Collections.Generic;
using System.Text;
using HSharp.Parsing.AbstractSnyaxTree;

namespace HSharp.Analysis.Verifying {
    
    public class ControlpathVerifier {

        public CompileResult Verify(AST ast) {

            VarScope vScope = new VarScope();

            foreach (ASTNode node in ast.Root) {

            }

            return new CompileResult(true);

        }

    }

}
