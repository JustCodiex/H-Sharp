namespace HSharp.Analysis.TypeData {

    public class ValueType : HSharpType, IValType { // may only be defined by the compiler

        public override bool IsReferenceType => false;

        public override string Name { get; }

        public override ushort Size { get; }

        public ValueType(string name, ushort size) {
            this.Name = name;
            this.Size = size;
        }

        public override string ToString() => this.Name;

    }

}
