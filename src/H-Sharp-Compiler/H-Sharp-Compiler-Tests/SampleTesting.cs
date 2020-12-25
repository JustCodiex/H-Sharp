using HSharp;
using HSharp.Analysis.Typechecking;
using HSharp.Analysis.Verifying;
using HSharp.Compiling;
using HSharp.Parsing;
using HSharp.Parsing.AbstractSnyaxTree;
using NUnit.Framework;

namespace H_Sharp_Compiler_Tests {

    using static TestUtility;

    [TestOf(typeof(Compiler))]
    [TestOf(typeof(ASTCompiler))]
    [TestOf(typeof(ASTBuilder))]
    [TestOf(typeof(Typechecker))]
    [TestOf(typeof(VarsVerifier))]
    public class SampleTesting { // Note, only testing if the compiler will compile, does not perform a correctness check!

        Compiler compiler;

        [SetUp]
        public void Setup() => this.compiler = new Compiler();

        #region Array Testing

        [Test]
        [Category("Array")]
        public void Array1() {
            string[] sample = {
                "// Create array of 10 elements",
                "int[] test = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };",
                "",
                "// Sum array",
                "test[0] + test[1] + test[2] + test[3] + test[4] + test[5] + test[6] + test[7] + test[8] + test[9];"
            };
            var result = this.compiler.CompileProject(FromText(sample, "Array1", "array1.bin"));
            Assert.That(result.Success);
        }

        [Test]
        [Category("Array")]
        public void Array2() {
            string[] sample = {
                "// create array",
                "int[] test = new int[5];",
                "",
                "// Assign values",
                "test[0] = 1;",
                "test[1] = 2;",
                "test[2] = test[0] * test[1];",
                "test[3] = test[1] * test[2];",
                "test[4] = test[2] * test[3];",
            };
            var result = this.compiler.CompileProject(FromText(sample, "Array2", "array2.bin"));
            Assert.That(result.Success);
        }

        #endregion

        #region Boolean testing

        [Test]
        [Category("Boolean Logic")]
        public void Bool1() {
            string[] sample = {
                "bool v1 = true;",
                "bool v2 = false;",
                "",
                "bool v3 = v2 | v1;",
                "bool v4 = !v2 & v1;"
            };
            var result = this.compiler.CompileProject(FromText(sample, "Bool1", "bool1.bin"));
            Assert.That(result.Success);
        }

        #endregion

        #region Class testing

        [Test]
        [Category("Class Declaration")]
        public void Class1() {
            string[] sample = {
                "class test {",
                "   int field;",
                "   func(int x, int y): int {",
                "       field = x * y;",
                "       int z = field / 2;",
                "       z;",
                "   }",
                "}",
            };
            var result = this.compiler.CompileProject(FromText(sample, "Class1", "class1.bin"));
            Assert.That(result.Success);
        }

        [Test]
        [Category("Class Declaration")]
        public void Class2() {
            string[] sample = {
                "class testClass {",
                "   int x;",
                "   testClass(int x) {",
                "       this.x = x;",
                "   }",
                "   GetX(): int {",
                "       return x;",
                "   }",
                "}",
                "testClass klass = new testClass(5);",
                "klass.GetX();"
            };
            var result = this.compiler.CompileProject(FromText(sample, "Class2", "class2.bin"));
            Assert.That(result.Success);
        }

        [Test]
        [Category("Class Declaration")]
        public void Class3() {
            string[] sample = {
                "public class baseClass {",
                "   private int fieldx;",
                "   public baseClass() {",
                "       this.fieldx = 1;",
                "   }",
                "   public GetX(int mul): int {",
                "       return mul * this.fieldx;",
                "   }",
                "}",
                "public class childClass : baseClass {",
                "   public childClass(int def) {",
                "       this.fieldx = def;",
                "   }",
                "}",
                "childClass klass = new childClass(5);",
                "klass.GetX();"
            };
            var result = this.compiler.CompileProject(FromText(sample, "Class3", "class3.bin"));
            Assert.That(result.Success);
        }

        [Test]
        [Category("Class Declaration")]
        public void Class4() {
            string[] sample = {
                "// Class declarations (Without body definition!)",
                "public class Person(int age);",
                "public class Teacher(int age, int subjectID) : Person(age);",
                "public class Student(int age, int subjectID) : Person(age);",
                "",
                "// Subject ID",
                "int computer_science = 1;",
                "",
                "// Instantiate objects",
                "Person teacher = new Teacher(38, computer_science);",
                "Person student1 = new Student(23, computer_science);",
                "Person student2 = new Student(26, computer_science);",
                "",
                "// Aggregate age",
                "teacher.age + student1.age + student2.age;"
            };
            var result = this.compiler.CompileProject(FromText(sample, "Class4", "class4.bin"));
            Assert.That(result.Success);
        }

        #endregion

        #region Flow Control Testing

        [Test]
        [Category("Flow Control (If)")]
        public void Control1() {
            string[] sample = {
                "if true {",
                "   return 5;",
                "} else {",
                "   return 1;",
                "}"
            };
            var result = this.compiler.CompileProject(FromText(sample, "Flow1", "flow1.bin"));
            Assert.That(result.Success);
        }

        [Test]
        [Category("Flow Control (If)")]
        public void Control2() {
            string[] sample = {
                "if true {",
                "   5;",
                "} else if !true {",
                "   7;",
                "} else {",
                "   7 * 5;",
                "}"
            };
            var result = this.compiler.CompileProject(FromText(sample, "Flow2", "flow2.bin"));
            Assert.That(result.Success);
        }

        [Test]
        [Category("Flow Control (While)")]
        public void Control3() {
            string[] sample = {
                "int x = 0;",
                "while x < 100 {",
                "   x++;",
                "}"
            };
            var result = this.compiler.CompileProject(FromText(sample, "Flow3", "flow3.bin"));
            Assert.That(result.Success);
        }

        [Test]
        [Category("Flow Control (While)")]
        public void Control4() {
            string[] sample = {
                "int x = 0;",
                "while (x < 100) {",
                "   x++;",
                "}"
            };
            var result = this.compiler.CompileProject(FromText(sample, "Flow4", "flow4.bin"));
            Assert.That(result.Success);
        }


        [Test]
        [Category("Flow Control (Do-While)")]
        public void Control5() {
            string[] sample = {
                "int x = 0;",
                "do {",
                "   x++;",
                "} while x < 100;"
            };
            var result = this.compiler.CompileProject(FromText(sample, "Flow5", "flow5.bin"));
            Assert.That(result.Success);
        }

        #endregion

        #region Function Declaration

        [Test]
        [Category("Functions")]
        public void Func1() {
            string[] sample = {
                "func(int x, int y): int {",
                "   int z = x / y;",
                "   z * x * y;",
                "}",
                "func(2,3);",
            };
            var result = this.compiler.CompileProject(FromText(sample, "Function1", "function1.bin"));
            Assert.That(result.Success);
        }

        [Test]
        [Category("Functions")]
        public void Func2() { // TODO: Rewrite this to also accept even if no syntax error is thrown but instead returned through the compiler result (which should be the proper handling of syntax errors)
            string[] sample = {
                "func(int x, int y): int {",
                "   int z = x / y;",
                "   z * x * y;",
                "}",
                "func(2,3)",
            };
            var excp = Assert.Throws<SyntaxError>(() => this.compiler.CompileProject(FromText(sample, "Function2", "function2.bin")));
            Assert.That(excp.Message.CompareTo("Expected ';' found <EOL>") == 0);
        }

        [Test]
        [Category("Functions")]
        public void Func3() {
            string[] sample = {
                "func(int x): int {",
                "   int z = x / 2;",
                "   z * x * 2;",
                "}",
                "func(3);",
            };
            var result = this.compiler.CompileProject(FromText(sample, "Function3", "function3.bin"));
            Assert.That(result.Success);
        }

        #endregion

        #region Variable Declaration Tests
        
        [Test]
        [Category("Variables")]
        public void VDecl1() {
            string[] sample = {
                "int x = 5;",
                "int y = x + 5 - 8;",
                "int z = y * x - 10;",
                "int q = 10 + x * y * z;",
            };
            var result = this.compiler.CompileProject(FromText(sample, "VariableDecl1", "variabledecl1.bin"));
            Assert.That(result.Success);
        }

        [Test]
        [Category("Variables")]
        public void VDecl2() {
            string[] sample = {
                "int x = 5;",
                "int y = {",
                "   int x = 11;",
                "   12 + (5 + x) / x;",
                "};",
                "y + x;"
            };
            var result = this.compiler.CompileProject(FromText(sample, "VariableDecl2", "variabledecl2.bin"));
            Assert.That(result.Success);
        }

        #endregion

    }

}
