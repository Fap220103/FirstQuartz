using BackgroundTask_Quartz.AppCode.BLL;
using BackgroundTask_Quartz.AppCode.DAL;
using Microsoft.Extensions.Configuration;
using Quartz;
using System.Configuration;
using static Quartz.Logging.OperationName;

namespace BackgroundTask_Quartz
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Environment.SetEnvironmentVariable("TNS_ADMIN", @"F:\ThucTap\windowx64\WINDOWS.X64_193000_db_home\network\admin");
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Services.AddScoped<clsORCLdao>();
            builder.Services.AddScoped<clsPackageManageBO>();

       
            // builder.Services.AddSingleton<QuartzTriggerUpdater>();
            var cronExpression = builder.Configuration.GetValue<string>("Quartz:CronExpression");

            builder.Services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                // Thêm Job
                q.AddJob<UpdateDatabaseJob>(opts => opts.WithIdentity("UpdateDatabaseJob"));

                // Thêm Trigger với thời gian từ cấu hình
                q.AddTrigger(opts => opts
                    .ForJob("UpdateDatabaseJob")
                    .WithIdentity("UpdateDatabaseTrigger")
                    .StartNow()
                    .WithCronSchedule(cronExpression));
            });
            // Thêm Quartz Hosted Service
            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            builder.Services.AddSingleton<IScheduler>(provider =>
            {
                var scheduler = provider.GetRequiredService<ISchedulerFactory>().GetScheduler().Result;
                return scheduler;
            });

            builder.Services.AddHostedService<QuartzConfigWatcherService>();
            var app = builder.Build();

           



            //var updater = app.Services.GetRequiredService<QuartzTriggerUpdater>();

            app.MapGet("/", () => "Hello World!");
        
            app.Run();
        }
    }
}