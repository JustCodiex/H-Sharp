using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HSharp.Compiling.Linking {
    
    public class LinkingType {
    
        public string FullName { get; set; }

        public ushort SizeInMemory { get; set; }

        public Dictionary<string,ushort> FieldPtrs { get; }

        public Dictionary<string, LinkFunction> MethodPtrs { get; }

        public LinkingType(string name, ushort sizeInMem) {
            this.FullName = name;
            this.SizeInMemory = sizeInMem;
            this.FieldPtrs = new Dictionary<string, ushort>();
            this.MethodPtrs = new Dictionary<string, LinkFunction>();
        }

        public byte[] ToBytes() {
            
            using MemoryStream stream = new MemoryStream();
            
            using BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(Encoding.UTF8.GetBytes(FullName));
            writer.Write((char)0x0);
            writer.Write(this.SizeInMemory);
            
            writer.Write(this.FieldPtrs.Count);
            foreach (var ptr in this.FieldPtrs) {
                writer.Write(ptr.Value); // Simply write the offset
            }

            writer.Write(this.MethodPtrs.Count);
            foreach (var mptr in this.MethodPtrs) {
                writer.Write((byte)(mptr.Value is LinkBindPtr ? 1 : 0));
                writer.Write(mptr.Value.Ptr);
            }
            
            return stream.ToArray();

        }

        public override string ToString() => $"{this.FullName} [{this.SizeInMemory} bytes]";

    }

}
