using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.HotelBookingRoom
{
    public class HotelBookingsRoomOptionalViewModel : HotelBookingRoomsOptional
    {
        public decimal TotalAmountPay { get; set; }
        public long SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? NumberOfAdult { get; set; }
        public int? NumberOfChild { get; set; }
        public int? NumberOfInfant { get; set; }
        public string RoomTypeId { get; set; }
        public string RoomTypeCode { get; set; }
        public string RoomTypeName { get; set; }
        public double SaleTotalAmount { get; set; }
        public double UnitPrice { get; set; }
        public double SaleNumberOfRoom { get; set; }
        public double Amount { get; set; }
    }
    public class HotelBookingRoomRatesOptionalViewModel : HotelBookingRoomRatesOptional
    {
        public string RatePlanCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? SalePrice { get; set; }
        public int? Nights { get; set; }
        public long HotelBookingRoomId { get; set; }
        public long SupplierId { get; set; }
        public string RatePlanId { get; set; }
        public DateTime StayDate { get; set; }
        public double? SaleTotalAmount { get; set; }
    }
}
