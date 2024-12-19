using ENTITIES.ViewModels.Client;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration configuration;
        private IUserRepository userRepository;
        public UserController(IConfiguration _configuration, IUserRepository _userRepository)
        {
            configuration = _configuration;
            userRepository = _userRepository;
        }

        [EnableCors("MyApi")]
        [HttpPost("insert-user-client.json")]
        public async Task<ActionResult> InsertUserClient(string token)
        {
            try
            {
                //#region Test
                //var j_param = new LoginModel()
                //{
                //    UserName = "minh.nq",
                //    Password = "e10adc3949ba59abbe56e057f20f883e"
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
                //#endregion

                JArray objParr = null;
                bool response_queue = false;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    List<UserClientModel> listUser = JsonConvert.DeserializeObject<List<UserClientModel>>(objParr[0].ToString());
                    var resultInsert = userRepository.InsertUserAndClient(listUser);
                    if(resultInsert == -1)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
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
                LogHelper.InsertLogTelegram("insert-user-client.json - UserController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }

        }
    }
}
