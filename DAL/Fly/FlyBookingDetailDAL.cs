using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.BookingFly;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL.Fly
{
    public class FlyBookingDetailDAL : GenericService<FlyBookingDetail>
    {
        private static DbWorker _DbWorker;
        public FlyBookingDetailDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public FlyBookingDetail GetDetail(long orderId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.FlyBookingDetail.AsNoTracking().FirstOrDefault(s => s.OrderId == orderId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - FlyBookingDetailDAL: " + ex);
                return null;
            }
        }

        public List<FlyBookingDetail> GetListByOrderId(long orderId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.OrderId == orderId).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - FlyBookingDetailDAL: " + ex);
                return null;
            }
        }
        public async Task<List<FlyBookingDetail>> GetFlyBookingById(long fly_booking_id)
        {
            try
            {

                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists_fly= await _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.Id == fly_booking_id).FirstOrDefaultAsync();
                    if(exists_fly!=null && exists_fly.Id > 0)
                    {
                        return  await _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.GroupBookingId == exists_fly.GroupBookingId).ToListAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFlyBookingById - FlyBookingDetailDAL: " + ex);
            }
            return null;
        }
        public async Task<DataTable> GetDetailFlyBookingDetailById(long FlyBookingDetailId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@FlyBookingDetailId", FlyBookingDetailId);

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetDetailFlyBookingDetailById, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailOrderServiceByOrderId - FlyBookingDetailDAL: " + ex);
            }
            return null;
        }
        public static int CreateFlyBookingDetail(FlyBookingDetailViewModel model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[53];
                objParam_order[0] = new SqlParameter("@OrderId", model.OrderId);
                objParam_order[1] = new SqlParameter("@PriceId", model.PriceId);
                objParam_order[2] = new SqlParameter("@BookingCode", model.BookingCode);
                objParam_order[3] = new SqlParameter("@Amount", model.Amount);
                objParam_order[4] = new SqlParameter("@Difference", model.Difference);
                objParam_order[5] = new SqlParameter("@Currency", model.Currency);
                objParam_order[6] = new SqlParameter("@Flight", model.Flight);
                objParam_order[7] = new SqlParameter("@ExpiryDate", model.ExpiryDate);
                objParam_order[8] = new SqlParameter("@Session", model.Session);
                objParam_order[9] = new SqlParameter("@Airline", model.Airline);
                objParam_order[10] = new SqlParameter("@StartPoint", model.StartPoint);
                objParam_order[11] = new SqlParameter("@EndPoint", model.EndPoint);
                objParam_order[12] = new SqlParameter("@GroupClass", model.GroupClass);
                objParam_order[13] = new SqlParameter("@Leg", model.Leg);
                objParam_order[14] = new SqlParameter("@AdultNumber", model.AdultNumber);
                objParam_order[15] = new SqlParameter("@ChildNumber", model.ChildNumber);
                objParam_order[16] = new SqlParameter("@InfantNumber", model.InfantNumber);
                objParam_order[17] = new SqlParameter("@FareAdt", model.FareAdt);
                objParam_order[18] = new SqlParameter("@FareChd", model.FareChd);
                objParam_order[19] = new SqlParameter("@FareInf", model.FareInf);
                objParam_order[20] = new SqlParameter("@TaxAdt", model.TaxAdt);
                objParam_order[21] = new SqlParameter("@TaxChd", model.TaxChd);
                objParam_order[22] = new SqlParameter("@TaxInf", model.TaxInf);
                objParam_order[23] = new SqlParameter("@FeeAdt", model.FeeAdt);
                objParam_order[24] = new SqlParameter("@FeeChd", model.FeeChd);
                objParam_order[25] = new SqlParameter("@FeeInf", model.FeeInf);
                objParam_order[26] = new SqlParameter("@ServiceFeeAdt", model.ServiceFeeAdt);
                objParam_order[27] = new SqlParameter("@ServiceFeeChd", model.ServiceFeeChd);
                objParam_order[28] = new SqlParameter("@ServiceFeeInf", model.ServiceFeeInf);
                objParam_order[29] = new SqlParameter("@AmountAdt", model.AmountAdt);
                objParam_order[30] = new SqlParameter("@AmountChd", model.AmountChd);
                objParam_order[31] = new SqlParameter("@AmountInf", model.AmountInf);
                objParam_order[32] = new SqlParameter("@TotalNetPrice", model.TotalNetPrice);
                objParam_order[33] = new SqlParameter("@TotalDiscount", model.TotalDiscount);
                objParam_order[34] = new SqlParameter("@TotalCommission", model.TotalCommission);
                objParam_order[35] = new SqlParameter("@TotalBaggageFee", model.TotalBaggageFee);
                objParam_order[36] = new SqlParameter("@StartDate", model.StartDate);
                objParam_order[37] = new SqlParameter("@EndDate", model.EndDate);
                objParam_order[38] = new SqlParameter("@BookingId", model.BookingId);
                objParam_order[39] = new SqlParameter("@Status", model.Status);
                objParam_order[40] = new SqlParameter("@Profit", model.Profit);
                if (model.ServiceCode != null)
                {
                    objParam_order[41] = new SqlParameter("@ServiceCode", model.ServiceCode);
                }
                else
                {
                    objParam_order[41] = new SqlParameter("@ServiceCode", DBNull.Value);

                }
                objParam_order[42] = new SqlParameter("@Price", model.Price);
                objParam_order[43] = new SqlParameter("@PriceAdt", model.PriceAdt);
                objParam_order[44] = new SqlParameter("@PriceChd", model.PriceChd);
                objParam_order[45] = new SqlParameter("@PriceInf", model.PriceInf);
                if (model.SupplierId != null)
                {
                    objParam_order[46] = new SqlParameter("@SupplierId", model.SupplierId);
                }
                else
                {
                    objParam_order[46] = new SqlParameter("@SupplierId", DBNull.Value);

                }
                if (model.Note != null)
                {
                    objParam_order[47] = new SqlParameter("@Note", model.Note);
                }
                else
                {
                    objParam_order[47] = new SqlParameter("@Note", DBNull.Value);

                }
                objParam_order[48] = new SqlParameter("@ProfitAdt", model.ProfitAdt);
                objParam_order[49] = new SqlParameter("@ProfitChd", model.ProfitChd);
                objParam_order[50] = new SqlParameter("@ProfitInf", model.ProfitInf);
                objParam_order[51] = new SqlParameter("@AdgCommission", model.Adgcommission);
                objParam_order[52] = new SqlParameter("@OthersAmount", model.OthersAmount);
                var id = _DbWorker.ExecuteNonQuery(StoreProceduresName.CreateFlyBookingDetail, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateFlyBookingDetail - FlyBookingDetailDAL: " + ex);
                return -1;
            }
        }
        public static int UpdateFlyBookingDetail(FlyBookingDetailViewModel model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[57];
                objParam_order[0] = new SqlParameter("@OrderId", model.OrderId);
                objParam_order[1] = new SqlParameter("@PriceId", model.PriceId);
                objParam_order[2] = new SqlParameter("@BookingCode", model.BookingCode);
                objParam_order[3] = new SqlParameter("@Amount", model.Amount);
                objParam_order[4] = new SqlParameter("@Difference", model.Difference);
                objParam_order[5] = new SqlParameter("@Currency", model.Currency);
                objParam_order[6] = new SqlParameter("@Flight", model.Flight);
                objParam_order[7] = new SqlParameter("@ExpiryDate", model.ExpiryDate);
                objParam_order[8] = new SqlParameter("@Session", model.Session);
                objParam_order[9] = new SqlParameter("@Airline", model.Airline);
                objParam_order[10] = new SqlParameter("@StartPoint", model.StartPoint);
                objParam_order[11] = new SqlParameter("@EndPoint", model.EndPoint);
                objParam_order[12] = new SqlParameter("@GroupClass", model.GroupClass);
                objParam_order[13] = new SqlParameter("@Leg", model.Leg);
                objParam_order[14] = new SqlParameter("@AdultNumber", model.AdultNumber);
                objParam_order[15] = new SqlParameter("@ChildNumber", model.ChildNumber);
                objParam_order[16] = new SqlParameter("@InfantNumber", model.InfantNumber);
                objParam_order[17] = new SqlParameter("@FareAdt", model.FareAdt);
                objParam_order[18] = new SqlParameter("@FareChd", model.FareChd);
                objParam_order[19] = new SqlParameter("@FareInf", model.FareInf);
                objParam_order[20] = new SqlParameter("@TaxAdt", model.TaxAdt);
                objParam_order[21] = new SqlParameter("@TaxChd", model.TaxChd);
                objParam_order[22] = new SqlParameter("@TaxInf", model.TaxInf);
                objParam_order[23] = new SqlParameter("@FeeAdt", model.FeeAdt);
                objParam_order[24] = new SqlParameter("@FeeChd", model.FeeChd);
                objParam_order[25] = new SqlParameter("@FeeInf", model.FeeInf);
                objParam_order[26] = new SqlParameter("@ServiceFeeAdt", model.ServiceFeeAdt);
                objParam_order[27] = new SqlParameter("@ServiceFeeChd", model.ServiceFeeChd);
                objParam_order[28] = new SqlParameter("@ServiceFeeInf", model.ServiceFeeInf);
                objParam_order[29] = new SqlParameter("@AmountAdt", model.AmountAdt);
                objParam_order[30] = new SqlParameter("@AmountChd", model.AmountChd);
                objParam_order[31] = new SqlParameter("@AmountInf", model.AmountInf);
                objParam_order[32] = new SqlParameter("@TotalNetPrice", model.TotalNetPrice);
                objParam_order[33] = new SqlParameter("@TotalDiscount", model.TotalDiscount);
                objParam_order[34] = new SqlParameter("@TotalCommission", model.TotalCommission);
                objParam_order[35] = new SqlParameter("@TotalBaggageFee", model.TotalBaggageFee);
                objParam_order[36] = new SqlParameter("@StartDate", model.StartDate);
                objParam_order[37] = new SqlParameter("@EndDate", model.EndDate);
                objParam_order[38] = new SqlParameter("@BookingId", model.BookingId);
                objParam_order[39] = new SqlParameter("@Status", model.Status);
                objParam_order[40] = new SqlParameter("@Profit", model.Profit);
                if (model.ServiceCode != null)
                {
                    objParam_order[41] = new SqlParameter("@ServiceCode", model.ServiceCode);
                }
                else
                {
                    objParam_order[41] = new SqlParameter("@ServiceCode", DBNull.Value);

                }
                objParam_order[42] = new SqlParameter("@Id", model.Id);
                objParam_order[43] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                if (model.SalerId != null)
                {
                    objParam_order[44] = new SqlParameter("@SalerId", model.SalerId);
                }
                else
                {
                    objParam_order[44] = new SqlParameter("@SalerId", DBNull.Value);

                }
                if (model.GroupBookingId != null)
                {
                    objParam_order[45] = new SqlParameter("@GroupBookingId", model.GroupBookingId);
                }
                else
                {
                    objParam_order[45] = new SqlParameter("@GroupBookingId", DBNull.Value);

                }
                objParam_order[46] = new SqlParameter("@Price", model.Price);
                objParam_order[47] = new SqlParameter("@PriceAdt", model.PriceAdt);
                if (model.PriceChd != null)
                {
                    objParam_order[48] = new SqlParameter("@PriceChd", model.PriceChd);
                }
                else
                {
                    objParam_order[48] = new SqlParameter("@PriceChd", DBNull.Value);

                }
                if (model.PriceInf != null)
                {
                    objParam_order[49] = new SqlParameter("@PriceInf", model.PriceInf);
                }
                else
                {
                    objParam_order[49] = new SqlParameter("@PriceInf", DBNull.Value);

                }
                if (model.SupplierId != null)
                {
                    objParam_order[50] = new SqlParameter("@SupplierId", model.SupplierId);
                }
                else
                {
                    objParam_order[50] = new SqlParameter("@SupplierId", DBNull.Value);

                }
                if (model.Note != null)
                {
                    objParam_order[51] = new SqlParameter("@Note", model.Note);
                }
                else
                {
                    objParam_order[51] = new SqlParameter("@Note", DBNull.Value);

                }
                objParam_order[52] = new SqlParameter("@ProfitAdt", model.ProfitAdt);
                objParam_order[53] = new SqlParameter("@ProfitChd", model.ProfitChd);
                objParam_order[54] = new SqlParameter("@ProfitInf", model.ProfitInf);
                objParam_order[55] = new SqlParameter("@AdgCommission", model.Adgcommission);
                objParam_order[56] = new SqlParameter("@OthersAmount", model.OthersAmount);
                var id = _DbWorker.ExecuteNonQuery("SP_UpdateFlyBookingDetail", objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateFlyBookingDetail - FlyBookingDetailDAL: " + ex);
                return -1;
            }
        }

    }
}
