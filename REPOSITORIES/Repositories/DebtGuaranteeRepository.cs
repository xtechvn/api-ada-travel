using DAL;
using Entities.ConfigModels;
using Entities.ViewModels;
using ENTITIES.Models;
using ENTITIES.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories
{
    public class DebtGuaranteeRepository : IDebtGuaranteeRepository
    {
        private readonly DebtGuaranteeDAL debtGuaranteeDAL;

        public DebtGuaranteeRepository(IOptions<DataBaseConfig> dataBaseConfig, ILogger<AllCodeRepository> logger)
        {
            debtGuaranteeDAL = new DebtGuaranteeDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
    
        public async Task<long> UpdateDebtGuarantee(int id, int Status, int CreatedBy)
        {
            try
            {
                var model = new DebtGuarantee();
                model.Id = id;
                model.UpdatedBy = CreatedBy;
                model.Status = Status;
                return await debtGuaranteeDAL.UpdateDebtGuarantee(model);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdataContactStatus - PolicyDetailDAL. " + ex);
                return 0;
            }
        }

        public async Task<DebtGuaranteeViewModel> GetDetailDebtGuarantee(int Id)
        {
            try
            {
                DataTable dt = await debtGuaranteeDAL.DetailDebtGuarantee(Id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<DebtGuaranteeViewModel>();
                    return data[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertDebtGuarantee - PolicyDetailDAL. " + ex);
                return null;
            }
        }
        public async Task<DebtGuaranteeViewModel> DetailDebtGuaranteebyOrderid(int Id)
        {
            try
            {
                DataTable dt = await debtGuaranteeDAL.DetailDebtGuaranteebyOrderid(Id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<DebtGuaranteeViewModel>();
                    return data[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertDebtGuarantee - PolicyDetailDAL. " + ex);
                return null;
            }
        }
    }
}
