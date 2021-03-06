using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;

namespace HSharp.Analysis.TypeData {
    
    public class ExternalFunctionType : FunctionType {

        public ExternalFunctionType(string methodname, ClassType owner, FuncDeclNode declOrigin, HSharpType returnType, List<HSharpType> parameters)
            : base(methodname, owner, declOrigin, returnType, parameters) {}

    }

}
