using ENTITIES.Models;
using ENTITIES.ViewModels.Transaction;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.Transaction
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private IConfiguration configuration;
        private ITransactionRepository transactionRepository;
        public TransactionController(IConfiguration _configuration, ITransactionRepository _transactionRepository)
        {
            configuration = _configuration;
            transactionRepository = _transactionRepository;
        }

        [EnableCors("MyApi")]
        [HttpPost("insert.json")]
        public async Task<ActionResult> InsertTransaction(string token)
        {
            try
            {
                //#region Test
                //var j_param = new Transactions()
                //{
                //    ClientId = 1,
                //    ServiceType = 1,
                //    Amount = 1000000,
                //    ContractNo = "ABC123",
                //    Status = 1,
                //    UserVerifyId = 10,
                //    VerifyDate = DateTime.Now,
                //    BankReference = "TECHCOMBANK",
                //    PaymentType = 0,
                //    Description = "Thanh toán đơn hàng ABC123",
                //    TransactionNo = "ABC123"
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
                //#endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    TransactionsViewModel transactions = JsonConvert.DeserializeObject<TransactionsViewModel>(objParr[0].ToString());
                    if (string.IsNullOrEmpty(transactions.Amount))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Số tiền không được để trống"
                        });
                    }
                    Regex regex = new Regex(@"^[0-9]+$*");
                    if (!regex.IsMatch(transactions.Amount))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Số tiền chỉ được nhập kí tự số"
                        });
                    }
                    var verifyDate = DateUtil.StringToDate(transactions.VerifyDate);
                    if (verifyDate == null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Ngày kiểm tra không đúng định dạng. Định đạng ngày kiểm tra: yyyy/MM/dd"
                        });
                    }
                    if (!string.IsNullOrEmpty(transactions.ContractNo) && transactions.ContractNo.Length > 50)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Số hợp đồng không vượt quá 50 kí tự, vui lòng kiểm tra lại"
                        });
                    }
                    if (!string.IsNullOrEmpty(transactions.BankReference) && transactions.BankReference.Length > 300)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Tên ngân hàng không quá 300 kí tự, vui lòng kiểm tra lại"
                        });
                    }
                    if (!string.IsNullOrEmpty(transactions.Description) && transactions.Description.Length > 400)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Mô tả không được quá 400 kí tự, vui lòng kiểm tra lại"
                        });
                    }
                    if (!string.IsNullOrEmpty(transactions.TransactionNo) && transactions.TransactionNo.Length > 100)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Mã giao dịch không quá 100 kí tự, vui lòng kiểm tra lại"
                        });
                    }
                    Transactions entity = new Transactions()
                    {
                        ClientId = transactions.ClientId,
                        ServiceType = transactions.ServiceType,
                        Amount = double.Parse(transactions.Amount),
                        ContractNo = transactions.ContractNo,
                        Status = transactions.Status,
                        UserVerifyId = transactions.UserVerifyId,
                        VerifyDate = DateUtil.StringToDate(transactions.VerifyDate).Value,
                        BankReference = transactions.BankReference,
                        PaymentType = transactions.PaymentType,
                        Description = transactions.Description,
                        TransactionNo = transactions.TransactionNo
                    };

                    var result = transactionRepository.Insert(entity);
                    if (result == -1)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Insert thất bại"
                        });
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Insert thành công"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertTransaction - TransactionController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }

        }

        //[EnableCors("MyApi")]
        //[HttpPost("get-token.json")]
        //public async Task<ActionResult> GetToken(TransactionsViewModel transactions)
        //{
        //    try
        //    {
        //        var transaction = JsonConvert.SerializeObject(transactions);
        //        string token = CommonHelper.Encode(transaction, configuration["DataBaseConfig:key_api:api_manual"]);
        //        return Ok(new
        //        {
        //            status = ResponseTypeString.Success,
        //            message = "Get Token success",
        //            token = token
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.InsertLogTelegram("Transactions - TransactionsController: " + ex);
        //        return Ok(new { status = ResponseTypeString.Fail, message = "error: " + ex.ToString(), token = "" });
        //    }

        //}

        //[EnableCors("MyApi")]
        //[HttpPost("get-Transactions-token.json")]
        //public async Task<ActionResult> GetTransactionsToken(string skip, string take)
        //{
        //    try
        //    {
        //        Regex regex = new Regex(@"^[0-9]+$");
        //        if (!regex.IsMatch(skip) || !regex.IsMatch(take))
        //        {
        //            return Ok(new
        //            {
        //                status = (int)ResponseType.ERROR,
        //                message = "Skip và take chỉ nhập ký tự số",
        //            });
        //        }

        //        var j_param = new Dictionary<string, string>
        //                {
        //                    {"skip", skip},
        //                    {"take",take }
        //                };
        //        var data_product = JsonConvert.SerializeObject(j_param);
        //        string token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
        //        return Ok(new
        //        {
        //            status = (int)ResponseType.SUCCESS,
        //            message = "Get Token success",
        //            token = token
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.InsertLogTelegram("Transactions - TransactionsController: " + ex);
        //        return Ok(new { status = (int)ResponseType.ERROR, message = "error: " + ex.ToString(), token = "" });
        //    }

        //}

        [EnableCors("MyApi")]
        [HttpPost("get-transaction.json")]
        public async Task<ActionResult> GetTransactionsList(string token)
        {
            //#region Test
            //var j_param = new Dictionary<string, string>
            //        {
            //            {"skip", "1"},
            //            {"take","5" }
            //        };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
            //#endregion
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    int skip = Convert.ToInt32(objParr[0]["skip"]);
                    int take = Convert.ToInt32(objParr[0]["take"]);

                    List<TransactionsView> data = await transactionRepository.GetAllTransactions(skip, take);
                    return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Success", data = data });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ",
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Transactions - TransactionsController: " + ex);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }

        }
    }
}
