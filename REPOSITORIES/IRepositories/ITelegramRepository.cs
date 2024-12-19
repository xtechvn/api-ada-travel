using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface ITelegramRepository
    {
        public Task<List<TelegramDetail>> GetAllBotList();
    }
}
