using ENTITIES.ViewModels.B2B;
using Entities.ViewModels;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Fly;
using Entities.ConfigModels;
using Microsoft.Extensions.Options;
using Utilities;

namespace REPOSITORIES.Repositories.Fly
{
    public class FlightWarehouseRepository : IFlightWarehouseRepository
    {
        private readonly FlightWarehouseBookingDAL flightWarehouseBookingDAL;
        public FlightWarehouseRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            var connectionString = dataBaseConfig.Value.SqlServer.ConnectionString;
            flightWarehouseBookingDAL = new FlightWarehouseBookingDAL(connectionString);
           
        }
        public async Task<List<FlightWarehouseBookingViewModel>> GetListFlightWarehouse(GetListFlightWarehouseModel searchModel)
        {
            try
            {
                return await flightWarehouseBookingDAL.GetListFlightWarehouse(searchModel);
                
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListFlightWarehouse - FlightWarehouseRepository: " + ex);
            }

            return null;
        }
        public async Task<FlightWarehouseBookingModel> GetBookingById(int id)
        {
            return await flightWarehouseBookingDAL.GetById(id);
        }
        public async Task<List<FlightWarehouseSegmentModel>> GetSegmentsByBookingId(int bookingId)
        {
            return await flightWarehouseBookingDAL.GetSegmentsByBookingId(bookingId);
        }
    }
}
