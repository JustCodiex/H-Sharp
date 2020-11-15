using System;
using System.Collections.Generic;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Statement;
using HSharp.Util.Functional;

namespace HSharp.Compiling.Branching {
    
    public class Branch {

        IBranch m_branch;
        List<ByteInstruction> m_condition;
        List<ByteInstruction> m_body;
        List<Branch> m_trailBranches;

        public Branch(IBranch branch) {
            this.m_branch = branch;
            this.m_trailBranches = new List<Branch>();
        }

        public void ToTree() {
            if (this.m_branch is IfStatement ifs) {
                IBranch trail = ifs.Trail;
                while (trail is not null) {
                    Branch sub = new Branch(trail);
                    this.m_trailBranches.Add(sub);
                    if (trail is IfStatement iff) {
                        trail = iff.Trail;
                    } else {
                        trail = null;
                    }
                }
            }
        }

        public void Optimize() { /* TODO: Implement */ }

        public void CompileBranch(ASTCompiler compiler, CompileContext context) {
            switch(this.m_branch)
            {
                case IfStatement ifs:
                    this.m_condition = BranchCompiler.CompileCondition(compiler, ifs.Condition, context);
                    this.m_body = compiler.CompileNode(ifs.Body as ASTNode, context);
                    foreach (Branch sub in this.m_trailBranches) {
                        sub.CompileBranch(compiler, context);
                    }
                    break;
                case ElseStatement els:
                    this.m_condition = new List<ByteInstruction>();
                    this.m_body = compiler.CompileNode(els.Body as ASTNode, context);
                    break;
                default: throw new NotImplementedException();
            };
        }

        public List<ByteInstruction> ToInstructions() {

            var instructions = new List<ByteInstruction>();
            (ByteInstruction[] condition, ByteInstruction[] body)[] ls = new (ByteInstruction[] condition, ByteInstruction[] body)[this.m_trailBranches.Count + 1];

            ls[0].condition = this.m_condition.AddAndThen(new ByteInstruction(Bytecode.JMPIFF)).ToArray();
            ls[0].body = this.m_body.AddAndThen(new ByteInstruction(Bytecode.JMP)).ToArray();

            int lastOp = ls[0].condition.Length + ls[0].body.Length;

            for (int i = 1; i <= this.m_trailBranches.Count; i++) {
                bool isLast = i == this.m_trailBranches.Count;
                if (isLast) {
                    if (this.m_trailBranches[i - 1].m_branch is ElseStatement) {
                        ls[i].condition = new ByteInstruction[0];

                    } else {
                        ls[i].condition = this.m_trailBranches[i - 1].m_condition.AddAndThen(new ByteInstruction(Bytecode.JMP)).ToArray();
                    }
                    ls[i].body = this.m_trailBranches[i - 1].m_body.ToArray();
                } else {
                    ls[i].condition = this.m_trailBranches[i - 1].m_condition.AddAndThen(new ByteInstruction(Bytecode.JMPIFF)).ToArray();
                    ls[i].body = this.m_trailBranches[i - 1].m_body.AddAndThen(new ByteInstruction(Bytecode.JMP)).ToArray();
                }
                lastOp += ls[i].condition.Length + ls[i].body.Length;
            }

            int currOp = 0;
            for (int i = 0; i < ls.Length; i++) {
                if (ls[i].condition.Length > 0) {
                    ls[i].condition[^1] = new ByteInstruction(Bytecode.JMPIFF, ls[i].body.Length);
                }
                currOp += ls[i].condition.Length + ls[i].body.Length;
                if (i + 1 < ls.Length) {
                    ls[i].body[^1] = new ByteInstruction(Bytecode.JMP, lastOp - currOp);
                }
                instructions.AddRange(ls[i].condition);
                instructions.AddRange(ls[i].body);
            }

            return instructions;

        }

    }

}
