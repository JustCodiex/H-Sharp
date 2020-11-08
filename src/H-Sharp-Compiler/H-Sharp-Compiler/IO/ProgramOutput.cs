using System;
using System.IO;
using System.Collections.Generic;
using HSharp.Compiling;
using System.Collections.ObjectModel;

namespace HSharp.IO {

    public class ProgramOutput {

        private ByteInstruction[] m_instructions;
        private Dictionary<string, long> m_funcOffsets;
        private long m_instructionOffset;
        private byte[] m_constBytes;
        private ReadOnlyDictionary<string, ulong> m_strings;

        public ProgramOutput() {
            this.m_strings = new ReadOnlyDictionary<string, ulong>(new Dictionary<string, ulong>());
        }

        public void SetOffsets(Dictionary<string, long> offsets, long instructionOffset) {
            this.m_funcOffsets = offsets;
            this.m_instructionOffset = instructionOffset;
        }

        public void SetInstructions(ByteInstruction[] instructions) => this.m_instructions = instructions;

        public void SetBytes(byte[] bytes) => this.m_constBytes = bytes;

        public void SetStrings(ReadOnlyDictionary<string, ulong> strings) => this.m_strings = strings;

        public void Save(string outputPath) {

            using BinaryWriter writer = new BinaryWriter(File.OpenWrite(outputPath));

            // header stuff
            writer.Write(this.m_instructionOffset);

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

        }

    }

}
