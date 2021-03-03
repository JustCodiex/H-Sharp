using System;

namespace HSharp.IO {
    
    [Serializable]
    public record SourceProjectReference(string ReferenceName, string ReferencePath, string ReferenceVersion) {
        
        /// <summary>
        /// The standard library for H#
        /// </summary>
        public static SourceProjectReference StdLib => new SourceProjectReference("StdLib", "@stdlib", "");

    }

}
