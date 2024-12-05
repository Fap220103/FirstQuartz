using BackgroundTask_Quartz.AppCode.DAL;

namespace BackgroundTask_Quartz.AppCode.BLL
{
    public class clsPackageManageBO
    {
        private readonly clsORCLdao _clsORCLdao;
        private readonly ILogger<clsPackageManageBO> _logger;

        public clsPackageManageBO(clsORCLdao clsORCLdao,ILogger<clsPackageManageBO> logger)
        {
            _clsORCLdao = clsORCLdao;
            _logger = logger;
        }
        public bool updateEmployeeSalary(ref string returnCode, ref string returnMess)
        {
            if (_clsORCLdao.updateEmployeeSalary_dao(ref returnCode, ref returnMess) > 0)
            {
                return true;
            }
            else return false;
            
        }
    }
}
