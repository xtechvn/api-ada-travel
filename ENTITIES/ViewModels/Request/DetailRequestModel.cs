using ENTITIES.Models;
using ENTITIES.ViewModels.Hotel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Request
{
   public class DetailRequestModel
    {
        public int RequestId { get; set; }
        public string RequestNo { get; set; }
        public string HotelName { get; set; }
        public string HotelId { get; set; }
        public string arrivalDate { get; set; }
        public string departureDate { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public List<HotelBookingRooms> Rooms { get; set; }
        public List<HotelBookingRoomRates> Rates { get; set; }
        public List<HotelBookingRoomExtraPackages> ExtraPackages { get; set; }
        public HotelOrderDataVoucher voucher { get; set; }

    }
    public class RequestDetailModel : ENTITIES.Models.Request
    {
        public string SalerName { get; set; }
        public string StatusName { get; set; }
        public string OrderNo { get; set; }
        public string HotelName { get; set; }
        public string Email { get; set; }
    }
}
