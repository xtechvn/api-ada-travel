using DAL;
using DAL.Permission;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.Client;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using System.Linq;


/// <summary>
/// Database TRAVEL
/// </summary>
namespace REPOSITORIES.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAL userDAL;
        private readonly UserAgenDAL userAgenDAL;
        private readonly ClientDAL clientDAL;
        private readonly PermissionDAL permissionDAL;
        public UserRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            userDAL = new UserDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            userAgenDAL = new UserAgenDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            clientDAL = new ClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            permissionDAL = new PermissionDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public User GetByUserAndPassword(string userName, string password)
        {
            return userDAL.GetByUserAndPassword(userName, password);
        }
        public User GetDetail(long userId)
        {
            return userDAL.GetDetail(userId);
        }

        public int InsertUserAndClient(List<UserClientModel> listUser)
        {
            try
            {
                foreach (var item in listUser)
                {
                    var client = new Client();
                    var user = new User();
                    var userAgent = new UserAgent();
                    clientDAL.Insert(client);
                    userDAL.Insert(user);
                    userAgent.ClientId = client.Id;
                    userAgent.UserId = user.Id;

                }
                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertUserAndClient - UserRepository: " + ex);
                return -1;
            }
        }
        public async Task<User> GetChiefofDepartmentByServiceType(int service_type)
        {
            try
            {
                switch (service_type)
                {
                    case (int)ServicesType.OthersHotelRent:
                    case (int)ServicesType.VINHotelRent:
                        {
                            return await userDAL.GetChiefofDepartmentByRoleID((int)RoleType.TPDHKS);
                        }
                    case (int)ServicesType.FlyingTicket:
                        {
                            return await userDAL.GetChiefofDepartmentByRoleID((int)RoleType.TPDHVe);

                        }
                    case (int)ServicesType.Tourist:
                        {
                            return await userDAL.GetChiefofDepartmentByRoleID((int)RoleType.TPDHTour);

                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetChiefofDepartmentByServiceType - UserRepository: " + ex);
            }
            return null;
        }
        public List<string> getManagerEmailByUserId(int user_id)
        {
            try
            {
                var obj_manager = permissionDAL.getManagerByUserId(user_id);
                if (obj_manager.Rows.Count > 0)
                {
                    // var arr = obj_manager.AsEnumerable().Select(n => n.Field<int>("UserId"));  //Convert.ToInt32(obj_manager.Rows[0]["UserId"]);                    
                    List<int> userIdList = obj_manager.AsEnumerable().Select(n => n.Field<int>("UserId")).ToList();
                    var result = new List<string>();
                    if (userIdList != null && userIdList.Count > 0)
                    {
                        foreach (var userid in userIdList)
                        {
                            var exists_user = userDAL.GetDetail(userid);
                            if (exists_user != null && exists_user.Id > 0)
                            {
                                result.Add(exists_user.Email);
                            }
                        }
                    }
                    return result;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("NotifyRepository - getManagerByUserId: " + ex);
                return null;
            }
        }

        public List<string> GetLeaderEmailByUserId(int user_id)
        {
            try
            {
                var obj_manager = permissionDAL.GetLeaderByUserId(user_id);
                if (obj_manager.Rows.Count > 0)
                {
                    // var arr = obj_manager.AsEnumerable().Select(n => n.Field<int>("UserId"));  //Convert.ToInt32(obj_manager.Rows[0]["UserId"]);                    
                    List<int> userIdList = obj_manager.AsEnumerable().Select(n => n.Field<int>("UserId")).ToList();
                    var result = new List<string>();
                    if (userIdList != null && userIdList.Count > 0)
                    {
                        foreach (var userid in userIdList)
                        {
                            var exists_user = userDAL.GetDetail(userid);
                            if (exists_user != null && exists_user.Id > 0)
                            {
                                result.Add(exists_user.Email);
                            }
                        }
                    }
                    return result;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("NotifyRepository - getManagerByUserId: " + ex);
                return null;
            }
        }
        public long? GetUserAgentByClientId(long clientId)
        {
            try
            {
                return userAgenDAL.GetUserAgentByClientId(clientId);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UserRepository - GetUserAgentByClientId: " + ex);
                return null;
            }
        }
        public User? GetHeadOfDepartmentByRoleID(long role_id)
        {
            try
            {
                return userDAL.GetHeadOfDepartmentByRoleID(role_id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UserRepository - GetHeadOfDepartmentByRoleID: " + ex);
                return null;
            }
        }
    }
}
