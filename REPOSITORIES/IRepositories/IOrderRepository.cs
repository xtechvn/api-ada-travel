using Entities.ViewModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.Order;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IOrderRepository
    {
        Order getDetail(long orderId);
        Task<List<List_OrderViewModel>> GetOrderByClientId(long client_id);
        Task<List<List_OrderViewModel>> getOrderByOrderIdPagingList(int PageSize, int pageNumb, long client_id);
        Task<List<OrderDetailViewModel>> getOrderDetail(long order_id, long client_id);
        //Task<List<OrderViewDonHang>> getOrderByOrderIdPagingList(int skip, int take);
        OrderViewAPIdetail getOrderOrderViewdetail(long order_id,int type);
        Task<long> CreateOrderNo(string order_no);
        Task<string> getOrderNoByOrderId(long order_id);
        Task<List<OrderDetailViewModel>> getOrderDetailBySessionId(string session_id);
        Task<bool> UpdateCheckedBookingSession(string session_id, long client_id);
        Task<bool> BackupBookingInfo(long order_id, string j_booking_info);

        Task<HotelBookingB2BPagingViewModel> GetHotelOrderB2BPaging(int PageSize, int pageNumb, long client_id, string account_client_id, DateTime start_date, DateTime end_date);
        Task<HotelB2BOrderDetailViewModel> GetHotelOrderDetailB2B(long order_id);
        Task<int> GetTotalVoucherUse(long voucher_id, long account_client_id);
        Task<Order> GetOrderByOrderNo(string order_no);
        Task<List<OrderVinWonderDetailViewModel>> getOrderVinWonderDetail(long order_id, long client_id);
        Task<List<OrderServiceViewModel>> GetAllServiceByOrderId(long OrderId);
        Task<GenericViewModel<OrderaffViewModel>> GetListOrrderbyUtmSource(OrderViewSearchModel searchModel, int currentPage, int pageSize);
        Task<List<GetListOrdersResponseModel>> GetListOrder(long clientId, DateTime fromDate, DateTime toDate, string OrderStatus = "");

    }
}
