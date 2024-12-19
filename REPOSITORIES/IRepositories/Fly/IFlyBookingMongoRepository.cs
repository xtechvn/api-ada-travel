using ENTITIES.ViewModels.Booking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories.Fly
{
    public interface IFlyBookingMongoRepository
    {
        string saveBooking(BookingFlyMongoDbModel data);
        Task<string> DeleteBookingBySessionId(BookingFlyMongoDbModel data);
        //-- int client_id ->int account_client_id
        Task<List<BookingFlyMongoDbModel>> getBookingByBookingIdAsync(List<int> booking_id, int account_client_id);
        //-- int client_id ->int account_client_id
        Task<List<BookingFlyMongoDbModel>> getBookingBySessionId(string[] list_session_id, int account_client_id);
    }
}
