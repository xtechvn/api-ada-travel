using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.PricePolicy
{
    public class HotelPricePolicyActiveViewModel
    {
        public int PriceDetailId { get; set; }
        public int ProductServiceId { get; set; }
        public string ProgramName { get; set; }
        public int HotelId { get; set; }
        public int ProgramId { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public int PackageId { get; set; }
        public int RoomId { get; set; }

        public string RoomName { get; set; }
        public string AllotmentsId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double Profit { get; set; }
        public short UnitId { get; set; }
    }
}
