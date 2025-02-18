using ENTITIES.Models;
using ENTITIES.ViewModels.HotelBookingRoom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IHotelBookingRoomRepository
    {
        Task<List<HotelBookingRooms>> GetByHotelBookingID(long hotel_booking_id);
        Task<List<HotelBookingRoomViewModel>> GetHotelBookingRoomByHotelBookingID(long HotelBookingId, long status);
        
        Task<List<HotelBookingRoomRatesOptionalViewModel>> GetHotelBookingRoomRatesOptionalByBookingId(long HotelBookingId);

    }
}
