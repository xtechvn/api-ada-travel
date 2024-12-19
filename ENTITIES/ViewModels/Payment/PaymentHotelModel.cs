using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Payment
{
    public class PaymentHotelModel
    {
        public int payment_type { get; set; }
        public string bank_name { get; set; }
        public string short_name { get; set; }
        public string bank_code { get; set; }
        public string bank_account { get; set; }
        public string booking_id { get; set; }
        public decimal amount { get; set; }
        public long order_id { get; set; }
        public string order_no { get; set; }
        public int event_status { get; set; }
    }
}
