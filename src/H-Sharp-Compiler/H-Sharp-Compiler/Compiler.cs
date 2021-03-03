using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using HSharp.Analysis;
using HSharp.Analysis.Linking;
using HSharp.Analysis.Typechecking;
using HSharp.Analysis.Verifying;
using HSharp.Compiling;
using HSharp.Compiling.Linking;
using HSharp.IO;
using HSharp.Metadata;
using HSharp.Parsing;
using HSharp.Parsing.AbstractSnyaxTree;

namespace HSharp {
    
    public class Compiler {

        private Log m_thisLog;
        private SourceProject m_currentProject;
        private Stopwatch m_timer;
        private CompileResult m_lastResult;

        public bool HasProject => this.m_currentProject is not null;

        public double ElapsedTime => this.m_timer.ElapsedMilliseconds / 1000.0;

        public CompileResult Result => this.m_lastResult;

        public void Reset() {
            this.m_thisLog = null;
            this.m_currentProject = null;
            this.m_timer = null;
            this.m_lastResult = new CompileResult(true, "No compilation");
        }

        public CompileResult CompileProject(SourceProject project) {

            // Set active project
            this.m_currentProject = project;

            // Meta data generation
            this.m_thisLog = new Log();
            this.m_timer = Stopwatch.StartNew();

            // Parse all files
            AST[] asts = new AST[this.m_currentProject.Sources.Length];
            for (int i = 0; i < this.m_currentProject.Sources.Length; i++) {
                var parseResult = ParseFile(this.m_currentProject.Sources[i], out AST ast);
                if (!parseResult) {
                    Log.WriteLine($"Failed to parse {this.m_currentProject.Sources[i].Name} :\n\t{"Some currently un-generated error!"}");
                    Log.WriteLine();
                    return parseResult;
                }
                asts[i] = ast;
            }

            // Create the global domain and import external references
            Domain globalDomain = Domain.GetGlobalDomain();
            var result = ImportReferences(globalDomain, this.m_currentProject);
            if (!result) {
                return this.FatalError(result);
            }

            // Detect types
            result = StaticTypeDetector.Detect(asts, globalDomain);
            if (!result) {
                return this.FatalError(result);
            }

            // Define types
            result = StaticTypeDefiner.DefineAllTypes(asts, globalDomain);
            if (!result) {
                return this.FatalError(result);
            }

            // Solve potential inheritance problems etc.
            result = InheritanceSolver.Solve(asts, globalDomain);
            if (!result) {
                return this.FatalError(result);
            }

            // TODO: Run static checks

            // Verify variables
            VarsVerifier vVerifier = new VarsVerifier();
            for (int i = 0; i < asts.Length; i++) {
                result = vVerifier.Vars(asts[i]);
                if (!result) {
                    return this.FatalError(result);
                }
            }

            // Verify control paths
            ControlpathVerifier pathVerifier = new ControlpathVerifier();
            for (int i = 0; i < asts.Length; i++) {
                result = pathVerifier.Verify(asts[i]);
                if (!result) {
                    return this.FatalError(result);
                }
            }

            // Run static  typecheck
            Typechecker typechecker = new Typechecker();
            for (int i = 0; i < asts.Length; i++) {
                result = typechecker.Typecheck(asts[i], globalDomain);
                if (!result) {
                    return this.FatalError(result);
                }
            }

            // Compile Application
            ASTCompiler astCompiler = new ASTCompiler(asts);
            result = astCompiler.Compile();
            if (!result) {
                return this.FatalError(result);
            }

            // Apply linking
            Linker linker = new Linker(astCompiler, globalDomain);
            result = linker.Link();
            if (!result) {
                return this.FatalError(result);
            }

            // Stop timer
            this.m_timer.Stop();

            // Get compile output
            ProgramOutput compilerOutput = this.GetCompileOutput(astCompiler, linker, globalDomain);

            // If successful compile, save, otherwise log error
            if (compilerOutput is not null) {
                compilerOutput.Save(project.Output);
                compilerOutput.SaveAsText(project.Output.Replace(".bin", ".txt").Replace(".hlib", ".txt"));
                Log.WriteLine($"Compiled \"{project.Name}\" successfully in {this.m_timer.ElapsedMilliseconds / 1000.0}s.");
                Log.WriteLine();
                this.m_thisLog.SaveAndClose(project.Output.Replace(".bin", ".log").Replace(".hlib", ".log"));
                return new CompileResult(true);
            } else {
                Log.WriteLine($"Compile Error \"{project.Name}\" : {result}");
                Log.WriteLine();
                this.m_thisLog.SaveAndClose(project.Output.Replace(".bin", ".log").Replace(".hlib", ".log"));
                return new CompileResult(false);
            }

        }

        private CompileResult FatalError(CompileResult result) {
            Log.WriteLine($"Fatal compile error encountered after {this.m_timer.ElapsedMilliseconds / 1000.0 :0.00}s");
            Log.WriteLine($"Compile Error \"{this.m_currentProject.Name}\" : {result}");
            Log.WriteLine();
            this.m_thisLog.SaveAndClose(this.m_currentProject.Output.Replace(".bin", ".log").Replace(".hlib", ".log"));
            return this.m_lastResult = result;
        }

        private static CompileResult ParseFile(SourceProjectFile sourceFile, out AST ast) {

            // If not virtual, make sure the file exists
            if (!sourceFile.IsVirtual) {
                if (!File.Exists(sourceFile.Value)) {
                    ast = null;
                    return new CompileResult(false, $"Source file \"{sourceFile.Value}\" not found.");
                }
            }

            // Lex program
            Lexer lexer = new Lexer();
            LexToken[] tokens = sourceFile.IsVirtual ? lexer.Lex(sourceFile.Value) : lexer.LexFile(sourceFile.Value);

            // Build AST and return it
            ASTBuilder builder = new ASTBuilder(tokens);
            ParsingResult result = builder.Parse();
            if (result.Success) {
                ast = builder.Build();
                ast.SetSource(sourceFile);
                return new CompileResult(true);
            } else {
                ast = null;
                return new CompileResult(false);
            }

        }

        private static CompileResult ImportReferences(Domain globalDomain, SourceProject project) {

            Log.WriteLine(" WARNING: References not handled");

            return new CompileResult(true);

        }

        private ProgramOutput GetCompileOutput(ASTCompiler compiler, Linker linker, Domain globalDomain) {

            ProgramOutput program = new ProgramOutput();
            Dictionary<string, long> offsets = new Dictionary<string, long>();
            List<ByteInstruction> allInstructions = new List<ByteInstruction>();
            long offset = 0;

            var context = compiler.GetCompileContext();
            var funcs = compiler.GetCompiledBytecode();

            if (funcs is not null) {

                for (int i = 0; i < funcs.Length; i++) {

                    offsets.Add(funcs[i].Name, offset);

                    for (int j = 0; j < funcs[i].Instructions.Count; j++) {
                        ByteInstruction instruction = funcs[i].Instructions[j];
                        allInstructions.Add(instruction);
                        offset += instruction.GetSize();
                    }

                    long remainder = offset % 4;
                    if (remainder > 0) {
                        for (int j = 0; j < remainder; j++) {
                            allInstructions.Add(new ByteInstruction(Bytecode.NOP));
                        }
                        offset += remainder;
                    }

                }

            }

            // Set instructions, offsets, bytes, strings etc...
            program.SetProgramType(this.m_currentProject.ProjectType);
            program.SetInstructions(allInstructions.ToArray());
            program.SetOffsets(offsets, offset);
            program.SetBytes(context?.ConstBytes);
            program.SetStrings(context?.Strings);
            program.SetDeclaredTypes(linker.ExportTypes);

            // Set program
            return program;

        }

    }

}
