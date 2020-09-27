using System;
using System.Collections.Generic;
using System.Text;
using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree;

namespace HSharp.Compiling {

    public class ASTCompiler {

        private AST[] m_asts;
        private CompiledFunction[] m_compiledFuncs;

        public ASTCompiler(AST[] asts) {
            this.m_asts = asts;
        }

        public ProgramOutput GetProgram() {

            ProgramOutput program = new ProgramOutput();
            Dictionary<string, long> offsets = new Dictionary<string, long>();
            List<ByteInstruction> allInstructions = new List<ByteInstruction>();
            long offset = 0;

            for (int i = 0; i < this.m_compiledFuncs.Length; i++) {

                offsets.Add(this.m_compiledFuncs[i].Name, offset);

                for (int j = 0; j < this.m_compiledFuncs[i].Instructions.Count; j++) {
                    ByteInstruction instruction = this.m_compiledFuncs[i].Instructions[j];
                    allInstructions.Add(instruction);
                    offset += instruction.GetSize();
                }

                long remainder = offset % 4;
                if (remainder != 0) {
                    for (int j = 0; j < remainder; j++) {
                        allInstructions.Add(new ByteInstruction(Bytecode.NOP));
                    }
                    offset += remainder;
                }

            }

            program.SetInstructions(allInstructions.ToArray());
            program.SetOffsets(offsets, offset);

            return program;

        }

        public CompileResult Compile() {

            foreach (AST ast in this.m_asts) {
                CompileResult cResult = this.CompileAst(ast);
                if (!cResult) {
                    return cResult;
                }
            }

            return new CompileResult(true);

        }

        private CompileResult CompileAst(AST ast) {

            List<CompiledFunction> compiledInstructions = new List<CompiledFunction>();

            CompileUnitNode unit = ast.Root;
            CompiledFunction topLevelFunc = new CompiledFunction("");

            CompileContext context = new CompileContext();
            foreach (ASTNode node in unit) {

                // TODO: Check node for being a declaration of any kind

                // ... else:
                topLevelFunc.Instructions.AddRange(this.CompileNode(node, context));
                if (!context.Result) {
                    return context.Result;
                }

            }

            compiledInstructions.Add(topLevelFunc);

            this.m_compiledFuncs = compiledInstructions.ToArray();

            return new CompileResult(true);

        }

        static List<ByteInstruction> Instruction(ByteInstruction instruction) 
            => new List<ByteInstruction>() { instruction };

        private List<ByteInstruction> CompileNode(ASTNode node, CompileContext context) {
            List<ByteInstruction> instructions = node switch
            {
                IntLitNode intLitNode => this.CompileConstant(intLitNode),
                IdentifierNode idNode => Instruction(new ByteInstruction(Bytecode.PUSH, context.Lookup(idNode.Content))),
                BinOpNode binOpNode => this.CompileBinaryOperation(binOpNode, context),
                VarDeclNode vDeclNode => this.CompileVariableDeclaration(vDeclNode, context),
                ScopeNode scopeNode => this.CompileScope(scopeNode, context),
                GroupedExpressionNode groupNode => this.CompileGroupedExpression(groupNode, context),
                _ => null,
            };
            return instructions;
        }

        private List<ByteInstruction> CompileBinaryOperation(BinOpNode op, CompileContext context) {
            List<ByteInstruction> instructions = new List<ByteInstruction>();
            instructions.AddRange(this.CompileNode(op.Left, context));
            instructions.AddRange(this.CompileNode(op.Right, context));
            if (!context.Result) { return null; }
            Bytecode bytecode = op.Content switch
            {
                "+" => Bytecode.ADD,
                "-" => Bytecode.SUB,
                "/" => Bytecode.DIV,
                "*" => Bytecode.MUL,
                _ => Bytecode.NOP,
            };
            if (bytecode is Bytecode.NOP) {
                context.UpdateResultIfErr(new CompileResult(false));
                return null;
            }
            instructions.Add(new ByteInstruction(bytecode));
            return instructions;
        }

        private List<ByteInstruction> CompileVariableDeclaration(VarDeclNode decl, CompileContext context) {
            List<ByteInstruction> instructions = new List<ByteInstruction>();
            instructions.AddRange(this.CompileNode(decl.AssignToExpr, context));
            if (!context.Result) {
                return null;
            }
            instructions.Add(new ByteInstruction(Bytecode.ENTER, context.Enter(decl.VarName)));
            return instructions;
        }

        private List<ByteInstruction> CompileConstant(ASTNode node) {
            List<ByteInstruction> instructions = new List<ByteInstruction>();
            switch (node) {
                case IntLitNode ilit:
                    instructions.Add(new ByteInstruction(Bytecode.LCSI32, ilit.Integer));
                    break;
                default:
                    break;
            }
            return instructions;
        }

        private List<ByteInstruction> CompileScope(ScopeNode node, CompileContext context) {
            List<ByteInstruction> instructions = new List<ByteInstruction>();
            for (int i = 0; i < node.Nodes.Count; i++) {
                instructions.AddRange(this.CompileNode(node[i], context));
                if (!context.Result) {
                    return null;
                }
            }
            // foreach variable introdced in scope
                // exit scope
            return instructions;
        }

        private List<ByteInstruction> CompileGroupedExpression(GroupedExpressionNode node, CompileContext context) {
            List<ByteInstruction> instructions = new List<ByteInstruction>();
            for (int i = 0; i < node.Nodes.Count; i++) {
                instructions.AddRange(this.CompileNode(node[i], context));
                if (!context.Result) {
                    return null;
                }
            }
            return instructions;
        }

    }

}
