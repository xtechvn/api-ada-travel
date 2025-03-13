using ENTITIES.ViewModels;
using ENTITIES.ViewModels.Notify;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories.Notify
{
    public interface INotifyRepository
    {
       
        DataTable getListUserByRoleId(int role_id);
        List<int> getManagerByUserId(int user_id);
        DataTable getSalerIdByOrderNo(string order_no);
        DataTable getListOperatorByOrderNo(string order_no);
        DataTable getSalerIdByContractNo(string contract_no);
        List<int> GetLeaderByUserId(int user_id);
        Task<DebtGuaranteeViewModel> GetDetailDebtGuaranteeByDebtGuaranteeCode(string Code);
    }
}
