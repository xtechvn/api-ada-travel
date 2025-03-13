using Elasticsearch.Net;
using ENTITIES.Models;
using ENTITIES.ViewModels.ElasticSearch;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

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
        public async Task<List<OrderElasticsearchViewModel>> GetOrderByIds(string order_ids, string index_name = "adavigo_sp_getorder")
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
                     .Size(10000)
                     .Query(q =>
                     {
                         if (order_ids.Trim() == "-1")
                         {
                             // Return all documents when input is "-1"
                             return q.MatchAll();
                         }
                         else
                         {
                             // Split the comma-separated string and search for matching order numbers
                             var orderArray = order_ids.Split(',')
                                     .Select(x => x.Trim())
                                     .Where(x => !string.IsNullOrEmpty(x))
                                     .Select(x => long.Parse(x))  // Convert to long
                                     .ToArray();

                             return q.Terms(t => t
                                 .Field("id")
                                 .Terms(orderArray)
                             );
                         }
                     })
                 );
                if (search_response.IsValid)
                {
                    result = search_response.Documents as List<OrderElasticsearchViewModel>;
                    if(result!=null && result.Count>0)
                    {
                        return result;
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrderByOrderNo - OrderESRepository. " + ex);
            }
            return null;

        }
        public async Task<List<OrderElasticsearchViewModel>> GetListOrder(long accountclientid, List<int> order_status,List<int> payment_status, DateTime fromDate, DateTime toDate, string index_name = "adavigo_sp_getorder")
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response =  elasticClient.Search<OrderElasticsearchViewModel>(s => s
                     .Index(index_name) // Replace with your index name
                     .Query(q => q
                         .Bool(b => b
                             .Must(mu => mu
                                 .Term(t => t
                                     .Field(x=>x.accountclientid)
                                     .Value(accountclientid)
                                 )

                             )
                              .Filter(f => f
                                    .DateRange(dr => dr
                                        .Field("createtime") // Replace with your date field name
                                        .GreaterThanOrEquals(fromDate)
                                        .LessThanOrEquals(toDate)
                                    )
                                  
                                )
                             
                         )
                     )
                );
                if (search_response.IsValid)
                {
                    var data= search_response.Documents as List<OrderElasticsearchViewModel>;
                    if(data != null && data.Count > 0)
                    {
                        data = data.Where(x => order_status.Contains((int)x.orderstatus) && (payment_status == null || payment_status.Contains((int)x.paymentstatus))).ToList();
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByClientId - OrderESRepository. " + ex);
            }
            return null;

        }
        public async Task<List<OrderElasticsearchViewModel>> GetListOrderCheckinNow(long accountclientid, List<int> order_status, DateTime fromDate, DateTime toDate, string index_name = "adavigo_sp_getorder")
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<OrderElasticsearchViewModel>(s => s
                     .Index(index_name) // Replace with your index name
                     .Query(q => q
                         .Bool(b => b
                             .Must(mu => mu
                                 .Term(t => t
                                     .Field(x => x.accountclientid)
                                     .Value(accountclientid)
                                 )

                             )
                              .Filter(f => f
                                    .DateRange(dr => dr
                                        .Field("startdate") // Replace with your date field name
                                        .GreaterThanOrEquals(fromDate)
                                        .LessThanOrEquals(toDate)
                                    )

                                )

                         )
                     )
                );
                if (search_response.IsValid)
                {
                    var data = search_response.Documents as List<OrderElasticsearchViewModel>;
                    if (data != null && data.Count > 0)
                    {
                        data = data.Where(x => order_status.Contains((int)x.orderstatus)).ToList();
                    }
                    return data;

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByClientId - OrderESRepository. " + ex);
            }
            return null;

        }
        public async Task<List<OrderElasticsearchViewModel>> GetListOrderCheckoutNow(long accountclientid, List<int> order_status, DateTime fromDate, DateTime toDate, string index_name = "adavigo_sp_getorder")
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<OrderElasticsearchViewModel>(s => s
                     .Index(index_name) // Replace with your index name
                     .Query(q => q
                         .Bool(b => b
                             .Must(mu => mu
                                 .Term(t => t
                                      .Field(x => x.accountclientid)
                                     .Value(accountclientid)
                                 )
                                 & mu.Terms(t => t
                                        .Field("status") // Replace with your status field name
                                        .Terms(order_status)
                                    )

                             )
                              .Filter(f => f
                                    .DateRange(dr => dr
                                        .Field("enddate") // Replace with your date field name
                                        .GreaterThanOrEquals(fromDate)
                                        .LessThanOrEquals(toDate)
                                    )

                                )

                         )
                     )
                );
                if (search_response.IsValid)
                {
                    return search_response.Documents as List<OrderElasticsearchViewModel>;

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByClientId - OrderESRepository. " + ex);
            }
            return null;

        }
    }
}
