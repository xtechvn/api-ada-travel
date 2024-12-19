using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.Repositories
{
    public class AttachFileRepository : IAttachFileRepository
    {
        private readonly ILogger<AttachFileRepository> _logger;
        private readonly AttachFileDAL _AttachFileDAL;

        public AttachFileRepository(IOptions<DataBaseConfig> dataBaseConfig,ILogger<AttachFileRepository> logger)
        {
            _logger = logger;
            _AttachFileDAL = new AttachFileDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public async Task<long> Delete(long Id, int userLogin)
        {
            try
            {
                var model = await _AttachFileDAL.FindAsync(Id);
                if (model.UserId != userLogin)
                {
                    return -1;
                }

                await _AttachFileDAL.DeleteAsync(Id);
                return Id;
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete - AttachFileRepository : " + ex.Message);
                return 0;
            }
        }

        public async Task<List<AttachFile>> GetListByType(long DataId, int Type)
        {
            try
            {
                return await _AttachFileDAL.GetListByType(DataId, Type);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetListByType - AttachFileRepository : " + ex);
                return null;
            }
        }

        public async Task<List<object>> CreateMultiple(List<AttachFile> models)
        {
            try
            {
                var _ListRs = new List<object>();
                foreach (var item in models)
                {
                    item.CreateDate = DateTime.Now;
                    var _AttachId = await _AttachFileDAL.CreateAsync(item);
                    _ListRs.Add(new
                    {
                        Id = _AttachId,
                        FilePath = item.Path
                    });
                }
                return _ListRs;
            }
            catch (Exception ex)
            {
                _logger.LogError("MultipleCreate - AttachFileRepository: " + ex);
                return null;
            }
        }
    }
}
