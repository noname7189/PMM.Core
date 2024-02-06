using PMM.Core.Interface;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.Interface
{
    public interface IProvider
    {
        // TODO : Deprecate
        public LibProvider GetLibProviderType();
        public S AddStreamCore<S>() where S : IStreamCore, new();
    }
}
