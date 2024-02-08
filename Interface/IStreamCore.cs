using PMM.Core.EntityClass;
using PMM.Core.Enum;
using PMM.Core.Provider.DataClass.Rest;
using PMM.Core.Provider.DataClass.Stream;
using PMM.Core.Provider.Enum;
using PMM.Core.Provider.Interface;

namespace PMM.Core.Interface
{
    public abstract class IStreamCore
    {
        #region Property
        public abstract Symbol Symbol { get; }
        public abstract Interval Interval { get; }
        public abstract List<Action<OrderStreamRecv>> OrderCallbackList { get; }
        public abstract Action<List<KlineData>> OnGetBaseCandle { get; }
        public abstract Action<KlineStreamRawData> OnGetStreamData { get; }

        #endregion
        #region Util
        internal abstract bool Exists(Symbol symbol, Interval interval);
        internal abstract bool AddedCandleExists();
        #endregion
        public abstract IStreamCore AddStrategy<S>() where S : IStrategy, new();
        public abstract void PreStreamInit();
        public abstract void PostStreamInit();
        public abstract void InitStreamWithoutAdditionalCandles();
        public abstract void InitStreamWithAdditionalCandles();

        #region Interface Declaration
        internal abstract void BindStrategy(IRestClientAdapter adapter);
        internal abstract void ExecuteChain_TryToMakeNewIndicator();
        internal abstract void ExecuteChain_ProcessWithSameCandle(KlineStreamRawData klines);
        internal abstract void ExecuteChain_ProcessWithDifferentCandle(KlineStreamRawData klines, BaseCandle prevCandle);
        internal abstract void ExecuteChain_PreStrategyInit();
        internal abstract void ExecuteChain_InitStrategyWithoutAdditionalCandles();
        internal abstract void ExecuteChain_InitStrategyWithAdditionalCandles();
        internal abstract void ExecuteChain_PostStrategyInit();
        #endregion
    }
}
