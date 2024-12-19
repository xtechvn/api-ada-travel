using DAL;
using Entities.ConfigModels;
using Microsoft.Extensions.Options;
using System;
using ENTITIES.Model;
using System.Text;
using Utilities;
using REPOSITORIES.IRepositories.BankingAccount;

namespace REPOSITORIES.Repositories.BankingAccount
{
    public class BankingAccountRepository : IBankingAccountRepository
    {
        private readonly BankingAccountDAL bankingAccountDAL;
        public BankingAccountRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            bankingAccountDAL = new BankingAccountDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public ENTITIES.Models.BankingAccount GetById(int bankAccountId)
        {
            try
            {
                var data = bankingAccountDAL.GetById(bankAccountId);
                return data;
            }
            catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("GetById - BankingAccountRepository: " + ex.ToString());
                return null;
            }
           
        }
        public int InsertBankingAccount(ENTITIES.Models.BankingAccount model)
        {
            try
            {
                var Insert = bankingAccountDAL.InsertBankingAccount(model);
                return Insert;
            }
            catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertBankingAccount - BankingAccountRepository: " + ex.ToString());
                return 0;
            }
        }
        public int UpdateBankingAccount(ENTITIES.Models.BankingAccount model)
        {
            try
            {
                var Update = bankingAccountDAL.UpdateBankingAccount(model);
                return Update;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateBankingAccount - BankingAccountRepository: " + ex.ToString());
                return 0;
            }
        }
    }
}
