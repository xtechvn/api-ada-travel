using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface ILocationRepository
    {
        public  Task<List<ENTITIES.Models.National>> GetNationalByListID(string ids);
        public Task<List<ENTITIES.Models.Province>> GetProvinceByListID(string ids);
    }
}
