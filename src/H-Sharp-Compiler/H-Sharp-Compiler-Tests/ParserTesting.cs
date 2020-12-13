using HSharp.Parsing;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Expression;
using NUnit.Framework;

namespace H_Sharp_Compiler_Tests {

    [TestOf(typeof(ASTBuilder))]
    public class ParserTesting {

        Lexer lexer;
        ASTBuilder builder;

        [SetUp]
        public void Setup() { this.lexer = new Lexer(); }

        private AST BuildAST(string rawCode) {
            this.builder = new ASTBuilder(this.lexer.Lex(rawCode));
            Assert.That(builder.Parse().Success is true); 
            return this.builder.Build();
        }

        [Test]
        [Category("Unary Binary Parsing")]
        public void PostBinary1() {
            var ast = this.BuildAST("x++;");
            Assert.That(ast is not null, () => "Expected a root AST node");
            Assert.That(ast.Root.Sequence[0] is UnaryOpNode, () => "UnaryOpNode not found");
        }

    }

}
