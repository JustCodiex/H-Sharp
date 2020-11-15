using System;
using System.Collections.Generic;
using System.Text;
using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Statement {
    
    public class IfStatement : ASTNode, IStatement, IBranch {

        protected IBranch m_follow;

        public IExpr Condition { get; }

        public IExpr Body { get; }

        public IBranch Trail => this.m_follow;

        public bool HasTrailingBranch => this.m_follow is not null;

        public IfStatement(IExpr condition, IExpr body, SourcePosition position) : base(position, "if", LexTokenType.None) {
            this.Condition = condition;
            this.Body = body;
            this.m_follow = null;
        }

        public void SetTrail(IBranch branch) => this.m_follow = branch;

        public override string ToString() 
            => $"if {this.Condition}{(this.Condition is not IGroupedASTNode && this.Body is not IGroupedASTNode ? ";":string.Empty)} {this.Body}{(this.HasTrailingBranch?$" {this.m_follow}":string.Empty)}";

    }

}
