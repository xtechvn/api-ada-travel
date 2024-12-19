using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Hotel
{
    public class HotelPricePolicyViewModel
    {
        public string CampaignCode { get; set; }
        public string RoomCode { get; set; }
        public string HotelCode { get; set; }

        public int PriceDetailId { get; set; }
        public int ProductServiceId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramCode { get; set; }
        public int HotelId { get; set; }
        public int ProgramId { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public int PackageId { get; set; }
        public int RoomId { get; set; }
        public string RoomTypeCode { get; set; }

        public string RoomName { get; set; }
        public string AllotmentsId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double Profit { get; set; }
        public short UnitId { get; set; }
    }
}
