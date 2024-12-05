using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BackgroundTask_Quartz.AppCode.DAL
{
    public class clsORCLdao
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<clsORCLdao> _logger;
        private readonly string _connectionString;

        public clsORCLdao(IConfiguration configuration,ILogger<clsORCLdao> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration["ConnectionStrings:OracleConnection"];
        }

        public int updateEmployeeSalary_dao(ref string returnCode, ref string returnMess)
        {
            int iReturnInt = 0;
            try
            {
                using (OracleConnection conn = new OracleConnection(_connectionString))
                {
                    OracleCommand objCommand = new OracleCommand();
                    objCommand.Connection = conn;
                    objCommand.CommandType = CommandType.StoredProcedure;
                    objCommand.CommandText = "spc##usertest_temployee_update";
                    OracleParameter[] arrParams = new OracleParameter[2];

                    arrParams[0] = new OracleParameter("preturnmess", OracleDbType.NVarchar2, 200);
                    arrParams[0].Direction = ParameterDirection.Output;
                    arrParams[0].Value = DBNull.Value;

                    arrParams[1] = new OracleParameter("preturncode", OracleDbType.Int16);
                    arrParams[1].Direction = ParameterDirection.Output;
                    arrParams[1].Value = DBNull.Value;

                    conn.Open();
                    objCommand.Parameters.AddRange(arrParams);
                    objCommand.ExecuteNonQuery();
                    returnCode = arrParams[1].Value.ToString();
                    returnMess = arrParams[0].Value.ToString();

                    conn.Close();
                    iReturnInt = int.Parse(returnCode);
                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating employee salary");
                throw; 
            }
            return iReturnInt;
        }
    }
}
