using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HSharp.IO.Windows {
    
    public class Lib : ExternContainer {

        private List<DllFunction> m_mangledExposedfuncs;

        public Lib() {
            this.m_mangledExposedfuncs = new List<DllFunction>();
        }

        public override DllFunction[] GetFunctions() => this.m_mangledExposedfuncs.ToArray();

        public bool FromFile(string libraryFile) {

            string sourceName = Path.GetFileName(libraryFile).Replace(".lib", ".dll");

            using MemoryStream reader = new(File.ReadAllBytes(libraryFile)) {
                Position = 0
            };

            byte[] sig = new byte[8];
            reader.Read(sig, 0, 8);

            if (Encoding.ASCII.GetString(sig) != "!<arch>\n") {
                return false;
            }

            byte[] scanstr = { 0x5f, 0x5f, 0x69, 0x6D, 0x70, 0x5f, 0x3f };

            HashSet<string> funcs = new HashSet<string>();

            while (reader.Position + scanstr.Length < reader.Length) {

                byte[] buff = new byte[scanstr.Length];
                reader.Read(buff, 0, buff.Length);

                bool isStr = true;

                for (int i = 0; i < buff.Length; i++) {
                    if (buff[i] != scanstr[i]) {
                        isStr = false;
                        break;
                    }
                }

                if (isStr) {

                    StringBuilder name = new StringBuilder();
                    while (reader.Position < reader.Length) {
                        int c = reader.ReadByte();
                        if (c == 0) {
                            break;
                        } else {
                            name.Append((char)c);
                        }
                    }

                    funcs.Add(name.ToString());

                } else {
                    reader.Position -= scanstr.Length - 1;
                }

            }

            Regex regex = new Regex(@"((?<class>\w(\w|\d)+)_)?(?<name>(\w|\d)+)(?<mangle>@.*)?");

            foreach (var libfunc in funcs) {

                var match = regex.Match(libfunc);
                string klass = match.Groups["class"].Value;
                string name = match.Groups["name"].Value;
                string mangle = match.Groups["mangle"].Value;

                this.m_mangledExposedfuncs.Add(new DllFunction(klass, name, mangle, sourceName));

            }

            return true;

        }

    }

}
