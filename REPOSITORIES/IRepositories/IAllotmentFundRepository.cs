using ENTITIES.Models;
using ENTITIES.ViewModels.AllotmentFunds;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IAllotmentFundRepository
    {
        public Task<long> AddAllotmentHistory(ENTITIES.Models.AllotmentHistory model);
        public Task<long> UpdateFundBalanceByTransfer(AllotmentFundTransferViewModel model);
        public int AddAllotmentUse(AllotmentUse model);
        Task<long> UpdateFundBalanceByAllotmentUse(AllotmentUse model);
    }
}
