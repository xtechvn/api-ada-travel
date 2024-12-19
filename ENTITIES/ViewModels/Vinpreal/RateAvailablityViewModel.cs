using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Vinpreal
{
    class RateAvailablityViewModel
    {
        public DateTime arrivalDate { get; set; }
        public DateTime departureDate { get; set; }
        public string numberOfRoom { get; set; }
        public string hotelID { get; set; }
        public string numberOfChild { get; set; }

        public string numberOfAdult { get; set; }
        public string numberOfInfant { get; set; }
        public string clientType { get; set; }
      
    }
}
