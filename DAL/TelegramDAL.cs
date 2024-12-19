using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class TelegramDAL:GenericService<TelegramDetail>
    {
        private static DbWorker _DbWorker;
        public TelegramDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<List<TelegramDetail>> GetAllBotList()
        {
            using (var _DbContext = new EntityDataContext(_connection))
            {
                return await _DbContext.TelegramDetail.AsNoTracking().ToListAsync();
            }
        }
    
    }
}
