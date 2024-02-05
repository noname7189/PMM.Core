using Binance.Net.Enums;
using Newtonsoft.Json;
using PMM.Core.Enum;
using PMM.Core.Provider.Converter;
using PMM.Core.Provider.DataClass.Rest;
using PMM.Core.Provider.DataClass.Stream;
using PMM.Core.Provider.Enum;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PMM.Core.Provider.Binance
{
    internal class SelfProvider : BaseProvider
    {
        private const string AccountEndpoint = @"/fapi/v2/account";
        private const string KlinesEndpoint = @"/fapi/v1/klines";
        private const string ListenkeyEndpoint = @"/fapi/v1/listenKey";
        private const string OrderEndpoint = @"/fapi/v1/order";

        private readonly HttpClient Client = new();
        private readonly HMACSHA256 Encryptor = new();
        private long _timeOffset = 0;
        internal override void InitContext()
        {
            Client.BaseAddress = new Uri("https://fapi.binance.com");
            Client.DefaultRequestHeaders.Add("X-MBX-APIKEY", PublicKey);
            Encryptor.Key = Encoding.UTF8.GetBytes(SecretKey);

            _timeOffset = GetOffset().Result;
        }

        internal override void CreateContext(ProviderType type)
        {

        }

        public override async Task<Response<AccountInfo>> GetAccountInfoAsync()
        {
            HttpResponseMessage response = await Client.GetAsync(GetEntireRouteForSigned(AccountEndpoint));

            return await ResponseWrapper<AccountInfo>(response);
        }


        public override async Task<Response<List<KlineData>>> GetKlinesAsync(Symbol symbol, Interval interval, int? limit)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "symbol", symbol.ToString() },
                { "interval", JsonConvert.SerializeObject(interval, new IntervalConverter())},
            };
            if (limit.HasValue) { parameters.Add("limit", limit.Value.ToString()); }

            string query = GetEntireQuery(parameters);

            HttpResponseMessage response = await Client.GetAsync(GetEntireRoute(KlinesEndpoint, query));

            return await ResponseWrapper<List<KlineData>>(response);
        }


        public override async Task<Response<string>> GetListenKey()
        {
            HttpResponseMessage response = await Client.PostAsync(GetEntireRouteForSigned(ListenkeyEndpoint), null);
            return await ResponseWrapper<string>(response);
        }

        public override async Task<Response<OrderResult>> PlaceOrderAsync(Symbol symbol, OrderPosition position, decimal price, decimal quantity)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "symbol", symbol.ToString() },
                { "side", OrderPositionConverter.GetValue(position) },
                { "type", OrderTypeConverter.GetValue(OrderType.Limit) },
                { "timeInForce", GoodTillDateConverter.GetValue(GoodTillDate.GoodTillCanceled) },
                { "quantity", quantity.ToString() },
                { "price", price.ToString() },
            };

            string query = GetEntireQuery(parameters);

            HttpResponseMessage response = await Client.PostAsync(GetEntireRouteForSigned(OrderEndpoint, query), null);
            return await ResponseWrapper<OrderResult>(response);
        }

        public override Task<Response<OrderResult>> CancelOrderAsync(Symbol symbol, long orderId)
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

        private static string GetEntireQuery(Dictionary<string, string> parameters)
        {
            return string.Join("&", parameters.Select(a => $"{a.Key}={a.Value}"));
        }


        private static async Task<Response<T>> ResponseWrapper<T>(HttpResponseMessage response) where T : class
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return new()
                {
                    StatusCode = response.StatusCode,
                    Msg = null,
                    Data = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync())
                };
            }
            else
            {
                return new()
                {
                    StatusCode = response.StatusCode,
                    Msg = JsonConvert.DeserializeObject<ErrorWrapper>(await response.Content.ReadAsStringAsync())!.msg,
                    Data = null
                };
            }
        }


        private async Task<long> GetOffset()
        {
            long timeResult = long.Parse((await (await Client.GetAsync("/fapi/v1/time")).Content.ReadAsStringAsync()).Split(":")[1][0..^1]);


            long timestamp = (long)DateTimeConverter.ConvertToMilliseconds(DateTime.UtcNow)!;

            return timeResult - timestamp;
        }

        private string GetEntireRoute(string endpoint, string? query = null)
        {
            if (query == null) return $"{endpoint}";
            else return $"{endpoint}?{query}";
        }


        private string GetEntireRouteForSigned(string endpoint, string? query = null)
        {
            string body;
            string res;

            if (query == null || query == string.Empty)
            {
                body = $"timestamp={GetAdjustedCurrentTimestamp()}";
            }
            else
            {
                body = $"{query}&timestamp={GetAdjustedCurrentTimestamp()}";
            }

            res = $"{body}&signature={ByteToString(Encryptor.ComputeHash(Encoding.UTF8.GetBytes(body)))}";


            return $"{endpoint}?{res}";
        }
        private long GetAdjustedCurrentTimestamp()
        {
            return (long)DateTimeConverter.ConvertToMilliseconds(DateTime.UtcNow)! + _timeOffset;
        }

        private static string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2"); /* hex format */
            return sbinary;
        }

    }
}
