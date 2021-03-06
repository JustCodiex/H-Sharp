using System.Collections;
using System.Collections.Generic;
using HSharp.IO;

namespace HSharp.Compiling.Linking {
    
    public class BindTable : IEnumerable<KeyValuePair<ulong, BindTable.BindValue>> {

        public readonly struct BindValue {
            public readonly string Source;
            public readonly string SourceName;
            public BindValue(string source, string srcName) {
                this.Source = source;
                this.SourceName = srcName;
            }
            public override string ToString() => $"{this.Source}+{this.SourceName}";
        }

        ulong m_nextPtr;
        Dictionary<ulong, BindValue> m_boundMethods;
        Dictionary<string, ulong> m_dllNameLookup;

        public int Size => this.m_boundMethods.Count;

        public BindTable() {
            this.m_boundMethods = new Dictionary<ulong, BindValue>();
            this.m_dllNameLookup = new Dictionary<string, ulong>();
            this.m_nextPtr = 0;
        }

        public LinkBindPtr Register(DllFunction function) {
            if (this.m_dllNameLookup.TryGetValue($"{function}@{function.Source}", out ulong ptr)) {
                return new LinkBindPtr(function.Source, ptr);
            } else {
                BindValue bv = new BindValue(function.Source, function.ToString());
                this.m_boundMethods.Add(this.m_nextPtr, bv);
                this.m_dllNameLookup.Add($"{function}@{function.Source}", this.m_nextPtr);
                LinkBindPtr nPtr = new LinkBindPtr(function.Source, this.m_nextPtr);
                this.m_nextPtr++;
                return nPtr;
            }
        }

        public IEnumerator<KeyValuePair<ulong, BindValue>> GetEnumerator() => ((IEnumerable<KeyValuePair<ulong, BindValue>>)this.m_boundMethods).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.m_boundMethods).GetEnumerator();

    }

}
