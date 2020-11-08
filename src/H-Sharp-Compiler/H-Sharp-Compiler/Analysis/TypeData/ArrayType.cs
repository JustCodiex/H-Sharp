namespace HSharp.Analysis.TypeData {
    
    public class ArrayType : ReferenceType {

        public override string Name => $"{this.ReferencedType.Name}[]";

        public override ushort Size { get; }

        public ArrayType(HSharpType type) : base(type) {
        }

        public override bool Equals(object obj) {
            if (obj is ArrayType other) {
                return this.ReferencedType == other.ReferencedType;
            } else {
                return false;
            }
        }

        public override string ToString() => this.Name;

        public override int GetHashCode() => base.GetHashCode();

    }

}
