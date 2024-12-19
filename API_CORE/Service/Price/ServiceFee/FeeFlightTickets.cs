using API_CORE.Service.Price.BaseFee;
using ENTITIES.ViewModels.Price;
using System;
using System.Collections.Generic;
using Utilities.Constants;

namespace API_CORE.Service.Price.ServiceFee
{
    class FeeFlightTickets : BaseFlightTicketsFee
    {
        //Inject đầu vào
        public FeeFlightTickets(int service_type, double price, List<PriceViewModel> price_policy,int group_provider_type)
            : base( price, price_policy, group_provider_type) { }

        // Hàm tính phí vé máy bay
        // Được phép chỉnh sửa các hàm của lớp cha nhưng không được thay đổi tính đúng đắn của lớp cha        
        public override double CommissionB2BFee()
        {
            return base.CommissionB2BFee();
        }

        // return ra phí chêch lệch
        public Dictionary<string, double> getFlightTicketsFee()
        {
            try
            {
                var shipping_fee = new Dictionary<string, double>
                {
                    { FeeCommissionType.b2b_amount_last.ToString(),CommissionB2BFee()}, // Giá về tay vé máy bay cho B2B
                    { FeeCommissionType.b2c_amount_last.ToString(),CommissionB2CFee()} // Giá về tay vé máy bay cho B2C                     
                };
                return shipping_fee;
            }
            catch (Exception ex)
            {
                // LogHelper.InsertLogTelegram("getAmazoneShippingFee" + ex.Message);
                return null;

            }
        }


    }
}
