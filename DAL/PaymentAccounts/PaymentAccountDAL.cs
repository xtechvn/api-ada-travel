using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using System;
using System.Linq;
using Utilities;

namespace DAL.PaymentAccounts
{
    public class PaymentAccountDAL : GenericService<PaymentAccount>
    {
        private static DbWorker _DbWorker;
        public PaymentAccountDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public PaymentAccount GetByClientMapId(long clientId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.PaymentAccount.FirstOrDefault(n => n.ClientId == clientId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByClientMapId - PaymentAccountDAL: " + ex);
                return null;
            }
        }
    }
}
