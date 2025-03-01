using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.APPModels.PushHotel;
using ENTITIES.Models;
using ENTITIES.ViewModels.Hotel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL.Hotel
{
    public class HotelPositionDAL : GenericService<ENTITIES.Models.HotelPosition>
    {
        private static DbWorker _DbWorker;
        public HotelPositionDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<List<HotelPosition>> GetListHotelActivePosition()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.HotelPosition.AsNoTracking().Where(x => x.Status==1).ToListAsync();

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByPositionType - HotelPositionDAL. " + ex);
                return null;
            }
        }
       
    }
}
