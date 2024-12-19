using DAL;
using Entities.ConfigModels;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ArticleTagDAL articleTagDAL;
        private readonly TagDAL _tagDAL;

        public TagRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            articleTagDAL = new ArticleTagDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _tagDAL = new TagDAL(dataBaseConfig.Value.SqlServer.ConnectionString);

        }
        public async Task<List<string>> GetAllTagByArticleID(long articleID)
        {
            var tag_id_list=articleTagDAL.GetTagIDByArticleID(articleID);
            return await _tagDAL.GetTagByListID(tag_id_list);
        }
    }
}
