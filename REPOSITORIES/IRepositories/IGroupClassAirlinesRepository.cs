using ENTITIES.Models;
using System.Collections.Generic;

namespace REPOSITORIES.IRepositories
{
    public interface IGroupClassAirlinesRepository
    {
        GroupClassAirlines GetGroupClassAirlines(string air_line, string class_code, string fare_type);
        List<GroupClassAirlines> GetAllData();
    }
}
