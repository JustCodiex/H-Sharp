using HSharp.IO;
using HSharp.Language;
using HSharp.Parsing;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;
using HSharp.Parsing.AbstractSnyaxTree.Directive;
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

        [Test]
        [Category("Loop Testing")]
        public void DoWhileParse1() {
            string[] code = {
                "int x = 0;",
                "do {",
                "   x++;",
                "} while x < 100;"
            };
            var ast = this.BuildAST(ToSingleText(code));
            Assert.That(ast is not null, () => "Expected a root AST node");
            Assert.IsInstanceOf<VarDeclNode>(ast.Root.Sequence[0], "VarDeclNode not found");
            Assert.IsInstanceOf<DoWhileStatement>(ast.Root.Sequence[1], "DoWhileStatement not found");
            Assert.IsInstanceOf<ScopeNode>((ast.Root.Sequence[1] as DoWhileStatement).Body, "WhileStatement body was invalid");
            Assert.IsInstanceOf<UnaryOpNode>(((ast.Root.Sequence[1] as DoWhileStatement).Body as ScopeNode)[0], "WhileStatement body (unop) was invalid");
            Assert.IsInstanceOf<BinOpNode>((ast.Root.Sequence[1] as DoWhileStatement).Condition, "WhileStatement condition was invalid");
        }

        [Test]
        [Category("Loop Testing")]
        public void ForParse1() {
            string[] code = {
                "int x = 0;",
                "for (int i = 0; i < 10; i++) {",
                "   x++;",
                "}"
            };
            var ast = this.BuildAST(ToSingleText(code));
            Assert.That(ast is not null, () => "Expected a root AST node");
            Assert.IsInstanceOf<VarDeclNode>(ast.Root.Sequence[0], "VarDeclNode not found");
            Assert.IsInstanceOf<ForStatement>(ast.Root.Sequence[1], "ForStatement not found");
            Assert.IsInstanceOf<ScopeNode>((ast.Root.Sequence[1] as ForStatement).Body, "ForStatement body was invalid");
            Assert.IsInstanceOf<UnaryOpNode>(((ast.Root.Sequence[1] as ForStatement).Body as ScopeNode)[0], "ForStatement body (unop) was invalid");
            Assert.IsInstanceOf<BinOpNode>((ast.Root.Sequence[1] as ForStatement).Condition, "ForStatement condition was invalid");
            Assert.IsInstanceOf<UnaryOpNode>((ast.Root.Sequence[1] as ForStatement).After, "ForStatement after was invalid");
            Assert.IsInstanceOf<VarDeclNode>((ast.Root.Sequence[1] as ForStatement).Init, "ForStatement init was invalid");
        }

        [Test]
        [Category("Loop Testing")]
        public void ForParse2() {
            string[] code = {
                "int i = 0;",
                "for (i = 1; i < 10; i++) {",
                "   x++;", // This right here should cause a compiler error later on (But should parse correctly).
                "}"
            };
            var ast = this.BuildAST(ToSingleText(code));
            Assert.That(ast is not null, () => "Expected a root AST node");
            Assert.IsInstanceOf<VarDeclNode>(ast.Root.Sequence[0], "VarDeclNode not found");
            Assert.IsInstanceOf<ForStatement>(ast.Root.Sequence[1], "ForStatement not found");
            Assert.IsInstanceOf<ScopeNode>((ast.Root.Sequence[1] as ForStatement).Body, "ForStatement body was invalid");
            Assert.IsInstanceOf<UnaryOpNode>(((ast.Root.Sequence[1] as ForStatement).Body as ScopeNode)[0], "ForStatement body (unop) was invalid");
            Assert.IsInstanceOf<BinOpNode>((ast.Root.Sequence[1] as ForStatement).Condition, "ForStatement condition was invalid");
            Assert.IsInstanceOf<UnaryOpNode>((ast.Root.Sequence[1] as ForStatement).After, "ForStatement after was invalid");
            Assert.IsInstanceOf<AssignmentNode>((ast.Root.Sequence[1] as ForStatement).Init, "ForStatement init was invalid");
        }

        #endregion

        #region Namespace Testing

        [Test]
        [Category("Namespace Testing")]
        public void NamespaceParse1() {
            string[] code = {
                "namespace app.space {",
                "   public class KlassA(int a, int b);",
                "   public class KlassB(int a, int b);",
                "   public class KlassC(int a, int b);",
                "}"
            };
            var ast = this.BuildAST(ToSingleText(code));
            Assert.That(ast is not null, () => "Expected a root AST node");
            Assert.IsInstanceOf<NamespaceDirectiveNode>(ast.Root.Sequence[0], "NamespaceDirectiveNode not found");
            Assert.AreEqual(3, (ast.Root.Sequence[0] as NamespaceDirectiveNode).Body.Nodes.Count);
            Assert.IsInstanceOf<ClassDeclNode>((ast.Root.Sequence[0] as NamespaceDirectiveNode).Body[0], "ClassDeclNode not found (@1)");
            Assert.IsInstanceOf<ClassDeclNode>((ast.Root.Sequence[0] as NamespaceDirectiveNode).Body[1], "ClassDeclNode not found (@2)");
            Assert.IsInstanceOf<ClassDeclNode>((ast.Root.Sequence[0] as NamespaceDirectiveNode).Body[2], "ClassDeclNode not found (@3)");
        }

        #endregion

        #region Modifier Testing

        #region Access Modifier Testing
        [Test]
        [Category("Access Modifier Testing")]
        public void ExternalAccessTesting1() {
            string[] code = {
                "public class Example {",
                "   public external getX(): int;",
                "}"
            };
            var ast = this.BuildAST(ToSingleText(code));
            Assert.That(ast is not null, () => "Expected a root AST node");
            Assert.IsInstanceOf<ClassDeclNode>(ast.Root.Sequence[0], "ClassDeclNode not found");
            Assert.IsInstanceOf<FuncDeclNode>((ast.Root.Sequence[0] as ClassDeclNode).Methods[0], "FuncDeclNode not found in ClassDeclNode");
            Assert.AreEqual(AccessModifier.Public | AccessModifier.External, (ast.Root.Sequence[0] as ClassDeclNode).Methods[0].GetAccessModifier(), "FuncDeclNode does not have a public external access modifier");
        }

        #endregion

        #endregion

        #region Syntax Error Detection Testing

        [Test]
        [Category("Syntax Error Detection")]
        public void SyntaxError1() {
            string[] code = {
                "namespace library {",
                "   class LibKlass {",
                "       five(): int {",
                "           return 5",
                "       }",
                "   }",
                "}"
            };
            var err = Assert.Throws<SyntaxError>(() => this.BuildAST(ToSingleText(code)));
            Assert.That(err.SyntaxViolationCode, Is.EqualTo(0));
            Assert.That(err.Origin, Is.EqualTo(new SourcePosition(4, 12, "unknown.hsharp")));
            Assert.That(err.Message, Is.EqualTo("Expected ';' but found EOS."));
        }

        #endregion

    }

}
