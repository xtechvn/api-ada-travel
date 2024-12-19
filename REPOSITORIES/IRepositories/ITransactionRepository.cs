using ENTITIES.Models;
using ENTITIES.ViewModels.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface ITransactionRepository
    {
        public Task<List<TransactionsView>> GetAllTransactions(int skip, int take);
        long Insert(Transactions transactions);
    }
}
