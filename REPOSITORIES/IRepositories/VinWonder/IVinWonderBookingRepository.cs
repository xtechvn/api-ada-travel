using ENTITIES.Models;
using ENTITIES.ViewModels.MongoDb;
using ENTITIES.ViewModels.Order;
using ENTITIES.ViewModels.VinWonder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories.VinWonder
{
    public interface IVinWonderBookingRepository
    {

        Task<List<ListVinWonderViewModel>> GetVinWonderByAccountClientId(long AccountClientId, long PageIndex, long PageSize, string keyword);

        string saveBooking(BookingVinWonderMongoDbModel data);
        public List<BookingVinWonderMongoDbModel> GetBookingById(string[] id);
        Task<List<PriceVinWonderViewModels>> getVinWonderPricePolicyByServiceId(string rate_code, string service_id);
        Task<List<OrderVinWonderDetailViewModel>> GetVinWonderByBookingId(string bookingId);
        Task<List<VinWonderBooking>> GetVinWonderBookingByOrderId(long order_id);
        Task<List<ListVinWonderemialViewModel>> GetVinWonderBookingEmailByOrderID(long orderid);
        Task<List<VinWonderBooking>> GetVinWonderBookingByOrderID(long orderid);
        Task<List<VinWonderBookingTicket>> GetVinWonderBookingTicketByBookingID(long BookingId);
        Task<List<VinWonderBookingTicketCustomer>> GetVinWondeCustomerByBookingId(long BookingId);
    }
}
