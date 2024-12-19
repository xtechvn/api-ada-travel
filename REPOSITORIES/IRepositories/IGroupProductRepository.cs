using ENTITIES.Models;
using ENTITIES.ViewModels.Articles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IGroupProductRepository
    {
       public Task<string> GetGroupProductNameAsync(int cateID);
       public Task<List<ArticleGroupViewModel>> GetArticleCategoryByParentID(long parent_id);
        public Task<List<ArticleGroupViewModel>> GetFooterCategoryByParentID(long parent_id);
        public Task<List<ProductGroupViewModel>> GetProductGroupByParentID(long parent_id, string url_static);

    }
}
