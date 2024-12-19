using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels;
using ENTITIES.ViewModels.Price;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.Repositories
{
  public  class ServicePiceRepository : IServicePiceRepository
    {

        private readonly ServicePriceDAL _ServicePriceDAL;
        private readonly IOptions<DataBaseConfig> _DataBaseConfig;
        public ServicePiceRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _DataBaseConfig = dataBaseConfig;
            _ServicePriceDAL = new ServicePriceDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        /// <summary>
        /// Chính sách giá
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<List<PriceDetail>> getServicePrice(int service_type)
        {
            try
            {
                return await _ServicePriceDAL.getServicePrice(service_type);
            }
            catch (Exception ex)
            {

                throw;
            }            
        }

        /// <summary>
        /// Lấy ra chính sách giá phòng
        /// </summary>
        /// <param name="service_type"></param>
        /// <returns></returns>
        public async Task<List<PriceViewModel>> getRoomPriceService(int group_provider_type, string allotment_id, string provider_id, string package_id, string room_id, DateTime from_date, DateTime to_date)
        {
            try
            {
                return await _ServicePriceDAL.getPricePolicyRoom( group_provider_type,  allotment_id,  provider_id,  package_id,  room_id,  from_date,  to_date);
            }
            catch (Exception ex)
            {

                throw;
            }
        }



       
    }
}
