using CryptoExchange.Net.Objects;

namespace PMM.Core.Utils
{
    public static class Util
    {
        private static readonly object obj = new();
        private static int appSignalId = 1;
        public static int AppSignalId
        {
            get
            {
                lock (obj)
                {
                    return appSignalId++;
                }
            }
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
