using System.Collections.Generic;

namespace HSharp.Compiling {
    
    public class CompileContext {

        public CompileResult Result { get; set; }

        public CompileContext() {
            this.Result = new CompileResult(true);
        }

        public void UpdateResultIfErr(CompileResult result) {
            if (!result.Success) {
                this.Result = result;
            }
        }

    }

}
