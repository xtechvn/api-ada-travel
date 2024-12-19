using Entities.ViewModels;
using ENTITIES.Models;
using ENTITIES.ViewModels;
using ENTITIES.ViewModels.HotelBooking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IHotelBookingRepositories
    {
        Task<HotelBooking> GetHotelBookingByID(long id);
        Task<List<HotelBookingDetailViewModel>> GetHotelBookingById(long HotelBookingId);
        Task<ServiceDeclinesViewModel> GetServiceDeclinesByServiceId(string ServiceId, int type);
        Task<List<HotelBookingViewModel>> GetDetailHotelBookingByID(long HotelBookingId);
        List<HotelBooking> GetListByOrderId(long OrderId);
        Task<List<HotelBookingRooms>> GetHotelBookingRoomsByID(long id);
        int CreateHotelBooking(HotelBooking booking);
        int CreateHotelBookingRooms(HotelBookingRooms booking);
        int CreateHotelBookingRoomRates(HotelBookingRoomRates booking);
        Task<List<HotelBookingRoomExtraPackages>> GetListHotelBookingRoomsExtraPackageByBookingId(long HotelBookingId);
        Task<List<HotelBookingRooms>> GetHotelBookingRoomsByHotelBookingID(long HotelBookingId);
        Task<List<HotelBookingRoomRates>> GetHotelBookingRoomRatesByBookingRoomsRateByHotelBookingID(long HotelBookingId);
    }
}
