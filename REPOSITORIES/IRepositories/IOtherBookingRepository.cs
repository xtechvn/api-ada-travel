using ENTITIES.Models;
using ENTITIES.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IOtherBookingRepository
    {
        public Task<OtherBooking> GetOtherBookingById(long booking_id);
        Task<List<OtherBooking>> GetOtherBookingByOrderId(long order_id);
        Task<List<OtherBookingViewModel>> GetDetailOtherBookingById(int OtherBookingId);

    }
}
