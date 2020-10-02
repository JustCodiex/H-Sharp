namespace HSharp.Analysis.TypeData {

    public abstract class HSharpType : Domain {
    
        public abstract bool IsReferenceType { get; }

        public abstract ushort Size { get; }

    }

}
