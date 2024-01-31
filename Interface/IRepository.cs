using Microsoft.EntityFrameworkCore;
using PMM.Core.EntityClass;

namespace PMM.Core.Interface
{
    public interface ICandleRepository<X, C> where X : DbContext where C : BaseCandle
    {
        public DbSet<C> CandleRepo(X db);
    }
    public interface IIndicatorRepository<X, I> where X : DbContext where I : BaseIndicator
    {
        public DbSet<I> IndicatorRepo(X db);
    }
    public interface ISignalRepository<X, S> where X : DbContext where S : BaseSignal
    {
        public DbSet<S> SignalRepo(X db);
    }
    public interface IOrderRepository<X, O> where X : DbContext where O : BaseOrder
    {
        public DbSet<O> OrderRepo(X db);
    }
}
