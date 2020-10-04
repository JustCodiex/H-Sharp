using System.Collections.Generic;

namespace HSharp.Analysis.TypeData {
    
    public class FunctionType : HSharpType, IValType {
    
        public override string Name { get; }

        public ClassType Owner { get; }

        public HSharpType ReturnType { get; }

        public List<HSharpType> ParameterTypes { get; }

        public override bool IsReferenceType => false;

        public override ushort Size => 0;

        public FunctionType(string methodname, ClassType owner, HSharpType returnType, List<HSharpType> parameters) {
            this.Name = methodname;
            this.Owner = owner;
            this.ReturnType = returnType;
            this.ParameterTypes = parameters;
        }

        public override string ToString() => $"{this.Name}({string.Join(',', this.ParameterTypes)}): {this.ReturnType}";

    }

}
