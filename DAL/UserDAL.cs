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

namespace DAL
{
    public class UserDAL : GenericService<User>
    {
        private static DbWorker _DbWorker;
        private static string sqlInsertUserAndClient = String.Empty;
        public UserDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public User GetDetail(long userId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.User.AsNoTracking().FirstOrDefault(s => s.Id == userId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - UserDAL: " + ex);
                return null;
            }
        }
        public User GetByEmail(string email)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.User.AsNoTracking().FirstOrDefault(s => s.Email.ToLower().Trim().Contains(email.ToLower().Trim()));
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByEmail - UserDAL: " + ex.ToString());
                return null;
            }
        }
        public User GetByUserAndPassword(string userName, string password)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.User.AsNoTracking().FirstOrDefault(s => s.UserName.Equals(userName) && s.Password.Equals(password));
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetClientByUserAndPassword - UserDAL: " + ex);
                return null;
            }
        }
        public long Insert(User user)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    _DbContext.User.Add(user);
                    _DbContext.SaveChanges();
                    return user.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Insert - UserDAL: " + ex);
                return -1;
            }
        }
        public async Task<User> GetChiefofDepartmentByRoleID(int role_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var user_role = await _DbContext.UserRole.Where(s => s.RoleId == role_id).FirstOrDefaultAsync();
                    if (user_role != null && user_role.Id > 0)
                    {
                        return await _DbContext.User.Where(s => s.Id == user_role.UserId).FirstOrDefaultAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetChiefofDepartmentByServiceType - UserDAL: " + ex);
            }
            return null;
        }
        public User? GetHeadOfDepartmentByRoleID(long role_id)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@RoleId", role_id);

                DataTable tb = new DataTable();
                _DbWorker.Fill(tb, "SP_GetHeadOfDepartmentByRoleID", objParam);

                var s = tb.ToList<User>().FirstOrDefault();
                if (s != null && s.Id > 0) return s;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UserDAL GetHeadOfDepartmentByRoleID" + ex.ToString());
            }
            return null;

        }
    }
}
