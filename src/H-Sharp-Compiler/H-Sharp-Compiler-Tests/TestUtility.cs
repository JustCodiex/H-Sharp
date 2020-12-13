using System;
using System.Collections.Generic;
using HSharp.IO;

namespace H_Sharp_Compiler_Tests {
    
    public static class TestUtility {

        public static string ToSingleText(IEnumerable<string> content)
            => string.Join(Environment.NewLine, content);

        public static SourceProject FromText(IEnumerable<string> content, string name, string output) 
            => new SourceProject(name, output, SourceProjectFile.FromText(ToSingleText(content)));

    }

}
