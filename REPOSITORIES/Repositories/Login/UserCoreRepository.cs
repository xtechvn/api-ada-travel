using DAL.Login;
using Entities.ConfigModels;
using ENTITIES.ViewModels.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories.Login
{
    public class UserCoreRepository: IUserCoreRepository
    {
        private  UserCoreDAL userDAL;
        private readonly UserCoreDAL userCoreDALTravel;
        private readonly UserCoreDAL userCoreDALPQ;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public UserCoreRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            dataBaseConfig = _dataBaseConfig;
            userCoreDALTravel = new UserCoreDAL(dataBaseConfig.Value.SqlServer.ConnectionStringTravel);
            userCoreDALPQ = new UserCoreDAL(dataBaseConfig.Value.SqlServer.ConnectionStringPQ);
            userDAL = new UserCoreDAL(dataBaseConfig.Value.SqlServer.ConnectionStringUser);

        }
        public async Task<List<UserMasterViewModel>> getDetail(long user_id, string username, string password)
        {
            try
            {
                return await userDAL.getDetail(user_id, username, password);
            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("checkAuthent, username = " + username + " - UserRepository: " + ex);
                return null;
            }
        }
        public async Task<UserMasterViewModel> checkAuthent(string username, string password)
        {
            try
            {
               return await userDAL.getAuthentUserInfo(username, password);
            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("checkAuthent, username = " + username + " - UserRepository: " + ex);
                return null;
            }
        }
        public async Task<long> upsertUser(UserMasterViewModel model)
        {
            try
            {
                UserCoreDAL userDAL2 = new UserCoreDAL(dataBaseConfig.Value.SqlServer.ConnectionStringUser);
                var id = await userDAL2.upsertUser(model);
                if(id <= 0)
                {
                    return id;
                }
                var company_type = model.CompanyType!=null && model.CompanyType.Trim()!=""? model.CompanyType.Split(","):null;
                if(company_type!=null && company_type.Length > 0)
                {
                    foreach(var ct in company_type)
                    {
                        switch (ct.Trim()){
                            case "0":
                                {
                                    UserCoreDAL userCoreDALTravel2 = new UserCoreDAL(dataBaseConfig.Value.SqlServer.ConnectionStringTravel);
                                    await userCoreDALTravel2.upsertUser(model);
                                }break;
                            case "1":
                                {
                                    UserCoreDAL userCoreDALPQ2 = new UserCoreDAL(dataBaseConfig.Value.SqlServer.ConnectionStringPQ);
                                    await userCoreDALPQ2.upsertUser(model);

                                }
                                break;
                            case "2":
                                {
                                    
                                }
                                break;
                        }
                    }
                }
                return id;
            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("upsertUse, user = "+ JsonConvert.SerializeObject(model) + " - UserRepository: " + ex);
                return -1;
            }
        }

        public async Task<long> changePassword(string username, string password)
        {
            try
            {
                userDAL = new UserCoreDAL(dataBaseConfig.Value.SqlServer.ConnectionStringUser);
                LogHelper.InsertLogTelegram("upsertUse, user = " );

                return await userDAL.changePassword( username,  password);

            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("upsertUse, user = " + username + " - UserRepository: " + ex);
                return -1;
            }
        }     



        public async Task<long> updateActive2Fa(long user_id)
        {
            try
            {
                return await userDAL.updateActive2Fa(user_id);

            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("updateActive2Fa, user_id = " + user_id + " - UserRepository: " + ex);
                return -1;
            }
        }

      
    }
}
