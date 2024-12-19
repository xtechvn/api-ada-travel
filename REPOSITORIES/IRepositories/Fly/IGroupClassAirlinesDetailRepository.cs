using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace REPOSITORIES.IRepositories.Fly
{
    public interface IGroupClassAirlinesDetailRepository
    {
        GroupClassAirlinesDetail GetDetailGroupClassAirlines(int groupClassId);
    }
}
