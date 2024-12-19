using API_CORE.Controllers.PAYMENT.Base;
using API_CORE.Controllers.PAYMENT.ONEPAY.Base;
using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Fly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.API.Service.Queue;

namespace API_CORE.Controllers.PAYMENT
{
    //Document OnePay : https://drive.google.com/open?id=1ONy_RROunTPeEf73hoDTyijPud0wdX9e  
    [Route("api")]
    [ApiController]
    public class PaymentController : Controller
    {
        private IConfiguration configuration;
        private IOrderRepository ordersRepository;
        private IIdentifierServiceRepository identifierServiceRepository;
        private IDepositHistoryRepository iDepositHistoryRepository;
        private IFlyBookingMongoRepository bookingRepository;

        public PaymentController(IConfiguration _configuration, IOrderRepository _ordersRepository, IIdentifierServiceRepository _identifierServiceRepository, IDepositHistoryRepository _iDepositHistoryRepository, IFlyBookingMongoRepository _bookingRepository)
        {
            configuration = _configuration;
            ordersRepository = _ordersRepository;
            identifierServiceRepository = _identifierServiceRepository;
            iDepositHistoryRepository = _iDepositHistoryRepository;
            bookingRepository = _bookingRepository;
        }
        #region CHECKOUT VÉ MÁY BAY B2C
        /// <summary>
        /// source_payment_type: Phân biệt giữa các hệ thống push với nhau
        /// </summary>
        /// <param name="token"></param>
        /// <param name="source_payment_type"></param>
        /// <returns></returns>
        [HttpPost("payment/checkout.json")]
        public async Task<ActionResult> checkoutFly(string token, int source_payment_type)
        {
            try
            {
                JArray objParr = null;
                string url = string.Empty;
                long order_id = -1;

                #region Test

                var j_param = new Dictionary<string, object>
                {
                  {"payment_type", "2"},
                  {"return_url", "https://qc-api.adavigo.com/api/onepay/reveiver-data"},
                  {"client_id", "123"},
                  {"bank_code", ""},
                  {"booking_cart_id","64ac24ff9e494ec0edcdd595"}, // là khóa chính của giỏ hàng chứa danh sách các thông tin vé
                  {"event_status","1"},
                  {"order_id","-1"}, // >0 thanh toan lai
                  {"amount","100000"}// Tông số tiền càn thanh toán
                };
                var data_product = JsonConvert.SerializeObject(j_param);

                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion


                // LogHelper.InsertLogTelegram("payment / checkout.json--> token " + token.ToString());

                #region check key
                string private_token_key=configuration["DataBaseConfig:key_api:b2c"] ;
                if (!CommonHelper.GetParamWithKey(token, out objParr, private_token_key))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }

                #endregion
                var payment_service = new PaymentService();
                string order_no = string.Empty;
                string return_url = objParr[0]["return_url"].ToString();
                //-- Convert to AccountClientID:
                long account_client_id = Convert.ToInt64(objParr[0]["client_id"]); //173

                int payment_type = Convert.ToInt32(objParr[0]["payment_type"]);
                string bank_code = objParr[0]["bank_code"].ToString();
                string session_id = objParr[0]["session_id"].ToString(); //"DC12508SGNHAN010623100230714";
                int event_status = Convert.ToInt32(objParr[0]["event_status"]);
                int order_id_push = Convert.ToInt32(objParr[0]["order_id"]); // >0 là thanh toán lại  | -1 là tạo mới

                var booking_detail = await bookingRepository.getBookingBySessionId(session_id.Split(","), Convert.ToInt32(account_client_id));
                var baseUrl = Request.Scheme + "://" + Request.Host.Value;
                double amount = Convert.ToDouble(objParr[0]["amount"]);  // tổng số tiền thanh toán trước giảm từ voucher
                double discount = await payment_service.getDiscountBySessionId(booking_detail, baseUrl, account_client_id, amount, private_token_key); // get discount
                amount = amount - discount; // tru giam gia neu co

                #region Detect Payment
                switch (payment_type)
                {
                    case (Int16)PaymentType.CHUYEN_KHOAN_TRUC_TIEP:
                        break;

                    case (Int16)PaymentType.ATM:
                        bank_code = objParr[0]["bank_code"].ToString();
                        bank_code = string.IsNullOrEmpty(bank_code) ? "DOMESTIC" : objParr[0]["bank_code"].ToString();
                        break;

                    case (Int16)PaymentType.VISA_MASTER_CARD:
                        bank_code = "INTERNATIONAL";
                        break;

                    case (Int16)PaymentType.QR_PAY:
                        bank_code = "QR";
                        break;
                    case (Int16)PaymentType.GIU_CHO:
                        break;
                    default:
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Hình thức thanh toán không hợp lệ"
                        });
                }
                #endregion

                #region Get OrderNo

                if (event_status == (Int16)PaymentEventType.TAOMOI)
                {
                    order_no = await identifierServiceRepository.buildOrderNo((Int16)ServicesType.FlyingTicket, source_payment_type);

                    // Create new order no
                    order_id = await ordersRepository.CreateOrderNo(order_no);
                    if (order_id < 0)
                    {
                        LogHelper.InsertLogTelegram("payment / checkout.json error: Lỗi không tạo được OrderID token = " + token);
                    }
                }
                else if (event_status == (Int16)PaymentEventType.THANH_TOAN_LAI)
                {
                    order_no = await ordersRepository.getOrderNoByOrderId(order_id_push);
                    order_id = order_id_push;
                    if (order_no == "")
                    {
                        LogHelper.InsertLogTelegram("payment / checkout.json error: Lỗi không lay ra duoc so hop dong order_id_push = " + order_id_push + " token = " + token);
                    }
                }
                else
                {
                    LogHelper.InsertLogTelegram("payment/checkout.json THANH_TOAN_LAI Even Type ko hop le" + token);
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Even Type ko hop le"
                    });
                }
                #endregion

                #region Create new Order --> Push to Queue
                var j_param_queue = new Dictionary<string, string>
                {
                    {"payment_type",payment_type.ToString()},
                    {"account_client_id", account_client_id.ToString()},
                    {"bank_code", bank_code},
                    {"session_id",session_id},
                    {"order_no", order_no},
                    {"order_id", order_id.ToString()},
                    {"event_status",event_status.ToString()},// 1: thanh toan lai | 0: tao moi don hang
                    {"service_type", ((Int16)ServicesType.FlyingTicket).ToString()},
                    {"source_payment_type", source_payment_type.ToString()},
                    {"client_type",source_payment_type == (int)SourcePaymentType.b2c ? ((Int16)ClientType.CUSTOMER).ToString() : ((Int16)ClientType.TIER_1_AGENT).ToString() }
                };

                #region Thực hiện cập nhật lại orderno vào SQL và backup input queue. Mục đích: re-push queue nếu lỗi bot tạo đơn
                await ordersRepository.BackupBookingInfo(order_id, JsonConvert.SerializeObject(j_param_queue));
                #endregion

                var work_queue = new WorkQueueClient(configuration);
                var queue_setting = new QueueSettingViewModel
                {
                    host = configuration["Queue:Host"],
                    v_host = configuration["Queue:V_Host"],
                    port = Convert.ToInt32(configuration["Queue:Port"]),
                    username = configuration["Queue:Username"],
                    password = configuration["Queue:Password"]
                };
                var response_queue = work_queue.InsertQueueSimpleWithDurable(queue_setting, JsonConvert.SerializeObject(j_param_queue), QueueName.CheckoutOrder);
                if (!response_queue) LogHelper.InsertLogTelegram("payment / checkout.json: push queue thất bại. CHeck lại Consummer Checkout");
                #endregion


                if (payment_type != (Int16)PaymentType.CHUYEN_KHOAN_TRUC_TIEP && payment_type != (Int16)PaymentType.GIU_CHO)
                {
                    #region Kết nối với ONEPAY
                    string vpc_MerchTxnRef = CommonHelper.buildTimeSpanCurrent();
                    var pay = new OnePayService(configuration, "TNHH Adavigo", vpc_MerchTxnRef, order_no, amount, return_url, (int)account_client_id, "Số 289A Khuất Duy Tiến, Cầu Giấy, Hà Nội", "0936191192", "email", bank_code);
                    url = pay.sendPaymentToOnePay();
                }
                #endregion

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Transaction is successful",
                    url = url,
                    order_no = order_no,
                    order_id = order_id,
                    content = order_no + " CHUYEN KHOAN"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("payment / checkout.json" + ex.ToString() + "token =" + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Transaction Error !!!"
                });
            }

        }
        #endregion

        /// <summary>
        #region GIAO DỊCH TRANSACTION B2B
        /// </summary>
        /// <param name="token"></param>
        /// <param name="source_payment_type"></param>
        /// <returns></returns>
        [HttpPost("b2b/trans/payment/checkout.json")]
        public async Task<ActionResult> checkoutTrans(string token)
        {
            try
            {
                JArray objParr = null;
                string url = string.Empty;
                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"payment_type", "1"},  //  1: CHuyển khoản ngân hàng                
                    {"user_id", "159"},
                    {"trans_no", "KS2300036"},
                    {"bank_name","Vietcombank"}
                };
                var data = JsonConvert.SerializeObject(j_param);

                //  token = CommonHelper.Encode(data, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                #region check key                
                if (!CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }
                #endregion 


                int payment_type = Convert.ToInt32(objParr[0]["payment_type"]);
                Int64 user_id = Convert.ToInt64(objParr[0]["user_id"]);
                string trans_no = objParr[0]["trans_no"].ToString();
                string bank_name = objParr[0]["bank_name"].ToString();

                var is_checkout = await iDepositHistoryRepository.checkOutDeposit(user_id, trans_no, bank_name);
                if (is_checkout)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Transaction is successful"
                    });
                }
                else
                {
                    LogHelper.InsertLogTelegram("b2b/trans/payment/checkout.json error token =" + token);
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Transaction is FAILED"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("payment / checkoutTrans.json" + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Transaction Error !!!"
                });
            }

        }

        [HttpPost("b2b/payment/update-proof-trans.json")]
        public async Task<ActionResult> updateProofTrans(string token)
        {
            try
            {
                JArray objParr = null;
                string url = string.Empty;
                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"user_id", "159"},
                    {"trans_no", "KS2300036"},
                    {"link_proof","link_proof"}
                };
                var data = JsonConvert.SerializeObject(j_param);

                // token = CommonHelper.Encode(data, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                #region check key                
                if (!CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }
                #endregion

                Int64 user_id = Convert.ToInt64(objParr[0]["user_id"]);
                string trans_no = objParr[0]["trans_no"].ToString();
                string link_proof = objParr[0]["link_proof"].ToString();

                var is_checkout = await iDepositHistoryRepository.updateProofTrans(user_id, trans_no, link_proof);
                if (is_checkout)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Transaction is successful"
                    });
                }
                else
                {
                    LogHelper.InsertLogTelegram("b2b/trans/payment/checkout.json error token =" + token);
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Transaction is FAILED"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("payment / checkout.json" + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Transaction Error !!!"
                });
            }

        }

        [HttpPost("b2b/payment/verify-trans.json")]
        public async Task<ActionResult> VerifyTrans(string token)
        {
            try
            {
                JArray objParr = null;
                string url = string.Empty;
                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"role_verify", "1"}, // 0: bot | 1: ke toan
                    {"trans_no", "KS2300036"},
                    {"status", "1"} // 

                };
                var data = JsonConvert.SerializeObject(j_param);

                // token = CommonHelper.Encode(data, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                #region check key                
                if (!CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }
                #endregion

                Int64 user_id = Convert.ToInt64(objParr[0]["user_id"]);
                string trans_no = objParr[0]["trans_no"].ToString();
                string link_proof = objParr[0]["link_proof"].ToString();

                var is_checkout = await iDepositHistoryRepository.updateProofTrans(user_id, trans_no, link_proof);
                if (is_checkout)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Transaction is successful"
                    });
                }
                else
                {
                    LogHelper.InsertLogTelegram("b2b/trans/payment/checkout.json error token =" + token);
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Transaction is FAILED"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("payment / checkout.json" + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Transaction Error !!!"
                });
            }

        }

        [HttpPost("b2b/trans/bot-verify/update.json")]
        public async Task<ActionResult> BotVerifyTrans(string token)
        {
            try
            {
                JArray objParr = null;
                string url = string.Empty;
                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"trans_no", "KS2300036"}
                };
                var data = JsonConvert.SerializeObject(j_param);

                // token = CommonHelper.Encode(data, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                #region check key                
                if (!CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }
                #endregion

                string trans_no = objParr[0]["trans_no"].ToString();

                var is_checkout = await iDepositHistoryRepository.BotVerifyTrans(trans_no);
                if (is_checkout)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Verify Transaction is successful"
                    });
                }
                else
                {
                    LogHelper.InsertLogTelegram("b2b/trans/bot-verify/update.json error iDepositHistoryRepository token =" + token);
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Verify Transaction is FAILED"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("b2b/trans/bot-verify/update.json error token =" + token + "--. ex=" + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Transaction Error !!!"
                });
            }

        }

        #region KE TOAN DUYỆT TRANS
        [HttpPost("b2b/trans/accountant-verify/update.json")]
        public async Task<ActionResult> AccountantVerifyTrans(string token)
        {
            try
            {
                JArray objParr = null;
                string url = string.Empty;
                #region Test
                var j_param = new Dictionary<string, object>
                {
                     {"trans_no", "KS2300036"},
                     {"is_verify", "0"}, // 0: dong y, 1 tu choi
                     {"user_verify", "165"}, // Account clientid kiem duyet
                     {"note", "chua du tien"}, // ly do tu choi
                     {"contract_pay_id", "1111"} //id của phiếu thu sao khi tạo phiếu thu thành công
                };
                var data = JsonConvert.SerializeObject(j_param);


                // token = CommonHelper.Encode(data, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                #region check key                
                if (!CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }
                #endregion
                Int16 user_verify = Convert.ToInt16(objParr[0]["user_verify"]);
                string trans_no = objParr[0]["trans_no"].ToString();
                Int16 is_verify = Convert.ToInt16(objParr[0]["is_verify"]);
                string note = objParr[0]["note"] == null ? "" : objParr[0]["note"].ToString();
                var contract_pay_id = Convert.ToInt32(objParr[0]["contract_pay_id"]);
                var is_checkout = await iDepositHistoryRepository.VerifyTrans(trans_no, is_verify, note, user_verify, contract_pay_id);
                if (is_checkout)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success"
                    });
                }
                else
                {
                    LogHelper.InsertLogTelegram("b2b/trans/accountant-verify/update.json error AccountantVerifyTrans token =" + token);
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "FAILED"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("b2b/trans/accountant-verify/update.json error token =" + token + "--. ex=" + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Verify Error !!!"
                });
            }

        }
        #endregion

        #endregion

        #region CHECKOUT KHÁCH SẠN B2B
        /// <summary>
        /// source_payment_type: Phân biệt giữa các hệ thống push với nhau
        /// </summary>
        /// <param name="token"></param>
        /// <param name="source_payment_type"></param>
        /// <returns></returns>
        [HttpPost("b2b/hotel/payment/checkout.json")]
        public async Task<ActionResult> checkoutHotel(string token)
        {
            try
            {
                JArray objParr = null;
                string url = string.Empty;
                long order_id = -1;

                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"booking_id","63ee01d6ba042fe41bedc223,63ee01d6ba042fe41bedc223,63ee01d6ba042fe41bedc223"}, // danh sách booking của khách sạn là key trong bảng BookingHotel
                    {"payment_type", "2"}, // Loại hình thanh toán
                    {"return_url", "https://qc-b2b.adavigo.com/onepay/reveiver-data"}, // Là link chuyển tới sau khi khách hàng thanh toán thành công bên cổng thanh toán
                    {"client_id", "123"}, // user id login thực hiện giao dịch này
                    {"bank_code", "VCB"}, // mã ngân hàng khi khách chọn ngoài cổng thanh toán
                    {"event_status","1"}, //0 thanh toan lai : 1 tạo mới
                    {"order_id","-1"}, // mã đơn hàng
                    {"amount","100000"},// Tông số tiền càn thanh toán
                    {"system_type","0"},//0= b2b ,1= b2c
                };
                var data_product = JsonConvert.SerializeObject(j_param);

                //  token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                //LogHelper.InsertLogTelegram("payment / checkout.json--> token " + token.ToString());

                #region check key
                string private_token_key = configuration["DataBaseConfig:key_api:b2b"];
                if (!CommonHelper.GetParamWithKey(token, out objParr, private_token_key))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }

                #endregion                

                string order_no = string.Empty;
                string return_url = objParr[0]["return_url"].ToString();
                //-- Convert to AccountClientID:
                long account_client_id = Convert.ToInt64(objParr[0]["client_id"]);

                int payment_type = Convert.ToInt32(objParr[0]["payment_type"]);
                string bank_code = objParr[0]["bank_code"].ToString();
                string booking_id = objParr[0]["booking_id"].ToString();
                int event_status = Convert.ToInt32(objParr[0]["event_status"]);
                int order_id_push = Convert.ToInt32(objParr[0]["order_id"]); // >0 là thanh toán lại  | -1 là tạo mới
                double amount = Convert.ToDouble(objParr[0]["amount"]); // tổng số tiền thanh toán
                int system_type = Convert.ToInt32(objParr[0]["system_type"]);//0= b2b ,1= b2c
                #region Detect Payment
                switch (payment_type)
                {
                    case (Int16)PaymentType.CHUYEN_KHOAN_TRUC_TIEP:
                        break;

                    case (Int16)PaymentType.ATM:
                        bank_code = objParr[0]["bank_code"].ToString();
                        bank_code = string.IsNullOrEmpty(bank_code) ? "DOMESTIC" : objParr[0]["bank_code"].ToString();
                        break;

                    case (Int16)PaymentType.VISA_MASTER_CARD:
                        bank_code = "INTERNATIONAL";
                        break;

                    case (Int16)PaymentType.QR_PAY:
                        bank_code = "QR";
                        break;
                    case (Int16)PaymentType.GIU_CHO:
                        break;
                    case (Int16)PaymentType.KY_QUY:
                        break;
                    default:
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Hình thức thanh toán không hợp lệ"
                        });
                }
                #endregion

                #region Get OrderNo

                if (event_status == (Int16)PaymentEventType.TAOMOI)
                {
                    order_no = await identifierServiceRepository.buildOrderNo((Int16)ServicesType.VINHotelRent, system_type);

                    // Create new order no
                    order_id = await ordersRepository.CreateOrderNo(order_no);
                    if (order_id < 0)
                    {
                        LogHelper.InsertLogTelegram("b2b/hotel/payment/checkout.json error: Lỗi không tạo được OrderID token = " + token);
                    }
                }
                else if (event_status == (Int16)PaymentEventType.THANH_TOAN_LAI)
                {
                    order_no = await ordersRepository.getOrderNoByOrderId(order_id_push);
                    order_id = order_id_push;
                    if (order_no == "")
                    {
                        LogHelper.InsertLogTelegram("b2b/hotel/payment/checkout.json Lỗi không lay ra duoc so hop dong order_id_push = " + order_id_push + " token = " + token);
                    }
                }
                else
                {
                    LogHelper.InsertLogTelegram("b2b/hotel/payment/checkout.json THANH_TOAN_LAI Even Type ko hop le" + token);
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Even Type ko hop le"
                    });
                }
                #endregion

                #region Create new Order --> Push to Queue
                var j_param_queue = new Dictionary<string, string>
                {
                    {"payment_type",payment_type.ToString()},
                    {"account_client_id", account_client_id.ToString()},
                    {"bank_code", bank_code},
                    {"booking_id",booking_id},
                    {"order_no", order_no},
                    {"order_id", order_id.ToString()},
                    {"event_status",event_status.ToString()},
                    {"service_type", ((Int16)ServicesType.VINHotelRent).ToString()},
                    {"source_payment_type", ((Int16)SourcePaymentType.b2b).ToString()},
                    {"client_type", ((Int16)ClientType.TIER_1_AGENT).ToString() }
                };
                var work_queue = new WorkQueueClient(configuration);
                var queue_setting = new QueueSettingViewModel
                {
                    host = configuration["Queue:Host"],
                    v_host = configuration["Queue:V_Host"],
                    port = Convert.ToInt32(configuration["Queue:Port"]),
                    username = configuration["Queue:Username"],
                    password = configuration["Queue:Password"]
                };
                var response_queue = work_queue.InsertQueueSimpleWithDurable(queue_setting, JsonConvert.SerializeObject(j_param_queue), QueueName.CheckoutOrder);
                if (!response_queue) LogHelper.InsertLogTelegram("payment / checkout.json: push queue thất bại. CHeck lại Consummer Checkout");
                #endregion

                if (payment_type != (Int16)PaymentType.CHUYEN_KHOAN_TRUC_TIEP && payment_type != (Int16)PaymentType.GIU_CHO)
                {
                    #region Kết nối với ONEPAY
                    string vpc_MerchTxnRef = CommonHelper.buildTimeSpanCurrent();
                    var pay = new OnePayService(configuration, "TNHH Adavigo", vpc_MerchTxnRef, order_no, amount, return_url, (int)account_client_id, "Số 289A Khuất Duy Tiến, Cầu Giấy, Hà Nội", "0936191192", "email", bank_code);
                    url = pay.sendPaymentToOnePay();
                }
                #endregion

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Transaction is successful",
                    url = url,
                    order_no = order_no,
                    order_id = order_id,
                    content = order_no + " CHUYEN KHOAN KHACH SAN"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("b2b/hotel/payment/checkout.json" + ex.ToString() + "token =" + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Transaction Error !!!"
                });
            }

        }


        #endregion

        #region CHECKOUT KHÁCH SẠN B2C
        /// <summary>
        /// source_payment_type: Phân biệt giữa các hệ thống push với nhau
        /// </summary>
        /// <param name="token"></param>
        /// <param name="source_payment_type"></param>
        /// <returns></returns>
        [HttpPost("b2c/hotel/checkout.json")]
        public async Task<ActionResult> CheckoutHotelB2C(string token)
        {
            try
            {
                JArray objParr = null;
                string url = string.Empty;
                long order_id = -1;

                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"booking_id","63ee01d6ba042fe41bedc223,63ee01d6ba042fe41bedc223,63ee01d6ba042fe41bedc223"}, // danh sách booking của khách sạn là key trong bảng BookingHotel
                    {"payment_type", "2"}, // Loại hình thanh toán
                    {"return_url", "https://qc-b2b.adavigo.com/onepay/reveiver-data"}, // Là link chuyển tới sau khi khách hàng thanh toán thành công bên cổng thanh toán
                    {"client_id", "123"}, // user id login thực hiện giao dịch này
                    {"bank_code", "VCB"}, // mã ngân hàng khi khách chọn ngoài cổng thanh toán
                    {"event_status","1"}, //0 thanh toan lai : 1 tạo mới
                    {"order_id","-1"}, // mã đơn hàng
                    {"amount","100000"},// Tông số tiền càn thanh toán
                    {"system_type","0"},//0= b2b ,1= b2c
                };
                var data_product = JsonConvert.SerializeObject(j_param);

                //  token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                //LogHelper.InsertLogTelegram("payment / checkout.json--> token " + token.ToString());

                #region check key
                string private_token_key = configuration["DataBaseConfig:key_api:b2c"];
                if (!CommonHelper.GetParamWithKey(token, out objParr, private_token_key))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }

                #endregion                
               
                string order_no = string.Empty;
                string return_url = objParr[0]["return_url"].ToString();
                //-- Convert to AccountClientID:
                long account_client_id = Convert.ToInt64(objParr[0]["client_id"]);

                int payment_type = Convert.ToInt32(objParr[0]["payment_type"]);
                string bank_code = objParr[0]["bank_code"].ToString();
                string booking_id = objParr[0]["booking_id"].ToString();
                int event_status = Convert.ToInt32(objParr[0]["event_status"]);
                int order_id_push = Convert.ToInt32(objParr[0]["order_id"]); // >0 là thanh toán lại  | -1 là tạo mới
                double amount = Convert.ToDouble(objParr[0]["amount"]); // tổng số tiền thanh toán
                #region Detect Payment
                switch (payment_type)
                {
                    case (Int16)PaymentType.CHUYEN_KHOAN_TRUC_TIEP:
                        break;

                    case (Int16)PaymentType.ATM:
                        bank_code = objParr[0]["bank_code"].ToString();
                        bank_code = string.IsNullOrEmpty(bank_code) ? "DOMESTIC" : objParr[0]["bank_code"].ToString();
                        break;

                    case (Int16)PaymentType.VISA_MASTER_CARD:
                        bank_code = "INTERNATIONAL";
                        break;

                    case (Int16)PaymentType.QR_PAY:
                        bank_code = "QR";
                        break;
                    case (Int16)PaymentType.GIU_CHO:
                        break;
                    default:
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Hình thức thanh toán không hợp lệ"
                        });
                }
                #endregion

                #region Get OrderNo

                if (event_status == (Int16)PaymentEventType.TAOMOI)
                {
                    order_no = await identifierServiceRepository.buildOrderNo((Int16)ServicesType.VINHotelRent, 1);

                    // Create new order no
                    order_id = await ordersRepository.CreateOrderNo(order_no);
                    if (order_id < 0)
                    {
                        LogHelper.InsertLogTelegram("b2b/hotel/payment/checkout.json error: Lỗi không tạo được OrderID token = " + token);
                    }
                }
                else if (event_status == (Int16)PaymentEventType.THANH_TOAN_LAI)
                {
                    order_no = await ordersRepository.getOrderNoByOrderId(order_id_push);
                    order_id = order_id_push;
                    if (order_no == "")
                    {
                        LogHelper.InsertLogTelegram("b2b/hotel/payment/checkout.json Lỗi không lay ra duoc so hop dong order_id_push = " + order_id_push + " token = " + token);
                    }
                }
                else
                {
                    LogHelper.InsertLogTelegram("b2b/hotel/payment/checkout.json THANH_TOAN_LAI Even Type ko hop le" + token);
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Even Type ko hop le"
                    });
                }
                #endregion

                #region Create new Order --> Push to Queue
                var j_param_queue = new Dictionary<string, string>
                {
                    {"payment_type",payment_type.ToString()},
                    {"account_client_id", account_client_id.ToString()},
                    {"bank_code", bank_code},
                    {"booking_id",booking_id},
                    {"order_no", order_no},
                    {"order_id", order_id.ToString()},
                    {"event_status",event_status.ToString()},
                    {"service_type", ((Int16)ServicesType.VINHotelRent).ToString()},
                    {"source_payment_type", ((Int16)SourcePaymentType.b2c).ToString()},
                    {"client_type", ((Int16)ClientType.CUSTOMER).ToString() }
                };
                var work_queue = new WorkQueueClient(configuration);
                var queue_setting = new QueueSettingViewModel
                {
                    host = configuration["Queue:Host"],
                    v_host = configuration["Queue:V_Host"],
                    port = Convert.ToInt32(configuration["Queue:Port"]),
                    username = configuration["Queue:Username"],
                    password = configuration["Queue:Password"]
                };
                var response_queue = work_queue.InsertQueueSimpleWithDurable(queue_setting, JsonConvert.SerializeObject(j_param_queue), QueueName.CheckoutOrder);
                if (!response_queue) LogHelper.InsertLogTelegram("payment / checkout.json: push queue thất bại. CHeck lại Consummer Checkout");
                #endregion

                if (payment_type != (Int16)PaymentType.CHUYEN_KHOAN_TRUC_TIEP && payment_type != (Int16)PaymentType.GIU_CHO)
                {
                    #region Kết nối với ONEPAY
                    string vpc_MerchTxnRef = CommonHelper.buildTimeSpanCurrent();
                    var pay = new OnePayService(configuration, "TNHH Adavigo", vpc_MerchTxnRef, order_no, amount, return_url, (int)account_client_id, "Số 289A Khuất Duy Tiến, Cầu Giấy, Hà Nội", "0936191192", "email", bank_code);
                    url = pay.sendPaymentToOnePay();
                }
                #endregion

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Transaction is successful",
                    url = url,
                    order_no = order_no,
                    order_id = order_id,
                    content = order_no + " CHUYEN KHOAN"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("b2c/hotel/payment/checkout.json" + ex.ToString() + "token =" + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Transaction Error !!!"
                });
            }

        }


        #endregion

        #region CHECKOUT vinwonder
        /// <summary>
        /// source_payment_type: Phân biệt giữa các hệ thống push với nhau
        /// OmMxIDteKxQVHEUAE1dne2NwY2hjIyQ2NDMvHTQjLwkMWD4NRgQGc2l7NTRCWEcsG1MHCRIuVj5JGTsDXiQIUVsmGilbDExXQVwhHxshKAQYNgMuO2hOYFMdPFY9OTo8A1tDTENSd0ZdcRELKQBpBhoyVgkDFRJVBzE3WV8BJT5ZTW5hKyQcLj0DFwZQNCcHbQAqbEJmYEIRLkhoJ3UBN2VRWTw+Q0RKVw8aF0p7S1UJWTQsRzRETWNQfHRcelZUGwlXQgxdRToKAHtpHl5AVFAeEBQ9GxIFGSMQNlt5Rzo=
        /// </summary>
        /// <param name="token"></param>
        /// <param name="source_payment_type"></param>
        /// <returns></returns>
        [HttpPost("payment/vinwonder/checkout.json")]
        public async Task<ActionResult> checkoutVinWonder(string token, int source_payment_type)
        {
            try
            {
                JArray objParr = null;
                string url = string.Empty;
                long order_id = -1;

                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"payment_type", "2"},
                    {"return_url", "https://qc-api.adavigo.com/api/onepay/reveiver-data"},
                    {"client_id", "123"},
                    {"bank_code", ""},
                    {"booking_cart_id",""},
                    {"event_status","1"},
                    {"order_id","-1"}, // >0 thanh toan lai
                    {"amount","100000"}// Tông số tiền càn thanh toán
                };
                var data_product = JsonConvert.SerializeObject(j_param);

                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion


                // LogHelper.InsertLogTelegram("payment / checkout.json--> token " + token.ToString());

                #region check key
                string private_token_key = source_payment_type == ((int)SourcePaymentType.b2c) ? configuration["DataBaseConfig:key_api:b2c"] : configuration["DataBaseConfig:key_api:b2b"];
                if (!CommonHelper.GetParamWithKey(token, out objParr, private_token_key))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }

                #endregion
                var payment_service = new PaymentService();
                string order_no = string.Empty;
                string return_url = objParr[0]["return_url"].ToString();
                //-- Convert to AccountClientID:
                long account_client_id = Convert.ToInt64(objParr[0]["client_id"]); //173

                int payment_type = Convert.ToInt32(objParr[0]["payment_type"]);
                string bank_code = objParr[0]["bank_code"].ToString();
                string booking_cart_id = objParr[0]["booking_cart_id"].ToString(); //"DC12508SGNHAN010623100230714";
                int event_status = Convert.ToInt32(objParr[0]["event_status"]);
                int order_id_push = Convert.ToInt32(objParr[0]["order_id"]); // >0 là thanh toán lại  | -1 là tạo mới

                // var booking_detail = await bookingRepository.getBookingBySessionId(booking_cart_id.Split(","), Convert.ToInt32(account_client_id));
                var baseUrl = Request.Scheme + "://" + Request.Host.Value;
                double amount = Convert.ToDouble(objParr[0]["amount"]);  // tổng số tiền thanh toán trước giảm từ voucher


                #region Detect Payment
                switch (payment_type)
                {
                    case (Int16)PaymentType.CHUYEN_KHOAN_TRUC_TIEP:
                        break;

                    case (Int16)PaymentType.ATM:
                        bank_code = objParr[0]["bank_code"].ToString();
                        bank_code = string.IsNullOrEmpty(bank_code) ? "DOMESTIC" : objParr[0]["bank_code"].ToString();
                        break;

                    case (Int16)PaymentType.VISA_MASTER_CARD:
                        bank_code = "INTERNATIONAL";
                        break;

                    case (Int16)PaymentType.QR_PAY:
                        bank_code = "QR";
                        break;
                    case (Int16)PaymentType.GIU_CHO:
                        break;
                    default:
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Hình thức thanh toán không hợp lệ"
                        });
                }
                #endregion

                #region Get OrderNo

                if (event_status == (Int16)PaymentEventType.TAOMOI)
                {
                    order_no = await identifierServiceRepository.buildOrderNo((Int16)ServicesType.VinWonderTicket, source_payment_type);

                    // Create new order no
                    order_id = await ordersRepository.CreateOrderNo(order_no);
                    if (order_id < 0)
                    {
                        LogHelper.InsertLogTelegram("payment / checkout.json error: Lỗi không tạo được OrderID token = " + token);
                    }
                }
                else if (event_status == (Int16)PaymentEventType.THANH_TOAN_LAI)
                {
                    order_no = await ordersRepository.getOrderNoByOrderId(order_id_push);
                    order_id = order_id_push;
                    if (order_no == "")
                    {
                        LogHelper.InsertLogTelegram("payment / checkout.json error: Lỗi không lay ra duoc so hop dong order_id_push = " + order_id_push + " token = " + token);
                    }
                }
                else
                {
                    LogHelper.InsertLogTelegram("payment/checkout.json THANH_TOAN_LAI Even Type ko hop le" + token);
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Even Type ko hop le"
                    });
                }
                #endregion

                #region Create new Order --> Push to Queue
                var j_param_queue = new Dictionary<string, string>
                {
                    {"payment_type",payment_type.ToString()},
                    {"account_client_id", account_client_id.ToString()},
                    {"bank_code", bank_code},
                    {"booking_cart_id",booking_cart_id},
                    {"order_no", order_no},
                    {"order_id", order_id.ToString()},
                    {"event_status",event_status.ToString()},// 1: thanh toan lai | 0: tao moi don hang
                    {"service_type", ((Int16)ServicesType.VinWonderTicket).ToString()},
                    {"source_payment_type", source_payment_type.ToString()},
                    {"client_type",source_payment_type == (int)SourcePaymentType.b2c ? ((Int16)ClientType.CUSTOMER).ToString() : ((Int16)ClientType.TIER_1_AGENT).ToString() }
                };

                #region Thực hiện cập nhật lại orderno vào SQL và backup input queue. Mục đích: re-push queue nếu lỗi bot tạo đơn
                await ordersRepository.BackupBookingInfo(order_id, JsonConvert.SerializeObject(j_param_queue));
                #endregion

                var work_queue = new WorkQueueClient(configuration);
                var queue_setting = new QueueSettingViewModel
                {
                    host = configuration["Queue:Host"],
                    v_host = configuration["Queue:V_Host"],
                    port = Convert.ToInt32(configuration["Queue:Port"]),
                    username = configuration["Queue:Username"],
                    password = configuration["Queue:Password"]
                };
                var response_queue = work_queue.InsertQueueSimpleWithDurable(queue_setting, JsonConvert.SerializeObject(j_param_queue), QueueName.CheckoutOrder);
                if (!response_queue) LogHelper.InsertLogTelegram("payment / checkout.json: push queue thất bại. CHeck lại Consummer Checkout");
                #endregion


                if (payment_type != (Int16)PaymentType.CHUYEN_KHOAN_TRUC_TIEP && payment_type != (Int16)PaymentType.GIU_CHO)
                {
                    #region Kết nối với ONEPAY
                    string vpc_MerchTxnRef = CommonHelper.buildTimeSpanCurrent();
                    var pay = new OnePayService(configuration, "TNHH Adavigo", vpc_MerchTxnRef, order_no, amount, return_url, (int)account_client_id, "Số 289A Khuất Duy Tiến, Cầu Giấy, Hà Nội", "0936191192", "email", bank_code);
                    url = pay.sendPaymentToOnePay();
                }
                #endregion

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Transaction is successful",
                    url = url,
                    order_no = order_no,
                    order_id = order_id,
                    content = order_no + " CHUYEN KHOAN"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("payment / checkout.json" + ex.ToString() + "token =" + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Transaction Error !!!"
                });
            }
        }

        /// <summary>
        /// DAT TOUR
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("booking/tour.json")]
        public async Task<ActionResult> bookingTour(string token)
        {
            try
            {
                JArray objParr = null;
              
                string private_token_key = configuration["DataBaseConfig:key_api:b2c"];
                if (!CommonHelper.GetParamWithKey(token, out objParr, private_token_key))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }
                else
                {
                    var work_queue = new WorkQueueClient(configuration);

                    // sinh mã đơn
                    string order_no = await identifierServiceRepository.buildOrderNo((Int16)ServicesType.Tourist, (Int16)SourcePaymentType.b2c);

                    // Create new order no
                    long order_id = await ordersRepository.CreateOrderNo(order_no);
                    if (order_id < 0)
                    {
                        LogHelper.InsertLogTelegram("payment / booking/tour.json error: Lỗi không tạo được tour  token = " + token);
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Booking Tour thất bại. Vui lòng liên hệ với bộ phận chăm sóc khách hàng để được hỗ trợ"
                        });
                    }

                    #region Create new Order --> Push to Queue
                    var j_param_queue = new Dictionary<string, string>
                    {                  
                        {"order_no", order_no},
                        {"order_id", order_id.ToString()},
                         {"service_type", ((Int16)ServicesType.Tourist).ToString()},
                        {"tour_info", objParr.ToString()}
                    };

                    #region Thực hiện cập nhật lại orderno vào SQL và backup input queue. Mục đích: re-push queue nếu lỗi bot tạo đơn
                    await ordersRepository.BackupBookingInfo(order_id, JsonConvert.SerializeObject(j_param_queue));
                    #endregion

                    var queue_setting = new QueueSettingViewModel
                    {
                        host = configuration["Queue:Host"],
                        v_host = configuration["Queue:V_Host"],
                        port = Convert.ToInt32(configuration["Queue:Port"]),
                        username = configuration["Queue:Username"],
                        password = configuration["Queue:Password"]
                    };
                    var response_queue = work_queue.InsertQueueSimpleWithDurable(queue_setting, JsonConvert.SerializeObject(j_param_queue), QueueName.CheckoutOrder);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Booking thành công",
                        order_no = order_no
                    });
                }
                #endregion   
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("payment /booking/tour.json" + ex.ToString() + "token =" + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Transaction Error !!!"
                });
            }

        }
        [HttpPost("b2b/tour/payment/checkout.json")]
        public async Task<ActionResult> checkoutTour(string token)
        {
            try
            {
                JArray objParr = null;
                string url = string.Empty;
                long order_id = -1;

                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"booking_id","63ee01d6ba042fe41bedc223,63ee01d6ba042fe41bedc223,63ee01d6ba042fe41bedc223"}, // danh sách booking của khách sạn là key trong bảng BookingHotel
                    {"payment_type", "2"}, // Loại hình thanh toán
                    {"return_url", "https://qc-b2b.adavigo.com/onepay/reveiver-data"}, // Là link chuyển tới sau khi khách hàng thanh toán thành công bên cổng thanh toán
                    {"client_id", "123"}, // user id login thực hiện giao dịch này
                    {"bank_code", "VCB"}, // mã ngân hàng khi khách chọn ngoài cổng thanh toán
                    {"event_status","1"}, //0 thanh toan lai : 1 tạo mới
                    {"order_id","-1"}, // mã đơn hàng
                    {"amount","100000"},// Tông số tiền càn thanh toán
                    {"system_type","0"},//0= b2b ,1= b2c
                };
                var data_product = JsonConvert.SerializeObject(j_param);

                //  token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                //LogHelper.InsertLogTelegram("payment / checkout.json--> token " + token.ToString());

                #region check key
                string private_token_key = configuration["DataBaseConfig:key_api:b2b"];
                if (!CommonHelper.GetParamWithKey(token, out objParr, private_token_key))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }

                #endregion                

                string order_no = string.Empty;
                string return_url = objParr[0]["return_url"].ToString();
                //-- Convert to AccountClientID:
                long account_client_id = Convert.ToInt64(objParr[0]["client_id"]);

                int payment_type = Convert.ToInt32(objParr[0]["payment_type"]);
                string bank_code = objParr[0]["bank_code"].ToString();
                string booking_id = objParr[0]["booking_id"].ToString();
                int event_status = Convert.ToInt32(objParr[0]["event_status"]);
                int order_id_push = Convert.ToInt32(objParr[0]["order_id"]); // >0 là thanh toán lại  | -1 là tạo mới
                double amount = Convert.ToDouble(objParr[0]["amount"]); // tổng số tiền thanh toán
                int system_type = Convert.ToInt32(objParr[0]["system_type"]);//0= b2b ,1= b2c
                #region Detect Payment
                switch (payment_type)
                {
                    case (Int16)PaymentType.CHUYEN_KHOAN_TRUC_TIEP:
                        break;

                    case (Int16)PaymentType.ATM:
                        bank_code = objParr[0]["bank_code"].ToString();
                        bank_code = string.IsNullOrEmpty(bank_code) ? "DOMESTIC" : objParr[0]["bank_code"].ToString();
                        break;

                    case (Int16)PaymentType.VISA_MASTER_CARD:
                        bank_code = "INTERNATIONAL";
                        break;

                    case (Int16)PaymentType.QR_PAY:
                        bank_code = "QR";
                        break;
                    case (Int16)PaymentType.GIU_CHO:
                        break;
                    case (Int16)PaymentType.KY_QUY:
                        break;
                    default:
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Hình thức thanh toán không hợp lệ"
                        });
                }
                #endregion

                #region Get OrderNo

                if (event_status == (Int16)PaymentEventType.TAOMOI)
                {
                    order_no = await identifierServiceRepository.buildOrderNo((Int16)ServicesType.Tourist, system_type);

                    // Create new order no
                    order_id = await ordersRepository.CreateOrderNo(order_no);
                    if (order_id < 0)
                    {
                        LogHelper.InsertLogTelegram("b2b/tour/payment/checkout.json error: Lỗi không tạo được OrderID token = " + token);
                    }
                }
                else if (event_status == (Int16)PaymentEventType.THANH_TOAN_LAI)
                {
                    order_no = await ordersRepository.getOrderNoByOrderId(order_id_push);
                    order_id = order_id_push;
                    if (order_no == "")
                    {
                        LogHelper.InsertLogTelegram("b2b/tour/payment/checkout.json Lỗi không lay ra duoc so hop dong order_id_push = " + order_id_push + " token = " + token);
                    }
                }
                else
                {
                    LogHelper.InsertLogTelegram("b2b/tour/payment/checkout.json THANH_TOAN_LAI Even Type ko hop le" + token);
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Even Type ko hop le"
                    });
                }
                #endregion

                #region Create new Order --> Push to Queue
                var j_param_queue = new Dictionary<string, string>
                {
                    {"payment_type",payment_type.ToString()},
                    {"account_client_id", account_client_id.ToString()},
                    {"bank_code", bank_code},
                    {"booking_id",booking_id},
                    {"order_no", order_no},
                    {"order_id", order_id.ToString()},
                    {"event_status",event_status.ToString()},
                    {"service_type", ((Int16)ServicesType.Tourist).ToString()},
                    {"source_payment_type", ((Int16)SourcePaymentType.b2b).ToString()},
                    {"client_type", ((Int16)ClientType.TIER_1_AGENT).ToString() }
                };
                var work_queue = new WorkQueueClient(configuration);
                var queue_setting = new QueueSettingViewModel
                {
                    host = configuration["Queue:Host"],
                    v_host = configuration["Queue:V_Host"],
                    port = Convert.ToInt32(configuration["Queue:Port"]),
                    username = configuration["Queue:Username"],
                    password = configuration["Queue:Password"]
                };
                var response_queue = work_queue.InsertQueueSimpleWithDurable(queue_setting, JsonConvert.SerializeObject(j_param_queue), QueueName.CheckoutOrder);
                if (!response_queue) LogHelper.InsertLogTelegram("payment / checkout.json: push queue thất bại. CHeck lại Consummer Checkout");
                #endregion

                if (payment_type != (Int16)PaymentType.CHUYEN_KHOAN_TRUC_TIEP && payment_type != (Int16)PaymentType.GIU_CHO)
                {
                    #region Kết nối với ONEPAY
                    string vpc_MerchTxnRef = CommonHelper.buildTimeSpanCurrent();
                    var pay = new OnePayService(configuration, "TNHH Adavigo", vpc_MerchTxnRef, order_no, amount, return_url, (int)account_client_id, "Số 289A Khuất Duy Tiến, Cầu Giấy, Hà Nội", "0936191192", "email", bank_code);
                    url = pay.sendPaymentToOnePay();
                }
                #endregion

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Transaction is successful",
                    url = url,
                    order_no = order_no,
                    order_id = order_id,
                    content = order_no + " CHUYEN KHOAN TOUR"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("b2b/tour/payment/checkout.json" + ex.ToString() + "token =" + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Transaction Error !!!"
                });
            }

        }
        #endregion
    }
}
