using HSharp.Language;

namespace HSharp.Parsing.AbstractSnyaxTree.Declaration {
    
    public interface IStorageModifiable {

        void AddStorageModifier(StorageModifier modifier);

        StorageModifier GetStorageModifier();

        bool IsAllowedStorageModifier();

    }

}
