using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.ReportRevenue;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL.ReportRevenue
{
    public class ReportRevenueDAL : GenericService<Order>
    {
        private static DbWorker _DbWorker;

        public ReportRevenueDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public async Task<ReportRevenueModel> ReportRevenueByCurrentDate(string formdate, string todate)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[2];
                objParam[0] = formdate == null ? new SqlParameter("@FromDate", formdate) : new SqlParameter("@FromDate", formdate);
                objParam[1] = todate == null ? new SqlParameter("@ToDate", todate) : new SqlParameter("@ToDate", todate);
                DataTable dt = _DbWorker.GetDataTable(StoreProceduresName.SP_Report_TotalRevenueOrderByDay, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                  var model = dt.ToList<ReportRevenueModel>();
                    return model[0];
                }

                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ReportRevenueByCurrentDate - ReportRevenueDAL: " + ex);
            }
            return null;
        } 
        public async Task<TotalAccountBalanceModel> GetTotalAccountBalance(DateTime? formdate, DateTime? todate)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[2];
                objParam[0] = new SqlParameter("@FromDate", formdate);
                objParam[1] = new SqlParameter("@ToDate", todate);
                DataTable dt = _DbWorker.GetDataTable(StoreProceduresName.sp_GetTotalAccountBalance, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                  var model = dt.ToList<TotalAccountBalanceModel>();
                    return model[0];
                }

                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalAccountBalance - ReportRevenueDAL: " + ex);
            }
            return null;
        }
    }
}
