using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceReceiverMedia.Models;
using ServiceReceiverMedia.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace ServiceReceiverMedia.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private IConfiguration _configuration;
        private byte[] AESKey;
        private byte[] AESIV;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public VideoController(IConfiguration configuration, IWebHostEnvironment WebHostEnvironment)
        {
            _configuration = configuration;
            AESKey = EncryptService.Get_AESKey(EncryptService.ConvertBase64StringToByte(_configuration["key_api:AES_KEY"]));
            AESIV = EncryptService.Get_AESKey(EncryptService.ConvertBase64StringToByte(_configuration["key_api:AES_IV"]));
            _WebHostEnvironment = WebHostEnvironment;
        }

        [HttpPost("upload-video-b2c")]
        public async Task<IActionResult> UploadVideo([FromForm] VideoUploadModel model)
        {
            try
            {
                if (model.VideoFile == null || model.VideoFile.Length == 0)
                {
                    return Ok(new { status = (int)ResponseType.FAILED, message = "No file uploaded.", url_path = "" });

                }
                //model.token = CommonHelper.Encode(JsonConvert.SerializeObject(new { exprire = DateTime.Now.AddMinutes(15) }), _configuration["key_api:static"]);
                if (string.IsNullOrEmpty(model.token))
                {
                    return Ok(new { status = (int)ResponseType.FAILED, message = "No token provided.", url_path = "" });

                }
                // Validate the token
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(model.token, out objParr, _configuration["key_api:static"]))
                {
                    try
                    {
                        DateTime? exprire = Convert.ToDateTime(objParr[0]["exprire"].ToString());
                        if (exprire == null || exprire < DateTime.Now|| exprire > DateTime.Now.AddMinutes(30))
                        {
                            return Ok(new { status = (int)ResponseType.FAILED, message = "Token exprired.", url_path = "" });
                        }
                    }
                    catch
                    {
                        return BadRequest(new { status = (int)ResponseType.FAILED, message = "Data Invailid", url_path = "" });

                    }
                    // Check for valid video extensions
                    var validExtensions = new[] { ".mp4", ".mkv", ".avi", ".mov" };
                    var fileExtension = Path.GetExtension(model.VideoFile.FileName);

                    if (!Array.Exists(validExtensions, ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)))
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, message = "Unsupported file format.", url_path = "" });

                    }
                    string year = DateTime.Now.Year.ToString();
                    string month = DateTime.Now.Month.ToString();
                    // Save the file to a specific path
                    var uploadPath = Path.Combine(_configuration["File:MainFolder"], _configuration["File:Videos"] , year, month);
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    var fileName = Path.GetRandomFileName() + fileExtension;
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Save the video file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.VideoFile.CopyToAsync(stream);
                    }

                    var fileLocation = "/"+ _configuration["File:MainFolder"] + "/"+ _configuration["File:Videos"] + "/"+year+"/"+ month + "/"+ fileName;

                    return Ok(new { status = (int)ResponseType.SUCCESS, message = "Images Received", url_path = fileLocation });

                }
            }
            catch(Exception ex)
            {
                return BadRequest(new { status = (int)ResponseType.ERROR, message = "On Execution", url_path = "" });

            }
            return BadRequest(new { status = (int)ResponseType.FAILED, message = "Data Invailid", url_path = "" });


        }


    }
   
}
