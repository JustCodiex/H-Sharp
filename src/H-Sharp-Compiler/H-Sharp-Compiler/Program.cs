using System;
using System.IO;
using HSharp.IO;
using HSharp.Metadata;

namespace HSharp {
    
    public class Program {

        private static string inputFile = null;
        private static string outputFile = null;
        private static bool regressionTest = false;
        private static bool isStdLibCompile = false;

        public static Compiler Current { get; set; }

        public static void Main(string[] args) {

            string envSource = new string(Environment.CurrentDirectory);

            ParseArguments(args);

            Current = new Compiler();
            Log _fullLog = new Log();

            if (inputFile is not null && outputFile is not null) {

            } else if (inputFile is not null && outputFile is null) {

                if (File.Exists(inputFile)) {

                    bool isProject = inputFile.EndsWith(".hsharpproj");

                    if (isProject) {
                        Log.WriteLine($"Compiling project {inputFile}.");
                        SourceProject project = SourceProject.FromJson(inputFile);
                        Log.WriteLine($"\nName: {project.Name}");
                        Log.WriteLine($"Files: {project.Sources.Length}");
                        if (project.Sources.Length > 0) {
                            Log.WriteLine($"Type: {project.ProjectType}");
                            Environment.CurrentDirectory = Path.GetDirectoryName(inputFile);
                            _ = Current.CompileProject(project);
                        } else {
                            Log.WriteLine(" ERROR: Unable to compile empty project.");
                        }
                    } else {
                        Log.WriteLine(" ERROR: Can only compile project files if no output file is given.");
                    }
                
                } else {
                    Log.WriteLine($" ERROR: Failed to locate input file {inputFile}.");
                }

            }

            // If regression
            if (regressionTest) {

                int s = 0, f = 0;
                string[] source = Directory.GetFiles("regression\\", "*.hsharp");

                for (int i = 0; i < source.Length; i++) {
                    SourceProject project = new SourceProject(null, null, SourceProjectType.ConsoleApplication, SourceProjectFile.FromSource(source[i]));
                    var r = Current.CompileProject(project);
                    if (r) {
                        s++;
                    } else {
                        f++;
                    }
                    project.SaveProject(source[i] + "proj");
                }

                Log.WriteLine($"Regression test: {s} succeeded, {f} failed. ({s + f} total)");

            }

            // Set back environment source
            Environment.CurrentDirectory = envSource;

            // Save the full log
            _fullLog.SaveAndClose("last-run.log");

        }

        public static void ParseArguments(string[] args) {

            for (int i = 0; i < args.Length; i++) {

                if (args[i].CompareTo("-c") == 0 && i + 1 < args.Length) {
                    inputFile = args[++i].Trim('"');
                } else if (args[i].CompareTo("-o") == 0 && i + 1 < args.Length) {
                    outputFile = args[++i].Trim('"');
                } else if (args[i].CompareTo("-testall") == 0) {
                    regressionTest = true;
                } else if (args[i].CompareTo("-stdlib_bind") == 0) {
#if DEBUG
                    isStdLibCompile = true;
#endif
                }

            }

        }

    }

}
