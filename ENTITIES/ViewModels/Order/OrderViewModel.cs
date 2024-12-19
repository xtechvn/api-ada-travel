using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Order
{
    public class OrderViewModel
    {
        public string StartPoint { get; set; }
        public string StartDistrict { get; set; }
        public string Startime { get; set; }
        public string EndPoint { get; set; }
        public string EndDistrict { get; set; }
        public string Endtime { get; set; }
        public string FlightNumber { get; set; }//mã chuyến bay   
        public double? Amount { get; set; }//tong tien
        public int? BookingId { get; set; }
        public int Duration { get; set; }
        public string AirlineLogo { get; set; }
        public string AirlineName_Vi { get; set; }
        public string voucher_code { get; set; }
        public int? Leg { get; set; }
        public int VoucherId { get; set; }
        public int PercentDecrease { get; set; }
        public bool? HasStop { get; set; }

    }
    public class List_OrderViewModel
    {
        public long? OrderId { get; set; }
        public long? ClientId { get; set; }
        public string OrderNo { get; set; } //mã đơn hang
        public string order_status_name { get; set; }//trang thai thanh toán
        public string color_code { get; set; }
        public byte? OrderStatus { get; set; }
        public double? OrderAmount { get; set; }
        public string UserName { get; set; }
        public string clientName { get; set; }//ContactClient
        public string phone { get; set; }//ContactClient
        public string email { get; set; }
        public double? Profit { get; set; }
        public string CreateTime { get; set; }
        public string PaymentDate { get; set; }//ngày thanh toán
        public DateTime? ExpiryDate { get; set; }
        public int service_type { get; set; }
        public int PercentDecrease { get; set; }
        public int Discount { get; set; }
   
        public double? Price { get; set; }
        public string product_service { get; set; }
        public string voucher_code { get; set; }
        public int VoucherId { get; set; }

        public List<OrderViewModel> list_Order { get; set; }
    }
    public class OrderViewCaching
    {
        public long client_id { get; set; }
        public int source_type { get; set; }
    }
    public class OrderViewDonHang
    {
        public long? OrderId { get; set; }
        public long? ClientId { get; set; }
        public string OrderNo { get; set; }
        public string UserName { get; set; }
        public string clientName { get; set; }//ContactClient
        public string phone { get; set; }//ContactClient
        public string email { get; set; }
        public string order_status_name { get; set; }
        public byte? OrderStatus { get; set; }
        public double? amount { get; set; }
        public double? Profit { get; set; }
        public string CreateTime { get; set; }
        public string PaymentDate { get; set; }//ngày thanh toán
        public List<listbooking> listbookings { get; set; }
    }
    public class listbooking
    {
        public long FlyBookingId { get; set; }
        public string FlightNumber { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public int? Leg { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
    public class OrderaffViewModel
    {
        public string OrderId { get; set; }
        public string OrderCode { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ClientName { get; set; }
        public long? ClientId { get; set; }
        public string ClientNumber { get; set; }
        public string ClientEmail { get; set; }
        public string Note { get; set; }
        public double Payment { get; set; }
        public double Amount { get; set; }
        public string UtmSource { get; set; }
        public double Profit { get; set; }
        //public List<Source> StatusDetail { get; set; } = new List<Source>();
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public int PayDetailId { get; set; }
        public string CreateDate { get; set; }
        public string CreateName { get; set; }
        public string UpdateName { get; set; }
        public string UpdateDate { get; set; }
        public string SalerName { get; set; }
        public string SalerUserName { get; set; }
        public string SalerEmail { get; set; }
        public string SaleGroupName { get; set; }
        public string PaymentStatus { get; set; }
        public double TotalDisarmed { get; set; }
        public double TotalAmount { get; set; }
        public double TotalNeedPayment { get; set; }
        public string UsUpdateName { get; set; }
        public string CreatedName { get; set; }
        public string ServiceType { get; set; }
        public string Vouchercode { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDisabled { get; set; }
        public string PaymentStatusName { get; set; }
        public string PermisionTypeName { get; set; }
        public string OperatorIdName { get; set; }



    }
}
