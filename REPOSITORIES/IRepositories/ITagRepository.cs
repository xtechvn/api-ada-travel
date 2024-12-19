using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface ITagRepository
    {
        public Task<List<string>> GetAllTagByArticleID(long articleID);
    }
}
