using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.Clients;
using System.Threading.Tasks;
using System;
using Utilities;

namespace REPOSITORIES.Repositories.Clients
{
    public class AccountClientRepository : IAccountClientRepository
    {
        private readonly AccountClientDAL accountClientDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public AccountClientRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            accountClientDAL = new AccountClientDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            dataBaseConfig = _dataBaseConfig;
        }
        public AccountClient GetByUsername(string username)
        {
            return accountClientDAL.GetByUserName(username);
        }

        public int UpdatePassword(string email, string password)
        {
            return accountClientDAL.UpdatePassword(email, password);
        }
        public async Task<AccountClient> GetAccountClient(long account_client_id)
        {
            return await accountClientDAL.GetByID(account_client_id);

        }

        public AccountClient GetAccountClientByUserName(string username, int type)
        {
            return  accountClientDAL.GetAccountClientByUserName(username, type);
        }
    }
}
