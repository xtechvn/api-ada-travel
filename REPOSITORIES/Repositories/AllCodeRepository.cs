using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.Repositories
{
    public class AllCodeRepository : IAllCodeRepository
    {
        private readonly AllCodeDAL _allCodeDAL;

        public AllCodeRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<MailConfig> mailConfig)
        {
            _allCodeDAL = new AllCodeDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public Task<List<AllCode>> GetAllTelegramBot()
        {
            return _allCodeDAL.GetAllTelegramBot();
        }
        public Task<List<AllCode>> GetAllCodeByType(string type)
        {
            return _allCodeDAL.GetAllCodeByType(type);
        }
        public async Task<List<Province>> GetProvinceList()
        {
            return await _allCodeDAL.GetProvinceList();
        }
        public async Task<List<District>> GetDistrictListByProvinceId(string provinceId)
        {
            return await _allCodeDAL.GetDistrictListByProvinceId(provinceId);
        }
        public async Task<List<Ward>> GetWardListByDistrictId(string districtId)
        {
            return await _allCodeDAL.GetWardListByDistrictId(districtId);
        }
    }
}
