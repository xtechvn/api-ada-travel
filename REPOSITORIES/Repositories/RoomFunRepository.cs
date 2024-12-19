using DAL;
using Entities.ConfigModels;
using ENTITIES.APPModels.PushHotel;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System.Threading.Tasks;

namespace REPOSITORIES.Repositories
{
    public class RoomFunRepository : IRoomFunRepository
    {
        private readonly RoomFunDAL _roomFunDAL;

        public RoomFunRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<MailConfig> mailConfig)
        {
            _roomFunDAL = new RoomFunDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public async Task<int> CreateOrUpdateRoomFun(HotelContract detail)
        {
            return await _roomFunDAL.CreateOrUpdateRoomFun(detail);
        }

        public string GetNameByID(string allotment_id)
        {
            return _roomFunDAL.GetNameByID(allotment_id);
        }

    }
}
