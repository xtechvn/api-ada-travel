using Caching.RedisWorker;
using ENTITIES.ViewModels.Order;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.ORDER.Base
{
    public class OrderService
    {
        private IConfiguration configuration;
        private IOrderRepository ordersRepository;
        private readonly RedisConn redisService;
        private readonly IAccountB2BRepository accountB2BRepository;
        private readonly IAccountRepository accountRepository;
        public OrderService(IConfiguration _configuration, IOrderRepository _ordersRepository, RedisConn _redisService, IAccountB2BRepository _accountB2BRepository, IAccountRepository _accountRepository)
        {
            configuration = _configuration;
            redisService = _redisService;
            ordersRepository = _ordersRepository;
            accountB2BRepository = _accountB2BRepository;
            accountRepository = _accountRepository;
        }
        public async Task< List<List_OrderViewModel>> getOrderByClientId(long client_id, int source_type)
        {
            try
            {
                int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                string cache_name = CacheName.Order_Client + client_id;
                var obj_lst_order = new List<List_OrderViewModel>();
                var strDataCache = redisService.Get(cache_name, db_index);
                // Kiểm tra có data trong cache ko
                if (!string.IsNullOrEmpty(strDataCache))
                    // nếu có trả ra luôn object 
                    obj_lst_order = JsonConvert.DeserializeObject<List<List_OrderViewModel>>(strDataCache);

                else
                {
                    // nếu chưa thì vào db lấy
                    var listid = await accountB2BRepository.GetListAccountClient(client_id, -1, -1, -1, -1, null);
                    if(listid != null)
                    {
                        var list_account_clientId = listid.Select(i => i.Id).ToList();
                        foreach (var item in list_account_clientId)
                        {
                            if (item > 0)
                            {
                                var accountclient = await accountRepository.GetAccountClient(item);
                                var listOrder = await ordersRepository.GetOrderByClientId((long)accountclient.ClientId);
                                obj_lst_order.AddRange(listOrder);
                            }
                            else
                            {
                                var listOrder = await ordersRepository.GetOrderByClientId(client_id);
                                obj_lst_order.AddRange(listOrder);
                            }

                        }

                    }
                    else
                    {
                        obj_lst_order = await ordersRepository.GetOrderByClientId(client_id);
                    }
         
                    // Kiem tra db co data khong
                    if (obj_lst_order == null)
                    {
                        LogHelper.InsertLogTelegram("Không lấy được danh sách order theo mã. ClientId: " + client_id );
                        return null;
                    }
                    else
                    {
                        // Gán vào Cache
                        obj_lst_order = obj_lst_order.GroupBy(s => s.OrderId).Select(s => s.First()).ToList();
                        try
                        {
                            redisService.Set(cache_name, JsonConvert.SerializeObject(obj_lst_order), db_index);
                        }
                        catch
                        {
                            return obj_lst_order;
                        }
                        
                    }
                }
                return obj_lst_order;
            }
            catch (Exception ex)
            {                
                LogHelper.InsertLogTelegram("getOrderByClientId(client_id=" + client_id + ") in OrderService" + ex.ToString());
                return null;
            }
        }

    }
}
