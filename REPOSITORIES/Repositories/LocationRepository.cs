using DAL;
using DAL.DepositHistory;
using DAL.Fly;
using DAL.Hotel;
using DAL.Locations;
using DAL.MongoDB.Flight;
using DAL.Orders;
using Entities.ConfigModels;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories
{
    public class LocationRepository: ILocationRepository
    {
        private readonly LocationDAL _locationDAL;


        public LocationRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {

            _locationDAL = new LocationDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            

        }

        public async Task<List<ENTITIES.Models.National>> GetNationalByListID(string ids)
        {
            try
            {

                DataTable data = await _locationDAL.GetNationalByListID(ids);
                var listData = data.ToList<ENTITIES.Models.National>();
                if (listData.Count > 0)
                {
                    return listData;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetNationalByListID - LocationRepository. " + ex);
            }
            return null;
        }
        public async Task<List<ENTITIES.Models.Province>> GetProvinceByListID(string ids)
        {
            try
            {

                DataTable data = await _locationDAL.GetProvinceByListID(ids);
                var listData = data.ToList<ENTITIES.Models.Province>();
                if (listData.Count > 0)
                {
                    return listData;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProvinceByListID - LocationRepository. " + ex);
            }
            return null;
        }
    }
}
