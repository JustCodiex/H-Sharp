using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace HSharp.Metadata {
    
    public class Log {

        StringBuilder m_log;

        private static List<Log> __logs = new List<Log>();

        public Log() {
            this.m_log = new StringBuilder();
            __logs.Add(this);
        }

        public void SaveAndClose(string outpath) {
            File.WriteAllText(outpath, this.m_log.ToString());
            __logs.Remove(this);
        }

        public static void WriteLine(string msg) {
            Console.WriteLine(msg);
            __logs.ForEach(x => x.m_log.Append($"{msg}{Environment.NewLine}"));
        }

        public static void WriteLine() => WriteLine(string.Empty);

    }

}
