using ENTITIES.Models;
using ENTITIES.ViewModels.APP.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IAccountRepository
    {
        public Task<AccountClient> GetAccountClient(long account_client_id);
        public Task<AccountClient> GetByClientId(long client_id);
        long  updateClientChangePassword(ClientChangePasswordViewModel model);
    }
}
