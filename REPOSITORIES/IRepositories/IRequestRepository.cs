using ENTITIES.Models;
using ENTITIES.ViewModels;
using ENTITIES.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IRequestRepository
    {
        Task<int> InsertRequest(Request Model); 
        Task<List<RequestViewModel>> GetPagingList(RequestSearchModel Model);
        Task<Request> GetDetailByBookingId(long BookingId);
        Task<long> UpdateRequest(Request model);
        Task<RequestDetailModel> GetDetailRequestByRequestId(long RequestId);
    }
}
