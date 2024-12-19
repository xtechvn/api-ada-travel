using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using REPOSITORIES.IRepositories;

namespace API_CORE.Controllers
{
    [Route("api")]
    [ApiController]
    public class PriceServiceController : ControllerBase
    {
        private IConfiguration configuration;
        private IServicePiceRepository price_repository;
        public PriceServiceController(IConfiguration _configuration, IServicePiceRepository _price_repository)
        {
            configuration = _configuration;
            price_repository = _price_repository;
        }

        //[EnableCors("MyApi")]
        //[HttpPost("service/get-fee.json")]
        //public async Task<ActionResult> getServiceFee(string token)
        //{
        //    try
        //    {
        //        #region Test
        //        var j_param = new Dictionary<string, string>
        //        {
        //            {"service_type", "2"},
        //            {"price","2000000" }
        //        };
        //        var data_product = JsonConvert.SerializeObject(j_param);
        //        token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
        //        #endregion

        //        JArray objParr = null;
        //        if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
        //        {
        //            int service_type = Convert.ToInt32(objParr[0]["service_type"]);
        //            double price = Convert.ToDouble(objParr[0]["price"]);

        //            var fee = new Fee(price_repository);
        //            var _price_detail = fee.getHotelFee(service_type, price,);

        //            return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Success", price_detail = _price_detail });
        //        }
        //        else
        //        {
        //            return Ok(new
        //            {
        //                status = (int)ResponseType.ERROR,
        //                msg = "Key không hợp lệ"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
        //    }

        //}

       

        
    }
}
