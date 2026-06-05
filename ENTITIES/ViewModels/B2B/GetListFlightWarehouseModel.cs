using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.B2B
{
    public class GetListFlightWarehouseModel
    {
        public string BookingCode { get; set; }
        public string DeparturePoint { get; set; }
        public string ArrivalPoint { get; set; }
        public string Airline { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Date { get; set; }
        public int FundType { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}
