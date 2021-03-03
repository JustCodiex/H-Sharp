using System.Collections.Generic;
using System.Linq;

namespace HSharp.Analysis {
    
    public class NamespaceDomain : Domain {

        public override string Name { get; }

        public NamespaceDomain(string name, Domain parentDomain) {

            this.Name = name;
            this.m_subDomains = new List<Domain>();
            this.m_parent = parentDomain;

        }

        public NamespaceDomain GetOrCreateSubNamespace(string name) {
            if (this.m_subDomains.Any(x => x.Name.CompareTo(name) == 0 && x is NamespaceDomain)) {
                return this.m_subDomains.First(x => x.Name.CompareTo(name) == 0) as NamespaceDomain;
            } else {
                NamespaceDomain domain = new NamespaceDomain(name, this);
                this.AddSubdomain(domain);
                return domain;
            }
        }

    }

}
