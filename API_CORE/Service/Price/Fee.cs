using API_CORE.Service.Price.ServiceFee;
using ENTITIES.ViewModels.Price;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using Utilities.Contants;

namespace API_CORE.Service.Price
{
    /// <summary>
    /// Chính sách giá cho các dịch vụ
    /// </summary>
    public class Fee
    {
        private readonly IServicePiceRepository price_repository;
        public Fee(IServicePiceRepository _price_repository)
        {
            price_repository = _price_repository;
        }

        /// <summary>
        /// price: chi phí gốc
        /// allotment_id: id hợp đồng
        /// provider_id: là hotel_id
        /// room_id: id phòng
        /// </summary>
        /// <param name="service_type"></param>
        /// <param name="allotment_id"></param>
        /// <param name="provider_id"></param>
        /// <param name="room_id"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, List<PriceViewModel>>> getRoomFee(int group_provider_type, string allotment_id, string provider_id, string package_id, string room_id,double price, DateTime from_date, DateTime to_date)
        {
            try
            {
                var fee = new Dictionary<string, List<PriceViewModel>>();
                switch (Convert.ToInt16(group_provider_type))
                {
                    case (Int16)PriceServiceType.ROOM_VIN:
                    case (Int16)PriceServiceType.ROOM_MANUAL:
                        // Lấy ra thông tin chính sách giá theo dịch vụ

                        var price_policy = await price_repository.getRoomPriceService( group_provider_type,  allotment_id,  provider_id,  package_id,  room_id,  from_date,  to_date);
                        var calulator = new FeeHotel(price, price_policy, group_provider_type);

                        fee = calulator.getHotelFee();
                        break;
                }


                return fee;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        //public async Task<Dictionary<string, double>> getFlightTicketFee(DateTime fr, DateTime td)
        //{
        //    try
        //    {
        //        var calulator = new FeeFlightTickets();
        //        var fee = calulator.getFlightTicketsFee();

        //        return fee;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //public async Task<Dictionary<string, double>> getSafariFee(DateTime fr, DateTime td)
        //{
        //    try
        //    {
        //        var calulator = new FeeFlightTickets();
        //        var fee = calulator.getFlightTicketsFee();

        //        return fee;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}


        //public async Task<Dictionary<string, double>> getCarRentalTravelFee(DateTime fr, DateTime td)
        //{
        //    try
        //    {
        //        var calulator = new FeeFlightTickets();
        //        var fee = calulator.getFlightTicketsFee();

        //        return fee;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //public async Task<Dictionary<string, double>> getComboTourFee(DateTime fr, DateTime td)
        //{
        //    try
        //    {
        //        var calulator = new FeeFlightTickets();
        //        var fee = calulator.getFlightTicketsFee();

        //        return fee;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
    }
}
