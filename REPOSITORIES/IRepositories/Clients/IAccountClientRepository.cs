using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories.Clients
{
    public interface IAccountClientRepository
    {
        AccountClient GetByUsername(string username);
        int UpdatePassword(string email, string password);

        Task<AccountClient> GetAccountClient(long account_client_id);
        AccountClient GetAccountClientByUserName(string username, int type);
    }
}
