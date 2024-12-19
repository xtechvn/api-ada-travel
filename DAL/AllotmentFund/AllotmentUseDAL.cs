using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.DepositHistory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL.AllotmentFund
{
    public class AllotmentUseDAL : GenericService<ENTITIES.Models.AllotmentUse>
    {

        private static DbWorker _DbWorker;

        public AllotmentUseDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public int CreateAllotmentUse(ENTITIES.Models.AllotmentUse model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    SqlParameter[] objParam = new SqlParameter[]
                    {
                        new SqlParameter("@DataId", model.DataId ),
                        new SqlParameter("@ServiceType", model.ServiceType ),
                        new SqlParameter("@AmountUse", model.AmountUse ),
                        new SqlParameter("@AllomentFundId", model.AllomentFundId ),
                        new SqlParameter("@AccountClientId", model.AccountClientId ),
                        new SqlParameter("@ClientId", model.ClientId ),

                    };
                    return _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_InsertAllotmentUse, objParam);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateAllotmentUse - AllotmentUseDAL: " + ex.ToString());
                return -1;
            }
        }

    }
}
