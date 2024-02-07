using PMM.Core.Interface;

namespace PMM.Core.Provider.Interface
{
    public interface IProvider
    {
        public S AddStreamCore<S>() where S : IStreamCore, new();
    }
}
