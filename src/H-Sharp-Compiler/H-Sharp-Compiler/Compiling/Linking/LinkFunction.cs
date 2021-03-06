namespace HSharp.Compiling.Linking {
    
    public abstract class LinkFunction {
        public ulong Ptr { get; protected set; }
    }

    public class LinkInternalPtr : LinkFunction { 
        public LinkInternalPtr(ulong ptr) {
            this.Ptr = ptr;
        }
        public override string ToString() => this.Ptr.ToString();
    }

    public class LinkBindPtr : LinkFunction {
        public string Bind { get; }
        public LinkBindPtr(string bindSource, ulong importTablePtr) {
            this.Bind = bindSource;
            this.Ptr = importTablePtr;
        }
        public override string ToString() => $"{this.Bind}@{this.Ptr}";
    }

}
