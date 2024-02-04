using PMM.Core.EntityClass;
using PMM.Core.Enum;
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
        public abstract List<Action<OrderStreamData>> OrderCallbackList { get; }
        #endregion
        #region Util
        internal abstract bool Exists(Symbol symbol, Interval interval);
        internal abstract bool AddedCandleExists();
        #endregion
        public abstract IStreamCore AddStrategy<S>() where S : IStrategy, new();
        public abstract void PreStreamInit();
        public abstract void PostStreamInit();
        public abstract Action<List<KlineStreamData>> OnGetBaseCandle();
        public abstract Action<KlineStreamData> OnGetStreamData();
        public abstract void InitStreamWithoutAdditionalCandles();
        public abstract void InitStreamWithAdditionalCandles();

        #region Interface Declaration
        internal abstract void BindStrategy(IRestClientAdapter adapter);
        internal abstract void ExecuteChain_TryToMakeNewIndicator();
        internal abstract void ExecuteChain_ProcessWithSameCandle(KlineStreamData klines);
        internal abstract void ExecuteChain_ProcessWithDifferentCandle(KlineStreamData klines, BaseCandle prevCandle);
        internal abstract void ExecuteChain_PreStrategyInit();
        internal abstract void ExecuteChain_InitStrategyWithoutAdditionalCandles();
        internal abstract void ExecuteChain_InitStrategyWithAdditionalCandles();
        internal abstract void ExecuteChain_PostStrategyInit();
        #endregion
    }
}
