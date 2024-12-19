using API_CORE.Model.Base;
using ENTITIES.Models;
using ENTITIES.ViewModels;
using ENTITIES.ViewModels.Price;
using Repositories.ShippingFeeRepositories.BaseFee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CORE.Service.Price.BaseFee
{
    class BaseFlightTicketsFee : BaseFeeProductRepository
    {
        public BaseFlightTicketsFee( double price, List<PriceViewModel> price_policy, int group_provider_type)
           : base( price, price_policy, group_provider_type) { }

        // Công thức tính giá cho B2B
        public virtual double CommissionB2BFee()
        {
            return 0;//service_price.interest_b2b_price + service_price.price;
        }

        // Công thức tính giá cho B2C
        public virtual double CommissionB2CFee()
        {
            return 0;
        }

    }
}
