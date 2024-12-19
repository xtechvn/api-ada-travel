using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.APPModels.ReadBankMessages;
using ENTITIES.Models;
using ENTITIES.ViewModels.APP.ReadBankMessages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class PaymentDAL : GenericService<Payment>
    {
        private static DbWorker _DbWorker;

        public PaymentDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
      
        public async Task<long> CreatePayment(Payment payment)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists_payment = await _DbContext.Payment.AsNoTracking().FirstOrDefaultAsync(s => s.Id == payment.Id);
                    if (exists_payment!=null && exists_payment.Id >= 0)
                    {
                        return -1;

                    }
                    else
                    {
                        _DbContext.Payment.Add(payment);
                        await _DbContext.SaveChangesAsync();
                        return payment.Id;
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreatePayment - PaymentDAL: " + ex);
                return -2;
            }
        }
    }
}
