using System.Collections.Generic;

namespace HSharp.Util {
    
    public sealed class LookupStack<K, V> : Map<K, PrimitiveStack<V>> where V : struct {

        public LookupStack() : base() { }

        public LookupStack(LookupStack<K, V> copy) : base(copy) { }

        public V Top(K key) => this.Lookup(key, new PrimitiveStack<V>(default)).Peek();

        public void Enter(K key, V top) {
            if (this.Exists(key)) {
                this.Lookup(key).Push(top);
            } else {
                this.Insert(key, new PrimitiveStack<V>(top));
            }
        }

        public void Exit(K key) => this.Lookup(key).Pop();

        public (K key, PrimitiveStack<V> value)[] ToArray() {
            (K, PrimitiveStack<V>)[] pairs = new (K, PrimitiveStack<V>)[this.Size];
            int i = 0;
            foreach(KeyValuePair<K, PrimitiveStack<V>> pair in this.m_internalDictionary) {
                pairs[i] = (pair.Key, pair.Value);
                i++;
            }
            return pairs;
        }

    }

}
