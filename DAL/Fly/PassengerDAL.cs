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
    public class PassengerDAL : GenericService<Passenger>
    {
        private static DbWorker _DbWorker;
        public PassengerDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public List<Passenger> GetPassengers(long orderId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Passenger.AsNoTracking().Where(s => s.OrderId == orderId).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPassengers - PassengerDAL: " + ex);
                return null;
            }
        }
    }
}
