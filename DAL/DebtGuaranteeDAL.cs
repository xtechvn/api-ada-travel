using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class DebtGuaranteeDAL : GenericService<Client>
    {
        private DbWorker _dbWorker;
        public DebtGuaranteeDAL(string connection) : base(connection)
        {
            _dbWorker = new DbWorker(connection);
        }
        public async Task<long> UpdateDebtGuarantee(DebtGuarantee model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[6];
                objParam[0] = new SqlParameter("@Id", model.Id);
                objParam[1] = new SqlParameter("@Code",  model.Code);
                objParam[2] = new SqlParameter("@Orderid", model.Orderid);
                objParam[3] = new SqlParameter("@ClientId", model.ClientId);
                objParam[4] = new SqlParameter("@Status", model.Status);
                objParam[5] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                return _dbWorker.ExecuteNonQuery(StoreProceduresName.sp_UpdateDebtGuarantee, objParam);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DebtGuaranteeDAL UpdateDebtGuarantee" + ex);
                return -1;
            }
        }
        public async Task<DataTable> DetailDebtGuarantee(int Id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@Id", Id);

                return _dbWorker.GetDataTable(StoreProceduresName.SP_GeDetailDebtGuarantee, objParam);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DebtGuaranteeDAL DetailDebtGuarantee" + ex);
                return null;
            }
        }
        public async Task<DataTable> DetailDebtGuaranteebyOrderid(int Orderid)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@Orderid", Orderid);

                return _dbWorker.GetDataTable(StoreProceduresName.SP_GetDetailDebtGuaranteeByOrderid, objParam);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DebtGuaranteeDAL DetailDebtGuarantee" + ex);
                return null;
            }
        }
    }
}
