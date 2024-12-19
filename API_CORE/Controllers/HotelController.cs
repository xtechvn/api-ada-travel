using Caching.RedisWorker;
using ENTITIES.ViewModels.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.PQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : Controller
    {
        private IConfiguration configuration;
      
        private IHotelDetailRepository _hotelDetailRepository;
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;

        private readonly RedisConn redisService;
        public HotelController(IConfiguration _configuration, IHotelDetailRepository hotelDetailRepository,   RedisConn _redisService, IClientRepository clientRepository, IAccountRepository accountRepository)
        {
            configuration = _configuration;
            _hotelDetailRepository = hotelDetailRepository;
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            redisService = _redisService;
         
        }
        [HttpPost("pricepolicy.json")]
        public async Task<ActionResult> GetPricePolicy(string token)
        {
            #region Test
            //APIHotelPricePolicyRequest input = new APIHotelPricePolicyRequest()
            //{
            //    account_client_id = 182,
            //    from_date = new DateTime(2023, 06, 01),
            //    to_date = new DateTime(2023, 06, 03),
            //    hotel_id = "340e8b59-4b88-9b69-5283-9922b91c6236",
            //    rooms = new List<APIHotelPricePolicyRequestPriceDetail>()
            //    {
            //        new APIHotelPricePolicyRequestPriceDetail()
            //        {
            //            price=1000000,
            //            room_id="9b5db22f-3b10-2a99-96f7-a2888f922e53"
            //        },
            //        new APIHotelPricePolicyRequestPriceDetail()
            //        {
            //             price=2340000,
            //             room_id="8ca0384d-1862-3fb4-7191-6317982c2091"
            //        }
            //    }
            //};
            //var data_product = JsonConvert.SerializeObject(input);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            JArray objParr = null;
            try
            {
                #region test:
                //APIHotelPricePolicyRequest input = new APIHotelPricePolicyRequest()
                //{
                //     account_client_id= 180,
                //     from_date=DateTime.Now,
                //     to_date=DateTime.Now.AddDays(3),
                //     hotel_id= "1104",
                //     rooms=new List<APIHotelPricePolicyRequestPriceDetail>()
                //     {
                //         new APIHotelPricePolicyRequestPriceDetail()
                //         {
                //             price=1700000,
                //             room_id="69"
                //         },
                //         new APIHotelPricePolicyRequestPriceDetail()
                //         {
                //             price=2200000,
                //             room_id="70"
                //         }
                //     }
                //};
                //var data_product = JsonConvert.SerializeObject(input);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                bool decode_success = false;
                APIHotelPricePolicyRequest model = new APIHotelPricePolicyRequest();
                List<APIHotelPricePolicyResponse> response = new List<APIHotelPricePolicyResponse>();
                string client_types = "";
                //-- B2B
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    decode_success = true;
                }
                //-- B2C
                else if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    decode_success = true;

                }
                if (decode_success)
                {
                    model = JsonConvert.DeserializeObject<APIHotelPricePolicyRequest>(objParr[0].ToString());
                    if(model == null ||model.hotel_id==null || model.hotel_id.Trim()=="" || model.from_date<=DateTime.MinValue|| model.to_date<=DateTime.MinValue
                        || model.account_client_id<=0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Dữ liệu không hợp lệ"

                        });
                    }
                    var account_client  =await _accountRepository.GetAccountClient(model.account_client_id);
                    var client =  _clientRepository.GetDetail((long)account_client.ClientId);
                    switch (account_client.ClientType)
                    {
                        case (int)ClientType.AGENT:
                            {
                                client_types = "1,2,3,4";
                            }break;
                        case (int)ClientType.TIER_1_AGENT:
                            {
                                client_types = "2,3,4";
                            }
                            break;
                        case (int)ClientType.TIER_2_AGENT:
                            {
                                client_types = "3,4";
                            }
                            break;
                        case (int)ClientType.TIER_3_AGENT:
                            {
                                client_types = "4";
                            }
                            break;
                        case (int)ClientType.CUSTOMER:
                            {
                                client_types = "5";
                            }
                            break;
                        default:
                            {
                                client_types = account_client.ClientType.ToString();
                            }
                            break;
                    }
                    model.nights= (model.to_date-model.from_date).Days;
                    string room_string = model.rooms != null && model.rooms.Count > 0 ? string.Join(",", model.rooms.Select(x => x.room_id)) : "";
                    var policy_list = _hotelDetailRepository.GetHotelRoomPricePolicy(model.hotel_id,  client_types);
                    if(policy_list!=null && policy_list.Count > 0)
                    {
                        foreach (var room in model.rooms)
                        {
                            var policy_by_room = policy_list.Where(x=>x.RoomCode==room.room_id);
                            foreach(var policy in policy_by_room)
                            {
                                double profit_per_night = 0;
                                switch (policy.UnitId)
                                {
                                    case (short)PriceUnitType.VND:
                                        {
                                            profit_per_night = policy.Profit;
                                        }
                                        break;
                                    case (short)PriceUnitType.PERCENT:
                                        {
                                            profit_per_night = room.price * policy.Profit / (double)100;
                                        }
                                        break;
                                }
                                response.Add(new APIHotelPricePolicyResponse()
                                {
                                    amount= room.price + (profit_per_night * model.nights),
                                    hotel_id=model.hotel_id,
                                    price=room.price,
                                    profit= (profit_per_night * model.nights),
                                    room_id=room.room_id,
                                    allotment_id=policy.AllotmentsId,
                                    campaign_code=policy.CampaignCode,
                                    contract_no=policy.ProgramCode,
                                    package_id=policy.PackageCode
                                });
                            }
                        }
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Success",
                            data = response

                        });
                    }
                    else
                    {
                        if(model.rooms!=null && model.rooms.Count > 0)
                        {
                            foreach (var room in model.rooms)
                            {
                                var policy_by_room = policy_list.Where(x => x.RoomCode == room.room_id);
                                response.Add(new APIHotelPricePolicyResponse()
                                {
                                    amount = room.price,
                                    hotel_id = model.hotel_id,
                                    price = room.price,
                                    profit = 0,
                                    room_id = room.room_id,
                                    allotment_id = "",
                                    campaign_code = "",
                                    contract_no = "",
                                    package_id = "",
                                });
                            }
                        }
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "No Policy Found",
                            data = response

                        });
                    }
                   
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Key không hợp lệ"

                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPricePolicy - HotelController: " + ex);
            }
            return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
        }
    }
}
