using System.Collections.Generic;
using HSharp.Compiling.Hint;
using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree {

    public class ASTNode {

        private List<CompileHint> m_compileHints;

        public SourcePosition Pos { get; }

        public string Content { get; }

        public string NodeType => this.GetType().Name;

        public LexTokenType LexicalType { get; }

        public ASTNode(SourcePosition position, string content, LexTokenType tokenType) {
            this.Pos = position;
            this.Content = content;
            this.LexicalType = tokenType;
            this.m_compileHints = new List<CompileHint>();
        }

        public void AddCompilerHint(CompileHintType hintType, params object[] args)
            => this.m_compileHints.Add(new CompileHint(hintType, args));

        public CompileHint? GetHint(CompileHintType type) {
            for (int i = 0; i < this.m_compileHints.Count; i++) {
                if (this.m_compileHints[i].Type == type) {
                    return this.m_compileHints[i];
                }
            }
            return null;
        }

        public List<CompileHint> GetHints(CompileHintType type) {
            List<CompileHint> hints = new List<CompileHint>();
            for (int i = 0; i < this.m_compileHints.Count; i++) {
                if (this.m_compileHints[i].Type == type) {
                    hints.Add(this.m_compileHints[i]);
                }
            }
            return hints;
        }

        public bool HasHint(CompileHintType type) => this.GetHint(type) is not null;

        public override string ToString() => this.Content;

    }

}
