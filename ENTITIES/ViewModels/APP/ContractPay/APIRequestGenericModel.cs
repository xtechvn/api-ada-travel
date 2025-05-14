using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITIES.ViewModels.APP.ContractPay
{
    public class APIRequestGenericModel
    {
        public string token { get; set; }
        public string phone_number { get; set; }
        public string message { get; set; }
        public string name { get; set; }
        public string timestamp { get; set; }
        public string sim_id { get; set; }
    }
}
