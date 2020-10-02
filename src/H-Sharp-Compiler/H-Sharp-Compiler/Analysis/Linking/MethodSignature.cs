using System.Collections.Generic;
using HSharp.Analysis.TypeData;

namespace HSharp.Analysis.Linking {
    
    public class MethodSignature {
    
        public string Name { get; }

        public ClassType Owner { get; }

        public HSharpType ReturnType { get; }

        public List<HSharpType> ParameterTypes { get; }

        public MethodSignature(string methodname, ClassType owner, HSharpType returnType, List<HSharpType> parameters) {
            this.Name = methodname;
            this.Owner = owner;
            this.ReturnType = returnType;
            this.ParameterTypes = parameters;
        }

        public override string ToString() => $"{this.Name}({string.Join(',', this.ParameterTypes)}): {this.ReturnType}";

    }

}
