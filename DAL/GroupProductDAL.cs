using DAL.Generic;
using ENTITIES.Models;
using ENTITIES.ViewModels.Articles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace DAL
{
    public class GroupProductDAL : GenericService<GroupProduct>
    {
        public GroupProductDAL(string connection) : base(connection)
        {
        }

        public List<GroupProduct> GetByParentId(long parent_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.GroupProduct.Where(s => s.ParentId == parent_id && s.Status==(int)ArticleStatus.PUBLISH).ToList();
                }
            }
            catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByParentId - GroupProductDAL: " + ex);

            }
            return null;
        }
        public GroupProduct GetById(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.GroupProduct.Where(s => s.Id == id && s.Status == (int)ArticleStatus.PUBLISH).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetById - GroupProductDAL: " + ex);

            }
            return null;
        }
    }
}
