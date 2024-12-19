using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Client
{
    public class ClientInfoViewModel 
    {
        public long client_id { get; set; }
        public long account_client_id { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int gender { get; set; }
        public int client_type { get; set; }
        public string name { get; set; }

        public int birthday_year { get; set; }

        public int birthday_month { get; set; }

        public int birthday_day { get; set; }

        public int province_id { get; set; }

        public int district_id { get; set; }

        public int ward_id { get; set; }

        public string address { get; set; }
        public string password_old { get; set; }
        public string password_new { get; set; }
       public string confirm_password_new { get; set; }

    }
}
