using System.IO;

namespace HSharp.IO {
    
    public class SourceProject {

        private string[] m_files;

        public string[] CodeFiles => this.m_files;

        public string Name { get; set; }

        public string Output { get; set; }

        public SourceProject(params string[] files) {
            this.m_files = files;
            if (this.m_files.Length > 0) {
                this.Name = Path.GetFileNameWithoutExtension(this.m_files[0]);
                this.Output = this.m_files[0].Replace(".hsharp", ".bin");
            }
        }

    }

}
