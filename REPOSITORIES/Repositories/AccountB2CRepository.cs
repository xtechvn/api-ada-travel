using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ENTITIES.ViewModels.B2C.AccountB2CViewModel;

namespace REPOSITORIES.Repositories
{
    public class AccountB2CRepository : IAccountB2CRepository
    {
        private readonly AccountClientDAL accountClientDAL;


        public AccountB2CRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            accountClientDAL = new AccountClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<int> AddAccountB2C(AccountB2C accountB2)
        {
            
            return await accountClientDAL.AddAccountB2C(accountB2);
        }
        public async Task<bool> checkEmailExtisB2c(string email)
        {
            return await accountClientDAL.checkEmailExtisB2c(email);
        }
      
    }
}
