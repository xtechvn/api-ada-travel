using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CORE.Model.Base
{
    public class ServicePriceModel
    {
        public double price { get; set; } // giá gốc dịch vụ.
        public double interest_b2b_price { get; set; } // phí hoa hồng cho b2b
        public double interest_b2c_price { get; set; } // phí hoa hồng cho b2c
        public int service_type { get; set; } // loại dịch vụ.
    }
}
