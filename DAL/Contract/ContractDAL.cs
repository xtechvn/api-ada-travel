using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace DAL.Contracts
{
    public class ContractDAL : GenericService<Contract>
    {
        private static DbWorker _DbWorker;

        public ContractDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }


        public async Task<long> CountContractInYear()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Contract.AsNoTracking().Where(x => x.CreateDate.Year == DateTime.Now.Year).Count();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountContractInYear - OrderDAL: " + ex.ToString());
                return -1;
            }
        }

    }
}
