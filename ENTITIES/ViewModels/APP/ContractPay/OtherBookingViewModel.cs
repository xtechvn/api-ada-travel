using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels
{
   public class OtherBookingViewModel : ENTITIES.Models.OtherBooking
    {
        public string ServiceName { get; set; }
        public string OperatorName { get; set; }
    }
}
