using System;
using System.Collections.Generic;
using System.Linq;
using HSharp.Analysis.TypeData;

using ValueType = HSharp.Analysis.TypeData.ValueType;

namespace HSharp.Analysis {
    
    public abstract class Domain {

        protected List<Domain> m_subDomains;
        protected Domain m_parent;

        public abstract string Name { get; }

        public void AddSubdomain(Domain subDomain) {
            this.m_subDomains.Add(subDomain);
            subDomain.m_parent = this;
        }

        public bool HasDomain(string domainName) => this.m_subDomains.Any(x => x.Name.CompareTo(domainName) == 0);

        public List<T> OfType<T>() where T : HSharpType => this.m_subDomains.Where(x => x is T).Cast<T>().ToList();

        public T Get<T>(string locName) where T : HSharpType => (T)this.m_subDomains.FirstOrDefault(x => x.Name.CompareTo(locName) == 0);

        public T First<T>(string typeName) {

            int dotter = typeName.IndexOf('.');
            if (dotter > 0) {

                throw new NotImplementedException();

            } else {
                if (this.m_subDomains.FirstOrDefault(x => x.Name.CompareTo(typeName) == 0) is T self) {
                    return self;
                } else {
                    if (this.m_parent is not null && this.m_parent.First<T>(typeName) is T parent) {
                        return parent;
                    } else {
                        return default;
                    }
                }
            }

        }

        public override string ToString() => (this.m_parent is null || this.m_parent.Name.CompareTo("global") == 0) ? this.Name : $"{this.m_parent}.{this.Name}";

        public static Domain GetGlobalDomain() {

            // Create domain
            NamespaceDomain globDom = new NamespaceDomain("global", null);
            globDom.AddSubdomain(new ValueType("bool", sizeof(bool), true));
            globDom.AddSubdomain(new ValueType("int", sizeof(int), true));
            globDom.AddSubdomain(new ValueType("float", sizeof(float), true));

            // Return domain
            return globDom;

        }

    }

}
