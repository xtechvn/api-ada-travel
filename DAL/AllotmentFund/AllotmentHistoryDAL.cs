using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DAL.AllotmentFund
{
    public class AllotmentHistoryDAL : GenericService<ENTITIES.Models.AllotmentHistory>
    {

        private static DbWorker _DbWorker;

        public AllotmentHistoryDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<long> AddAllotmentHistory(ENTITIES.Models.AllotmentHistory model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    _DbContext.AllotmentHistory.Add(model);
                    await _DbContext.SaveChangesAsync();
                    return model.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddAllotmentHistory - AllotmentFundDAL: " + ex.ToString());
                return -2;
            }
        }
    }
}
