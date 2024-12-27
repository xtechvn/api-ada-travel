using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class HotelBookingRoomExtraPackagesDAL : GenericService<HotelBookingRoomExtraPackages>
    {
        private DbWorker dbWorker;
        public HotelBookingRoomExtraPackagesDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);
        }
      
        public async Task<List<HotelBookingRoomExtraPackages>> GetByBookingId(long hotel_booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingRoomExtraPackages.AsNoTracking().Where(x => x.HotelBookingId==hotel_booking_id).ToListAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByBookingId - HotelBookingRoomExtraPackagesDAL. " + ex);
                return null;
            }
        }
        public async Task<HotelBookingRoomExtraPackages> GetByIDAndBookingId(long id,long hotel_booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingRoomExtraPackages.AsNoTracking().Where(x => x.Id==id && x.HotelBookingId == hotel_booking_id).FirstOrDefaultAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByIDAndBookingId - HotelBookingRoomExtraPackagesDAL. " + ex);
                return null;
            }
        }
        public async Task<DataTable> Gethotelbookingroomextrapackagebyhotelbookingid(long HotelBookingId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingID", HotelBookingId);

                return dbWorker.GetDataTable(StoreProceduresName.sp_gethotelbookingroomextrapackagebyhotelbookingid, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Gethotelbookingroomextrapackagebyhotelbookingid - HotelBookingRoomExtraPackagesDAL: " + ex);
            }
            return null;
        }
        public int CreateHotelBookingRoomExtraPackages(HotelBookingRoomExtraPackages packages)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[17];
                objParam_order[0] = new SqlParameter("@PackageId", packages.PackageId);
                objParam_order[1] = new SqlParameter("@PackageCode", packages.PackageCode);
                objParam_order[2] = new SqlParameter("@HotelBookingId", packages.HotelBookingId);
                objParam_order[3] = new SqlParameter("@HotelBookingRoomID", packages.HotelBookingRoomId);
                objParam_order[4] = new SqlParameter("@Amount", packages.Amount);
                objParam_order[5] = new SqlParameter("@CreatedBy", packages.CreatedBy);
                objParam_order[6] = new SqlParameter("@CreatedDate", packages.CreatedDate);
                objParam_order[7] = new SqlParameter("@StartDate", packages.StartDate);
                objParam_order[8] = new SqlParameter("@EndDate", packages.EndDate);
                objParam_order[9] = new SqlParameter("@Profit", packages.Profit);
                objParam_order[10] = new SqlParameter("@PackageCompanyId", packages.PackageCompanyId);
                objParam_order[11] = new SqlParameter("@OperatorPrice", packages.OperatorPrice);
                objParam_order[12] = new SqlParameter("@SalePrice", packages.SalePrice);
                objParam_order[13] = new SqlParameter("@Nights", packages.Nights);
                objParam_order[14] = new SqlParameter("@Quantity", packages.Quantity);
                objParam_order[15] = new SqlParameter("@UnitPrice", packages.UnitPrice);
                if (packages.SupplierId != null)
                {
                    objParam_order[16] = new SqlParameter("@SupplierId", packages.SupplierId);
                }
                else
                {
                    objParam_order[16] = new SqlParameter("@SupplierId", DBNull.Value);
                }
                var id = dbWorker.ExecuteNonQuery(StoreProceduresName.InsertHotelBookingRoomExtraPackages, objParam_order);
                packages.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelBookingRoomExtraPackages - HotelBookingDAL. " + ex);
                return -1;
            }
        }
        public int UpdateHotelBookingExtraPackagesSP(HotelBookingRoomExtraPackages packages)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[17];
                objParam_order[0] = new SqlParameter("@PackageId", packages.PackageId);
                objParam_order[1] = new SqlParameter("@PackageCode", packages.PackageCode);
                objParam_order[2] = new SqlParameter("@HotelBookingId", packages.HotelBookingId);
                objParam_order[3] = new SqlParameter("@HotelBookingRoomID", packages.HotelBookingRoomId);
                objParam_order[4] = new SqlParameter("@Amount", packages.Amount);
                objParam_order[5] = new SqlParameter("@UnitPrice", packages.UnitPrice);
                objParam_order[6] = new SqlParameter("@UpdatedBy", packages.UpdatedBy);
                objParam_order[7] = new SqlParameter("@StartDate", packages.StartDate);
                objParam_order[8] = new SqlParameter("@EndDate", packages.EndDate);
                objParam_order[9] = new SqlParameter("@Profit", packages.Profit);
                objParam_order[10] = new SqlParameter("@PackageCompanyId", packages.PackageCompanyId);
                objParam_order[11] = new SqlParameter("@OperatorPrice", packages.OperatorPrice);
                objParam_order[12] = new SqlParameter("@SalePrice", packages.SalePrice);
                objParam_order[13] = new SqlParameter("@Nights", packages.Nights);
                objParam_order[14] = new SqlParameter("@Quantity", packages.Quantity);
                objParam_order[15] = new SqlParameter("@Id", packages.Id);
                if (packages.SupplierId != null)
                {
                    objParam_order[16] = new SqlParameter("@SupplierId", packages.SupplierId);
                }
                else
                {
                    objParam_order[16] = new SqlParameter("@SupplierId", DBNull.Value);
                }

                var id = dbWorker.ExecuteNonQuery(StoreProceduresName.SP_UpdateHotelBookingRoomExtraPackages, objParam_order);
                packages.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelBookingRoomExtraPackages - HotelBookingDAL. " + ex);
                return -1;
            }
        }
    }
}
