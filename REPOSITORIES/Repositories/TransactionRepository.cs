using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.Transaction;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TransactionsDAL transactionsDAL;
        public TransactionRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            transactionsDAL = new TransactionsDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<List<TransactionsView>> GetAllTransactions(int skip, int take)
        {
            return await transactionsDAL.GetAllTransactions(skip, take);
        }
        public long Insert(Transactions transactions)
        {
            return transactionsDAL.Insert(transactions);
        }
    }
}
