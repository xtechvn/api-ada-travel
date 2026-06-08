using ENTITIES.ViewModels.B2B;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Fly;

namespace REPOSITORIES.IRepositories
{
    public interface IFlightWarehouseRepository
    {
        Task<List<FlightWarehouseBookingViewModel>> GetListFlightWarehouse(GetListFlightWarehouseModel searchModel);
        Task<FlightWarehouseBookingModel> GetBookingById(int id);

        Task<List<FlightWarehouseSegmentModel>> GetSegmentsByBookingId(int bookingId);
        
    }
}
