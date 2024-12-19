using ENTITIES.ViewModels.ReportRevenue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories.IReportRevenue
{
    public interface IReportRevenueRepository
    {
        Task<ReportRevenueModel> ReportRevenueByCurrentDate(string formdate, string todate);
        Task<TotalAccountBalanceModel> GetTotalAccountBalance(DateTime? formdate, DateTime? todate);
    }
}
