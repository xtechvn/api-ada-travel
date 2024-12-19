using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.Repositories
{
    public class TelegramRepository : ITelegramRepository
    {
        private readonly TelegramDAL _telegramDAL;

        public TelegramRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<MailConfig> mailConfig)
        {
            _telegramDAL = new TelegramDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<List<TelegramDetail>> GetAllBotList()
        {
            return await _telegramDAL.GetAllBotList();
        }
    }
}
