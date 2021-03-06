using System;
using System.IO;

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
        string dllpath;

        [SetUp]
        public void Setup() {
            this.compiler = new Compiler(); 
            this.dllpath = Path.GetFullPath($"{Environment.CurrentDirectory}\\..\\..\\..\\..\\..\\..\\bin\\HSharp.dll");
        }

        #region Library (No external references)

        [Test]
        [Category("Library (Non C++)")]
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

        #endregion

        #region Library (With external references)

        [Test]
        [Category("Library (C++)")]
        public void LibraryCpp1() {
            if (!File.Exists(this.dllpath)) {
                Assert.Pass($"Missing DLL file '{this.dllpath}' - Test will not be run.");
            }
            string[] file1 = {
                "namespace library {",
                "   class Console {",
                "       public external GetColumn(): int;",
                "       public external GetRow(): int;",
                "   }",
                "}"
            };
            SourceProject project = new SourceProject() {
                Name = "LibraryCpp1",
                Output = "libraryCpp1.hlib",
                ProjectType = SourceProjectType.Library,
                References = new[] { 
                    new SourceProjectNativeReference("Hsharp", this.dllpath, "1.0.0", "C/C++", "Windows")
                },
                Sources = new[] {
                    new SourceProjectFile("file1", ToSingleText(file1), true),
                }
            };
            var result = this.compiler.CompileProject(project);
            Assert.That(result.Success);
        }

        #endregion

    }

}
