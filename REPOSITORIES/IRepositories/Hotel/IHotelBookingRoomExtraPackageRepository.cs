using ENTITIES.Models;

namespace Repositories.IRepositories
{
    public interface IHotelBookingRoomExtraPackageRepository
    {
        public int CreateHotelBookingRoomExtraPackages(HotelBookingRoomExtraPackages packages);
        public int UpdateHotelBookingExtraPackagesSP(HotelBookingRoomExtraPackages packages);
    }
}
