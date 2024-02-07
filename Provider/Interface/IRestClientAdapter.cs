using PMM.Core.Enum;
using PMM.Core.Provider.DataClass.Rest;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.Interface
{
    public interface IRestClientAdapter
    {
        // StartUserStreamAsync
        Task<Response<string>> GetListenKey();
        // GetAccountInfoASync
        Task<Response<AccountInfo>> GetAccountInfoAsync();
        // GetKlinesAsync
        Task<Response<List<KlineData>>> GetKlinesAsync(Symbol symbol, Interval interval, int? limit);

        Task<Response<OrderResult>> PlaceOrderAsync(Symbol symbol, OrderSide position, decimal price, decimal quantity);

        Task<Response<OrderResult>> CancelOrderAsync(Symbol symbol, long orderId);
    }
}
