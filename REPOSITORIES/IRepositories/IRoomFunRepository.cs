using ENTITIES.APPModels.PushHotel;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IRoomFunRepository
    {
        public Task<int> CreateOrUpdateRoomFun(HotelContract detail);
        public string GetNameByID(string allotment_id);
    }
}
