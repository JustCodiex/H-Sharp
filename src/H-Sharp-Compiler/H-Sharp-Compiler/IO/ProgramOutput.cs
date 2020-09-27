using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using HSharp.Compiling;

namespace HSharp.IO {

    public class ProgramOutput {

        private ByteInstruction[] m_instructions;
        private Dictionary<string, long> m_funcOffsets;
        private long m_instructionOffset;

        public ProgramOutput() {

        }

        public void SetOffsets(Dictionary<string, long> offsets, long instructionOffset) {
            this.m_funcOffsets = offsets;
            this.m_instructionOffset = instructionOffset;
        }

        public void SetInstructions(ByteInstruction[] instructions) => this.m_instructions = instructions;

        public void Save(string outputPath) {

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(outputPath))) {

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
                            default:
                                throw new NotSupportedException();
                        };
                    }
                }

                foreach (var pair in this.m_funcOffsets) {

                    writer.Write(pair.Value);

                }

                // offset stuff

            }

        }

    }

}
