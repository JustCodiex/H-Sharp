using HSharp.Language;

namespace HSharp.Parsing.AbstractSnyaxTree.Declaration {

    public interface IAccessModifiable {
    
        void SetAccessModifier(AccessModifier modifier);

        AccessModifier GetAccessModifier();

    }

}
