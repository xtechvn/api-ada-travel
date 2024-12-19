using Caching.Elasticsearch;
using Elasticsearch.Net;
using ENTITIES.ViewModels.Elasticsearch;
using ENTITIES.ViewModels.Tour;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using static Utilities.Contants.CommonConstant;

namespace CACHING.Elasticsearch
{
    public class TourESRepository : ESRepository<TourESViewModel>
    {

        public TourESRepository(string Host) : base(Host) { }
        public async Task<List<TourESViewModel>> GetListNational(string txtsearch, string index_name = "adavigo_sp_getnational")
        {
            List<TourESViewModel> result = new List<TourESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("adavigo_sp_getnational");
                var elasticClient = new ElasticClient(connectionSettings);
                if (txtsearch == null) txtsearch = "";
                var search_response = elasticClient.Search<TourESViewModel>(s => s
                          .Index(index_name)
                          .Size(top)
                          .Query(q =>
                            q.Bool(
                                qb => qb.Should(
                                    sh => sh.QueryString(m => m
                                    .DefaultField(f => f.name)
                                    .Query("*" + txtsearch + "*")),
                                    sh => sh.QueryString(m => m
                                    .DefaultField(f => f.code)
                                    .Query("*" + txtsearch + "*"))
                                ))
                           )
                          );
                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<TourESViewModel>;
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<List<ListTourProductViewModel>> GetListTour(string startpoint, string endpoint, int type, string transport, int pageindex, int pagesize,bool isdisplayweb,bool isselfdesigned, string index_name = "adavigo_sp_gettour")
        {
            List<ListTourProductViewModel> result = new List<ListTourProductViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("adavigo_sp_gettour");
                var elasticClient = new ElasticClient(connectionSettings);
                string typefalse = "false";
                string typetrue = "true";

               
                if (startpoint != null && startpoint != "" && startpoint != "-1" && endpoint != "-1")
                {
                    var search_response = elasticClient.Search<ListTourProductViewModel>(s => s
                       .Index(index_name)
                       .From(pagesize * (pageindex - 1))
                       .Size(pagesize)
                       .Query(q =>
                         q.Bool(
                             qb => qb.Must(
                               sh => sh.MatchPhrase(m => m
                               .Field(f => f.location_key)
                               .Query("*" + startpoint + "_" + endpoint + "*")),

                                 sh => sh.Term("tourtype", type.ToString())
                                 && sh.Term("status", ((int)CommonStatus.INACTIVE).ToString())
                                 && sh.Term("isdelete", typefalse)
                                 && sh.Term("isdisplayweb", isdisplayweb.ToString())
                                 && sh.Term("isselfdesigned", isselfdesigned.ToString())
                                  && ((string.IsNullOrEmpty(transport)|| transport.Trim()=="") ? null: sh.Bool(b => b.Must(transport.Split(',').Select(filter => (Func<QueryContainerDescriptor<ListTourProductViewModel>, QueryContainer>)(q => q.Term(t => t.Field("transportation").Value(filter)))).ToArray())))
                               )
                             )
                        ));
                    if (!search_response.IsValid)
                    {
                        return result;
                    }
                    else
                    {
                        result = search_response.Documents as List<ListTourProductViewModel>;
                        return result;
                    }
                }
                if (startpoint != null && startpoint != "" && startpoint == "-1" && endpoint != "-1")
                {
                    var search_response = elasticClient.Search<ListTourProductViewModel>(s => s
                       .Index(index_name)
                        .From(pagesize * (pageindex - 1))
                       .Size(pagesize)
                       .Query(q =>
                         q.Bool(
                            qb => qb.Must(
                               sh => sh.QueryString(m => m
                               .DefaultField(f => f.location_key)
                               .Query("*_" + endpoint)),
                                sh => sh.Term("tourtype", type.ToString())
                                && sh.Term("status", ((int)CommonStatus.INACTIVE).ToString())
                                && sh.Term("isdelete", typefalse)
                                 && sh.Term("isdisplayweb", isdisplayweb.ToString())
                                 && sh.Term("isselfdesigned", isselfdesigned.ToString())

                             ))

                        )

                       );
                    if (!search_response.IsValid)
                    {
                        return result;
                    }
                    else
                    {
                        result = search_response.Documents as List<ListTourProductViewModel>;
                        return result;
                    }
                }
                if (startpoint == "-1" && endpoint == "-1")
                {
                    var search_response = elasticClient.Search<ListTourProductViewModel>(s => s
                       .Index(index_name)
                       .From(pagesize * (pageindex - 1))
                       .Size(pagesize)
                       .Query(q =>
                         q.Bool(
                            qb => qb.Must(
                               sh => sh.Term("tourtype", type.ToString())
                                  && sh.Term("status", ((int)CommonStatus.INACTIVE).ToString())
                                 && sh.Term("isdelete", typefalse)
                                   && sh.Term("isdisplayweb", isdisplayweb.ToString())
                                 && sh.Term("isselfdesigned", isselfdesigned.ToString())

                             ))
                        )
                        );
                    if (!search_response.IsValid)
                    {
                        return result;
                    }
                    else
                    {
                        result = search_response.Documents as List<ListTourProductViewModel>;
                        return result;
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTour - TourIESRepository - : " + ex);
            }
            return result;

        }

        public async Task<List<ListTourProductViewModel>> GetListTourId(string TourId, string index_name = "adavigo_sp_gettour")
        {
            List<ListTourProductViewModel> result = new List<ListTourProductViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("adavigo_sp_gettour");
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<ListTourProductViewModel>(s => s
                       .Index(index_name)
                       .Size(top)
                       .Query(q =>
                         q.Bool(
                             qb => qb.Must(
                               sh => sh.QueryString(m => m
                               .DefaultField(f => f.Id)
                               .Query(TourId)))
                             )

                        )

                       );
                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<ListTourProductViewModel>;
                    return result;
                }



            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTourId - TourIESRepository - : " + ex);
                return null;
            }

        }
        public async Task<List<ListTourProductViewModel>> GetListTour(int type, int pageindex, int pagesize, bool isdisplayweb, bool isselfdesigned, string index_name = "adavigo_sp_gettour")
        {
            List<ListTourProductViewModel> result = new List<ListTourProductViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("adavigo_sp_gettour");
                var elasticClient = new ElasticClient(connectionSettings);
                string typefalse = "false";
                string typetrue = "true";
                var search_response = elasticClient.Search<ListTourProductViewModel>(s => s
                        .Index(index_name)
                        .From(pagesize * (pageindex - 1))
                        .Size(pagesize)
                        .Query(q =>
                          q.Bool(
                             qb => qb.Must(
                                 sh => sh.Term("tourtype", type.ToString())
                                  && sh.Term("status", ((int)CommonStatus.INACTIVE).ToString())
                                  && sh.Term("isdelete", typefalse)
                                && sh.Term("isdisplayweb", typetrue)
                                  && sh.Term("isdisplayweb", isdisplayweb.ToString())
                                 && sh.Term("isselfdesigned", isselfdesigned.ToString())
                              ))

                         )
                         .Sort(sort => sort.Field(c => c.updateddate, SortOrder.Descending))
                        );
                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<ListTourProductViewModel>;
                    return result;
                }



            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTour - TourIESRepository - : " + ex);
                return null;
            }

        }
        public async Task<List<TourProductDetailModel>> GetTourDetaiId(int Id, string index_name = "adavigo_sp_gettour")
        {
            List<TourProductDetailModel> result = new List<TourProductDetailModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("adavigo_sp_gettour");
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<TourProductDetailModel>(s => s
                        .Index(index_name)
                        .Size(top)
                        .Query(q =>
                          q.Bool(
                             qb => qb.Must(
                                sh => sh.Match(m => m
                                .Field(f => f.Id)
                                .Query(Id.ToString()))
                              ))

                         )

                        );
                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<TourProductDetailModel>;
                    return result;
                }



            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourDetaiId - TourIESRepository - : " + ex);
                return null;
            }

        }
    }
}
