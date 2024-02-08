using Binance.Net.Clients;
using PMM.Core.Provider.Binance;
using Quartz;
using Quartz.Impl;

namespace PMM.Core.Utils
{
    internal class KeepAliveJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            bool success = await BinanceProvider.KeepAlive();

            if (success) Console.WriteLine("KeepAlive Success");
            else Console.WriteLine("KeepAlive Fail");
        }
    }
    internal class KeepAliveScheduler
    {
        private static readonly IScheduler _scheduler = (IScheduler)new StdSchedulerFactory().GetScheduler();
        internal static void Run()
        {
            IJobDetail job = JobBuilder.Create<KeepAliveJob>()
                .WithIdentity("KeepAliveJob", "JobGroup")
                .Build();

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                .WithIdentity("RepeatingTrigger", "TriggerGroup")
                .WithCronSchedule("0 0/30 * * * ?")
                .StartNow()
                .Build();

            _ = _scheduler.ScheduleJob(job, trigger);
            _scheduler.Start();
        }
    }
}
