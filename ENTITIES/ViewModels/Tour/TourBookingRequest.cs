using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ENTITIES.ViewModels.Tour
{
    public class TourBookingRequest
    {
        public TourBookingRequestContact contact { get; set; }
        public long tour_product_id { get; set; }
        public long tour_product_package_id { get; set; }
        public DateTime start_date { get; set; }
        public TourBookingRequestGuest guest { get; set; }
        public string voucher_name { get; set; }
        public long account_client_id { get; set; }
        public string note { get; set; }
        public bool is_daily { get; set; }
        public string client_type { get; set; }
    }

    public class TourBookingRequestContact
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }

    }
    public class TourBookingRequestGuest
    {
        public int adult { get; set; }
        public int child { get; set; }
        public int infant { get; set; }


    }
}
