using System.Collections.Generic;
using HSharp.Analysis.TypeData;

namespace HSharp.Analysis.Typechecking {
    
    public class TypeEnvironment {

        private Dictionary<string, IValType> m_typeEnv;

        public TypeEnvironment() {
            this.m_typeEnv = new Dictionary<string, IValType>();
        }

        public TypeEnvironment(TypeEnvironment baseEnv) {
            this.m_typeEnv = new Dictionary<string, IValType>();
            foreach (var pair in baseEnv.m_typeEnv) {
                this.m_typeEnv.Add(pair.Key, pair.Value);
            }
        }

        public void MapsTo(string x, IValType type) {
            if (this.m_typeEnv.ContainsKey(x)) {
                this.m_typeEnv[x] = type;
            } else {
                this.m_typeEnv.Add(x, type);
            }
        }

        public IValType Lookup(string x) => this.m_typeEnv[x];

        public bool IsDefined(string x) => this.m_typeEnv.ContainsKey(x);

    }

}
