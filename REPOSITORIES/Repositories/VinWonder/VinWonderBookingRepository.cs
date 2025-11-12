using DAL.MongoDB.VinWonder;
using DAL.VinWonder;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.MongoDb;
using ENTITIES.ViewModels.Order;
using ENTITIES.ViewModels.VinWonder;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.VinWonder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utilities;
using Entities.ViewModels.Ticket;
using System.Linq;  // <-- REQUIRED for LINQ's .ToList()



namespace REPOSITORIES.Repositories.VinWonder
{
    public class VinWonderBookingRepository : IVinWonderBookingRepository
    {
        private readonly VinWonderMongoBookingDAL vinWonderMongoBookingDAL;
        private readonly VinWonderBookingDAL vinWonderBookingDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public VinWonderBookingRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            vinWonderBookingDAL = new VinWonderBookingDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            vinWonderMongoBookingDAL = new VinWonderMongoBookingDAL(_dataBaseConfig.Value.MongoServer.connection_string, _dataBaseConfig.Value.MongoServer.catalog_core);
            dataBaseConfig = _dataBaseConfig;
        }
        public async Task<List<ListVinWonderViewModel>> GetVinWonderByAccountClientId(long AccountClientId, long PageIndex, long PageSize, string keyword)
        {
            try
            {

                return await vinWonderBookingDAL.GetVinWonderByAccountClientId(AccountClientId, PageIndex, PageSize, keyword);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderByAccountClientId - VinWonderBookingRepository: " + ex.ToString());
            }
            return null;
        }
        public string saveBooking(BookingVinWonderMongoDbModel data)
        {
            try
            {
                data.GenID();
                var result = vinWonderMongoBookingDAL.InsertBooking(data);
                if (result != null)
                    return data._id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("saveBooking - VinWonderBookingRepository: " + ex);
                return null;
            }
            return null;
        }
        public List<BookingVinWonderMongoDbModel> GetBookingById(string[] id)
        {
            try
            {
                List<BookingVinWonderMongoDbModel> booking = new List<BookingVinWonderMongoDbModel>();
                if (id.Length > 0)
                {
                    foreach(var mongo_id in id)
                    {
                        var result = vinWonderMongoBookingDAL.GetBookingById(mongo_id);
                        if (result != null) booking.Add(result);
                    }
                }
                return booking;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetBookingById - VinWonderBookingRepository: " + ex);
                return null;
            }
        }
       
        public async Task<List<PriceVinWonderViewModels>> getVinWonderPricePolicyByServiceId(string rate_code, string service_id)
        {
            try
            {
                var result = await vinWonderBookingDAL.getVinWonderPricePolicyByServiceId(rate_code, service_id);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getVinWonderPricePolicyByServiceId - VinWonderBookingRepository: " + ex);
                return null;
            }
        }
        public async Task<List<OrderVinWonderDetailViewModel>> GetVinWonderByBookingId(string bookingId)
        {
            try
            {
                var result = await vinWonderBookingDAL.GetVinWonderByBookingId(bookingId);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderByBookingId - VinWonderBookingRepository: " + ex);
                return null;
            }
        }
        public async Task<List<VinWonderBooking>>  GetVinWonderBookingByOrderId(long order_id)
        {
            try
            {
                var result = await vinWonderBookingDAL.GetVinWonderBookingByOrderId(order_id);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingByOrderId - VinWonderBookingRepository: " + ex);
                return null;
            }
        }
        public async Task<List<ListVinWonderemialViewModel>> GetVinWonderBookingEmailByOrderID(long orderid)
        {
            try
            {
                var result = await vinWonderBookingDAL.GetVinWonderBookingEmailByOrderID(orderid);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingEmailByOrderID - VinWonderBookingRepository: " + ex);
                return null;
            }
        }
        public async Task<List<VinWonderBooking>> GetVinWonderBookingByOrderID(long orderid)
        {
            try
            {
                var result = await vinWonderBookingDAL.GetVinWonderBookingByOrderID(orderid);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingByOrderID - VinWonderBookingRepository: " + ex);
                return null;
            }
        }
        public async Task<List<VinWonderBookingTicket>> GetVinWonderBookingTicketByBookingID(long BookingId)
        {
            try
            {
                var result = await vinWonderBookingDAL.GetVinWonderBookingTicketByBookingID(BookingId);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingTicketByBookingID - VinWonderBookingRepository: " + ex);
                return null;
            }
        }  
        public async Task<List<VinWonderBookingTicketCustomer>> GetVinWondeCustomerByBookingId(long BookingId)
        {
            try
            {
                var result = await vinWonderBookingDAL.GetVinWondeCustomerByBookingId(BookingId);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingTicketByBookingID - VinWonderBookingRepository: " + ex);
                return null;
            }
        }

        public List<TicketSearchOffersViewModel> SearchOffers(
    int supplierId, DateTime visitDate,
    int adults, int children, int seniors,
    string search, int? categoryId, int? ticketTypeId, int? playZoneId, int? productId)
        {
            try
            {
                var dt = vinWonderBookingDAL.SearchOffers(
                    supplierId, visitDate, adults, children, seniors,
                    search, categoryId, ticketTypeId, playZoneId, productId);

                if (dt == null || dt.Rows.Count == 0)
                    return new List<TicketSearchOffersViewModel>();

                var list = dt.AsEnumerable().Select(r => new TicketSearchOffersViewModel
                {
                    TicketId = r.Field<int?>("TicketId") ?? 0,
                    ProductId = r.Field<int?>("ProductId") ?? 0,
                    ProductName = r.Field<string>("ProductName"),

                    // unit selling price
                    PriceAdult = r.Field<decimal?>("PriceAdult") ?? 0,
                    PriceChild = r.Field<decimal?>("PriceChild") ?? 0,
                    PriceSenior = r.Field<decimal?>("PriceSenior") ?? 0,

                    // unit base
                    BaseAdult = r.Field<decimal?>("BaseAdult"),
                    BaseChild = r.Field<decimal?>("BaseChild"),
                    BaseSenior = r.Field<decimal?>("BaseSenior"),

                    // unit profit
                    ProfitAdult = r.Field<decimal?>("ProfitAdult"),
                    ProfitChild = r.Field<decimal?>("ProfitChild"),
                    ProfitSenior = r.Field<decimal?>("ProfitSenior"),

                    // totals
                    TotalAmount = r.Field<decimal?>("TotalAmount"),
                    BaseTotalAmount = r.Field<decimal?>("BaseTotalAmount"),
                    ProfitTotalAmount = r.Field<decimal?>("ProfitTotalAmount"),

                    VisitDate = r.Field<DateTime?>("VisitDate") ?? visitDate
                }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("TicketRepository.SearchOffers: " + ex);
                return new List<TicketSearchOffersViewModel>();
            }
        }



    }
}

