using PMM.Core.Enum;
using PMM.Core.Provider.DataClass;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.Interface
{
    internal interface ISocketClientAdapter
    {
        Task SubscribeToUserDataUpdatesAsync();
        Task SubscribeToKlineUpdatesAsync(Symbol symbol, Interval interval, Action<KlineData> onGetStreamData);
    }
}
