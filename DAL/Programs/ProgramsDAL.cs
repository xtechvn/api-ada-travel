using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL.Programs
{
    public class ProgramsDAL : GenericService<ENTITIES.Models.Programs>
    {
        private static DbWorker _DbWorker;
        public ProgramsDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
     
        public List<ENTITIES.Models.Programs> GetProgramsbyProgramId(long id,int PageIndex, int PageSize)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var Programs = _DbContext.Programs.AsNoTracking().Where(s => s.Id == id).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                    if (Programs != null)
                    {
                        return Programs;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProgramsbyProgramId - ProgramsDAL: " + ex.ToString());
                return null;
            }
        }  
        public ENTITIES.Models.Programs GetProgramByCode(string program_code, int hotel_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var Programs = _DbContext.Programs.AsNoTracking().FirstOrDefault(x=>x.HotelId==hotel_id && x.ProgramCode==program_code);
                    if (Programs != null)
                    {
                        return Programs;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProgramByCode - ProgramsDAL: " + ex.ToString());
                return null;
            }
        }
        public ENTITIES.Models.ProgramPackage GetProgramPackagesByCode(string package_code, long program_id,int room_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var Programs = _DbContext.ProgramPackage.AsNoTracking().FirstOrDefault(x => x.ProgramId == program_id && x.PackageName == package_code&& x.RoomTypeId == room_id);
                    if (Programs != null)
                    {
                        return Programs;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProgramByCode - ProgramsDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<int> InsertPrograms(ENTITIES.Models.Programs Model)
        {
            try
            {
                System.Data.SqlClient.SqlParameter[] objParam = new System.Data.SqlClient.SqlParameter[16];
                objParam[0] = new System.Data.SqlClient.SqlParameter("@ProgramCode", Model.ProgramCode);
                objParam[1] = new System.Data.SqlClient.SqlParameter("@ProgramName", Model.ProgramName);
                objParam[2] = new System.Data.SqlClient.SqlParameter("@SupplierId", Model.SupplierId);
                objParam[3] = new System.Data.SqlClient.SqlParameter("@ServiceType", Model.ServiceType);
                objParam[4] = new System.Data.SqlClient.SqlParameter("@ServiceName", Model.ServiceName);
                objParam[5] = new System.Data.SqlClient.SqlParameter("@StartDate", Model.StartDate);
                objParam[6] = new System.Data.SqlClient.SqlParameter("@EndDate", Model.EndDate);
                objParam[7] =  new System.Data.SqlClient.SqlParameter("@Description", Model.Description);
                objParam[8] = new System.Data.SqlClient.SqlParameter("@Status", Model.Status);
                objParam[9] = new System.Data.SqlClient.SqlParameter("@UserVerify", DBNull.Value);
                objParam[10] = new System.Data.SqlClient.SqlParameter("@VerifyDate", DBNull.Value);
                objParam[11] = new System.Data.SqlClient.SqlParameter("@CreatedBy", Model.CreatedBy);
                objParam[12] = new System.Data.SqlClient.SqlParameter("@CreatedDate", DBNull.Value);
                objParam[13] = new System.Data.SqlClient.SqlParameter("@HotelId", Model.HotelId);
                objParam[14] = new System.Data.SqlClient.SqlParameter("@StayStartDate", Model.StartDate);
                objParam[15] = new System.Data.SqlClient.SqlParameter("@StayEndDate", Model.EndDate);
                return _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_InsertPrograms, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertPrograms - ProgramsDAL: " + ex);
                return 0;
            }

        }
        public async Task<int> UpdatePrograms(ENTITIES.Models.Programs Model)
        {
            try
            {
                System.Data.SqlClient.SqlParameter[] objParam = new System.Data.SqlClient.SqlParameter[16];
                objParam[0] = new System.Data.SqlClient.SqlParameter("@Id", Model.Id);
                objParam[1] = new System.Data.SqlClient.SqlParameter("@ProgramCode", Model.ProgramCode);
                objParam[2] = new System.Data.SqlClient.SqlParameter("@ProgramName", Model.ProgramName);
                objParam[3] = new System.Data.SqlClient.SqlParameter("@SupplierId", Model.SupplierId);
                objParam[4] = new System.Data.SqlClient.SqlParameter("@ServiceType", Model.ServiceType);
                objParam[5] = new System.Data.SqlClient.SqlParameter("@ServiceName", Model.ServiceName);
                objParam[6] = new System.Data.SqlClient.SqlParameter("@StartDate", Model.StartDate);
                objParam[7] = new System.Data.SqlClient.SqlParameter("@EndDate", Model.EndDate);
                objParam[8] =  new System.Data.SqlClient.SqlParameter("@Description", Model.Description);
                objParam[9] = new System.Data.SqlClient.SqlParameter("@Status", Model.Status);
                objParam[10] = new System.Data.SqlClient.SqlParameter("@UserVerify", Model.UserVerify);
                objParam[11] = new System.Data.SqlClient.SqlParameter("@VerifyDate", DateTime.Now);
                objParam[12] = new System.Data.SqlClient.SqlParameter("@UpdatedBy", Model.UpdatedBy);
                objParam[13] = new System.Data.SqlClient.SqlParameter("@HotelId", Model.HotelId);
                objParam[14] = new System.Data.SqlClient.SqlParameter("@StayStartDate", Model.StartDate);
                objParam[15] = new System.Data.SqlClient.SqlParameter("@StayEndDate", Model.EndDate);
                return _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_UpdatePrograms, objParam);
            }
            catch (Exception ex)
            {
                return 0;
                LogHelper.InsertLogTelegram("UpdatePrograms - ProgramsDAL: " + ex);
            }

        }

        public async Task<int> InsertProgramPackage(ProgramPackage Model)
        {
            try
            {
                //Model.ApplyDate = GetDate(Model.FromDate, Model.ToDate, Model.WeekDay);
                System.Data.SqlClient.SqlParameter[] objParam = new System.Data.SqlClient.SqlParameter[15];
                objParam[0] = new System.Data.SqlClient.SqlParameter("@PackageCode", Model.PackageCode);
                objParam[1] = new System.Data.SqlClient.SqlParameter("@ProgramId", Model.ProgramId);
                objParam[2] = new System.Data.SqlClient.SqlParameter("@RoomType", Model.RoomType);
                objParam[3] = new System.Data.SqlClient.SqlParameter("@Amount", Model.Amount);
                objParam[4] = new System.Data.SqlClient.SqlParameter("@FromDate", Model.FromDate);
                objParam[5] = new System.Data.SqlClient.SqlParameter("@ToDate", Model.ToDate);
                objParam[6] = new System.Data.SqlClient.SqlParameter("@WeekDay", Model.WeekDay);
                objParam[7] = Model.ApplyDate == null ? new System.Data.SqlClient.SqlParameter("@ApplyDate", DBNull.Value) : new System.Data.SqlClient.SqlParameter("@ApplyDate", Model.ApplyDate);
                objParam[8] = new System.Data.SqlClient.SqlParameter("@OpenStatus", Model.OpenStatus);
                objParam[9] = new System.Data.SqlClient.SqlParameter("@CreatedDate", DBNull.Value);
                objParam[10] = new System.Data.SqlClient.SqlParameter("@CreatedBy", Model.CreatedBy);
                objParam[11] = new System.Data.SqlClient.SqlParameter("@RoomTypeId", Model.RoomTypeId);
                objParam[12] = new System.Data.SqlClient.SqlParameter("@PackageName", Model.PackageName);
                objParam[13] = new System.Data.SqlClient.SqlParameter("@Price", Model.Price);
                objParam[14] = new System.Data.SqlClient.SqlParameter("@Profit", Model.Profit);
                return _DbWorker.ExecuteNonQuery(StoreProceduresName.sp_InsertProgramPackage, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertProgramPackage - ProgramsPackageDAL: " + ex);
                return 0;
            }

        }
        public async Task<int> UpdateProgramPackage(ProgramPackage Model)
        {
            try
            {
               
                System.Data.SqlClient.SqlParameter[] objParam = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@Id", Model.Id),
                    new System.Data.SqlClient.SqlParameter("@PackageCode", Model.PackageCode),
                    new System.Data.SqlClient.SqlParameter("@PackageName", Model.PackageName),
                    new System.Data.SqlClient.SqlParameter("@ProgramId", Model.ProgramId),
                    new System.Data.SqlClient.SqlParameter("@RoomType", Model.RoomType),
                    new System.Data.SqlClient.SqlParameter("@Amount", Model.Amount),
                    new System.Data.SqlClient.SqlParameter("@FromDate", Model.FromDate),
                    new System.Data.SqlClient.SqlParameter("@ToDate", Model.ToDate),
                    new System.Data.SqlClient.SqlParameter("@WeekDay", Model.WeekDay),
                    new System.Data.SqlClient.SqlParameter("@ApplyDate", Model.ApplyDate),
                    new System.Data.SqlClient.SqlParameter("@OpenStatus", Model.OpenStatus),
                    new System.Data.SqlClient.SqlParameter("@UpdatedBy", Model.UpdatedBy),
                    new System.Data.SqlClient.SqlParameter("@RoomTypeId", Model.RoomTypeId),
                    new System.Data.SqlClient.SqlParameter("@Price", Model.Price),
                    new System.Data.SqlClient.SqlParameter("@Profit", Model.Profit),

                };
                return _DbWorker.ExecuteNonQuery(StoreProceduresName.sp_UpdateProgramPackage, objParam);

                //using (var _DbContext = new EntityDataContext(_connection))
                //{
                //    var Programs = _DbContext.ProgramPackage.Update(Model);
                //    await _DbContext.SaveChangesAsync();
                //    return Model.Id;
                //}
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateProgramPackage - ProgramsPackageDAL: " + ex);
                return 0;
            }

        }
        public async Task<DataTable> GetListProgramsPackageExpired()
        {
            try
            {
                System.Data.SqlClient.SqlParameter[] objParam = new System.Data.SqlClient.SqlParameter[0];
                

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetListProgramsPackageExpired, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListProgramsPackageExpired - ProgramsDAL: " + ex);
                return null;
            }

        }
    }
}
