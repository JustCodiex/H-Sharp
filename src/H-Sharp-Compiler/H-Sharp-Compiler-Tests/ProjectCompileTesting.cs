using System;

using HSharp;
using HSharp.Compiling;
using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree;

using NUnit.Framework;

namespace H_Sharp_Compiler_Tests {

    using static TestUtility;

    [TestOf(typeof(Compiler))]
    [TestOf(typeof(ASTCompiler))]
    [TestOf(typeof(ASTBuilder))]
    public class ProjectCompileTesting {

        Compiler compiler;

        [SetUp]
        public void Setup() => this.compiler = new Compiler();

        [Test]
        [Category("Library")]
        public void Library1() {
            string[] file1 = {
                "namespace library {",
                "   class LibKlass {",
                "       five(): int {",
                "           return 5;",
                "       }",
                "   }",
                "}"
            };
            string[] file2 = {
                "namespace library {",
                "   class otherClass {",
                "       lib(): LibKlass {",
                "           return new LibKlass();",
                "       }",
                "   }",
                "}"
            };
            SourceProject project = new SourceProject() {
                Name = "Library1",
                Output = "library1.hlib",
                ProjectType = SourceProjectType.Library,
                References = Array.Empty<SourceProjectReference>(),
                Sources = new[] {
                    new SourceProjectFile("file1", ToSingleText(file1), true),
                    new SourceProjectFile("file2", ToSingleText(file2), true)
                }
            };
            var result = this.compiler.CompileProject(project);
            Assert.That(result.Success);
        }

    }

}
