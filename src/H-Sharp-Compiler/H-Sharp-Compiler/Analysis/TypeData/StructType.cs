using System;
using System.Collections.Generic;
using System.Text;

namespace HSharp.Analysis.TypeData {
    public class StructType : HSharpType {

        public override bool IsReferenceType => false;

        public override string Name { get; }

        public override ushort Size { get; }

    }
}
