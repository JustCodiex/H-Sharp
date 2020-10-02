using System.Collections.Generic;

namespace HSharp.Analysis {
    
    public class NamespaceDomain : Domain {

        public override string Name { get; }

        public NamespaceDomain(string name, Domain parentDomain) {

            this.Name = name;
            this.m_subDomains = new List<Domain>();
            this.m_parent = parentDomain;

        }


    }

}
