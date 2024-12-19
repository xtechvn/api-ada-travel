using API_CORE.Service.Price.BaseFee;
using ENTITIES.ViewModels.Price;
using System;
using System.Collections.Generic;
using Utilities;
using Utilities.Constants;

namespace API_CORE.Service.Price.ServiceFee
{
    class FeeHotel : BaseHotelFee
    {
        //Inject đầu vào
        public FeeHotel(double price, List<PriceViewModel> price_policy, int group_provider_type)
           : base(price, price_policy, group_provider_type) { }

        // Được phép chỉnh sửa các hàm của lớp cha nhưng không được thay đổi tính đúng đắn của lớp cha        
        public override List<PriceViewModel> FeeB2b()
        {
            return base.FeeB2b();
        }

        public override List<PriceViewModel> FeeB2c()
        {
            return base.FeeB2c();
        }

        // return ra phí chêch lệch
        public Dictionary<string, List<PriceViewModel>> getHotelFee()
        {
            try
            {
                var shipping_fee = new Dictionary<string, List<PriceViewModel>>
                {
                    { FeeCommissionType.b2b_amount_last.ToString(),FeeB2b()}, // Giá về tay  cho B2B
                    { FeeCommissionType.b2c_amount_last.ToString(),FeeB2c()} // Giá về tay cho B2C
                     
                };
                return shipping_fee;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getHotelFee" + ex.Message);
                return null;

            }
        }
    }
}
