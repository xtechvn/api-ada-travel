using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories.IReportRevenue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.ReportRevenue
{
    [Route("api")]
    [ApiController]
    public class ReportRevenueController : ControllerBase
    {
        private IConfiguration _configuration;
        private IReportRevenueRepository _reportRevenueRepository;
        public ReportRevenueController(IConfiguration configuration, IReportRevenueRepository reportRevenueRepository)
        {
            _configuration = configuration;
            _reportRevenueRepository = reportRevenueRepository;
        }
        [HttpPost("get-report-revenue-by-current-date.json")]
        public async Task<ActionResult> ReportRevenueByCurrentDate(string token)
        {
            try
            {
                #region Test
                //var j_param = new Dictionary<string, object>
                //{
                //    {"fromdate", "2024/10/22"},
                //    {"todate", "2024/10/22"},

                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, _configuration["DataBaseConfig:key_api:api_zalo"]);
                #endregion
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:api_zalo"]))
                {
                    string fromdate = objParr[0]["fromdate"].ToString();
                    string todate = objParr[0]["todate"].ToString();
                    var data = await _reportRevenueRepository.ReportRevenueByCurrentDate(fromdate, todate);
                    if (data == null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "FAILED",

                        });
                    }
                    var sb = new StringBuilder();
                    sb.AppendLine("== Báo cáo cuối ngày " + Convert.ToDateTime(fromdate).ToString("dd/MM/yyyy") + " ==");
                    sb.AppendLine("\n");
                    sb.AppendLine("==  DOANH THU TRONG NGÀY  ==");
                    sb.AppendLine("\nDoanh thu tổng : " + data.Amount.ToString("N0") + " VNĐ");
                    sb.AppendLine("\nLợi nhuận thuần: " + data.Profit.ToString("N0") + " VNĐ");
                    sb.AppendLine("\nTổng khách đã thanh toán trong ngày: ");
                    sb.AppendLine("\n" + data.AmountPay.ToString("N0") + " VNĐ");
                    //sb.AppendLine("\n== " + Convert.ToDateTime(fromdate).ToString("dd/MM/yyyy") + " ==");
                    string msg_send = sb.ToString();

                    #region OrderBookClosing
                    var date_time = DateTime.Now.AddMonths(-1);
                    int Month = Convert.ToInt32(date_time.Month.ToString());
                    int Year = Convert.ToInt32(date_time.Year.ToString());
                    int day = Convert.ToInt32(date_time.Day.ToString());
                    var datetime_now = new DateTime(Year, Month, day);
                    var fromdate_order = new DateTime(Year, Month, 01);
                    var todate_order = datetime_now.AddHours(23).AddMinutes(59).AddSeconds(59);
                    var data_order = await _reportRevenueRepository.GetTotalAccountBalance(fromdate_order, todate_order);
                    if (data_order == null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "FAILED",

                        });
                    }

                    DateTime startDate = new DateTime(Year, Month, 1);
                    startDate = startDate.AddDays((-startDate.Day) + 1);

                    DateTime endaDate = new DateTime(Year, Month, 1);
                    endaDate = endaDate.AddMonths(1);
                    endaDate = endaDate.AddDays(-(endaDate.Day));

                    var sb2 = new StringBuilder();
                    sb2.AppendLine("\n");
                    sb2.AppendLine("\n");
                    sb2.AppendLine("\n== DOANH THU KÝ MỚI ==");
                    sb2.AppendLine("\nTừ ngày : " + startDate.ToString("dd/MM/yyyy") + "  đến ngày :" + endaDate.ToString("dd/MM/yyyy"));
                    sb2.AppendLine("\nDoanh thu đơn hàng : " + data_order.AmountOrder.ToString("N0") + " VNĐ");
                    sb2.AppendLine("\nDoanh thu chốt đơn : " + data_order.AmountFinalize.ToString("N0") + " VNĐ");
                    if (data_order.AmountOrder != data_order.AmountFinalize)
                    {
                        sb2.AppendLine("\n Lệch số: " + (data_order.AmountFinalize - data_order.AmountOrder ).ToString("N0") + " VNĐ");
                    }
                    else
                    {
                        sb2.AppendLine("\n Khớp số ");
                    }
                    sb2.AppendLine("\n== END " + Convert.ToDateTime(fromdate).ToString("dd/MM/yyyy") + " ==");
                    string msg_send2 = sb2.ToString();
                    //LogHelper.InsertLogTelegram(msg_send2);
                    #endregion

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "SUCCESS",
                        data = msg_send + msg_send2
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetMsgZalo MsgZaloController- Token: " + token + ";" + ex.ToString());

                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }
        }
       
    }
}
