using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Payment
{
   public class OrderViewModel
    {
        public int order_id { get; set; }
        public string client_name { get; set; }
        public double amount { get; set; }
    }
}
