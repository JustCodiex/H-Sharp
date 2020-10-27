using System.IO;
using HSharp.IO;
using HSharp.Metadata;

namespace HSharp {
    
    public class Program {

        private static string inputFile;
        private static string outputFile;
        private static bool regressionTest;

        public static Compiler Current { get; set; }

        public static void Main(string[] args) {

            ParseArguments(args);

            Current = new Compiler();
            Log _fullLog = new Log();

            if (inputFile is not null && outputFile is not null) {



            }

            if (regressionTest) {

                int s = 0, f = 0;
                string[] source = Directory.GetFiles("regression\\", "*.hsharp");

                for (int i = 0; i < source.Length; i++) {
                    SourceProject project = new SourceProject(source[i]);
                    var r = Current.CompileProject(project);
                    if (r) {
                        s++;
                    } else {
                        f++;
                    }
                }

                Log.WriteLine($"Regression test: {s} succeeded, {f} failed. ({s + f} total)");

            }

            _fullLog.SaveAndClose("last-run.log");

        }

        public static void ParseArguments(string[] args) {

            for (int i = 0; i < args.Length; i++) {

                if (args[i].CompareTo("-c") == 0 && i + 1 < args.Length) {
                    inputFile = args[i++];
                } else if (args[i].CompareTo("-o") == 0 && i + 1 < args.Length) {
                    outputFile = args[i++];
                } else if (args[i].CompareTo("-testall") == 0) {
                    regressionTest = true;
                }

            }

        }

    }

}
