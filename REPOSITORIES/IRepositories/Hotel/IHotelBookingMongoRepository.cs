using ENTITIES.ViewModels.Booking;
using ENTITIES.ViewModels.Hotel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories.Hotel
{
    public interface IHotelBookingMongoRepository
    {
        Task<string> saveBooking(BookingHotelMongoViewModel data, string booking_id);
        Task<List<BookingHotelMongoViewModel>> getBookingByID(string[] booking_id);
    }
}
