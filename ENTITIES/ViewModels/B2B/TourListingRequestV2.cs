using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.B2B
{
    public class TourListingRequestV2
    {
        public string pageindex { get; set; }
        public string pagesize { get; set; }
        public string tourtype { get; set; }
        public string startpoint { get; set; }
        public string endpoint { get; set; }
        public string tourname { get; set; }
        public string month { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string noShopping { get; set; }
        public string isHoliday { get; set; }
        public string holdSlot { get; set; }
    }
}
