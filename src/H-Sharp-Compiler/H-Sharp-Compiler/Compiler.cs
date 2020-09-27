using System;
using HSharp.Analysis.Verifying;
using HSharp.Compiling;
using HSharp.IO;
using HSharp.Parsing;
using HSharp.Parsing.AbstractSnyaxTree;

namespace HSharp {
    
    public class Compiler {
    
        public CompileResult CompileProject(SourceProject project) {

            AST[] asts = new AST[project.CodeFiles.Length];
            for (int i = 0; i < project.CodeFiles.Length; i++) {
                var parseResult = this.ParseFile(project.CodeFiles[i], out AST ast);
                if (!parseResult) {
                    Console.WriteLine($"Failed to parse {project.CodeFiles[i]} :\n\t{"Some currently un-generated error!"}");
                    return parseResult;
                }
                asts[i] = ast;
            }

            VarsVerifier vVerifier = new VarsVerifier();
            for (int i = 0; i < asts.Length; i++) {
                var varsResult = vVerifier.Vars(asts[i]);
                if (!varsResult) {
                    return varsResult;
                }
            }

            // TODO: Run static checks

            ASTCompiler astCompiler = new ASTCompiler(asts);
            CompileResult result = astCompiler.Compile();
            if (!result) {
                Console.WriteLine("Failed to compile project. \n\tSome currently not-generated error");
                return result;
            }

            ProgramOutput compilerOutput = astCompiler.GetProgram();

            if (compilerOutput is not null) {
                compilerOutput.Save(project.Output);
                return new CompileResult(true);
            } else {
                return new CompileResult(false);
            }

        }

        private CompileResult ParseFile(string sourceFilePath, out AST ast) {

            Lexer lexer = new Lexer();
            LexToken[] tokens = lexer.Lex(sourceFilePath);

            Console.WriteLine($"Parsing {sourceFilePath}");

            ASTBuilder builder = new ASTBuilder(tokens);
            ParsingResult result = builder.Parse();
            if (result.Success) {
                ast = builder.Build();
                ast.SetSource(sourceFilePath);
                return new CompileResult(true);
            } else {
                ast = null;
                return new CompileResult(false);
            }

        }

    }

}
