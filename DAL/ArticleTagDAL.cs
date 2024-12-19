using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class ArticleTagDAL : GenericService<ArticleTag>
    {
        private static DbWorker _DbWorker;
        public ArticleTagDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public List<long> GetTagIDByArticleID(long articleID)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var a= _DbContext.ArticleTag.Where(s => s.ArticleId == articleID).Select(s => s.TagId);
                    if(a!=null && a.Count() > 0)
                    {
                       var json = JsonConvert.SerializeObject(a.Distinct().ToList());
                       return JsonConvert.DeserializeObject<List<long>>(json);
                    }
                }
            }
            catch
            {
            }
            return null;
        }
    }
}
