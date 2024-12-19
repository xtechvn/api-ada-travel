
using ENTITIES.ViewModels.Price;
using System;
using System.Collections.Generic;

namespace Repositories.ShippingFeeRepositories.BaseFee
{
    /// <summary>
    ///Áp dụng quy tắc SOLID: Liskov để định nghĩa 1 lớp PARENT trừu tượng là lớp phí mua hộ
    /// ///Lớp này là lớp Base và k dc phép sửa đổi
    /// </summary>
    public class BaseFeeProductRepository
    {
    
        public double _price { get; set; }
        public List<PriceViewModel> _price_policy { get; set; }
        public int _group_provider_type { get; set; }
        public BaseFeeProductRepository( double price, List<PriceViewModel> price_policy,int group_provider_type)
        {          
            _price = price;
            _price_policy = price_policy;
            _group_provider_type = group_provider_type;
        }

        // Tính phí tiền chênh lệch 
        /// <summary>
        /// Có thể là nhập vnđ hoặc nhập %
        /// </summary>
        /// <returns></returns>
        //public virtual double CommissionFee()
        //{
        //    try
        //    {               
        //        return service_price.interest_price + service_price.price;
        //    }
        //    catch (Exception ex)
        //    {
        //       // LogHelper.InsertLogTelegram("LuxuryFee - amz" + ex.Message);
        //        return 0;
        //    }
        //}

    }

}
