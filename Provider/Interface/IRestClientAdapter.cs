using PMM.Core.Enum;
using PMM.Core.Provider.DataClass;
using PMM.Core.Provider.DataClass.Stream;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.Interface
{
    internal interface IRestClientAdapter
    {
        // StartUserStreamAsync
        Task<string?> GetListenKey();
        // GetAccountInfoASync
        Task<AccountInfo?> GetAccountInfoAsync();
        // GetKlinesAsync
        Task<List<KlineData>?> GetKlinesAsync(Symbol symbol, Interval interval, int? limit);

        Task<OrderResult?> PlaceOrderAsync(Symbol symbol, OrderPosition position, decimal price, decimal quantity);

        Task<OrderResult?> CancelOrderAsync(Symbol symbol, long orderId);
    }
}
