using Caching.Elasticsearch;
using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.Elasticsearch;
using ENTITIES.ViewModels.ElasticSearch;
using ENTITIES.ViewModels.Hotel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.Elasticsearch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using static Utilities.Contants.HotelSearchViewType;

namespace REPOSITORIES.Repositories.Elasticsearch
{
    public class ElasticsearchDataRepository : IElasticsearchDataRepository
    {
        private IConfiguration configuration;
        private AllCodeDAL AllCodeDAL;
      
        public ElasticsearchDataRepository(IOptions<DataBaseConfig> dataBaseConfig, IConfiguration _configuration)
        {
            AllCodeDAL = new AllCodeDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            configuration = _configuration;
        }

        public async Task<HotelESViewModel> GetByHotelID(string hotel_id, string index_name)
        {
            try
            {
                IESRepository<OrderElasticsearchViewModel> _ESRepository = new ESRepository<OrderElasticsearchViewModel>(configuration["DataBaseConfig:Elastic:Host"]);


                return await _ESRepository.GetByHotelID(hotel_id, index_name);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ElasticsearchDataRepository - GetByHotelID: " + ex.ToString());
                return null;
            }
        }

        public async Task<List<ElasticsearchHotelViewModel>> GetElasticsearchHotel(string txtsearch,string Type= "hotel")
        {
            try
            {
                txtsearch = txtsearch.Trim();
                txtsearch = CommonHelper.RemoveUnicode(txtsearch);
                List<ElasticsearchHotelViewModel> elasticsearchHotels = new List<ElasticsearchHotelViewModel>();
                IESRepository<HotelESViewModel> _ESRepository = new ESRepository<HotelESViewModel>(configuration["DataBaseConfig:Elastic:Host"]);
                var result =await _ESRepository.GetListProduct(configuration["DataBaseConfig:Elastic:index_product_search"], txtsearch.ToLower(), Type);
                //Version 1: Giới hạn lại chỉ khách sạn VIN:
                result = result.Where(x => x.name.ToLower().Contains("vin")).ToList();
                result = result.GroupBy(x => x.name).Select(y => y.First()).ToList();

                var typeofroom_data =await AllCodeDAL.GetTypeOfRoom();
                var state_list = result.Where(x=>x.city!=null && x.city.ToLower().Contains(txtsearch.ToLower())).Select(x => CommonHelper.RemoveUnicode(x.city));
                if (state_list.Count() > 0)
                {
                    state_list = state_list.Distinct();
                    foreach (var state in state_list)
                    {
                        elasticsearchHotels.Add(new ElasticsearchHotelViewModel() { 
                            address="",
                            district="",
                            hotel_id= state,
                            name="Các khách sạn tại "+state,
                            product_type = (int)HotelSearchViewModelType.LOCATION
                        });
                    }
                }
                var group_list = result.Where(x => x.groupname!=null && x.groupname.ToLower().Contains(txtsearch.ToLower())).Select(x => x.groupname);
                if (group_list.Count() > 0)
                {
                    group_list = group_list.Distinct();
                    foreach (var group in group_list)
                    {
                        elasticsearchHotels.Add(new ElasticsearchHotelViewModel()
                        {
                            address = "",
                            district = "",
                            hotel_id = group,
                            name = "Các khách sạn thuộc chuỗi khách sạn " + group,
                            product_type = (int)HotelSearchViewModelType.GROUP_NAME
                        });
                    }
                }
                foreach (var item in result)
                {
                   ElasticsearchHotelViewModel model = new ElasticsearchHotelViewModel();
                   List< string> type_room_name = new List<string>();
                    foreach (var i in typeofroom_data)
                    {

                        if (item.typeofroom != null)
                        {
                            model.hotel_id = item.hotelid;
                            model.name = item.name;
                            model.address = item.street;
                            model.district = item.state;
                            model.type_of_room = item.typeofroom.Split(",").ToList();
                            type_room_name.Add(i.Description);
                            model.type_of_room_name = type_room_name;
                            model.product_type = (int)HotelSearchViewModelType.HOTEL;
                        }
                        else {
                            model.hotel_id = item.hotelid;
                            model.name = item.name;
                            model.address = item.street;
                            model.district = item.state;
                            model.product_type = (int)HotelSearchViewModelType.HOTEL;
                        }

                    }
                    elasticsearchHotels.Add(model);
                }
               
                return elasticsearchHotels;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ElasticsearchDataRepository - GetElasticsearchHotel: " + ex.ToString());
                return null;
            }
           
        }

        public async Task<List<OrderElasticsearchViewModel>> GetElasticsearchOrder(string index_name, string txtsearch, string Type)
        {
            try
            {
                IESRepository<OrderElasticsearchViewModel> _ESRepository = new ESRepository<OrderElasticsearchViewModel>(configuration["DataBaseConfig:Elastic:Host"]);
               
                
                return await _ESRepository.GetListProductOrder(index_name, txtsearch, Type);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ElasticsearchDataRepository - GetElasticsearchOrder: " + ex.ToString());
                return null;
            }
        }       

    }
}
