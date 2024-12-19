using DAL;
using Entities.ConfigModels;
using ENTITIES.ViewModels.VinWonder;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories
{
    public class PlayGroundDetailRepository : IPlayGroundDetailRepository
    {
        private readonly PlayGroundDetailDAL _playGroundDetailDAL;

        public PlayGroundDetailRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _playGroundDetailDAL = new PlayGroundDetailDAL(dataBaseConfig.Value.SqlServer.ConnectionString);

        }
        public VinWonderPlayGroundViewModel GetPlayGroundDetailByLocationCode(string location_code, int service_type, string url_static_domain)
        {
            VinWonderPlayGroundViewModel model = null;
            try
            {
                model = new VinWonderPlayGroundViewModel();
                DataTable dt = _playGroundDetailDAL.GetPlayGroundDetail(location_code,service_type);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = (from row in dt.AsEnumerable()
                             select new VinWonderPlayGroundViewModel
                             {
                                 content = row["Content"].ToString(),
                                 images = row["url_path"].ToString().Split(",").Select(x=> url_static_domain+x).ToList(),
                                 title = row["Title"].ToString(),
                                 lead = row["Lead"].ToString(),
                             }).FirstOrDefault();
                }
            
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetNewsDetailByLocationCode - PlayGroundDetailRepository: " + ex);
            }
            return model;
        }
    }
}
