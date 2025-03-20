using Elasticsearch.Net;
using Entities.ViewModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.Hotel;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Caching.Elasticsearch
{
    public class HotelESRepository : ESRepository<HotelESViewModel>
    {
        public string index_hotel = "adavigo_sp_gethotel";

        public HotelESRepository(string Host) : base(Host) { }

        public async Task<List<HotelESViewModel>> GetListProduct(string txtsearch, bool isvinhotel, string index_name = "adavigo_sp_gethotel", string Type = "product")
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);
                if (txtsearch == null) txtsearch = "";
                else txtsearch = txtsearch.Trim();
                ISearchResponse<HotelESViewModel> search_response;
                List<bool?> isvinhotels = isvinhotel ? new List<bool?> { true } : new List<bool?> { false, null };
                //search_response = elasticClient.Search<HotelESViewModel>(s => s
                //           .Index(index_name)
                //           .From(0)
                //           .Size(top)
                //           .Query(q => q.Match(m => m.Field(y => y.name).Query("*"+txtsearch+"*"))
                //                        &&
                //                       q.Terms(t => t.Field(f => f.isvinhotel).Terms(isvinhotels))
                //                       && q.Terms(t => t.Field(f => f.isvinhotel).Terms(isvinhotels))
                //             ));
                search_response = elasticClient.Search<HotelESViewModel>(s => s
                     .Index(index_name)
                     .Size(4000)
                     .Query(q => q
                         .MultiMatch(mm => mm
                             .Fields(f => f
                                 .Field(ff => ff.name)    // Trường name
                                 .Field(ff => ff.state)   // Trường state
                                 .Field(ff => ff.city)    // Trường city
                             )
                             .Query(txtsearch)
                             .Fuzziness(Fuzziness.Auto) // Cho phép sai lệch nhỏ
                             .Type(TextQueryType.BestFields) // Chỉ cần 1 trường khớp
                         )
                     )
                 );


                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
                    result = result.Where(x => isvinhotels.Contains(x.isvinhotel) && x.isdisplaywebsite == true).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<HotelESViewModel>> GetListCity(string txtsearch, bool isvinhotel, string index_name = "hotel_store", string Type = "product")
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);
                if (txtsearch == null) txtsearch = "";
                ISearchResponse<HotelESViewModel> search_response;
                search_response = elasticClient.Search<HotelESViewModel>(s => s
                     .Index(index_name)
                     .Size(4000)
                     .Query(q => q
                         .MultiMatch(mm => mm
                             .Fields(f => f
                                 .Field(ff => ff.name)    // Trường name
                                 .Field(ff => ff.state)   // Trường state
                                 .Field(ff => ff.city)    // Trường city
                             )
                             .Query(txtsearch)
                             .Fuzziness(Fuzziness.Auto) // Cho phép sai lệch nhỏ
                             .Type(TextQueryType.BestFields) // Chỉ cần 1 trường khớp
                         )
                     )
                 );
                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
                    result = result.GroupBy(x => new { x.street, x.state, x.city }).Select(x => x.First()).ToList();
                    result = result.Where(x => x.city != null && x.city.Trim() != "").GroupBy(x => x.city.Trim()).Select(x => x.First()).ToList();
                    result = result.Where(x => CommonHelper.RemoveUnicode(x.city).ToLower().Contains(CommonHelper.RemoveUnicode(txtsearch).ToLower())).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<HotelESViewModel>> GetListState(string txtsearch, bool isvinhotel, string index_name = "hotel_store", string Type = "product")
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);
                if (txtsearch == null) txtsearch = "";
                ISearchResponse<HotelESViewModel> search_response;
                search_response = elasticClient.Search<HotelESViewModel>(s => s
                           .Index(index_name)
                           .Query(q =>
                             q.Bool(
                                 qb => qb.Should(
                                     sh => sh.QueryString(m => m
                                     .DefaultField(f => f.name)
                                     .Query("*" + txtsearch + "*")),
                                     sh => sh.QueryString(m => m
                                     .DefaultField(f => f.street)
                                     .Query("*" + txtsearch + "*"))
                                       ,
                                     sh => sh.QueryString(m => m
                                     .DefaultField(f => f.state)
                                     .Query("*" + txtsearch + "*"))

                                 )
                             )
                             ).Source(sf => sf
                            .Includes(i => i
                                .Fields(
                                    f => f.city,
                                    f => f.state,
                                    f => f.street
                                )
                            ))
                           );
                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
                    result = result.GroupBy(x => new { x.street, x.state, x.city }).Select(x => x.First()).ToList();
                    result = result.Where(x => x.state != null && x.state.Trim() != "").GroupBy(x => x.state.Trim()).Select(x => x.First()).ToList();
                    result = result.Where(x => CommonHelper.RemoveUnicode(x.city).ToLower().Contains(CommonHelper.RemoveUnicode(txtsearch).ToLower())).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<HotelESViewModel>> GetListProductAll(string txtsearch, string index_name = "hotel_store", string Type = "product")
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);
                if (txtsearch == null) txtsearch = "";
                ISearchResponse<HotelESViewModel> search_response;
                search_response = elasticClient.Search<HotelESViewModel>(s => s
                           .Index(index_name)
                           .From(0)
                           .Size(top)
                           .Query(q =>
                             q.Bool(
                                 qb => qb.Should(
                                     sh => sh.QueryString(m => m
                                     .DefaultField(f => f.name)
                                     .Query("*" + txtsearch + "*")),
                                     sh => sh.QueryString(m => m
                                     .DefaultField(f => f.street)
                                     .Query("*" + txtsearch + "*"))

                                 )
                             )
                             )
                           );
                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
                    result = result.Where(x => x.isdisplaywebsite == true).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public HotelESViewModel FindByHotelId(string hotel_id)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("hotel");
                var elasticClient = new ElasticClient(connectionSettings);

                //var searchResponse = elasticClient.Search<HotelESViewModel>(s => s
                //    .Index(index_hotel)
                //    .Query(q => q.Bool(y=>y.Must(z=> z.Match(m => m.Field("hotelid").Query(hotel_id))) ))
                //);
                var searchResponse = elasticClient.Search<HotelESViewModel>(sd => sd
                    .Index(index_hotel)
                    .Size(4000)
                    .Query(q => q
                        .QueryString(m => m.DefaultField("hotelid").Query(hotel_id)
                        )));
                var JsonObject = JsonConvert.SerializeObject(searchResponse.Documents);
                var object_result = JsonConvert.DeserializeObject<List<HotelESViewModel>>(JsonObject);
                return object_result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<HotelESViewModel>> GetListByLocation(string name, int id, int type)
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("hotel_store");
                var elasticClient = new ElasticClient(connectionSettings);
                if (name == null) name = "";
                else name = name.Trim();
                string typefalse = "false";
                string typetrue = "true";
                ISearchResponse<HotelESViewModel> search_response = null;
                search_response = elasticClient.Search<HotelESViewModel>(s => s
                              .Index(index_hotel)
                                   .From(0)
                                   .Size(top)
                               .Query(q =>
                                 q.Bool(
                                     qb => qb.Must(
                                         sh => sh.Term("isdisplaywebsite", typetrue)

                                       ).Should(
                                        sh => sh.QueryString(m => m.DefaultField(f => f.city).Query("*" + name + "*")),
                                        sh => sh.QueryString(m => m.DefaultField(f => f.city).Query("*" + CommonHelper.RemoveUnicode(name) + "*")),
                                        sh => sh.QueryString(m => m.DefaultField(f => f.city).Query("*" + CommonHelper.RemoveUnicode(name).ToLower() + "*")),
                                         sh => sh.QueryString(m => m.DefaultField(f => f.state).Query("*" + name + "*")),
                                        sh => sh.QueryString(m => m.DefaultField(f => f.state).Query("*" + CommonHelper.RemoveUnicode(name) + "*")),
                                        sh => sh.QueryString(m => m.DefaultField(f => f.state).Query("*" + CommonHelper.RemoveUnicode(name).ToLower() + "*"))

                                         )
                                     )
                                ));

                if (search_response != null && !search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
                    return result.Where(x =>
                    (x.street != null && CommonHelper.RemoveUnicode(x.street).ToLower().Contains(CommonHelper.RemoveUnicode(name).ToLower()))
                    || (x.city != null && CommonHelper.RemoveUnicode(x.city).ToLower().Contains(CommonHelper.RemoveUnicode(name).ToLower()))
                    || (x.state != null && CommonHelper.RemoveUnicode(x.state).ToLower().Contains(CommonHelper.RemoveUnicode(name).ToLower()))

                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<List<HotelESViewModel>> GetAllHotels()
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 10000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("hotel_store");
                var elasticClient = new ElasticClient(connectionSettings);
                ISearchResponse<HotelESViewModel> search_response;
                search_response = elasticClient.Search<HotelESViewModel>(s => s
                           .Index(index_hotel)
                           .From(0)
                           .Size(top)
                           );
                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<HotelESViewModel>> GetListByLocationName(string name, string Type = "product")
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);

                ISearchResponse<HotelESViewModel> search_response;
                if (name == null || name.Trim() == "")
                {
                    search_response = elasticClient.Search<HotelESViewModel>(s => s
                              .Size(50)  // Get top 50 results
                              .Sort(sort => sort
                                  .Descending(f => f.id)  // Sort by CreatedDate descending (most recent first)
                              )
                          );
                }
                else
                {
                    search_response = elasticClient.Search<HotelESViewModel>(s => s
                               .Index(index_hotel)
                                .Sort(ss => ss
                                    .Descending(f => f.id) // Sorting by CreatedDate in descending order
                                )
                               .Query(q => q
                                   .Bool(b => b
                                       .Should(
                                           sh => sh.Match(m => m.Field(f => f.state).Query(name)),  // Match state
                                           sh => sh.Match(m => m.Field(f => f.city).Query(name))    // Match city
                                       )
                                       .MinimumShouldMatch(1) // Ensures at least one condition matches
                                   )
                               )
                           );
                }

                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
                    result = result.Where(x => x.isdisplaywebsite == true).ToList();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<HotelESViewModel>> GetListByLocationNamePagnition(string name, int? index = 1, int? size = 30, string Type = "product")
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);
                int skip = (index == null || size == null) ? 1 : ((int)index - 1) * (int)size; // Calculate how many documents to skip
                int take = size == null ? 30 : (int)size; // Number of documents per page

                ISearchResponse<HotelESViewModel> search_response;
                if (name == null || name.Trim() == "")
                {
                    search_response = elasticClient.Search<HotelESViewModel>(s => s
                               .From(skip) // Skip records for pagination
                                .Size(take) // Take only the required records
                                .Sort(ss => ss
                                    .Descending(f => f.id) // Sorting by CreatedDate in descending order
                                )
                          );
                }
                else
                {
                    search_response = elasticClient.Search<HotelESViewModel>(s => s
                               .Index(index_hotel)
                               .From(skip) // Skip records for pagination
                                .Size(take) // Take only the required records
                                .Sort(ss => ss
                                    .Descending(f => f.id) // Sorting by CreatedDate in descending order
                                )
                               .Query(q => q
                                   .Bool(b => b
                                       .Should(
                                           sh => sh.Match(m => m.Field(f => f.state).Query(name)),  // Match state
                                           sh => sh.Match(m => m.Field(f => f.city).Query(name))    // Match city
                                       )
                                       .MinimumShouldMatch(1) // Ensures at least one condition matches
                                   )
                               )
                           );
                }

                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
                    result = result.Where(x => x.isdisplaywebsite == true).ToList();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<HotelESViewModel>> GetByIds(List<int> hotel_ids)
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("0");
                var elasticClient = new ElasticClient(connectionSettings);

                ISearchResponse<HotelESViewModel> search_response = elasticClient.Search<HotelESViewModel>(s => s
                                                   .Index(index_hotel)
                                                    .Size(4000)
                                                    .Query(q => q
                                                        .Terms(t => t
                                                            .Field(f => f.hotelid)
                                                            .Terms(hotel_ids)
                                                        )
                                                    ));

                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
