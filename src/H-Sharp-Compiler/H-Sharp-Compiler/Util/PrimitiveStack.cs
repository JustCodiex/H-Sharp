using System;
using System.Collections.Generic;

namespace HSharp.Util {

    public sealed class PrimitiveStack<T> : Stack<T>, ICloneable where T : struct {

        public PrimitiveStack() : base() {}

        public PrimitiveStack(params T[] contents) : base() {
            if (contents != null) {
                for (int i = 0; i < contents.Length; i++) {
                    this.Push(contents[i]);
                }
            }
        }

        public object Clone() {
            PrimitiveStack<T> copy = new PrimitiveStack<T>();
            foreach (T t in this.ToArray()) {
                copy.Push(t);
            }
            return copy;
        }

    }

}
