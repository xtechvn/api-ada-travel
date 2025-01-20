using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace DAL.Login
{
    public class UserCoreDAL : GenericService<AccountClient>
    {
        private static DbWorker _DbWorker;
        public UserCoreDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Trả ra danh sách user theo công ty </returns>
        public async Task<UserMasterViewModel> getAuthentUserInfo(string username, string password)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[2];
                objParam[0] = new SqlParameter("@username", username);
                objParam[1] = new SqlParameter("@password", password);
                var dt = _DbWorker.GetDataTable("sp_getAuthentUserInfo", objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<UserMasterViewModel>();
                    return data[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getAuthentUserInfo - UserDAL: " + ex);
                return null;
            }
        }

        public async Task<List<UserMasterViewModel>> getDetail(long user_id, string username, string email)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[3];
                objParam[0] = new SqlParameter("@user_id", user_id);
                objParam[1] = new SqlParameter("@username", username);
                objParam[2] = new SqlParameter("@email", email);
                var dt = _DbWorker.GetDataTable("sp_getDetail", objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<UserMasterViewModel>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getDetail - UserDAL: " + ex);
                return null;
            }
        }

        public async Task<long> upsertUser(UserMasterViewModel model)
        {
            try
            {
                var objParam = new SqlParameter[18];
                objParam[0] = new SqlParameter("@UserId", model.Id);
                objParam[1] = new SqlParameter("@UserName", model.UserName);
                objParam[2] = new SqlParameter("@FullName", model.FullName);
                objParam[3] = new SqlParameter("@Password", model.Id < 0 ? "" : model.Password);
                objParam[4] = new SqlParameter("@ResetPassword", model.Id < 0 ? "" : model.ResetPassword);
                objParam[5] = new SqlParameter("@Phone", model.Phone);
                objParam[6] = new SqlParameter("@BirthDay", model.BirthDay);
                objParam[7] = new SqlParameter("@Gender", model.Gender);
                objParam[8] = new SqlParameter("@Email", model.Email);
                objParam[9] = new SqlParameter("@Avata", model.Avata);
                objParam[10] = new SqlParameter("@Address", model.Address);
                objParam[11] = new SqlParameter("@Status", model.Status);
                objParam[12] = new SqlParameter("@Note", model.Note == null ? "" : model.Note);
                objParam[13] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam[14] = new SqlParameter("@CreatedOn", DateTime.Now);
                objParam[15] = new SqlParameter("@ModifiedBy", model.ModifiedBy);
                objParam[16] = new SqlParameter("@ModifiedOn", DateTime.Now);
                objParam[17] = new SqlParameter("@CompanyType", model.CompanyType);

                var id = _DbWorker.ExecuteNonQuery("sp_UpsertUser", objParam);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("upsertUser - UserDAL: " + ex);
                return -1;
            }
        }

        public async Task<long> updateActive2Fa(long user_id)
        {
            try
            {
                var objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@userId", user_id);
               
                var id = _DbWorker.ExecuteNonQuery("sp_updateActive2Fa", objParam);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("updateActive2Fa - UserDAL: " + ex);
                return -1;
            }
        }

        public async Task<long> changePassword(string username, string password)
        {
            try
            {
                var objParam = new SqlParameter[2];
                objParam[0] = new SqlParameter("@username", username);
                objParam[1] = new SqlParameter("@password", password);

                var id = _DbWorker.ExecuteNonQuery("sp_ChangePassword", objParam);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("changePassword - UserDAL: " + ex);
                return -1;
            }
        }
        

    }
}
