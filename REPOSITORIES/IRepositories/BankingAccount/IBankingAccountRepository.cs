using System;
using System.Collections.Generic;
using System.Text;

namespace REPOSITORIES.IRepositories.BankingAccount
{
  public interface  IBankingAccountRepository
    {
        ENTITIES.Models.BankingAccount GetById(int bankAccountId);
        int InsertBankingAccount(ENTITIES.Models.BankingAccount model);
        int UpdateBankingAccount(ENTITIES.Models.BankingAccount model);
    }
}
