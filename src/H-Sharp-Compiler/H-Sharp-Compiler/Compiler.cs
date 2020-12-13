using System.Diagnostics;
using HSharp.Analysis;
using HSharp.Analysis.Linking;
using HSharp.Analysis.Typechecking;
using HSharp.Analysis.Verifying;
using HSharp.Compiling;
using HSharp.IO;
using HSharp.Metadata;
using HSharp.Parsing;
using HSharp.Parsing.AbstractSnyaxTree;

namespace HSharp {
    
    public class Compiler {
    
        public CompileResult CompileProject(SourceProject project) {

            var _thisLog = new Log();
            var _thistimer = Stopwatch.StartNew();

            AST[] asts = new AST[project.Sources.Length];
            for (int i = 0; i < project.Sources.Length; i++) {
                var parseResult = this.ParseFile(project.Sources[i], out AST ast);
                if (!parseResult) {
                    Log.WriteLine($"Failed to parse {project.Sources[i].Name} :\n\t{"Some currently un-generated error!"}");
                    Log.WriteLine();
                    return parseResult;
                }
                asts[i] = ast;
            }

            // Create the global domain
            Domain globalDomain = Domain.GetGlobalDomain();

            // Detect types
            CompileResult result = StaticTypeDetector.Detect(asts, globalDomain);
            if (!result) {
                Log.WriteLine($"Compile Error \"{project.Name}\" : {result}");
                Log.WriteLine();
                _thisLog.SaveAndClose(project.Output.Replace(".bin", ".log"));
                return result;
            }

            // Define types
            result = StaticTypeDefiner.DefineAllTypes(asts, globalDomain);
            if (!result) {
                Log.WriteLine($"Compile Error \"{project.Name}\" : {result}");
                Log.WriteLine();
                _thisLog.SaveAndClose(project.Output.Replace(".bin", ".log"));
                return result;
            }

            // Solve potential 
            result = InheritanceSolver.Solve(asts, globalDomain);
            if (!result) {
                Log.WriteLine($"Compile Error \"{project.Name}\" : {result}");
                Log.WriteLine();
                _thisLog.SaveAndClose(project.Output.Replace(".bin", ".log"));
                return result;
            }

            // TODO: Run static checks

            VarsVerifier vVerifier = new VarsVerifier();
            for (int i = 0; i < asts.Length; i++) {
                result = vVerifier.Vars(asts[i]);
                if (!result) {
                    Log.WriteLine($"Compile Error \"{project.Name}\" : {result}");
                    Log.WriteLine();
                    _thisLog.SaveAndClose(project.Output.Replace(".bin", ".log"));
                    return result;
                }
            }

            ControlpathVerifier pathVerifier = new ControlpathVerifier();
            for (int i = 0; i < asts.Length; i++) {
                result = pathVerifier.Verify(asts[i]);
                if (!result) {
                    Log.WriteLine($"Compile Error \"{project.Name}\" : {result}");
                    Log.WriteLine();
                    _thisLog.SaveAndClose(project.Output.Replace(".bin", ".log"));
                    return result;
                }
            }

            Typechecker typechecker = new Typechecker();
            for (int i = 0; i < asts.Length; i++) {
                result = typechecker.Typecheck(asts[i], globalDomain);
                if (!result) {
                    Log.WriteLine($"Compile Error \"{project.Name}\" : {result}");
                    Log.WriteLine();
                    _thisLog.SaveAndClose(project.Output.Replace(".bin", ".log"));
                    return result;
                }
            }

            ASTCompiler astCompiler = new ASTCompiler(asts);
            result = astCompiler.Compile();
            if (!result) {
                Log.WriteLine("Failed to compile project. \n\tSome currently not-generated error");
                Log.WriteLine();
                return result;
            }

            _thistimer.Stop();

            ProgramOutput compilerOutput = astCompiler.GetProgram();

            if (compilerOutput is not null) {
                compilerOutput.Save(project.Output);
                compilerOutput.SaveAsText(project.Output.Replace(".bin", ".txt"));
                Log.WriteLine($"Compiled \"{project.Name}\" successfully in {_thistimer.ElapsedMilliseconds / 1000.0}s.");
                Log.WriteLine();
                _thisLog.SaveAndClose(project.Output.Replace(".bin", ".log"));
                return new CompileResult(true);
            } else {
                Log.WriteLine($"Compile Error \"{project.Name}\" : {result}");
                Log.WriteLine();
                _thisLog.SaveAndClose(project.Output.Replace(".bin", ".log"));
                return new CompileResult(false);
            }

        }

        private CompileResult ParseFile(SourceProjectFile sourceFile, out AST ast) {

            Lexer lexer = new Lexer();
            LexToken[] tokens = sourceFile.IsVirtual ? lexer.Lex(sourceFile.Value) : lexer.LexFile(sourceFile.Value);

            Log.WriteLine($"Parsing {sourceFile.Name}");

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

    }

}
