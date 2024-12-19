using DAL.Generic;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace DAL
{
    public class AttachFileDAL : GenericService<AttachFile>
    {
        public AttachFileDAL(string connection) : base(connection) { }
        public async Task<List<AttachFile>> GetListByType(long DataId, int Type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.AttachFile.Where(s => s.DataId == DataId && s.Type == Type).ToListAsync();
                }
            }
            catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByType - AttachFileDAL: " + ex);
                return null;
            }
        }
    }
}
