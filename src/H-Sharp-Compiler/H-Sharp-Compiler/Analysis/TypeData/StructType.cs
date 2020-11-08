using System;

namespace HSharp.Analysis.TypeData {
    
    public class StructType : HSharpType, IValType, IExtendableType {

        public override bool IsReferenceType => false;

        public override string Name { get; }

        public override ushort Size { get; }

        public bool IsPrimitive => false;

        public IExtendableType Base { get; }

        public bool IsExtensionOf(IExtendableType type) => throw new NotImplementedException();
    }

}
