using BackgroundTask_Quartz.AppCode.BLL;
using BackgroundTask_Quartz.AppCode.DAL;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using static Quartz.Logging.OperationName;

namespace BackgroundTask_Quartz
{
    [DisallowConcurrentExecution]
    public class UpdateDatabaseJob : IJob
    {
        private readonly ILogger<UpdateDatabaseJob> _logger;
    

        private readonly clsPackageManageBO _clsPackageManageBO;

        public UpdateDatabaseJob(ILogger<UpdateDatabaseJob> logger, clsPackageManageBO clsPackageManageBO)
        {
            _logger = logger;
            _clsPackageManageBO = clsPackageManageBO;
        }
        public async Task Execute(IJobExecutionContext context)
        {

            string mess = "";
            string code = "";

            _logger.LogInformation($"RUN UPDATE SALARY TASK AT: {DateTime.Now}");


            if (_clsPackageManageBO.updateEmployeeSalary(ref code, ref mess))
            {
                _logger.LogInformation($"UPDATE SALARY SUCCESS: {mess}-{code}");
            }
            else
            {
                _logger.LogWarning($"UPDATE SALARY FAILURE: {mess}-{code}");
            }
          
            await Task.CompletedTask;
           
        }
    }
}
