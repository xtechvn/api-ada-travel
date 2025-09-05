using ENTITIES.Models;
using System.Collections.Generic;

namespace REPOSITORIES.IRepositories
{
    public interface IAirlinesRepository
    {
        Airlines GetByCode(string code);
        List<Airlines> GetAllData();
        GroupClassAirlines getDetailGroupClassAirlines(string classCode, string airline, string fairtype);
    }
}
