using ENTITIES.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IAttachFileRepository
    {
        Task<List<AttachFile>> GetListByType(long DataId, int type);
        Task<long> Delete(long Id, int userLogin);
        Task<List<object>> CreateMultiple(List<AttachFile> models);
    }
}
