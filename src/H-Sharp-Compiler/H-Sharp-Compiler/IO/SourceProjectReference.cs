using System;

namespace HSharp.IO {

    /// <summary>
    /// Represents a project reference.
    /// </summary>
    /// <param name="ReferenceName">The name of the reference.</param>
    /// <param name="ReferencePath">The absolute path of the referenced file.</param>
    /// <param name="ReferenceVersion">The reference version.</param>
    [Serializable]
    public record SourceProjectReference(string ReferenceName, string ReferencePath, string ReferenceVersion) {
        
        /// <summary>
        /// The standard library for H#
        /// </summary>
        public static SourceProjectReference StdLib => new SourceProjectReference("StdLib", "@stdlib", "");

    }

    /// <summary>
    /// Represents a reference to a native reference file. Extends <see cref="SourceProjectReference"/>.
    /// </summary>
    /// <param name="ReferenceName">The name of the reference.</param>
    /// <param name="ReferencePath">The absolute path of the referenced file.</param>
    /// <param name="ReferenceVersion">The reference version.</param>
    /// <param name="TargetLanguage">The targetted language (C/C++ or C#)</param>
    /// <param name="Platform">The platform that is being targetted (Windows, Linux)</param>
    [Serializable]
    public record SourceProjectNativeReference(string ReferenceName, string ReferencePath, string ReferenceVersion, string TargetLanguage, string Platform) 
        : SourceProjectReference(ReferenceName, ReferencePath, ReferenceVersion);

}
