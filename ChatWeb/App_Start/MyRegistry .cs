using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatWeb.App_Start
{
    public class MyRegistry: Registry
    {
        public MyRegistry()
        {
            // 立即执行每两秒一次的计划任务。（指定一个时间间隔运行，根据自己需求，可以是秒、分、时、天、月、年等。）
            //Schedule<MyJob>().ToRunNow().AndEvery(2).Seconds();

            // 延迟一个指定时间间隔执行一次计划任务。（当然，这个间隔依然可以是秒、分、时、天、月、年等。）
            //Schedule<MyJob>().ToRunOnceIn(5).Seconds();

            // 在一个指定时间执行计划任务（最常用。这里是在每天的下午 1:10 分执行）
            //Schedule(() => Console.WriteLine("It's 9:15 PM now.")).ToRunEvery(1).Days().At(21, 15);

            // 立即执行一个在每月的星期一 3:00 的计划任务（可以看出来这个一个比较复杂点的时间，它意思是它也能做到！）
            //Schedule<MyComplexJob>().ToRunNow().AndEvery(1).Months().OnTheFirst(DayOfWeek.Monday).At(3, 0);

            // 在同一个计划中执行两个（多个）任务
            //Schedule<MyJob>().AndThen<MyOtherJob>().ToRunNow().AndEvery(5).Minutes();

            //每n月的第1天的12点
            Schedule<SampleJob>().ToRunEvery(1).Months().On(1).At(12,0);
        }
        public class SampleJob : IJob
        {
            public void Execute()
            {
                DateTime times=DateTime.Now;                  
            }
        }
    }
}