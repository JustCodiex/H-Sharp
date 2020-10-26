using System;
using System.Collections.Generic;
using System.Text;

namespace HSharp.Analysis.TypeData {

    public class ClassType : HSharpType, IRefType, IExtendableType {

        private ClassType m_baseType;

        public override bool IsReferenceType => true;

        public override string Name { get; }

        public override ushort Size => 0; // TODO: Change

        public Dictionary<string, HSharpType> Fields { get; }

        public Dictionary<string, ClassType> Classes { get; }

        public Dictionary<string, FunctionType> Methods { get; }

        public List<FunctionType> Constructors { get; }

        public ClassType Base => this.m_baseType;

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

            if (this.m_baseType is not null) {
                return this.m_baseType.FindMember(membername);
            }

            return null;

        }

        public bool SetBase(ClassType baseClass) {
            if (this.m_baseType is null) {
                this.m_baseType = baseClass;
                return true;
            } else {
                return false;
            }
        }

        public bool IsExtensionOf(IExtendableType type) {
            if (this.m_baseType is not null) {
                return this.m_baseType == type || this.m_baseType.IsExtensionOf(type);
            } else {
                return false;
            }
        }

    }

}
