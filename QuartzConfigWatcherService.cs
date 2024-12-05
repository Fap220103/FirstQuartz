using Quartz;

namespace BackgroundTask_Quartz
{
    public class QuartzConfigWatcherService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IScheduler _scheduler;
        private readonly ILogger<QuartzConfigWatcherService> _logger;
        private string _currentIntervalSeconds;
        public DateTimeOffset? _startDate;
        public DateTimeOffset? _endDate;

        public QuartzConfigWatcherService(IConfiguration configuration, IScheduler scheduler, ILogger<QuartzConfigWatcherService> logger)
        {
            _configuration = configuration;
            _scheduler = scheduler;
            _logger = logger;
            _currentIntervalSeconds = _configuration.GetValue<string>("Quartz:CronExpression");
            _startDate = _configuration.GetValue<DateTimeOffset?>("Quartz:StartAt");
            _endDate = _configuration.GetValue<DateTimeOffset?>("Quartz:EndAt");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Lấy giá trị từ cấu hình
                    var cronExpression = _configuration.GetValue<string>("Quartz:CronExpression");
                    var startAt = _configuration.GetValue<DateTimeOffset?>("Quartz:StartAt");
                    var endAt = _configuration.GetValue<DateTimeOffset?>("Quartz:EndAt");



                    if (cronExpression != _currentIntervalSeconds || _startDate != startAt || _endDate != endAt)
                    {
                        _currentIntervalSeconds = cronExpression;
                        _startDate = startAt;
                        _endDate = endAt;
                        await UpdateTrigger(_scheduler, _currentIntervalSeconds, _startDate, _endDate);
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi để biết nguyên nhân
                    _logger.LogError(ex, "An error occurred while updating the trigger.");
                }
                await Task.Delay(5000, stoppingToken); // Kiểm tra lại sau mỗi 5 giây
            }
        }

        private async Task UpdateTrigger(IScheduler scheduler, string cronExpression, DateTimeOffset? startAt, DateTimeOffset? endAt)
        {
            try
            {
                // Kiểm tra xem job có tồn tại không, nếu không thì thêm lại
                var jobKey = new JobKey("UpdateDatabaseJob");
                var jobExists = await scheduler.CheckExists(jobKey);

                if (!jobExists)
                {
                    // Thêm lại job nếu chưa có
                    var job = JobBuilder.Create<UpdateDatabaseJob>()
                        .WithIdentity("UpdateDatabaseJob")
                        .Build();

                    await scheduler.AddJob(job, true);  // true có nghĩa là job sẽ được thay thế nếu đã tồn tại
                }

                // Tạo trigger mới với thời gian mới
                var oldTriggerKey = new TriggerKey("UpdateDatabaseTrigger");
                var newTrigger = TriggerBuilder.Create()
                    .ForJob(jobKey)
                    .WithIdentity("UpdateDatabaseTrigger")
                    .WithCronSchedule(cronExpression)
                    .StartAt(startAt ?? DateTimeOffset.Now)  
                    .EndAt(endAt)  
                    .Build();

                // Thêm trigger mới
                await scheduler.RescheduleJob(oldTriggerKey, newTrigger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the trigger or job.");
            }
        }
    }

}
