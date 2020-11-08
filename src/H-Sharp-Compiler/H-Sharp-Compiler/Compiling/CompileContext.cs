using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HSharp.Compiling {
    
    public class CompileContext {

        private byte[] m_bytes;
        private int m_bytePtr;
        private ulong m_stringNextID;
        private Dictionary<string, ulong> m_stringTable;

        public CompileResult Result { get; set; }

        public ReadOnlyDictionary<string, ulong> Strings => new ReadOnlyDictionary<string, ulong>(this.m_stringTable);

        public int BytePtr => this.m_bytePtr;

        public byte[] ConstBytes => this.m_bytes;

        public CompileContext() {
            this.Result = new CompileResult(true);
            this.m_stringTable = new Dictionary<string, ulong>();
            this.m_stringNextID = 0;
            this.m_bytes = new byte[0];
            this.m_bytePtr = 0;
        }

        public void UpdateResultIfErr(CompileResult result) {
            if (!result.Success) {
                this.Result = result;
            }
        }

        public ulong CreateString(string str) {
            if (this.m_stringTable.ContainsKey(str)) {
                return this.m_stringTable[str];
            } else {
                this.m_stringTable.Add(str, this.m_stringNextID++);
                return this.m_stringTable[str];
            }
        }

        public int AddConstBytes(byte[] bytes) {
            int ptr = this.m_bytePtr;
            byte[] cpy = new byte[this.m_bytes.Length + bytes.Length];
            Array.Copy(this.m_bytes, cpy, this.m_bytes.Length); 
            Array.Copy(bytes, 0, cpy, ptr, bytes.Length);
            this.m_bytes = cpy;
            this.m_bytePtr = this.m_bytes.Length;
            return ptr;
        }

    }

}
