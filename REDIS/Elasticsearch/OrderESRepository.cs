using Elasticsearch.Net;
using ENTITIES.ViewModels.ElasticSearch;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;

namespace Caching.Elasticsearch
{
    public class OrderESRepository :ESRepository<OrderElasticsearchViewModel>
    {
        public OrderESRepository(string Host) : base(Host) { }
        public async Task<OrderElasticsearchViewModel> GetOrderByOrderNo(string order_no, string index_name = "adavigo_sp_getorder")
        {
            List<OrderElasticsearchViewModel> result = new List<OrderElasticsearchViewModel>();
            try
            {
                int top = 30;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<OrderElasticsearchViewModel>(s => s
                    .Index(index_name)
                   .Query(q => q.Match(m => m.Field("orderno").Query(order_no.Trim())))
                   
                   
                   );
                if (search_response.IsValid)
                {
                    result = search_response.Documents as List<OrderElasticsearchViewModel>;
                    if(result!=null && result.Count>0)
                    {
                        return result[0];
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrderByOrderNo - OrderESRepository. " + ex);
            }
            return null;

        }

    }
}
