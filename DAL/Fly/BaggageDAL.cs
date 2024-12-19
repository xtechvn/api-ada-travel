using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace DAL.Fly
{
    public class BaggageDAL : GenericService<Baggage>
    {
        private static DbWorker _DbWorker;
        public BaggageDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public List<Baggage> GetBaggages(List<int> passengerIdList)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Baggage.AsNoTracking().Where(s => passengerIdList.Contains((int)s.PassengerId)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetBaggages - BaggageDAL: " + ex);
                return null;
            }
        }
    }
}
