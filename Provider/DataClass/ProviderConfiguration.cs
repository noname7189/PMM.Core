
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.DataClass
{
    public record ProviderConfiguration
    {
        public required Exchange Exchange { get; init; }
        public required LibProvider LibProvider { get; init; }
        public required string PublicKey { get; init; }
        public required string SecretKey{ get; init; }
    }
}
