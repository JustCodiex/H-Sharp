using System;
using System.IO;

namespace HSharp.IO {
    
    [Serializable]
    public record SourceProjectFile(string Name, string Value, bool IsVirtual) {
        public SourceProjectFile() : this(string.Empty, string.Empty, true) { }

        public static SourceProjectFile FromSource(string path) => new SourceProjectFile(Path.GetFileNameWithoutExtension(path), path, false);

        public static SourceProjectFile FromText(string txt) => new SourceProjectFile("0.hsharp", txt, true);

    }

}
