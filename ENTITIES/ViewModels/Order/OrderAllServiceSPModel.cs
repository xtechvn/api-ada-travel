using ENTITIES.ViewModels.HotelBooking;
using ENTITIES.ViewModels.Tour;
using ENTITIES.ViewModels.VinWonder;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Order
{
    public class OrderAllServiceSPModel
    {
        public long OrderId { get; set; }
        public long ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string OrderNo { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public string Type { get; set; }
        public string FullName { get; set; }
        public double Profit { get; set; }
        public double OrderAmount { get; set; }
        public double Discount { get; set; }
        public string code { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
    }
    public class OrderServiceViewModel
    {
        public string OrderId { get; set; }
        public string OrderNo { get; set; }
        public string ServiceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateTime { get; set; }
        public string Id { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public double Profit { get; set; }
        public double Discount { get; set; }
        public double OrderAmount { get; set; }
        public string Type { get; set; }
        public string FullName { get; set; }
        public string code { get; set; }
        public string Note { get; set; }
        public string StatusName { get; set; }
        public string ServiceCode { get; set; }
        public int Status { get; set; }
        public TourViewModel tour { get; set; }
        public List<HotelBookingViewModel> Hotel { get; set; }
        public FlyBookingdetail Flight { get; set; }
        public List<OtherBookingViewModel> OtherBooking { get; set; }
        public List<VinWonderDetailViewModel> VinWonderBooking { get; set; }

    }
}
