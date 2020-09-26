using System;
using System.Collections.Generic;
using System.Text;

namespace HSharp.IO {
    
    public class SourceProject {

        private string[] m_files;

        public string[] CodeFiles => this.m_files;

        public SourceProject(params string[] files) {
            this.m_files = files;
        }

    }

}
