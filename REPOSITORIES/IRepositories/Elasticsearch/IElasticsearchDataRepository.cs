using ENTITIES.Models;
using ENTITIES.ViewModels.Elasticsearch;
using ENTITIES.ViewModels.ElasticSearch;
using ENTITIES.ViewModels.Hotel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories.Elasticsearch
{
    public interface IElasticsearchDataRepository
    {
        Task<List<ElasticsearchHotelViewModel>> GetElasticsearchHotel(string txtsearch, string Type = "hotel");
        Task<List<OrderElasticsearchViewModel>> GetElasticsearchOrder(string index_name, string txtsearch, string Type);
        Task<HotelESViewModel> GetByHotelID(string hotel_id, string index_name);
    }
}
