using System.Collections.Generic;
using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;

namespace HSharp.Analysis.TypeData {
    
    public class FunctionType : HSharpType, IValType {
    
        public override string Name { get; }

        public ClassType Owner { get; }

        public HSharpType ReturnType { get; }

        public List<HSharpType> ParameterTypes { get; }

        public FuncDeclNode Origin { get; }

        public SourcePosition CodeOrigin => this.Origin.Pos;
        public bool IsPrimitive => false;

        public bool IsMethod => this.Owner is null;

        public override bool IsReferenceType => false;

        public override ushort Size => 0;

        public FunctionType(string methodname, ClassType owner, FuncDeclNode declOrigin, HSharpType returnType, List<HSharpType> parameters) {
            this.Name = methodname;
            this.Origin = declOrigin;
            this.Owner = owner;
            this.ReturnType = returnType;
            this.ParameterTypes = parameters;
        }

        public override string ToString() => $"{this.Name}({string.Join(',', this.ParameterTypes)}): {this.ReturnType}";

    }

}
