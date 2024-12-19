using ENTITIES.ViewModels.VinWonder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IPlayGroundDetailRepository
    {
        public VinWonderPlayGroundViewModel GetPlayGroundDetailByLocationCode(string location_code, int service_type,string url_static_domain);

    }
}
