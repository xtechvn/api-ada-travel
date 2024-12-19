using DAL.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using ENTITIES.Models;
using DAL.StoreProcedure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using ENTITIES.ViewModels;
using Utilities;
using ENTITIES.ViewModels.Transaction;

namespace DAL
{
    public class TransactionsDAL : GenericService<Transactions>
    {
        public TransactionsDAL(string connection) : base(connection)
        {
        }
        public async Task<List<TransactionsView>> GetAllTransactions(int skip, int take)
        {

            using (var _DbContext = new EntityDataContext(_connection))
            {
                var datalist = _DbContext.Transactions.AsQueryable();
                var datalist2 = _DbContext.Client.AsQueryable();
                var list = (from a in datalist
                            join b in datalist2
                            on a.ClientId equals b.Id
                            select new TransactionsView
                            {
                                Id = a.Id,
                                CreateDate = a.CreateDate,
                                TransactionNo = a.TransactionNo,
                                ClientName = b.ClientName,
                                Status = a.Status,
                                PaymentType = a.PaymentType,
                                ContractNo = a.ContractNo
                            });
                list = list.OrderByDescending(s => s.CreateDate).Skip(skip).Take(take);

                return list.ToList();
            }
        }

        public long Insert(Transactions entity)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    entity.CreateDate = DateTime.Now;
                    var result = _DbContext.Transactions.Add(entity);
                    _DbContext.SaveChanges();
                    return entity.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Insert - TransactionsDAL: " + ex);
                return -1;
            }
        }
    } 
}
