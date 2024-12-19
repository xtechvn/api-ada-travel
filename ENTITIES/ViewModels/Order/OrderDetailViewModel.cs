using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Order
{
   public class OrderDetailViewModel: OrderViewModel
    {

        public long? OrderId { get; set; }
        public long? ClientId { get; set; }
        public long? AccountClientId { get; set; }

        public string OrderNo { get; set; } //mã đơn hang
        public int? PaymentStatus { get; set; }//trang thai thanh toán
        public string PaymentStatusName { get; set; }//trang thai thanh toán
        public byte? OrderStatus { get; set; }
        public string BookingCode { get; set; }//order
        public DateTime? CreateTime { get; set; }//order
        public int? PaymentType { get; set; }//order
        public string PaymentTypeName { get; set; }//order
        public string Name { get; set; }//ContactClient
        public string Phone { get; set; }//ContactClient
        public string Email { get; set; }//ContactClient
        public string order_status_name { get; set; }//trang thai thanh toán
        public string color_code { get; set; }
        public string Sessionid { get; set; }
        public string voucher_code { get; set; }
        public long? FlyBookingID { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int is_lock { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public string vinWonderBookingId { get; set; }

        public List<Passenger>  Passenger { get; set; }//
        public vinWonderdetail vinWonderdetail { get; set; }
    }
    public class OrderViewAPIdetail
    {

        public long? OrderId { get; set; }
        public long? ClientId { get; set; }
        public long? AccountClientId { get; set; }

        public string OrderNo { get; set; }
        public string UserName { get; set; }
        public string ClientName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public double? Amount { get; set; }//tông tiền
        public double OrderAmount { get; set; }//tông tiền
        public string order_status_name { get; set; }
        public byte? OrderStatus { get; set; }

        public string? CreateTime { get; set; }
        public string? ExpriryDate { get; set; }
        public int? PaymentStatus { get; set; }//đã thanh toán hay chưa 0-null / 1 rồi
        public string Payment_Status_name { get; set; }//đã thanh toán hay chưa 0-null / 1 rồi
        public double? Profit { get; set; }
        public string PaymentNo { get; set; }//mã thanh toán
        public string PaymentDate { get; set; }
        public int PaymentAmount { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public string voucher_code { get; set; }
        public int VoucherId { get; set; }
        public long? UserUpdateId { get; set; }
        public long? UserCreateId { get; set; }
        public string? UpdateLast { get; set; }
        public List<ListPayment> ListPayment { get; set; }
        public List<ListPassenger> Passenger { get; set; }
        public List<Bookingdetail> bookingdetail { get; set; }
        public List<vinWonderdetail> vinWonderdetail { get; set; }
    }
    public class OrderViewdetail
    {

        public long? OrderId { get; set; }
        public long? ClientId { get; set; }
        public long? AccountClientId { get; set; }

        public string OrderNo { get; set; }
        public string UserName { get; set; }
        public string ClientName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public double? Amount { get; set; }//tông tiền
        public double OrderAmount { get; set; }//tông tiền
        public string order_status_name { get; set; }
        public byte? OrderStatus { get; set; }
       
        public string? CreateTime { get; set; }
        public string? ExpriryDate { get; set; }
        public int? PaymentStatus { get; set; }//đã thanh toán hay chưa 0-null / 1 rồi
        public string Payment_Status_name { get; set; }//đã thanh toán hay chưa 0-null / 1 rồi
        public double? Profit { get; set; }
        public string PaymentNo { get; set; }//mã thanh toán
        public string PaymentDate { get; set; }
        public int PaymentAmount { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public string voucher_code { get; set; }
        public int VoucherId { get; set; }
        public long? UserUpdateId { get; set; }
        public long? UserCreateId { get; set; }
        public string? UpdateLast { get; set; }
        public List<ListPayment> ListPayment { get; set; }
        public List<ListPassenger> Passenger { get; set; }
        public List<FlyBookingdetail> bookingdetail { get; set; }
        public List<vinWonderdetail> vinWonderdetail { get; set; }
    }
    public class Bookingdetail
    {
        public long? OrderId { get; set; }
        public string FlightNumber { get; set; }//mã chuyến bay   
        public string StartPoint { get; set; }
        public string StartDistrict { get; set; }
        public string EndPoint { get; set; }
        public string EndDistrict { get; set; }
        public string BookingCode { get; set; }
        public double Amount { get; set; }
        public double Discount { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int? Leg { get; set; }
        public string AirlineLogo { get; set; }
        public string AirlineName_Vi { get; set; }
    }
    public class FlyBookingdetail
    {
        public long Id { get; set; }
        public long? OrderId { get; set; }
        public string FlightNumber { get; set; }//mã chuyến bay   
        public string StartPoint { get; set; }
        public string StartDistrict { get; set; }
        public string StartDistrict2 { get; set; }
        public string EndPoint { get; set; }
        public string EndDistrict { get; set; }
        public string EndDistrict2 { get; set; }
        public string BookingCode { get; set; }
        public string BookingCode2 { get; set; }
        public double Amount { get; set; }
        public double Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? Leg { get; set; }
        public int? Leg2 { get; set; }
        public string AirlineLogo { get; set; }
        public string AirlineName_Vi { get; set; }
        public string AirlineName_Vi2 { get; set; }
        public string GroupBookingId { get; set; }
        public string SalerIdName { get; set; }
        public string ServiceCode { get; set; }
        public int AdultNumber { get; set; }
        public int ChildNumber { get; set; }
        public int InfantNumber { get; set; }
        public int SalerId { get; set; }
    }
    public class ListPayment :  ENTITIES.Models.Payment
    {
        public string Payment_Type_Name { get; set; }//tên kiểu thanh toán
        public string PaymentDate { get; set; }//tên kiểu thanh toán
    }
    public class ListPassenger : ENTITIES.Models.Passenger
    {
        public string Person_Type_Name { get; set; }
        public string Gender_Name { get; set; }
    }
    public class vinWonderdetail
    {
        public long? OrderId { get; set; }
        public long? BookingId { get; set; }
   
        public string SiteName { get; set; }
        public string SiteCode { get; set; }
        public List<VinWonderBookingTicketViewModel> VinWonderBookingTicket { get; set; }
}
    public class VinWonderBookingTicketViewModel
    {
        public string DateUsed { get; set; }
        public int adt { get; set; }
        public int child { get; set; }
        public int old { get; set; }
        public long BookingTicketId { get; set; }

        public string Name { get; set; }
      
    }
    public class OrderVinWonderDetailViewModel
    {

        public long? OrderId { get; set; }
        public long? ClientId { get; set; }
        public long? AccountClientId { get; set; }
        public double Amount { get; set; }
        public int VoucherId { get; set; }

        public string OrderNo { get; set; } //mã đơn hang
        public int? PaymentStatus { get; set; }//trang thai thanh toán
        public string PaymentStatusName { get; set; }//trang thai thanh toán
        public byte? OrderStatus { get; set; }
        public string BookingCode { get; set; }//order
        public DateTime? CreateTime { get; set; }//order
        public int? PaymentType { get; set; }//order
        public string PaymentTypeName { get; set; }//order
        public string Name { get; set; }//ContactClient
        public string Phone { get; set; }//ContactClient
        public string Email { get; set; }//ContactClient
        public string order_status_name { get; set; }//trang thai thanh toán
        public string voucher_code { get; set; }
        public int is_lock { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public string BookingId { get; set; }

        public List<Passenger> Passenger { get; set; }//
        public vinWonderdetail vinWonderdetail { get; set; }
    }
}
