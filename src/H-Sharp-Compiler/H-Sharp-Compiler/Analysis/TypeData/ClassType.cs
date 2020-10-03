using System;
using System.Collections.Generic;
using System.Text;
using HSharp.Analysis.Linking;

namespace HSharp.Analysis.TypeData {

    public class ClassType : HSharpType, IRefType {

        public override bool IsReferenceType => true;

        public override string Name { get; }

        public override ushort Size => 0; // TODO: Change

        public Dictionary<string, HSharpType> Fields { get; }

        public Dictionary<string, ClassType> Classes { get; }

        public Dictionary<string, MethodSignature> Methods { get; }

        public List<MethodSignature> Constructors { get; }

        public ClassType(string localClassName) {
            this.Name = localClassName;
            this.Constructors = new List<MethodSignature>();
            this.Fields = new Dictionary<string, HSharpType>();
            this.Classes = new Dictionary<string, ClassType>();
            this.Methods = new Dictionary<string, MethodSignature>();
            this.m_subDomains = new List<Domain>();
        }

    }

}
