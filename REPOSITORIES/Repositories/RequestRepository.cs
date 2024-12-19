using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels;
using ENTITIES.ViewModels.Request;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly RequestDAL _requestDAL;

        public RequestRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _requestDAL = new RequestDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<int> InsertRequest(Request Model)
        {
            try
            {
                return await _requestDAL.InsertRequest(Model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertRequest - RequestRepository: " + ex);
            }
            return -1;
        }
        public async Task<List<RequestViewModel>> GetPagingList(RequestSearchModel searchModel)
        {
            try
            {
                DataTable dt= await _requestDAL.GetPagingList(searchModel);
                if(dt!=null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<RequestViewModel>();
                    return data;
                }
                
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertRequest - RequestRepository: " + ex);
            }
            return null;
        }
        public async Task<Request> GetDetailByBookingId(long BookingId)
        {
            try
            {
               return await _requestDAL.GetDetailByBookingId(BookingId);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertRequest - RequestRepository: " + ex);
            }
            return null;
        }   
        public async Task<long> UpdateRequest(Request model)
        {
            try
            {
               return await _requestDAL.UpdateRequest(model);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertRequest - RequestRepository: " + ex);
            }
            return -1;
        }
        public async Task<RequestDetailModel> GetDetailRequestByRequestId(long RequestId)
        {

            try
            {
                return await _requestDAL.GetDetailRequestByRequestId(RequestId);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - RequestRepository: " + ex);
            }
            return null;
        }
    }
}
