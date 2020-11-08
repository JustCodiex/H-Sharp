namespace HSharp.Analysis.TypeData {

    public class VoidType : HSharpType, IValType {

        private static VoidType __self = new VoidType();

        public static VoidType Void => __self;

        public override bool IsReferenceType => false;

        public bool IsPrimitive => true;

        public override ushort Size => 0;

        public override string Name => "void";

        private VoidType() { } // Hide ctor for public use

    }

}
