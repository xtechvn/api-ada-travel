using ENTITIES.ViewModels.Affiliate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceReceiverMedia.Common;
using System;
using System.Collections.Generic;
using System.IO;
using Utilities;
using Utilities.Contants;

namespace ServiceReceiverMedia.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataFeedController : ControllerBase
    {
        private IConfiguration _configuration;
        public DataFeedController(IConfiguration configuration)
        {
             _configuration = configuration;

        }
        // GET: MediaController
        [Route("index")]
        public string Index()
        {
            return "Index View";
        }
        [HttpPost("upload-data-feed.json")]
        public IActionResult UploadDataFeed(string token)
        {
            var status = 0;
            var message = "Sucessfully";
            JArray objParr = null;
            try
            {
                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["key_api:static"]))
                {
                    string at_feed = objParr[0]["feed_data"].ToString();
                    string aff_name = objParr[0]["aff_name"].ToString();
                    switch (aff_name)
                    {
                        case "accesstrade":
                            {
                                var at_feed_data = JsonConvert.DeserializeObject<List<AccesstradeDataFeed>>(at_feed);
                                if (at_feed_data != null && at_feed_data.Count > 0)
                                {
                                    var path = ReadFile.LoadConfig().DATA_FEED_FILE_PATH + @"\at_feed.csv";
                                    var sever_path = ReadFile.LoadConfig().DATA_FEED_SERVER_PATH + @"/at_feed.csv";
                                    TextWriter sw = new StreamWriter(path, true);
                                    if (!System.IO.File.Exists(path))
                                    {
                                        //-- Create File:
                                        var file = System.IO.File.Create(path);
                                        file.Close();
                                        //--Write Header
                                        string text = "sku,name,id,price,retail_Price,url,image_url,category_id,category_name";
                                        sw = new StreamWriter(path, true);
                                        sw.WriteLine(text);
                                        sw.Close();
                                    }
                                    else
                                    {
                                        //--Clear Content:
                                        System.IO.File.WriteAllText(path, string.Empty);
                                        //--Write Header
                                        string text = "sku,name,id,price,retail_Price,url,image_url,category_id,category_name";
                                        sw = new StreamWriter(path, true);
                                        sw.WriteLine(text);
                                        sw.Close();
                                    }
                                    sw = new StreamWriter(path, true);
                                    foreach (var item in at_feed_data)
                                    {
                                        sw.WriteLine(item.sku.Replace(",", "") + "," + item.name.Replace(",", "") + "," + item.id.Replace(",", "") + "," + item.price + "," + item.retail_Price + ","
                                            + item.url.Replace(",", "") + "," + item.image_url.Replace(",", "") + "," + item.category_id.Replace(",", "") + "," + item.category_name.Replace(",", ""));
                                    }
                                    sw.Close();
                                    status = (int)ResponseType.SUCCESS;
                                    message = "Write Accesstrade Datafeed Successful.";
                                }
                                else
                                {
                                    status = (int)ResponseType.FAILED;
                                    message = "Null or Invalid Data";
                                }
                            }
                            break;
                        default:
                            {
                                status = (int)ResponseType.FAILED;
                                message = "Invalid Type of Datafeed";
                            }
                            break;
                    }
                }
                else
                {
                    status = (int)ResponseType.FAILED;
                    message = "Token Invalid";
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ServiceReceiverMedia - UploadDataFeed Error: " + ex.ToString());
                status = (int)ResponseType.ERROR;
                message = "Error on Excution.";
            }
            return Ok(new
            {
                status = status,
                message = message
            });
        }
    }
}
