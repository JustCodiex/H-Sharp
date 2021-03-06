using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using HSharp.Compiling;
using HSharp.Compiling.Linking;
using System.Text;

namespace HSharp.IO {

    public class ProgramOutput {

        private ByteInstruction[] m_instructions;
        private Dictionary<string, long> m_funcOffsets;
        private long m_instructionOffset;
        private byte[] m_constBytes;
        private ReadOnlyDictionary<string, ulong> m_strings;
        private SourceProjectType m_projType;
        private List<LinkingType> m_declaredTypes;
        private BindTable m_bindings;

        public ProgramOutput() {
            this.m_strings = new ReadOnlyDictionary<string, ulong>(new Dictionary<string, ulong>());
            this.m_declaredTypes = new List<LinkingType>();
        }

        public void SetOffsets(Dictionary<string, long> offsets, long instructionOffset) {
            this.m_funcOffsets = offsets;
            this.m_instructionOffset = instructionOffset;
        }

        public void SetProgramType(SourceProjectType type) => this.m_projType = type;

        public void SetInstructions(ByteInstruction[] instructions) => this.m_instructions = instructions;

        public void SetBytes(byte[] bytes) => this.m_constBytes = bytes;

        public void SetStrings(ReadOnlyDictionary<string, ulong> strings) => this.m_strings = strings;

        public void SetDeclaredTypes(List<LinkingType> types) => this.m_declaredTypes = types;

        public void SetBindTable(BindTable bindTable) => this.m_bindings = bindTable;

        public void Save(string outputPath) {

            // Delete any existing file
            if (File.Exists(outputPath)) {
                File.Delete(outputPath);
            }

            // Open writer
            using BinaryWriter writer = new BinaryWriter(File.OpenWrite(outputPath));

            // header stuff
            writer.Write((byte)this.m_projType);
            writer.Write(this.m_instructionOffset);
            writer.Write(this.m_declaredTypes.Count);
            writer.Write(this.m_bindings.Size);

            for (int i = 0; i < this.m_instructions.Length; i++) {
                writer.Write((byte)this.m_instructions[i].Op);
                for (int j = 0; j < this.m_instructions[i].Args.Length; j++) {
                    switch (this.m_instructions[i].Args[j]) {
                        case ushort:
                            writer.Write((ushort)this.m_instructions[i].Args[j]);
                            break;
                        case short:
                            writer.Write((short)this.m_instructions[i].Args[j]);
                            break;
                        case uint:
                            writer.Write((uint)this.m_instructions[i].Args[j]);
                            break;
                        case int:
                            writer.Write((int)this.m_instructions[i].Args[j]);
                            break;
                        case byte:
                            writer.Write((byte)this.m_instructions[i].Args[j]);
                            break;
                        case string:
                            writer.Write((string)this.m_instructions[i].Args[j]);
                            break;
                        default:
                            throw new NotSupportedException();
                    };
                }
            }

            foreach (var pair in this.m_funcOffsets) {

                writer.Write(pair.Value);

            }

            // offset stuff

            writer.Write(this.m_constBytes.LongLength);
            writer.Write(this.m_constBytes);

            foreach (var export in this.m_declaredTypes) {
                writer.Write(export.ToBytes());
            }

            foreach (var binding in this.m_bindings) {
                writer.Write(binding.Key);
                byte[] dll = Encoding.UTF8.GetBytes(binding.Value.Source);
                byte[] func = Encoding.UTF8.GetBytes(binding.Value.SourceName);
                writer.Write((byte)dll.Length);
                writer.Write((byte)func.Length);
                writer.Write(dll);
                writer.Write(func);
            }

        }

        public void SaveAsText(string outputPath) {

            using StreamWriter writer = new StreamWriter(outputPath);
            writer.WriteLine($"Instruction offset: 0x{this.m_instructionOffset:X8}");
            writer.WriteLine();

            writer.WriteLine("Instructions:");
            for (int i = 0; i < this.m_instructions.Length; i++) {

                writer.WriteLine($"\t[{i:0000}]\t{this.m_instructions[i].Op}{(this.m_instructions[i].Args.Length > 0 ? ($", {string.Join(", ", this.m_instructions[i].Args)}") : string.Empty)}");

            }

            writer.WriteLine();
            writer.WriteLine("Functions:");

            int j = 0;
            foreach (var pair in this.m_funcOffsets) {
                writer.WriteLine($"\t[{j:000}] {pair.Key}: 0x{pair.Value:X8}");
                j++;
            }

            writer.WriteLine();
            writer.WriteLine($"Const Bytes [#{this.m_constBytes.Length}]:");

            for (int i = 0; i < this.m_constBytes.Length; i++) {
                writer.Write($"0x{this.m_constBytes[i]:X1} ");
                if (i > 1 && (i+1) % 16 == 0) {
                    writer.Write("\n");
                }
            }

            if (this.m_declaredTypes.Count > 0) {

                writer.WriteLine();
                writer.WriteLine($"Exported Types [#{this.m_declaredTypes.Count}]:");

                foreach (var t in this.m_declaredTypes) {

                    writer.WriteLine($"\t[{t.FullName}]:");
                    writer.WriteLine($"\t\tSizeInMemory: {t.SizeInMemory}");

                    writer.WriteLine($"\t\tFields [#{t.FieldPtrs.Count}]:");
                    foreach (var p in t.FieldPtrs) {
                        writer.WriteLine($"\t\t\t.{p.Key}: 0x{p.Value:X2}");
                    }

                    writer.WriteLine($"\t\tMethods [#{t.MethodPtrs.Count}]:");
                    foreach (var m in t.MethodPtrs) {
                        writer.WriteLine($"\t\t\t.{m.Key}: {m.Value}");

                    }

                }

            }

            if (this.m_bindings.Size > 0) {

                writer.WriteLine();
                writer.WriteLine($"Bindings [#{this.m_bindings.Size}]:");

                foreach (var binding in this.m_bindings) {

                    writer.WriteLine($"\t0x{binding.Key:X8}: {binding.Value}");

                }

            }

        }

    }

}
