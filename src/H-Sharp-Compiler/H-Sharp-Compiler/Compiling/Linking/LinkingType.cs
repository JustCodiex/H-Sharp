using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSharp.Compiling.Linking {
    
    public class LinkingType {
    
        public string FullName { get; set; }

        public ushort SizeInMemory { get; set; }

        public Dictionary<string,ushort> FieldPtrs { get; }

        public Dictionary<string,ulong> MethodPtrs { get; }

        public LinkingType(string name, ushort sizeInMem) {
            this.FullName = name;
            this.SizeInMemory = sizeInMem;
            this.FieldPtrs = new Dictionary<string, ushort>();
            this.MethodPtrs = new Dictionary<string, ulong>();
        }

        public byte[] ToBytes() {
            
            using MemoryStream stream = new MemoryStream();
            
            using BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(Encoding.UTF8.GetBytes(FullName));
            writer.Write((byte)0x0);
            writer.Write(this.SizeInMemory);
            
            return stream.ToArray();

        }

        public override string ToString() => $"{this.FullName} [{this.SizeInMemory} bytes]";

    }

}
