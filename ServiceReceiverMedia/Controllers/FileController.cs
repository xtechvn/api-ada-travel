using ENTITIES.ViewModels.AttachFiles;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServiceReceiverMedia.Models;
using ServiceReceiverMedia.MongoDB;
using ServiceReceiverMedia.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace ServiceReceiverMedia.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private IConfiguration _configuration;
        private byte[] AESKey;
        private byte[] AESIV;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public FileController(IConfiguration configuration, IWebHostEnvironment WebHostEnvironment)
        {
           _configuration = configuration;
            AESKey= EncryptService.Get_AESKey(EncryptService.ConvertBase64StringToByte(_configuration["key_api:AES_KEY"]));
            AESIV = EncryptService.Get_AESIV(EncryptService.ConvertBase64StringToByte(_configuration["key_api:AES_IV"]));
            _WebHostEnvironment = WebHostEnvironment;
            //--Get Root:
        }
        /// <summary>
        /// Single File Upload
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("Upload")]
        public async Task<ActionResult> Upload([FromForm] FileViewModel model)
        {
            if (model == null || model.token==null || model.token.Trim()=="" || model.data_id<=0|| model.type<=0)
            {
                return BadRequest();
            }

            try
            {
                DateTime time_upload = DateTime.MinValue;
                try
                {
                    //var token = EncryptService.ConvertByteToBase64String(EncryptService.AES_EncryptToByte(DateTime.Now.ToString(), AESKey, AESIV));
                    string time = EncryptService.AES_DecryptToString(EncryptService.ConvertBase64StringToByte(model.token), AESKey, AESIV);
                    time_upload = Convert.ToDateTime(time.Replace("\"",""));
                    
                    if(time_upload<=DateTime.MinValue || time_upload>DateTime.Now.AddMinutes(1) || time_upload.AddMinutes(10) < DateTime.Now.AddMinutes(1))
                    {
                        time_upload = DateTime.MinValue;
                    }/*
                    time_upload = DateTime.Now.AddMinutes(-4);
                    */
                }
                catch { }
                if (time_upload > DateTime.MinValue && time_upload < DateTime.Now.AddMinutes(1))
                {
                    string root = _configuration["File:MainFolder"];
                    //-- Get Child Folder
                    List<string> child = new List<string>();
                    child.Add(_configuration["File:AttachFile"]);
                    child.Add(model.type.ToString());
                    child.Add(model.data_id.ToString());
                    //-- Create Folder if Not Exists
                    string folder = FileService.CheckAndCreateFolder(root, child);
                   
                    //-- Save file
                    string file_location = await FileService.SaveFile(model.data, folder, model.name);
                   
                    //--Get URL
                    string url = FileService.BuildURLFromPath(file_location);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Sucess",
                        url= url
                    });
                }
               
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Upload - FileController - : " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }
            return Ok(new
            {
                status = (int)ResponseType.ERROR,
                msg = "Token invalid"
            });
        }
    }
}
