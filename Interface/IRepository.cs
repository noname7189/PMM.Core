using Microsoft.EntityFrameworkCore;
using PMM.Core.EntityClass;

namespace PMM.Core.Interface
{
    public interface ICandleRepository<X, C> where X : DbContext where C : OHLCV
    {
        public DbSet<C> CandleRepo(X db);
    }
    public interface IIndicatorRepository<X, I> where X : DbContext where I : Indicator
    {
        public DbSet<I> IndicatorRepo(X db);
    }
    public interface ISignalRepository<X, S> where X : DbContext where S : Signal
    {
        public DbSet<S> SignalRepo(X db);
    }
    public interface IOrderRepository<X, O> where X : DbContext where O : Order
    {
        public DbSet<O> OrderRepo(X db);
    }
}
