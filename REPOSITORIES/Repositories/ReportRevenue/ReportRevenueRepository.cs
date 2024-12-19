using DAL.ReportRevenue;
using Entities.ConfigModels;
using ENTITIES.ViewModels.ReportRevenue;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.IReportRevenue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories.ReportRevenue
{
   public class ReportRevenueRepository : IReportRevenueRepository
    {
        private readonly ReportRevenueDAL reportRevenueDAL;

        public ReportRevenueRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            reportRevenueDAL = new ReportRevenueDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<ReportRevenueModel>  ReportRevenueByCurrentDate(string formdate, string todate)
        {
            
            try
            {
                return await reportRevenueDAL.ReportRevenueByCurrentDate(formdate, todate);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ReportRevenueByCurrentDate - ReportRevenueRepository: " + ex);
            }
            return null;
        }
        public async Task<TotalAccountBalanceModel> GetTotalAccountBalance(DateTime? formdate, DateTime? todate)
        {

            try
            {
                return await reportRevenueDAL.GetTotalAccountBalance(formdate, todate);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ReportRevenueByCurrentDate - ReportRevenueRepository: " + ex);
            }
            return null;
        }
    }
}
