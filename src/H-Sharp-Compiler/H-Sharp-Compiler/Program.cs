using System;
using System.IO;
using HSharp.IO;

namespace HSharp {
    
    public class Program {

        private static string inputFile;
        private static string outputFile;
        private static bool regressionTest;

        public static Compiler Current { get; set; }

        public static void Main(string[] args) {

            ParseArguments(args);

            Current = new Compiler();

            if (inputFile is not null && outputFile is not null) {



            }

            if (regressionTest) {

                string[] source = Directory.GetFiles("regression\\", "*.hsharp");
                for (int i = 0; i < source.Length; i++) {
                    SourceProject project = new SourceProject(source[i]);
                    var r = Current.CompileProject(project);
                    if (r) {
                        Console.WriteLine($"Compiled \"{source[i]}\" successfully.");
                    } else {
                        Console.WriteLine($"Compiled \"{source[i]}\" with error: {r}");
                    }
                }

            }

            Console.ReadLine();

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
