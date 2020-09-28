using System;
using System.Collections.Generic;

namespace HSharp.Util {
    
    public class Map<K, V> where V : ICloneable {

        protected Dictionary<K, V> m_internalDictionary;

        public int Size => this.m_internalDictionary.Count;

        public Map() {
            this.m_internalDictionary = new Dictionary<K, V>();
        }

        public Map(Map<K, V> copy) {
            this.m_internalDictionary = new Dictionary<K, V>();
            foreach (var pair in copy.m_internalDictionary) {
                this.m_internalDictionary.Add(pair.Key, (V)pair.Value.Clone());
            }
        }

        public V Lookup(K key) => this.Lookup(key, default);

        public V Lookup(K key, V def) {
            if (this.m_internalDictionary.ContainsKey(key)) {
                return this.m_internalDictionary[key];
            } else {
                return def;
            }
        }

        public bool Exists(K key) => this.m_internalDictionary.ContainsKey(key);

        public void Insert(K key, V val) => this.m_internalDictionary.Add(key, val);

    }

}
