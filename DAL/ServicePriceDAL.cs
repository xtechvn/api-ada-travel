using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;
using System.Linq;

using ENTITIES.ViewModels.Price;

namespace DAL
{
    public class ServicePriceDAL : GenericService<PriceDetail>
    {
        private static DbWorker _DbWorker;
        public ServicePriceDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        /// <summary>
        /// Lấy ra đơn vị tính theo dịch vụ 
        /// </summary>
        /// <param name="service_type"></param>
        /// <returns></returns>
        public async Task<List<PriceDetail>> getServicePrice(int service_type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var dataList = await (from p in _DbContext.PriceDetail.AsNoTracking()
                                          where p.ServiceType == service_type
                                          && p.FromDate <= DateTime.Now
                                          && p.ToDate >= DateTime.Now
                                          select p
                                   ).ToListAsync();
                    return dataList;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getServicePrice: " + ex);
                return null;
            }
        }


        /// <summary>
        /// Lấy ra lợi nhuận của loại phòng
        /// </summary>
        /// <param name="service_type"></param>
        /// group_provider_type: là nguồn dẫn từ đâu
        /// <returns></returns>
        public async Task<List<PriceViewModel>> getPricePolicyRoom(int group_provider_type, string allotment_id, string provider_id, string package_id, string room_id, DateTime from_date, DateTime to_date)
        {
            try
            {
                var price_policy = new List<PriceViewModel>();
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    // Bao gồm all client type
                    var product_service = await (from c in _DbContext.Campaign.AsNoTracking()
                                                 join s in _DbContext.ProductRoomService.AsNoTracking() on c.Id equals s.CampaignId into sp
                                                 from _room_service in sp.DefaultIfEmpty()
                                                 where (string.IsNullOrEmpty(_room_service.AllotmentsId) ? "-1" : _room_service.AllotmentsId) == allotment_id
                                                 //&& _room_service.ProviderId == provider_id
                                                 //&& _room_service.PackageId == package_id
                                                 //&& _room_service.RoomId == room_id
                                                 //&& _room_service.GroupProviderType == group_provider_type
                                                 select _room_service
                                   ).ToListAsync();

                    if (product_service.Count() > 0)
                    {
                        foreach (var item in product_service)
                        {
                            // quét theo từng loại khách hàng
                            price_policy = await (from c in _DbContext.PriceDetail
                                                  where c.ProductServiceId == item.Id
                                                   //&& ((c.FromDate <= to_date && to_date <= c.ToDate)
                                                   // || (c.FromDate <= from_date && from_date <= c.ToDate)
                                                   // || (from_date <= c.FromDate && c.ToDate <= to_date))
                                                  select new PriceViewModel
                                                  {
                                                      //hotel_id = item.ProviderId,
                                                      //room_id = item.RoomId,
                                                      price_id = c.Id,
                                                      price = c.Price,
                                                      service_type = c.ServiceType,
                                                      client_type_id = _DbContext.Campaign.FirstOrDefault(x => x.Id == item.CampaignId).ClientTypeId ?? 0,
                                                      profit = c.Profit,
                                                      unit_id = c.UnitId ?? 0,
                                                      from_date = c.FromDate ?? DateTime.Now,
                                                      to_date = c.ToDate ?? DateTime.Now,
                                                      //pakage_id = item.PackageId
                                                  }
                                                ).ToListAsync();
                        }
                        return price_policy;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getPricePolicyRoom: " + ex);
                return null;
            }
        }


      
    }
}
