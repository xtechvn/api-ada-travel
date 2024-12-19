using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace REPOSITORIES.IRepositories.Fly
{
    public interface IAirPortCodeRepository
    {
        List<AirPortCode> GetAirPortCodes();
    }
}
