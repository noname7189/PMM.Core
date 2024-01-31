using CryptoExchange.Net.Objects;

namespace PMM.Core.Utils
{
    public static class Util
    {
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
