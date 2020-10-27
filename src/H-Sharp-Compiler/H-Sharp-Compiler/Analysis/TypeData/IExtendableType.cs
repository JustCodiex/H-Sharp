namespace HSharp.Analysis.TypeData {
    
    public interface IExtendableType {

        IExtendableType Base { get; }

        bool IsExtensionOf(IExtendableType type);

    }

}
