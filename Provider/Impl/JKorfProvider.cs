using Binance.Net.Clients;
using Binance.Net.Objects.Models.Futures;
using PMM.Core.Provider.Enum;
using Binance.Net.Interfaces;
using PMM.Core.Utils;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Objects;
using Binance.Net.Enums;
using PMM.Core.Provider.DataClass.Stream;
using CryptoExchange.Net.Authentication;
using PMM.Core.Provider.DataClass.Rest;
using Binance.Net.Objects.Models.Futures.Socket;
using Binance.Net.Objects.Models;
using Symbol = PMM.Core.Enum.Symbol;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using PMM.Core.Provider.DataClass.Stream.EventRecvData;

namespace PMM.Core.Provider.Impl
{
    internal class JKorfProvider : BaseProvider
    {
        // TODO : Deprecate
        public override LibProvider GetLibProviderType()
        {
            return LibProvider.JKorf;
        }

        internal override void InitContext()
        {
            ApiCredentials credentials = new(PublicKey, SecretKey);
            BinanceRestClient.SetDefaultOptions(opt =>
            {
                opt.ApiCredentials = credentials;
            });
            BinanceSocketClient.SetDefaultOptions(opt =>
            {
                opt.ApiCredentials = credentials;
            });
        }
        internal override void CreateContext(ProviderType type)
        {
            if (type == ProviderType.Rest)
            {
                ClientContext = new BinanceRestClient();

            }
            else if (type == ProviderType.Socket)
            {
                ClientContext = new BinanceSocketClient();
            }
            else throw new NotImplementedException();
        }
        public async override Task<Response<AccountInfo>> GetAccountInfoAsync()
        {
            if (ClientContext is BinanceRestClient)
            {
                WebCallResult<BinanceFuturesAccountInfo> result = await ClientContext.UsdFuturesApi.Account.GetAccountInfoAsync();
                if (result.Success)
                {
                    BinanceFuturesAccountInfo data1 = result.Data;

                    AccountInfo data = new()
                    {
                        TotalInitialMargin = data1.TotalInitialMargin,
                        TotalMarginBalance = data1.TotalMarginBalance,
                        TotalUnrealizedProfit = data1.TotalUnrealizedProfit,
                        TotalWalletBalance = data1.TotalWalletBalance,
                        TotalCrossWalletBalance = data1.TotalCrossWalletBalance,
                        AvailableBalance = data1.AvailableBalance,
                        UpdateTime = data1.UpdateTime,
                    };

                    return Success(data);
                }
                else return Failure<AccountInfo, BinanceFuturesAccountInfo>(result);
            }
            else throw new Exception("ClientContext is not BinanceRestClient");
        }

        public async override Task<Response<List<KlineData>>> GetKlinesAsync(Symbol symbol, Interval interval, int? limit)
        {
            if (ClientContext is BinanceRestClient)
            {
                WebCallResult<IEnumerable<IBinanceKline>> result = await ClientContext.UsdFuturesApi.ExchangeData.GetKlinesAsync(symbol.ToString(), interval.ToBinanceInterval(), InitCandleCount);
                if (result.Success)
                {
                    IEnumerable<IBinanceKline> data1 = result.Data;
                    List<KlineData> res = [];
                    foreach (var item in data1)
                    {
                        res.Add(new KlineData()
                        {
                            StartTime = item.OpenTime,
                            Open = item.OpenPrice,
                            High = item.HighPrice,
                            Low = item.LowPrice,
                            Close = item.ClosePrice,
                            Volume = item.Volume,
                            TradeCount = item.TradeCount,
                        });
                    }
                    return Success(res);
                }
                else
                {
                    return Failure<List<KlineData>, IEnumerable<IBinanceKline>>(result);
                }
            }
            else throw new Exception("ClientContext is not BinanceRestClient");
        }


        public override async Task<Response<string>> GetListenKey()
        {
            if (ClientContext is BinanceRestClient)
            {
                WebCallResult<string> result = await ClientContext.UsdFuturesApi.Account.StartUserStreamAsync();
                if (result.Success)
                {
                    return Success(result.Data);
                }
                else return Failure<string, string>(result);
            }
            else throw new Exception("ClientContext is not BinanceRestClient");
        }


        public async override Task<Response<OrderResult>> PlaceOrderAsync(Symbol symbol, OrderPosition position, decimal price, decimal quantity)
        {
            using var client = new BinanceRestClient();

            var orderData = await client.UsdFuturesApi.Trading.PlaceOrderAsync(
                symbol: symbol.ToString(),
                side: SideConverter(position),
                type: FuturesOrderType.Limit,
                price: price,
                quantity: quantity,
                timeInForce: TimeInForce.GoodTillCanceled);

            if (orderData.Success)
            {
                BinanceUsdFuturesOrder data = orderData.Data;

                OrderResult res = new()
                {
                    OrderId = data.Id,
                    TradeId = null,
                    Symbol = symbol,
                    Side = SideConverter(data.Side),
                    Status = StatusConverter(data.Status),
                    Price = data.Price,
                    AveragePrice = data.AveragePrice,
                    QuantityFilled = data.QuantityFilled,
                    FulfilledQuantity = data.CummulativeQuantity ?? 0,
                    Quantity = data.Quantity,
                    Final = data.ClosePosition,
                    UpdateTime = data.UpdateTime,
                    CreateTime = data.CreateTime,
                };

                return Success(res);
            }
            else return Failure<OrderResult, BinanceUsdFuturesOrder>(orderData);
        }

        public async override Task<Response<OrderResult>> CancelOrderAsync(Symbol symbol, long orderId)
        {
            using var client = new BinanceRestClient();

            var cancelData = await client.UsdFuturesApi.Trading.CancelOrderAsync(symbol.ToString(), orderId: orderId);

            if (cancelData.Success)
            {
                BinanceUsdFuturesOrder data = cancelData.Data;

                OrderResult res = new()
                {
                    OrderId = data.Id,
                    TradeId = null,
                    Symbol = symbol,
                    Side = SideConverter(data.Side),
                    Status = StatusConverter(data.Status),
                    Price = data.Price,
                    AveragePrice = data.AveragePrice,
                    QuantityFilled = data.QuantityFilled,
                    FulfilledQuantity = data.CummulativeQuantity ?? 0,
                    Quantity = data.Quantity,
                    Final = data.ClosePosition,
                    UpdateTime = data.UpdateTime,
                    CreateTime = data.CreateTime,
                };
                return Success(res);
            }
            else return Failure<OrderResult, BinanceUsdFuturesOrder>(cancelData); 
        }

        public async override Task SubscribeToKlineUpdatesAsync(Symbol symbol, Interval interval, Action<KlineStreamRawData> onGetStreamData)
        {
            if (ClientContext is BinanceSocketClient)
            {
                Action<DataEvent<IBinanceStreamKlineData>> handler = (data) =>
                {
                    IBinanceStreamKline data1 = data.Data.Data;
                    onGetStreamData(new KlineStreamData()
                    {
                        StartTime = data1.OpenTime,
                        EndTime = data1.CloseTime,
                        Open = data1.OpenPrice,
                        High = data1.HighPrice,
                        Low = data1.LowPrice,
                        Close = data1.ClosePrice,
                        Volume = data1.Volume,
                        TradeCount = data1.TradeCount,
                        Final = data1.Final,
                    });
                };

                CallResult<UpdateSubscription> result = await ClientContext.UsdFuturesApi.SubscribeToKlineUpdatesAsync(symbol.ToString(), interval.ToBinanceInterval(), handler);
                if (result.Success == false)
                {
                    throw new Exception($"Subscribe for \"{symbol}\" is failed");
                }
            }
        }

        public async override Task SubscribeToUserDataUpdatesAsync()
        {            
            if (ClientContext is BinanceSocketClient)
            {
                Action<DataEvent<BinanceFuturesStreamAccountUpdate>>? onAccountUpdateHandler = OnAccountUpdate != null ? (data) =>
                {

                    BinanceFuturesStreamAccountUpdateData data1 = data.Data.UpdateData;
                    AccountStreamRecv data2 = new(data1, data.Data.TransactionTime);

                    OnAccountUpdate.Invoke(data2);
                }
                : null;

                Action<DataEvent<BinanceFuturesStreamOrderUpdate>>? onOrderUpdateHandler = CheckChainOnOrderUpdate() == true ? (data) =>
                {
                    BinanceFuturesStreamOrderUpdateData data1 = data.Data.UpdateData;

                    OrderStreamRecv data2 = new(data1, data.Data.EventTime);
                    OnOrderUpdate(data2);
                }
                : null;

                Action<DataEvent<BinanceStreamEvent>> onListenKeyExpiredHandler = OnListenKeyExpired != null ? (data) =>
                {
                    BaseStreamRecv e = new()
                    {
                        Event = StreamEventType.ListenkeyExpired,
                        EventTime = data.Data.EventTime,
                    };

                    OnListenKeyExpired.Invoke(e);
                }
                : (data) => { };

                CallResult<UpdateSubscription> result = await ClientContext.UsdFuturesApi.SubscribeToUserDataUpdatesAsync(ListenKey, null, null, onAccountUpdateHandler, onOrderUpdateHandler, onListenKeyExpiredHandler, null, null, null);
                if (result.Success == false)
                {
                    throw new Exception($"Subscribe for UserDataUpdate is failed");
                }
            }
        }

        private static Response<T> Success<T>(T data) where T : class
        {
            return new()
            {
                StatusCode = HttpStatusCode.OK,
                Msg = null,
                Data = data
            };
        }

        private static Response<T> Failure<T,S>(WebCallResult<S> result) where T : class where S : class
        {
            return new()
            {
                StatusCode = (HttpStatusCode)result.ResponseStatusCode,
                Msg = result.Error.Message,
                Data = null
            };
        }



        internal static Symbol SymbolConverter(string symbol)
        {
            return symbol switch
            {
                "ETHUSDT" => Symbol.ETHUSDT,
                "BTCUSDT" => Symbol.BTCUSDT,
                _ => throw new ArgumentException(symbol, nameof(symbol)),
            };
        }

        internal static UpdateReason? ReasonConverter(AccountUpdateReason reason)
        {
            return reason switch
            {
                AccountUpdateReason.FundingFee => UpdateReason.FundingFee,
                _ => null,
            };
        }


        internal static OrderPosition SideConverter(OrderSide side)
        {
            return side switch
            {
                OrderSide.Buy => OrderPosition.Long,
                OrderSide.Sell => OrderPosition.Short,
                _ => throw new NotImplementedException(),
            };
        }

        internal static OrderSide SideConverter(OrderPosition side)
        {
            return side switch
            {
                OrderPosition.Long => OrderSide.Buy,
                OrderPosition.Short => OrderSide.Sell,
                _ => throw new NotImplementedException(),
            };
        }

        internal static OrderStatusType StatusConverter(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.New => OrderStatusType.New,
                OrderStatus.PartiallyFilled => OrderStatusType.PartiallyFilled,
                OrderStatus.Filled => OrderStatusType.Filled,
                OrderStatus.Canceled => OrderStatusType.Canceled,
                _ => OrderStatusType.Others,
            };
        }

    }
}
