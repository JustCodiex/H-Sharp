using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;
using HSharp.Parsing.AbstractSnyaxTree.Expression;
using HSharp.Parsing.AbstractSnyaxTree.Literal;
using HSharp.Parsing.AbstractSnyaxTree.Statement;
using HSharp.Parsing.AbstractSnyaxTree.Initializer;
using HSharp.Compiling.Hint;
using HSharp.Compiling.Branching;
using HSharp.Analysis.TypeData;
using ValueType = HSharp.Analysis.TypeData.ValueType;
using HSharp.Parsing.AbstractSnyaxTree.Directive;

namespace HSharp.Compiling {

    public class ASTCompiler {

        private AST[] m_asts;
        private CompiledFunction[] m_compiledFuncs;
        private CompileContext m_context;

        public ASTCompiler(AST[] asts) {
            this.m_asts = asts;
        }

        public AST[] GetTrees() => this.m_asts;

        public CompiledFunction[] GetCompiledBytecode() => this.m_compiledFuncs;

        public CompileContext GetCompileContext() => this.m_context;

        public CompileResult Compile() {

            foreach (AST ast in this.m_asts) { // TODO: Merge into one
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
            CompiledFunction topLevelFunc = new CompiledFunction($"_<{Path.GetFileNameWithoutExtension(ast.Source.Name)}>");

            CompileContext context = new CompileContext();
            foreach (ASTNode node in unit) {
                var nodeResult = this.CompileToplevel(node, context, topLevelFunc, compiledInstructions);
                if (!nodeResult) {
                    return nodeResult;
                }
            }

            compiledInstructions.Add(topLevelFunc);

            this.m_compiledFuncs = compiledInstructions.ToArray();
            this.m_context = context;

            return new CompileResult(true);

        }

        private CompileResult CompileToplevel(ASTNode node, CompileContext context, CompiledFunction topLevelFunc, List<CompiledFunction> compiledInstructions) {

            if (node is IDecl decl && node is not VarDeclNode) {

                compiledInstructions.AddRange(this.CompileDeclaration(decl, context));
                if (!context.Result) {
                    return context.Result;
                }

            } else if (node is NamespaceDirectiveNode namespaceDirective) {

                foreach (var subNode in namespaceDirective.Body.Nodes) {
                    var nodeResult = this.CompileToplevel(subNode, context, null, compiledInstructions);
                    if (!nodeResult) {
                        return nodeResult;
                    }
                }

            } else {

                if (topLevelFunc is null) {
                    return new CompileResult(false, $"Failed to compile a top-level statement that was outside the top-level.").SetOrigin(node);
                }

                topLevelFunc.Instructions.AddRange(this.CompileNode(node, context));
                if (!context.Result) {
                    return context.Result;
                }

            }

            return new CompileResult(true);

        }

        private List<CompiledFunction> CompileDeclaration(IDecl decl, CompileContext context) {

            switch (decl) {
                case FuncDeclNode funcDecl:
                    return new List<CompiledFunction>() { this.CompileFunction(funcDecl, context) };
                case ClassDeclNode classDecl:
                    List<CompiledFunction> funcs = new List<CompiledFunction>();
                    funcs.AddRange(classDecl.Methods.Select(x => this.CompileFunction(x, context)));
                    if (!context.Result) {
                        return new List<CompiledFunction>();
                    }
                    bool allPass = classDecl.Classes.Select(x => this.CompileDeclaration(x, context)).All(k => {
                        funcs.AddRange(k);
                        return context.Result;
                    });
                    if (!allPass) {
                        return new List<CompiledFunction>();
                    }
                    return funcs;
                default:
                    return new List<CompiledFunction>();
            }

        }

        static List<ByteInstruction> Instruction(ByteInstruction instruction) 
            => new List<ByteInstruction>() { instruction };

        private CompiledFunction CompileFunction(FuncDeclNode node, CompileContext context) {

            List<ByteInstruction> instructions = new List<ByteInstruction>();

            for (int i = 0; i < node.Params.Count; i++) {
                instructions.Add(new ByteInstruction(Bytecode.ENTER, node.Params.GetParameterId(node.Params.Count - 1 - i).Index));
            }

            instructions.AddRange(this.CompileNode(node.Body, context));
            if (!context.Result) {
                return null;
            }

            instructions.Add(new ByteInstruction(Bytecode.RET));

            return new CompiledFunction(node.Name) {
                Instructions = instructions
            };

        }

        public List<ByteInstruction> CompileNode(ASTNode node, CompileContext context) {
            List<ByteInstruction> instructions = node switch
            {
                ThisNode => Instruction(new ByteInstruction(Bytecode.PUSH, 0)),
                BaseNode => Instruction(new ByteInstruction(Bytecode.PUSH, 0)),
                ILiteral litNode => this.CompileConstant(litNode as ASTNode),
                IdentifierNode idNode => this.CompileIdentifier(idNode),
                AssignmentNode assignmentNode => this.CompileAssignmentOperation(assignmentNode, context),
                BinOpNode binOpNode => this.CompileBinaryOperation(binOpNode, context),
                UnaryOpNode unaryOpNode => this.CompileUnaryOperation(unaryOpNode, context),
                VarDeclNode vDeclNode => this.CompileVariableDeclaration(vDeclNode, context),
                CallNode callNode => this.CompileCallExpression(callNode, context),
                ScopeNode scopeNode => this.CompileScope(scopeNode, context),
                ExpressionNode groupNode => this.CompileGroupedExpression(groupNode, context),
                MemberAccessNode accessNode => this.CompileMemberAccessNode(accessNode, context),
                ReturnStatement returnNode => this.CompileNode(returnNode.Expression as ASTNode, context),
                NewObjectNode newObjNode => this.CompileNewObject(newObjNode, context),
                NewArrayNode newArrNode => this.CompileNewArray(newArrNode, context),
                ValueListInitializerNode valListNode => this.CompileValueListInitializer(valListNode, context),
                LookupNode lookupNode => this.CompileLookupNode(lookupNode, context),
                IfStatement ifStatement => this.CompileBranch(ifStatement, context),
                WhileStatement whileStatement => this.CompileWhileNode(whileStatement, context),
                DoWhileStatement doWhileStatement => this.CompileDoWhileNode(doWhileStatement, context),
                ForStatement forStatement => this.CompileForNode(forStatement, context),
                _ => throw new NotImplementedException(),
            };
            return instructions;
        }

        private List<ByteInstruction> CompileIdentifier(IdentifierNode node) {
            if (node.IsFuncIdentifier) {
                return Instruction(new ByteInstruction(Bytecode.PUSH, node.Index.ToString()));
            } else {
                return Instruction(new ByteInstruction(Bytecode.PUSH, node.Index));
            }
        }

        private List<ByteInstruction> CompileAssignmentOperation(AssignmentNode node, CompileContext context) {
            var instructions = new List<ByteInstruction>();
            instructions.AddRange(this.CompileNode(node.Right, context));
            if (node.Left is LookupNode lookup) {
                instructions.AddRange(this.CompileIndexer(lookup.Index, context, out int dim));
                instructions.AddRange(this.CompileNode(lookup.Left as ASTNode, context));
                instructions.Add(new ByteInstruction(Bytecode.STOREELM, dim));
            } else if (node.Left is MemberAccessNode) {
                instructions.Add(new ByteInstruction(Bytecode.STOREFLD));
            } else { // Add other conditions here
                instructions.Add(new ByteInstruction(Bytecode.STORELOC));
            }
            return instructions;
        }

        private List<ByteInstruction> CompileBinaryOperation(BinOpNode op, CompileContext context) {
            List<ByteInstruction> instructions = new List<ByteInstruction>(); 
            Bytecode bytecode = op.Op switch
            {
                "+" => Bytecode.ADD,
                "-" => Bytecode.SUB,
                "/" => Bytecode.DIV,
                "*" => Bytecode.MUL,
                "|" => Bytecode.OR,
                "&" => Bytecode.AND,
                "=" => throw new Exception(),
                "<" => Bytecode.LE,
                ">" => Bytecode.GE,
                _ => Bytecode.NOP,
            };
            instructions.AddRange(this.CompileNode(op.Left, context));
            instructions.AddRange(this.CompileNode(op.Right, context));
            if (!context.Result) { 
                return new List<ByteInstruction>(); 
            }            
            if (bytecode is Bytecode.NOP) {
                context.UpdateResultIfErr(new CompileResult(false, $"Unknown binary operation '{op.Content}'").SetOrigin(op.Pos));
                return new List<ByteInstruction>();
            }
            instructions.Add(new ByteInstruction(bytecode));
            return instructions;
        }

        private List<ByteInstruction> CompileUnaryOperation(UnaryOpNode op, CompileContext context) {
            var instructions = new List<ByteInstruction>();
            Bytecode bytecode = op.Op switch
            {
                "!" => Bytecode.NEG,
                "++" => Bytecode.INC,
                "--" => Bytecode.DEC,
                _ => Bytecode.NOP,
            };
            instructions.AddRange(this.CompileNode(op.Expr, context)); if (!context.Result) {
                return new List<ByteInstruction>();
            }
            if (bytecode is Bytecode.NOP) {
                context.UpdateResultIfErr(new CompileResult(false, $"Unknown unary operation '{op.Content}'").SetOrigin(op.Pos));
                return new List<ByteInstruction>();
            }
            if (bytecode == Bytecode.INC || bytecode == Bytecode.DEC) {
                instructions.Add(new ByteInstruction(bytecode, op.IsPostOp ? (byte)1 : (byte)0));
            } else {
                instructions.Add(new ByteInstruction(bytecode));
            }
            return instructions;
        }

        private List<ByteInstruction> CompileVariableDeclaration(VarDeclNode decl, CompileContext context) {
            List<ByteInstruction> instructions = new List<ByteInstruction>();
            instructions.AddRange(this.CompileNode(decl.AssignToExpr, context));
            if (!context.Result) {
                return null;
            }
            instructions.Add(new ByteInstruction(Bytecode.ENTER, decl.EnterIndex));
            return instructions;
        }

        private List<ByteInstruction> CompileConstant(ASTNode node) {
            List<ByteInstruction> instructions = new List<ByteInstruction>();
            switch (node) {
                case IntLitNode ilit:
                    instructions.Add(new ByteInstruction(Bytecode.LCSI32, ilit.Integer));
                    break;
                case BoolLitNode blit:
                    instructions.Add(new ByteInstruction(Bytecode.LCSI8, blit.Boolean ? (byte)0x1 : (byte)0x0));
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
            foreach (ushort u in node.VarIndices) {
                instructions.Add(new ByteInstruction(Bytecode.EXIT, u));
            }
            return instructions;
        }

        private List<ByteInstruction> CompileGroupedExpression(ExpressionNode node, CompileContext context) {
            List<ByteInstruction> instructions = new List<ByteInstruction>();
            for (int i = 0; i < node.Nodes.Count; i++) {
                instructions.AddRange(this.CompileNode(node[i], context));
                if (!context.Result) {
                    return null;
                }
            }
            return instructions;
        }

        private List<ByteInstruction> CompileCallExpression(CallNode node, CompileContext context) {
            List<ByteInstruction> instructions = new List<ByteInstruction>();
            for (int i = 0; i < node.Arguments.Count; i++) {
                instructions.AddRange(this.CompileNode(node.Arguments[i], context));
                if (!context.Result) {
                    return null;
                }
            }
            instructions.AddRange(this.CompileNode(node.IdentifierNode, context));
            if (!context.Result) {
                return null;
            }
            instructions.Add(new ByteInstruction(Bytecode.CALL, (byte)node.Arguments.Count));
            return instructions;
        }

        private List<ByteInstruction> CompileMemberAccessNode(MemberAccessNode node, CompileContext context) {
            List<ByteInstruction> instructions = new List<ByteInstruction>();
            instructions.AddRange(this.CompileNode(node.Left as ASTNode, context));
            if (!context.Result) {
                return null;
            }
            // load member
            return instructions;
        }

        private List<ByteInstruction> CompileNewObject(NewObjectNode node, CompileContext context) {
            List<ByteInstruction> instructions = new List<ByteInstruction>();
            string t = node.Type.ToString();
            if (!t.StartsWith("global.")) {
                t = $"global.{t}";
            }
            for (int i = 0; i < node.CtorArguments.Count; i++) {
                instructions.AddRange(this.CompileNode(node.CtorArguments[i], context));
                if (!context.Result) {
                    return null;
                }
            }
            instructions.Add(new ByteInstruction(Bytecode.NEW, t));
            return instructions;
        }

        private List<ByteInstruction> CompileNewArray(NewArrayNode node, CompileContext context) {
            List<ByteInstruction> instructions = new List<ByteInstruction>();
            foreach (ASTNode dimNode in node.Indexer.Nodes) {
                instructions.AddRange(this.CompileNode(dimNode, context));
            }
            if (node.GetHint(CompileHintType.TypeHint).Value.Args[0] is ArrayType aType) {
                instructions.Add(new ByteInstruction(Bytecode.NEWARRAY, node.Indexer.Nodes.Count, aType.ReferencedType.ToString()));
                return instructions;
            } else {
                throw new Exception();
            }
        }

        private List<ByteInstruction> CompileValueListInitializer(ValueListInitializerNode valListNode, CompileContext context) {

            var instructions = new List<ByteInstruction>();
            var typeHint = valListNode.GetHint(CompileHintType.TypeHint);
            var suggestedType = typeHint.HasValue ? typeHint.Value.Args[0] as IValType : null;

            if (suggestedType?.IsPrimitive ?? false) {

                bool isConst = valListNode.All(x => x is ILiteral);

                if (isConst && suggestedType is ValueType vType) {

                    byte[] content = CompileHelper.ToByteArray(vType, valListNode);

                    int length = valListNode.Length;
                    int count = length * vType.Size;
                    int start = context.AddConstBytes(content);

                    instructions.Add(new ByteInstruction(Bytecode.LCSI32, length));
                    instructions.Add(new ByteInstruction(Bytecode.NEWARRAY, 1, vType.ToString()));
                    instructions.Add(new ByteInstruction(Bytecode.CCPY, start, count));

                } else {
                    
                    throw new NotImplementedException();

                }

            } else {

                throw new NotImplementedException();

            }

            return instructions;

        }

        private List<ByteInstruction> CompileLookupNode(LookupNode lookupNode, CompileContext context) {

            var instructions = this.CompileIndexer(lookupNode.Index, context, out int dim);
            instructions.AddRange(this.CompileNode(lookupNode.Left as ASTNode, context));
            instructions.Add(new ByteInstruction(Bytecode.LOADELM, dim));
            return instructions;

        }

        private List<ByteInstruction> CompileIndexer(IndexerNode indexer, CompileContext context, out int dimensions) {
            var res = new List<ByteInstruction>();
            dimensions = indexer.Nodes.Count;
            foreach (ASTNode node in indexer.Nodes) {
                res.AddRange(this.CompileNode(node, context));
            }
            return res;
        }

        private List<ByteInstruction> CompileBranch(IfStatement statement, CompileContext context) {

            // Create head branch and then compile it
            Branch entryBranch = new Branch(statement);
            entryBranch.ToTree();
            entryBranch.Optimize();
            entryBranch.CompileBranch(this, context);

            // Get the instructions
            var instructions = entryBranch.ToInstructions();

            // Return instructions
            return instructions;

        }

        private List<ByteInstruction> CompileWhileNode(WhileStatement whileStatement, CompileContext context) {
            var res = this.CompileNode(whileStatement.Condition as ASTNode, context);
            var body = this.CompileNode(whileStatement.Body as ASTNode, context);
            res.Add(new ByteInstruction(Bytecode.JMPIFF, body.Count + 1));
            body.Add(new ByteInstruction(Bytecode.JMP, -(body.Count + 1 + res.Count)));
            res.AddRange(body);
            return res;
        }

        private List<ByteInstruction> CompileDoWhileNode(DoWhileStatement whileStatement, CompileContext context) {
            var body = this.CompileNode(whileStatement.Body as ASTNode, context);
            var condition = this.CompileNode(whileStatement.Condition as ASTNode, context);
            condition.Add(new ByteInstruction(Bytecode.JMPIFT, -(condition.Count + 1 + body.Count)));
            var result = body.Union(condition).ToList();
            return result;
        }

        private List<ByteInstruction> CompileForNode(ForStatement forStatement, CompileContext context) {
            var init = this.CompileNode(forStatement.Init, context);
            var cond = this.CompileNode(forStatement.Condition as ASTNode, context);
            var after = this.CompileNode(forStatement.After as ASTNode, context);
            var body = this.CompileNode(forStatement.Body as ASTNode, context);
            cond.Add(new ByteInstruction(Bytecode.JMPIFF, after.Count + body.Count + 1));
            after.Add(new ByteInstruction(Bytecode.JMP, -(body.Count + cond.Count + after.Count + 1)));
            var result = init.Union(cond).Union(body).Union(after).ToList();
            return result;
        }

    }

}
