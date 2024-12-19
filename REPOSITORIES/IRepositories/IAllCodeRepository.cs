using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IAllCodeRepository
    {
        public  Task<List<AllCode>> GetAllTelegramBot();
        public  Task<List<AllCode>> GetAllCodeByType(string type);
        Task<List<Province>> GetProvinceList();
        Task<List<District>> GetDistrictListByProvinceId(string provinceId);
        Task<List<Ward>> GetWardListByDistrictId(string districtId);
    }
}
