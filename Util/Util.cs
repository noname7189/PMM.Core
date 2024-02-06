using CryptoExchange.Net.Objects;
using PMM.Core.Provider.Converter;

namespace PMM.Core.Utils
{
    public static class Util
    {
        public static DateTime GetDateTimeFromMilliSeconds(long milliseconds)
        {
            return DateTimeConverter.ConvertFromMilliseconds(milliseconds);
        }
        public static async Task HandleRequest<T>(Func<Task<WebCallResult<T>>> request, Action<T> outputData)
        {
            WebCallResult<T> result = await request();
            if (result.Success)
            {
                {
                    outputData(result.Data);
                }
            }
        }
    }
}
