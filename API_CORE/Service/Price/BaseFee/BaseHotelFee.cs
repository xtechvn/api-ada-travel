using ENTITIES.ViewModels.Price;
using Repositories.ShippingFeeRepositories.BaseFee;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Contants;

namespace API_CORE.Service.Price.BaseFee
{
    /// <summary>
    /// Hàm tính giá cho dịch vụ thuê phòng khách sạn
    /// </summary>
    class BaseHotelFee : BaseFeeProductRepository
    {
        public BaseHotelFee(double price, List<PriceViewModel> price_policy, int group_provider_type)
           : base(price, price_policy, group_provider_type) { }


        /// Công thức tính giá cho đại lý
        /// Ouput: Trả ra 1 list giá có hiệu lực theo giá gốc
        /// TÍnh theo vnđ
        public virtual List<PriceViewModel> FeeB2b()
        {
            try
            {
                var amount_list = new List<PriceViewModel>();
                var price_b2b = _price_policy.Where(x => x.client_type_id == (int)ClientType.AGENT);
                foreach (var item in price_b2b)
                {
                    var model = new PriceViewModel
                    {
                        hotel_id = item.hotel_id,
                        room_id = item.room_id,
                        client_type_id = item.client_type_id,
                        price_id = item.price_id,
                        service_type = item.service_type,
                        price = _group_provider_type == (Int16)PriceServiceType.ROOM_VIN ? item.price : _price,
                        profit = item.profit,
                        unit_id = item.unit_id,
                        from_date = item.from_date, //).ToString("dd-MM-yyyy HH:mm"),
                        to_date = item.to_date,
                        amount = item.unit_id == (Int16)UnitType.PERCENT ? _price + (_price * item.profit) / 100 : _price + item.profit,
                        pakage_id = item.pakage_id
                    };
                    amount_list.Add(model);
                }
                return amount_list;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// Công thức tính giá cho khách lẻ
        public virtual List<PriceViewModel> FeeB2c()
        {
            try
            {
                var amount_list = new List<PriceViewModel>();
                var price_b2c = _price_policy.Where(x => x.client_type_id == (int)ClientType.CUSTOMER);
                foreach (var item in price_b2c)
                {
                    var model = new PriceViewModel
                    {
                        hotel_id = item.hotel_id,
                        room_id = item.room_id,
                        client_type_id = item.client_type_id,
                        price_id = item.price_id,
                        price = _group_provider_type == (Int16)PriceServiceType.ROOM_VIN ? item.price : _price,
                        profit = item.profit,
                        service_type = item.service_type,
                        unit_id = item.unit_id,
                        from_date = item.from_date, //).ToString("dd-MM-yyyy HH:mm"),
                        to_date = item.to_date,
                        amount = item.unit_id == (Int16)UnitType.PERCENT ? _price + (_price * item.profit) / 100 : _price + item.profit,
                        pakage_id = item.pakage_id

                    };
                    amount_list.Add(model);
                }
                return amount_list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
