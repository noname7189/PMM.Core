using Binance.Net.Clients;
using Quartz;
using Quartz.Impl;

namespace PMM.Core.Utils
{
    internal class KeepAliveJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            string listenKey = context.MergedJobDataMap.GetString("ListenKey") ?? throw new Exception($"JobDataMap - listenkey null exception");
            using var client = new BinanceRestClient();
            var result = await client.UsdFuturesApi.Account.KeepAliveUserStreamAsync(listenKey);
            if (result.Success) Console.WriteLine("KeepAlive Success");
            else Console.WriteLine("KeepAlive Fail");
        }
    }
    internal static class KeepAliveScheduler
    {
        private static readonly IScheduler _scheduler = (IScheduler)new StdSchedulerFactory().GetScheduler();
        internal static void Run(string listenKey)
        {
            IJobDetail job = JobBuilder.Create<KeepAliveJob>()
                .WithIdentity("KeepAliveJob", "JobGroup")
                .UsingJobData("ListenKey", listenKey)
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
