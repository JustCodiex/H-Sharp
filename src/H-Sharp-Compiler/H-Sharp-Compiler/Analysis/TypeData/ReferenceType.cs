using System;

namespace HSharp.Analysis.TypeData {

    public class ReferenceType : HSharpType, IValType {

        public override bool IsReferenceType => true;

        public override ushort Size => 4;

        public override string Name => $"Ref({this.ReferencedType.Name})";

        public HSharpType ReferencedType { get; }

        public ReferenceType(HSharpType refType) {
            if (refType is not IRefType) {
                throw new ArgumentException();
            }
            this.ReferencedType = refType;
        }

        public override string ToString() => this.Name;

    }

}
