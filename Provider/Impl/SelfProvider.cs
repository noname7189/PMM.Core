using PMM.Core.Enum;
using PMM.Core.Provider.DataClass.Rest;
using PMM.Core.Provider.DataClass.Stream;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.Binance
{
    internal class SelfProvider : BaseProvider
    {
        internal override void CreateContext(ProviderType type)
        {

        }

        public override Task<OrderResult?> CancelOrderAsync(Symbol symbol, long orderId)
        {
            throw new NotImplementedException();
        }


        public override Task<AccountInfo?> GetAccountInfoAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<List<KlineData>?> GetKlinesAsync(Symbol symbol, Interval interval, int? limit)
        {
            throw new NotImplementedException();
        }

        public override Task<string?> GetListenKey()
        {
            throw new NotImplementedException();
        }

        internal override void InitContext()
        {
            throw new NotImplementedException();
        }

        public override Task<OrderResult?> PlaceOrderAsync(Symbol symbol, OrderPosition position, decimal price, decimal quantity)
        {
            throw new NotImplementedException();
        }

        public override Task SubscribeToKlineUpdatesAsync(Symbol symbol, Interval interval, Action<KlineStreamData> onGetStreamData)
        {
            throw new NotImplementedException();
        }

        public override Task SubscribeToUserDataUpdatesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
