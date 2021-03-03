using System;
using System.Collections.Generic;
using HSharp.IO;

namespace H_Sharp_Compiler_Tests {
    
    public static class TestUtility {

        public static string ToSingleText(IEnumerable<string> content)
            => string.Join(Environment.NewLine, content);

        public static SourceProject FromText(IEnumerable<string> content, string name, string output)
            => FromText(content, name, output, SourceProjectType.ConsoleApplication);

        public static SourceProject FromText(IEnumerable<string> content, string name, string output, SourceProjectType projectType)
            => new SourceProject(name, output, projectType, SourceProjectFile.FromText(ToSingleText(content)));

    }

}
