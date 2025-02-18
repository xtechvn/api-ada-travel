using ENTITIES.Models;
using ENTITIES.ViewModels.HotelBookingRoom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IHotelBookingRoomExtraPackageRepository
    {
        public int CreateHotelBookingRoomExtraPackages(HotelBookingRoomExtraPackages packages);
        public int UpdateHotelBookingExtraPackagesSP(HotelBookingRoomExtraPackages packages);
        Task<List<HotelBookingRoomExtraPackagesViewModel>> Gethotelbookingroomextrapackagebyhotelbookingid(long HotelBookingId);
        Task<List<HotelBookingRoomExtraPackages>> GetByBookingID(long hotel_booking_id);
    }
}
