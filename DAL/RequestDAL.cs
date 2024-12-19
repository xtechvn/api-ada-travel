using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels;
using ENTITIES.ViewModels.Request;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
   public class RequestDAL : GenericService<Request>
    {
        private static DbWorker _DbWorker;
        public RequestDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<int> InsertRequest(Request Model)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[18];
                objParam[0] = new SqlParameter("@RoomTypeId", Model.RoomTypeId);
                objParam[1] = new SqlParameter("@PackageId", Model.PackageId);
                objParam[2] = new SqlParameter("@HotelId", Model.HotelId);
                objParam[3] = new SqlParameter("@FromDate", Model.FromDate);
                objParam[4] = new SqlParameter("@ToDate", Model.ToDate);
                objParam[5] = new SqlParameter("@Price", Model.Price);
                objParam[6] = new SqlParameter("@Note", Model.Note);
                objParam[7] = new SqlParameter("@Status", Model.Status);
                objParam[8] = new SqlParameter("@SalerId", Model.SalerId);
                objParam[9] = new SqlParameter("@OrderId", Model.OrderId);
                objParam[10] = new SqlParameter("@CreatedBy", Model.CreatedBy);
                objParam[11] = new SqlParameter("@BookingId", Model.BookingId);
                objParam[12] = new SqlParameter("@ClientId", Model.ClientId);
                objParam[13] = new SqlParameter("@RequestNo", Model.RequestNo);
                if(Model.VoucherId!=null && Model.VoucherId > 0)
                {
                    objParam[14] = new SqlParameter("@VoucherId", Model.VoucherId);
                }
                else
                {
                    objParam[14] = new SqlParameter("@VoucherId", DBNull.Value);
                }
                if (Model.VoucherName != null && Model.VoucherName.Trim()!="")
                {
                    objParam[15] = new SqlParameter("@VoucherName", Model.VoucherName);
                }
                else
                {
                    objParam[15] = new SqlParameter("@VoucherName", DBNull.Value);
                }
                if (Model.Amount != null && Model.Amount > 0)
                {
                    objParam[16] = new SqlParameter("@Amount", Model.Amount);
                }
                else
                {
                    objParam[16] = new SqlParameter("@Amount", DBNull.Value);
                }
                if (Model.Discount != null && Model.Discount > 0)
                {
                    objParam[17] = new SqlParameter("@Discount", Model.Discount);
                }
                else
                {
                    objParam[17] = new SqlParameter("@Discount", DBNull.Value);
                }

                return  _DbWorker.ExecuteNonQuery(StoreProceduresName.sp_InsertRequest, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertRequest - RequestDAL: " + ex);
            }
            return -1;
        }
        public async Task<DataTable> GetPagingList(RequestSearchModel searchModel)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[7];
                objParam[0] = new SqlParameter("@RequestId", searchModel.RequestId);
                objParam[1] = new SqlParameter("@CreateDateFrom", DBNull.Value);
                objParam[2] = new SqlParameter("@CreateDateTo", DBNull.Value);
                objParam[3] = new SqlParameter("@SalerId", searchModel.SalerId);
                objParam[4] = new SqlParameter("@ClientId", searchModel.ClientId);
                objParam[5] = new SqlParameter("@PageIndex", searchModel.PageIndex);
                objParam[6] = new SqlParameter("@PageSize", searchModel.PageSize);

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetListRequest, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - RequestDAL: " + ex);
            }
            return null;
        }
        public async Task<Request> GetDetailByBookingId(long BookingId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Request.Where(s => s.BookingId== BookingId).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - RequestDAL: " + ex);
            }
            return null;
        }
        public async Task<long>UpdateRequest(Request Model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[16];
                objParam[0] = new SqlParameter("@RequestId", Model.RequestId);
                if (Model.RoomTypeId != null && Model.RoomTypeId > 0)
                {
                    objParam[1] = new SqlParameter("@RoomTypeId", Model.RoomTypeId);
                }
                else
                {
                    objParam[1] = new SqlParameter("@RoomTypeId", DBNull.Value);
                }
                if (Model.PackageId != null && Model.PackageId > 0)
                {
                    objParam[2] = new SqlParameter("@PackageId", Model.PackageId);
                }
                else
                {
                    objParam[2] = new SqlParameter("@PackageId", DBNull.Value);
                }
                if (Model.HotelId != null)
                {
                    objParam[3] = new SqlParameter("@HotelId", Model.HotelId);
                }
                else
                {
                    objParam[3] = new SqlParameter("@HotelId", DBNull.Value);
                }
                if (Model.FromDate != null )
                {
                    objParam[4] = new SqlParameter("@FromDate", Model.FromDate);
                }
                else
                {
                    objParam[4] = new SqlParameter("@FromDate", DBNull.Value);
                }
                if (Model.ToDate != null)
                {
                    objParam[5] = new SqlParameter("@ToDate", Model.ToDate);
                }
                else
                {
                    objParam[5] = new SqlParameter("@ToDate", DBNull.Value);
                }
                if (Model.Price != null)
                {
                    objParam[6] = new SqlParameter("@Price", Model.Price);
                }
                else
                {
                    objParam[6] = new SqlParameter("@Price", DBNull.Value);
                }
                if (Model.Note != null)
                {
                    objParam[7] = new SqlParameter("@Note", Model.Note);
                }
                else
                {
                    objParam[7] = new SqlParameter("@Note", DBNull.Value);
                }
                if (Model.Status != null)
                {
                    objParam[8] = new SqlParameter("@Status", Model.Status);
                }
                else
                {
                    objParam[8] = new SqlParameter("@Status", DBNull.Value);
                }
                if (Model.SalerId != null)
                {
                    objParam[9] = new SqlParameter("@SalerId", Model.SalerId);
                }
                else
                {
                    objParam[9] = new SqlParameter("@SalerId", DBNull.Value);
                }
                if (Model.UpdatedBy != null)
                {
                    objParam[10] = new SqlParameter("@UpdatedBy", Model.UpdatedBy);
                }
                else
                {
                    objParam[10] = new SqlParameter("@UpdatedBy", DBNull.Value);
                }
                //if (Model.BookingId != null)
                //{
                //    objParam[11] = new SqlParameter("@BookingId", Model.BookingId);
                //}
                //else
                //{
                //    objParam[11] = new SqlParameter("@BookingId", DBNull.Value);
                //}
                //if (Model.ClientId != null)
                //{
                //    objParam[12] = new SqlParameter("@ClientId", Model.ClientId);
                //}
                //else
                //{
                //    objParam[12] = new SqlParameter("@ClientId", DBNull.Value);
                //}
                //if (Model.RequestNo != null)
                //{
                //    objParam[13] = new SqlParameter("@RequestNo", Model.RequestNo);
                //}
                //else
                //{
                //    objParam[13] = new SqlParameter("@RequestNo", DBNull.Value);
                //}
                if (Model.Discount != null && Model.Discount > 0)
                {
                    objParam[12] = new SqlParameter("@Discount", Model.Discount);
                }
                else
                {
                    objParam[12] = new SqlParameter("@Discount", DBNull.Value);
                }
                objParam[13] = new SqlParameter("@OrderId", Model.OrderId);
                if (Model.VoucherId != null && Model.VoucherId > 0)
                {
                    objParam[14] = new SqlParameter("@VoucherId", Model.VoucherId);
                }
                else
                {
                    objParam[14] = new SqlParameter("@VoucherId", DBNull.Value);
                }
                if (Model.VoucherName != null && Model.VoucherName.Trim() != "")
                {
                    objParam[15] = new SqlParameter("@VoucherName", Model.VoucherName);
                }
                else
                {
                    objParam[15] = new SqlParameter("@VoucherName", DBNull.Value);
                }
                if (Model.Amount != null && Model.Amount > 0)
                {
                    objParam[11] = new SqlParameter("@Amount", Model.Amount);
                }
                else
                {
                    objParam[11] = new SqlParameter("@Amount", DBNull.Value);
                }
                

                return _DbWorker.ExecuteNonQuery(StoreProceduresName.sp_UpdateRequest, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - RequestDAL: " + ex);
            }
            return -1;
        }
        public async Task<RequestDetailModel> GetDetailRequestByRequestId(long RequestId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@RequestId", RequestId);

                DataTable dt= _DbWorker.GetDataTable(StoreProceduresName.sp_GetDetailRequest, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<RequestDetailModel>();
                    return data[0];
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - RequestDAL: " + ex);
            }
            return null;
        }
    }
}
