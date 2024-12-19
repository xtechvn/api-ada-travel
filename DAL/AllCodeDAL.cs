using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class AllCodeDAL : GenericService<AllCode>
    {
        private static DbWorker _DbWorker;
        public AllCodeDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<List<AllCode>> GetAllTelegramBot()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.AllCode.AsNoTracking().Where(s => s.Type== AllCodeType.TELEGRAM_BOT).ToListAsync();

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FindByLabelId - LabelDAL: " + ex);
                return null;
            }
        }
        public async Task<List<AllCode>> GetTypeOfRoom()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.TYPE_OF_ROOM).ToListAsync();

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FindByLabelId - LabelDAL: " + ex);
                return null;
            }
        }
        public async Task<List<AllCode>> GetAllCodeByType(string type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.AllCode.AsNoTracking().Where(s => s.Type == type).ToListAsync();

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FindByLabelId - AllCodeDAL: " + ex);
                return null;
            }
        }


        public async Task<List<Province>> GetProvinceList()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Province.AsNoTracking().ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProvinceList - CommonDAL: " + ex);
                return null;
            }
        }
        public async Task<List<District>> GetDistrictListByProvinceId(string provinceId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.District.Where(s => s.ProvinceId == provinceId).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDistrictListByProvinceId - AllCodeDAL: " + ex);
                return null;
            }
        }

        public async Task<List<Ward>> GetWardListByDistrictId(string districtId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Ward.Where(s => s.DistrictId == districtId).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetWardListByDistrictId - AllCodeDAL: " + ex);
                return null;
            }
        }
    }
}
