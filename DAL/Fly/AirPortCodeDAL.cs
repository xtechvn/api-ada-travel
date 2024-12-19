using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace DAL.Fly
{
    public class AirPortCodeDAL : GenericService<AirPortCode>
    {
        private static DbWorker _DbWorker;
        public AirPortCodeDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public List<AirPortCode> GetAirPortCodes()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.AirPortCode.AsNoTracking().ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetBaggages - BaggageDAL: " + ex);
                return new List<AirPortCode>();
            }
        }
    }
}
