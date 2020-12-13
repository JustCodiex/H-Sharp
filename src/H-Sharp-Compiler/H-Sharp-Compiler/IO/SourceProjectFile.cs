using System.IO;

namespace HSharp.IO {
    
    public readonly struct SourceProjectFile {
    
        public string Name { get; }

        public string Value { get; }

        public bool IsVirtual { get; }

        private SourceProjectFile(string name, string val, bool isVirt) {
            this.Name = name;
            this.Value = val;
            this.IsVirtual = isVirt;
        }

        public static SourceProjectFile FromSource(string path) => new SourceProjectFile(Path.GetFileNameWithoutExtension(path), path, false);

        public static SourceProjectFile FromText(string txt) => new SourceProjectFile("0.hsharp", txt, true);

    }

}
