using System;

namespace HSharp.IO {
    
    public class SourceProject {

        private SourceProjectFile[] m_files;

        public SourceProjectFile[] Sources => this.m_files;

        public string Name { get; set; }

        public string Output { get; set; }

        public SourceProject(string name, string output, params SourceProjectFile[] files) {
            this.m_files = files;
            if (this.m_files.Length > 0) {
                if (!this.m_files[0].IsVirtual) {
                    this.Name = string.IsNullOrEmpty(name) ? this.m_files[0].Name : name;
                    this.Output = string.IsNullOrEmpty(output) ? this.m_files[0].Value.Replace(".hsharp", ".bin") : output;
                } else if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(output)) {
                    throw new ArgumentException("name or output parameter was null - this is not allowed for virtual projects");
                } else {
                    this.Name = name;
                    this.Output = output;
                }
            }
        }

    }

}
