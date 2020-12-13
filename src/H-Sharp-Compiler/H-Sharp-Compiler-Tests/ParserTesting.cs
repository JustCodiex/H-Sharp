using System;
using System.Collections.Generic;
using HSharp.Parsing;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;
using HSharp.Parsing.AbstractSnyaxTree.Expression;
using HSharp.Parsing.AbstractSnyaxTree.Statement;
using NUnit.Framework;

namespace H_Sharp_Compiler_Tests {

    using static TestUtility;

    [TestOf(typeof(ASTBuilder))]
    public class ParserTesting {

        Lexer lexer;
        ASTBuilder builder;

        [SetUp]
        public void Setup() => this.lexer = new Lexer();

        private AST BuildAST(string rawCode) {
            this.builder = new ASTBuilder(this.lexer.Lex(rawCode));
            Assert.That(builder.Parse().Success is true); 
            return this.builder.Build();
        }

        #region Unary Testing

        [Test]
        [Category("Unary Binary Parsing")]
        public void PostBinary1() {
            var ast = this.BuildAST("x++;");
            Assert.That(ast is not null, () => "Expected a root AST node");
            Assert.IsInstanceOf<UnaryOpNode>(ast.Root.Sequence[0], "UnaryOpNode not found");
            Assert.That((ast.Root.Sequence[0] as UnaryOpNode).Op.CompareTo("++") == 0, "Incorrect operator found");
            Assert.That((ast.Root.Sequence[0] as UnaryOpNode).IsPostOp, "Incorrect operator position found");
            Assert.IsInstanceOf<IdentifierNode>((ast.Root.Sequence[0] as UnaryOpNode).Expr, "IdentifierNode not found");
            Assert.That(((ast.Root.Sequence[0] as UnaryOpNode).Expr as IdentifierNode).Content.CompareTo("x") == 0, "Incorrect identifier found");
        }

        [Test]
        [Category("Unary Binary Parsing")]
        public void PostBinary2() {
            var ast = this.BuildAST("{ x++; }");
            Assert.That(ast is not null, () => "Expected a root AST node");
            Assert.That(ast.Root.Sequence.Count > 0);
            var enclosingScope = ast.Root.Sequence[0] as ScopeNode;
            Assert.That(enclosingScope is not null);
            Assert.IsInstanceOf<UnaryOpNode>(enclosingScope[0], "UnaryOpNode not found");
            Assert.That((enclosingScope[0] as UnaryOpNode).Op.CompareTo("++") == 0, "Incorrect operator found");
            Assert.That((enclosingScope[0] as UnaryOpNode).IsPostOp, "Incorrect operator position found");
            Assert.IsInstanceOf<IdentifierNode>((enclosingScope[0] as UnaryOpNode).Expr, "IdentifierNode not found");
            Assert.That(((enclosingScope[0] as UnaryOpNode).Expr as IdentifierNode).Content.CompareTo("x") == 0, "Incorrect identifier found");
        }

        [Test]
        [Category("Unary Binary Parsing")]
        public void PreBinary1() {
            var ast = this.BuildAST("++x;");
            Assert.That(ast is not null, () => "Expected a root AST node");
            Assert.IsInstanceOf<UnaryOpNode>(ast.Root.Sequence[0], "UnaryOpNode not found");
            Assert.That((ast.Root.Sequence[0] as UnaryOpNode).Op.CompareTo("++") == 0, "Incorrect operator found");
            Assert.That(!(ast.Root.Sequence[0] as UnaryOpNode).IsPostOp, "Incorrect operator position found");
            Assert.IsInstanceOf<IdentifierNode>((ast.Root.Sequence[0] as UnaryOpNode).Expr, "IdentifierNode not found");
            Assert.That(((ast.Root.Sequence[0] as UnaryOpNode).Expr as IdentifierNode).Content.CompareTo("x") == 0, "Incorrect identifier found");
        }

        #endregion

        #region Loop Testing

        [Test]
        [Category("Loop Testing")]
        public void WhileParse1() {
            string[] code = {
                "int x = 0;",
                "while x < 100 {",
                "   x++;",
                "}"
            };
            var ast = this.BuildAST(ToSingleText(code));
            Assert.That(ast is not null, () => "Expected a root AST node");
            Assert.IsInstanceOf<VarDeclNode>(ast.Root.Sequence[0], "VarDeclNode not found");
            Assert.IsInstanceOf<WhileStatement>(ast.Root.Sequence[1], "WhileStatement not found");
            Assert.IsInstanceOf<ScopeNode>((ast.Root.Sequence[1] as WhileStatement).Body, "WhileStatement body was invalid");
            Assert.IsInstanceOf<UnaryOpNode>(((ast.Root.Sequence[1] as WhileStatement).Body as ScopeNode)[0], "WhileStatement body (unop) was invalid");
            Assert.IsInstanceOf<BinOpNode>((ast.Root.Sequence[1] as WhileStatement).Condition, "WhileStatement condition was invalid");
        }

        #endregion

    }

}
