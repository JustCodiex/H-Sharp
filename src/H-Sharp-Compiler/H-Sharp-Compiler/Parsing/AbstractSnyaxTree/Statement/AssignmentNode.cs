using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Statement {
    
    public class AssignmentNode : BinOpNode, IStatement, ILR {

        public AssignmentNode(SourcePosition position) : base(position) {
            this.m_op = "=";
        }

    }

}
