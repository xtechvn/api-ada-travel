using ENTITIES.Models;
using ENTITIES.ViewModels.Order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories.Fly
{
    public interface IFlyBookingDetailRepository
    {
        FlyBookingDetail GetByOrderId(long orderId);
        List<FlyBookingDetail> GetListByOrderId(long orderId);
        Task<List<FlyBookingDetail>> GetFlyBookingById(long fly_booking_id);
        Task<FlyBookingdetail> GetDetailFlyBookingDetailById(int FlyBookingId);
    }
}
