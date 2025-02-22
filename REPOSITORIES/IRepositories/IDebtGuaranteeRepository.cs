using Entities.ViewModels;
using ENTITIES.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IDebtGuaranteeRepository
    {
        Task<long> UpdateDebtGuarantee(int id, int Status, int CreatedBy);
        Task<DebtGuaranteeViewModel> GetDetailDebtGuarantee(int Id);
        Task<DebtGuaranteeViewModel> DetailDebtGuaranteebyOrderid(int Orderid);
    }
}
