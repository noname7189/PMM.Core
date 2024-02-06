
using PMM.Core.Provider.DataClass.Rest;
using PMM.Core.Provider.DataClass.Stream;
using PMM.Core.Provider.DataClass.Stream.EventRecvData;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.DataClass
{
    public record ProviderConfiguration
    {
        public required Exchange Exchange { get; init; }
        public required LibProvider LibProvider { get; init; }
        public required string PublicKey { get; init; }
        public required string SecretKey{ get; init; }

        public required Action<AccountStreamRecv>? OnAccountUpdate {  get; init; }
        public required Action<AccountInfo>? OnGetAccountInfo { get; init; }
        public required Action<BaseStreamRecv>? OnListenKeyExpired { get; init; }
    }
}
