using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.APP.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountClientDAL accountClientDAL;

        public AccountRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            accountClientDAL = new AccountClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<AccountClient> GetAccountClient(long account_client_id)
        {
            var rs = string.Empty;
            try
            {
                return await accountClientDAL.GetByID(account_client_id);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAccountClient - AccountRepository: " + ex);
            }
            return null;
        }

        public async Task<AccountClient> GetByClientId(long client_id)
        {
            var rs = string.Empty;
            try
            {
                return await accountClientDAL.GetByClientId(client_id);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAccountClient - AccountRepository: " + ex);
            }
            return null;
        }
        public long updateClientChangePassword(ClientChangePasswordViewModel model)
        {
            try
            {
                var account = accountClientDAL.GetByUserName(model.Email);
                if (account != null)
                {
                    account.Password = model.PasswordNew;
                    account.PasswordBackup = model.ConfirmPasswordNew;
                    var updata = accountClientDAL.UpdataAccountClient(account);
                    if (updata > 0)
                    {
                        return 1;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }
}
