using System;
using System.IO;
using System.Text.Json;

namespace HSharp.IO {
    
    public enum SourceProjectType {
        ConsoleApplication,
        WindowApplication,
        Library,
    }

    [Serializable]
    public class SourceProject {

        private SourceProjectFile[] m_files;
        private SourceProjectReference[] m_references;

        public string Name { get; set; }

        public string Output { get; set; }

        public SourceProjectType ProjectType { get; set; }

        public SourceProjectFile[] Sources { get => this.m_files; set => this.m_files = value; }

        public SourceProjectReference[] References { get => this.m_references; set => this.m_references = value; }

        public SourceProject() {
            this.Name = "Untitled Project";
            this.Output = "No Output";
            this.m_files = Array.Empty<SourceProjectFile>();
            this.m_references = Array.Empty<SourceProjectReference>();
            this.ProjectType = SourceProjectType.ConsoleApplication;
        }

        public SourceProject(string name, string output, SourceProjectType projectType, params SourceProjectFile[] files) {
            
            // Assign project type
            this.ProjectType = projectType;
            
            // Set private fields
            this.m_files = files;
            this.m_references = Array.Empty<SourceProjectReference>();
            
            // Add project files
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

        public void SetReferences(params SourceProjectReference[] references) => this.m_references = references;

        public void SaveProject(string saveFile) => File.WriteAllText(saveFile, JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true }));

        public static SourceProject FromJson(string jsonFile) => JsonSerializer.Deserialize<SourceProject>(File.ReadAllText(jsonFile));

    }

}
