namespace HSharp.IO {

    public readonly struct DllFunction {

        public readonly string Class;
        public readonly string Name;
        public readonly string Mangle;
        public readonly string Source;

        public DllFunction(string klass, string name, string mangle, string source) {
            this.Class = klass;
            this.Name = name;
            this.Mangle = mangle;
            this.Source = source;
        }

        public override string ToString() => $"{this.Class}_{this.Name}{this.Mangle}";

    }


}
