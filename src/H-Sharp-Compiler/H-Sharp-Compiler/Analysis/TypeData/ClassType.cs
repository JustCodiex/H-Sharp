using System;
using System.Collections.Generic;
using System.Text;

namespace HSharp.Analysis.TypeData {

    public class ClassType : HSharpType, IRefType {

        public override bool IsReferenceType => true;

        public override string Name { get; }

        public override ushort Size => 0; // TODO: Change

        public Dictionary<string, HSharpType> Fields { get; }

        public Dictionary<string, ClassType> Classes { get; }

        public Dictionary<string, FunctionType> Methods { get; }

        public List<FunctionType> Constructors { get; }

        public ClassType(string localClassName) {
            this.Name = localClassName;
            this.Constructors = new List<FunctionType>();
            this.Fields = new Dictionary<string, HSharpType>();
            this.Classes = new Dictionary<string, ClassType>();
            this.Methods = new Dictionary<string, FunctionType>();
            this.m_subDomains = new List<Domain>();
        }

        public HSharpType FindMember(string membername) {

            if (this.Fields.ContainsKey(membername)) {
                return this.Fields[membername];
            } else if (this.Methods.ContainsKey(membername)) {
                return this.Methods[membername];
            }

            return null;

        }

    }

}
