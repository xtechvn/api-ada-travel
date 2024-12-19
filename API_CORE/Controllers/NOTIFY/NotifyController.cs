using DAL.MongoDB.Notify;
using ENTITIES.ViewModels.Notify;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories.Notify;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.NOTIFY
{
    [Route("api")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly INotifyRepository notifyRepository;
        private readonly ISubscriber _subscriber;
        private readonly NotifyMongoDAL _notifyMongoDAL;
        
        public NotifyController(IConfiguration configuration, INotifyRepository _notifyRepository)
        {
            _configuration = configuration;
            notifyRepository = _notifyRepository;
            var connection = ConnectionMultiplexer.Connect(configuration["DataBaseConfig:Redis:Host"] + ":" + configuration["DataBaseConfig:Redis:Port"]);
            _subscriber = connection.GetSubscriber();
            _notifyMongoDAL = new NotifyMongoDAL(configuration);
        }
        /// <summary>
        /// Lấy ra danh sách noti của user:
        /// 1. Tổng số msg chưa đọc
        /// 2. Danh sách chi tiết các message chưa đọc
        /// Khi load page sẽ call lại api này để pub lại
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("notify/get-list.json")]
        public async Task<ActionResult> getListNotify(string token)
        {
            try
            {
                JArray objParr = null;
                bool is_public_noti = false;
                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"user_id", "34"},
                    {"pageindex", "1"},
                    {"pagesize", "50"}
                };
                var data_product = JsonConvert.SerializeObject(j_param);

                //token = CommonHelper.Encode(data_product, _configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:b2c"]))
                {

                  //  int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    var user_id = Convert.ToInt32(objParr[0]["user_id"]);
                    var pageindex = Convert.ToInt32(objParr[0]["pageindex"]);
                    var pagesize = Convert.ToInt32(objParr[0]["pagesize"]);
                    string cache_name = "NOTIFY_" + user_id;
                    var obj_notify = new NotifySummeryViewModel();
                  
                    //obj_notify = await _notifyMongoDAL.getListNotify(user_id);
                    obj_notify = await _notifyMongoDAL.getListNotifyPage(user_id, pageindex, pagesize);
                    if (obj_notify != null)
                    {
                        if (!(obj_notify.total_not_seen == 0 && obj_notify.lst_not_seen_detail.Count == 0))
                        {
                            is_public_noti = true;
                            _subscriber.Publish(cache_name, JsonConvert.SerializeObject(obj_notify));
                        }
                    }

                    return Ok(new
                    {
                        status = is_public_noti ? (int)ResponseType.SUCCESS : (int)ResponseType.EMPTY,
                        msg = is_public_noti ? "Thông tin notify của user_id" + user_id + " đã public thành công" : "Hiện tại không có notify nào của user này",
                        total = obj_notify.lst_not_seen_detail.Count
                    });
                }
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Token khong hop le"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("notify/get-list.json" + ex.ToString() + "token =" + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Transaction Error !!!"
                });
            }

        }

        [HttpPost("notify/message/send.json")]
        public async Task<ActionResult> sendNotify(string token)
        {
            try
            {
                JArray objParr = null;
                string content = string.Empty;
                string module_label = string.Empty;
                string action_label = string.Empty;
                var user_receiver_id = new List<int>();
                #region Test
                //var j_param = new Dictionary<string, object>
                //{
                //    {"user_name_send", "Phạm ANh Hiếu"}, //tên người gửi
                //    {"user_id_send", "34"}, //id người gửi
                //    {"service_code", "KS0001"}, // max dichj vu
                //    {"code", "O23G00963"}, // mã đối tượng gửi
                //    {"link_redirect", "/Order/12095"}, // Link mà khi người dùng click vào detail item notify sẽ chuyển sang đó
                //    {"module_type", "1"}, // loại module thực thi luồng notify. Ví dụ: Đơn hàng, khách hàng.......
                //    {"action_type", "0"}, // action thực hiện. Ví dụ: Duyệt, tạo mới, từ chối....
                //    {"role_type", "6"} // quyền mà sẽ gửi tới
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);

                // token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion


                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:b2c"]))
                {

                    var user_name_send = objParr[0]["user_name_send"].ToString();
                    var code = objParr[0]["code"].ToString();
                    var link_redirect = objParr[0]["link_redirect"].ToString();
                    var user_id_send = Convert.ToInt16(objParr[0]["user_id_send"]);
                    var module_type = Convert.ToInt16(objParr[0]["module_type"]);
                    var action_type = Convert.ToInt16(objParr[0]["action_type"]);
                    //var role_type = Convert.ToInt16(objParr[0]["role_type"]);
                    var service_code = objParr[0]["service_code"] == null ? "" : objParr[0]["service_code"].ToString();

                    module_label = NotifyLabelName.getModuleName(module_type);
                    action_label = NotifyLabelName.getActionName(action_type);
                    // Thiết lập notify mặc định
                    content = user_name_send + " đã " + action_label + " " + module_label + " " + code;

                    // Nội dung notify
                    if (user_name_send != string.Empty && module_label != string.Empty && code != string.Empty && action_label != string.Empty)
                    {
                        // Quy trình push notify
                        switch (action_type)
                        {
                            case (Int16)ActionType.TAO_MOI_HOP_DONG:
                            case (Int16)ActionType.NHAN_TRIEN_KHAI:
                                // Gửi lên cho trường phòng của nv bán chính hợp đồng
                                var lst_user_manager_id = notifyRepository.getManagerByUserId(user_id_send);
                                if (lst_user_manager_id.Count > 0)
                                {
                                    content = user_name_send + " đã " + action_label + " " + module_label + " " + code;
                                    user_receiver_id = lst_user_manager_id;
                                }
                                else
                                {
                                    LogHelper.InsertLogTelegram("notify/message/send.json" + "token =" + token + "==> Khong tim thay user nao thuoc quyen truong phong cua saler_id = " + user_id_send);
                                    return Ok(new { status = (int)ResponseType.FAILED, msg = "Khong tim thay user nao thuoc quyen truong phong cua saler_id = " + user_id_send });
                                }
                                break;                       
                            case (Int16)ActionType.DUYET_HOP_DONG:
                                // Gửi lên cho bộ phận duyet hop dong                                    
                                var user_verify = notifyRepository.getListUserByRoleId((Int16)RoleType.DUYETHD);
                                if (user_verify.Rows.Count > 0)
                                {
                                    content = "Hợp đồng "+ code +" của "+ user_name_send + " cần được phê duyệt";
                                    var accountants = user_verify.AsEnumerable();
                                    foreach (var item in accountants)
                                    {
                                        user_receiver_id.Add(item.Field<int>("UserId"));
                                    }
                                }
                                else
                                {
                                    LogHelper.InsertLogTelegram("notify/message/send.json" + "token =" + token + "==> Khong tim thay user nao thuoc quyen ke toan");
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.EMPTY,
                                        msg = "Khong tim thay user nao thuoc quyen user_verify",
                                        token = token
                                    });
                                }
                                break;
                            case (Int16)ActionType.TAO_YEU_CAU_CHI:
                            case (Int16)ActionType.DUYET_YEU_CAU_CHI:
                            case (Int16)ActionType.BO_DUYET_YEU_CAU_CHI:
                            case (Int16)ActionType.TU_CHOI_DUYET_YEU_CAU_CHI:
                            case (Int16)ActionType.QUYET_TOAN:
                                // Gửi lên cho bộ phận kế toán                                    
                                var user_accountant = notifyRepository.getListUserByRoleId((Int16)RoleType.KT);
                                if (user_accountant.Rows.Count > 0)
                                {
                                    var accountants = user_accountant.AsEnumerable();
                                    foreach (var item in accountants)
                                    {
                                        user_receiver_id.Add(item.Field<int>("UserId"));
                                    }
                                }
                                else
                                {
                                    LogHelper.InsertLogTelegram("notify/message/send.json" + "token =" + token + "==> Khong tim thay user nao thuoc quyen ke toan");
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.EMPTY,
                                        msg = "Khong tim thay user nao thuoc quyen ke toan",
                                        token = token
                                    });
                                }
                                break;
                            case (Int16)ActionType.TU_CHOI_DON_HANG:
                            case (Int16)ActionType.HUY:
                            case (Int16)ActionType.HOAN_THANH:
                            case (Int16)ActionType.TRA_CODE:
                            case (Int16)ActionType.TAO_MOI_PHIEU_THU:                         
                                // Gửi về cho nv bán chính đơn hàng đó
                                var sale_main = notifyRepository.getSalerIdByOrderNo(code); // code o day la so don hang
                                if (sale_main.Rows.Count > 0)
                                {
                                    content = user_name_send + " đã chuyển đổi trạng thái " + module_label + " " + code + " của bạn sang " + action_label;
                                    if (action_type == (Int16)ActionType.TAO_MOI_PHIEU_THU)
                                    {
                                        content = user_name_send + " đã " + action_label + " cho đơn hàng " + code + " của bạn";
                                    }
                                    else if (action_type == (Int16)ActionType.TRA_CODE)
                                    {
                                        content = "Điều hành " + user_name_send + " đã trả code cho dịch vụ " + service_code + " của đơn hàng " + code;
                                    }
                                    var sale_id = Convert.ToInt32(sale_main.Rows[0]["SalerId"]);
                                    user_receiver_id.Add(sale_id);
                                }
                                break;
                            case (Int16)ActionType.DA_DUYET: // Duyệt hợp đồng thành công
                            case (Int16)ActionType.TU_CHOI_HOP_DONG:
                                // Gửi về cho nv tạo hợp đồng đó
                                var sale_create = notifyRepository.getSalerIdByContractNo(code); 
                                if (sale_create.Rows.Count > 0)
                                {
                                    content = user_name_send + " đã " + action_label + " hợp đồng " + code + " của bạn";
                                    var sale_id = Convert.ToInt32(sale_create.Rows[0]["UserIdCreate"]);
                                    user_receiver_id.Add(sale_id);
                                }
                                break;
                            case (Int16)ActionType.GUI_DIEU_HANH_DUYET:
                            case (Int16)ActionType.DUYET_DICH_VU:
                                // gửi lên cho điều hành tương ứng với các dịch vụ trong đơn hàng
                                var obj_operator_list = notifyRepository.getListOperatorByOrderNo(code); // code o day la so don hang
                                if (obj_operator_list.Rows.Count > 0)
                                {
                                    var obj_operator = obj_operator_list.AsEnumerable();
                                    var operator_id = obj_operator.First().Field<string>("OperatorId");
                                    user_receiver_id = operator_id.Split(",").ToList().ConvertAll(int.Parse);
                                    // content
                                    if (action_type == (Int16)ActionType.DUYET_DICH_VU)
                                    {
                                        content = user_name_send + " đã gửi dịch vụ có mã " + service_code + " của đơn hàng " + code + " chờ bạn xử lý";
                                    }
                                    else
                                    {
                                        content = "Dịch vụ trong hợp đồng " + code + " đã được thanh toán.";
                                    }

                                }
                                else
                                {
                                    LogHelper.InsertLogTelegram("notify/message/send.json" + "token =" + token + "==> Khong tim thay user nao thuoc quyen dieu hanh");
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.EMPTY,
                                        msg = "Khong tim thay user nao thuoc quyen dieu hanh",
                                        token = token
                                    });
                                }
                                break;

                            default:
                                break;
                        }
                        // Lưu thông tin notify
                        var model = new MessageViewModel
                        {
                            content = content,
                            code = code,
                            module_type = module_type,
                            send_date = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                            user_id_send = user_id_send,
                            user_name_send = user_name_send
                        };
                        var data_id_notify = await _notifyMongoDAL.pushMessage(model);
                        if (data_id_notify != "")
                        {
                            // Lưu thông tin người sẽ nhận notify này
                            if (user_receiver_id.Count > 0)
                            {
                                foreach (var id in user_receiver_id)
                                {
                                    var receiver_model = new ReceiverMessageViewModel
                                    {
                                        seen_status = (Int16)SeenType.NOT_SEEN,
                                        notify_id = data_id_notify,
                                        seen_date = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                                        user_receiver_id = id,
                                        link_redirect = link_redirect,
                                        content = content,
                                        company_type = 0
                                    };
                                    var data_notify = _notifyMongoDAL.pushReceiverReadMessage(receiver_model);

                                    // xóa cache cho user này
                                    string cache_name = "NOTIFY_" + receiver_model.user_receiver_id;
                                   // int db_index = Convert.ToInt32(_configuration["Redis:Database:db_core"]);
                                    //redisService.clear(cache_name, db_index);

                                    // Update lại cho cache này
                                    var obj_notify = await _notifyMongoDAL.getListNotify(receiver_model.user_receiver_id);
                                    if (obj_notify != null)
                                    {                                        
                                        _subscriber.Publish(cache_name, JsonConvert.SerializeObject(obj_notify));
                                    }
                                }

                                return Ok(new
                                {
                                    status = (int)ResponseType.SUCCESS,
                                    msg = "SUCCESS with preview notify: " + content
                                });
                            }
                            else
                            {
                                return Ok(new
                                {
                                    status = (int)ResponseType.EMPTY,
                                    msg = "Không tìm thấy người sẽ nhận notify có token = " + token
                                });
                            }
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                msg = "FAILED with  notify: " + token
                            });
                        }
                    }
                    else
                    {
                        LogHelper.InsertLogTelegram("notify/message/send.json" + "token =" + token + "==> Notify không thể gửi vì các tham số bị thiếu.");
                        return Ok(new
                        {
                            status = (int)ResponseType.EMPTY,
                            msg = "Notify missing !!!"
                        });
                    }
                }
                else
                {

                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key invalid!",

                    });
                }
            }
            catch (Exception ex)
            {

                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Notify missing !!!" + ex.ToString()
                });
            }
        }

        [HttpPost("notify/message/update-status-view.json")]
        public async Task<ActionResult> updateStatusReadNotify(string token)
        {
            try
            {
                JArray objParr = null;
                var j_param = new Dictionary<string, object>
                {
                    {"notify_id", "64c4fab98a016634cdddced5,64c4fac68a016634cdddced7"},
                    {"user_seen_id", "34"},
                    {"seen_status", "1"}
                };
                var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                int user_seen_id = -1;
                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:b2c"]))
                {
                    var notify_id = objParr[0]["notify_id"].ToString();
                    var seen_status = Convert.ToInt16(objParr[0]["seen_status"]);
                    user_seen_id = Convert.ToInt32(objParr[0]["user_seen_id"]);

                    var arr_notify_id = notify_id.Split(",");

                    var rs = await _notifyMongoDAL.updateSeenNotify(arr_notify_id.ToList(), seen_status, user_seen_id);
                    if (rs)
                    {
                        // clear cache
                        string cache_name = "NOTIFY_" + user_seen_id;
                        int db_index = Convert.ToInt32(_configuration["Redis:Database:db_core"]);
                       // redisService.clear(cache_name, db_index);

                        // Update lại cho cache này
                        var obj_notify = await _notifyMongoDAL.getListNotify(user_seen_id);
                        if (obj_notify != null)
                        {
                            //redisService.Set(cache_name, JsonConvert.SerializeObject(obj_notify), db_index);
                            _subscriber.Publish(cache_name, JsonConvert.SerializeObject(obj_notify));
                        }
                    }
                    return Ok(new
                    {
                        status = rs ? (int)ResponseType.SUCCESS : (int)ResponseType.ERROR,
                    });

                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "error with preview notify: " + token
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "error with preview notify:  ex " + ex.ToString()
                });
            }
        }



    }
}
