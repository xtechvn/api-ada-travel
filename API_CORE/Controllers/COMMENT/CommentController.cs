using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using REPOSITORIES.IRepositories.Comment;
using StackExchange.Redis;
using System.Threading.Tasks;
using System;
using Utilities.Contants;
using Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ENTITIES.ViewModels.Comment;
using Caching.RedisWorker;
using System.Linq;
using System.ComponentModel.Design;
using System.IO;

namespace API_CORE.Controllers.COMMENT
{
    [Route("api")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICommentRepository _commentRepository;
        private readonly ISubscriber _subscriber;
        private readonly RedisConn _redisService;
        public CommentController(IConfiguration configuration, ICommentRepository commentRepository)
        {
            _configuration = configuration;
            _commentRepository = commentRepository;
            var connection = ConnectionMultiplexer.Connect(configuration["DataBaseConfig:Redis:Host"] + ":" + configuration["DataBaseConfig:Redis:Port"]);
            _subscriber = connection.GetSubscriber();
            _redisService = new RedisConn(_configuration);
            _redisService.Connect();
        }

        [HttpPost("comment/get-list.json")]
        public async Task<ActionResult> getListComment(string token)
        {
            try
            {
                JArray objParr = null;
                bool is_public_comment = false;

                //#region Test
                //var j_param = new Dictionary<string, object>
                //{
                //    {"request_id", "1"}
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);

                //token = CommonHelper.Encode(data_product, _configuration["DataBaseConfig:key_api:b2b"]);
                //#endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:b2b"]))
                {
                    var requestId = Convert.ToInt32(objParr[0]["request_id"]);
                    string cache_name = "COMMENT_" + requestId;

                    // Kiểm tra dữ liệu trong Redis
                    int redisDbIndex = Convert.ToInt32(_configuration["DataBaseConfig:Redis:Database:db_comment"]);
                    var cacheData = _redisService.Get(cache_name, redisDbIndex);

                    if (!string.IsNullOrEmpty(cacheData))
                    {
                        // Cache hit: Lấy dữ liệu từ Redis
                        var cachedComments = JsonConvert.DeserializeObject<List<CommentViewModel>>(cacheData);
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Lấy dữ liệu từ cache thành công.",
                            data = cachedComments
                        });
                    }

                    // Gọi repository để lấy danh sách comment
                    var obj_comments = await _commentRepository.GetListCommentsByRequestId(requestId);

                    if (obj_comments != null && obj_comments.Count > 0)
                    {
                        is_public_comment = true;
                        // Lưu vào Redis sau khi lấy từ DB
                        _redisService.Set(cache_name, JsonConvert.SerializeObject(obj_comments), redisDbIndex);
                        //_subscriber.Publish(cache_name, JsonConvert.SerializeObject(obj_comments));
                    }

                    return Ok(new
                    {
                        status = is_public_comment ? (int)ResponseType.SUCCESS : (int)ResponseType.EMPTY,
                        msg = is_public_comment ? $"Danh sách comment của request_id {requestId} đã được public thành công" : "Hiện tại không có comment nào cho request này",
                        data = obj_comments
                    });
                }

                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Token không hợp lệ"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("comment/get-list.json - Error: " + ex.ToString() + " token = " + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Transaction Error!"
                });
            }
        }


        [HttpPost("comment/add.json")]
        public async Task<ActionResult> AddComment(string token)
        {
            try
            {
                JArray objParr = null;

                if (!CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:b2b"]))
                {
                    return Unauthorized(new { status = (int)ResponseType.ERROR, msg = "Token không hợp lệ" });
                }

                var requestId = Convert.ToInt32(objParr[0]["request_id"]);
                var userId = Convert.ToInt32(objParr[0]["userid"]);
                var type = Convert.ToInt32(objParr[0]["type"]);

                var content = objParr[0]["content"].ToString();
                var userType  = Convert.ToInt32(objParr[0]["user_type"]);
                var createdBy = Convert.ToInt32(objParr[0]["created_by"]);

                var attachFiles = objParr[0]["attach_files"]?.ToObject<List<AttachFileViewModel>>();


                // Thêm comment và lấy dữ liệu đầy đủ
                var newCommentId = await _commentRepository.InsertComment(requestId,  content, createdBy,userType);

                if (newCommentId > 0 )
                {
                    // Lấy chi tiết comment vừa được thêm
                    var newComment = await _commentRepository.GetCommentDetail(newCommentId);
                    if (attachFiles != null && attachFiles.Any())
                    {
                        foreach (var file in attachFiles)
                        {
                            await _commentRepository.InsertAttachFiles(requestId, userId, 0, file.Url, Path.GetExtension(file.Url), 0);
                        }

                        newComment.AttachFiles = attachFiles; // Gán file đính kèm vào comment mới
                    }


                    string cacheName = "COMMENT_" + requestId;
                    int redisDbIndex = Convert.ToInt32(_configuration["DataBaseConfig:Redis:Database:db_comment"]);

                    var existingComments = _redisService.Get(cacheName, redisDbIndex);
                    List<CommentViewModel> comments = !string.IsNullOrEmpty(existingComments)
                        ? JsonConvert.DeserializeObject<List<CommentViewModel>>(existingComments)
                        : new List<CommentViewModel>();

                    comments.Add(newComment);
                    // Update Redis cache
                    _redisService.Set(cacheName, JsonConvert.SerializeObject(comments), redisDbIndex);
                    _subscriber.Publish(cacheName, JsonConvert.SerializeObject(newComment));

                    return Ok(new { status = (int)ResponseType.SUCCESS, msg = "Thêm comment thành công!", data = newComment });
                }

                return Ok(new { status = (int)ResponseType.FAILED, msg = "Không thể thêm comment!" });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram($"AddComment Error: {ex}");
                return Ok(new { status = (int)ResponseType.ERROR, msg = "Error while adding comment!" });
            }
        }




        /// <summary>
        /// Cập nhật comment
        /// </summary>
        //[HttpPost("comment/update.json")]
        //public async Task<ActionResult> UpdateComment(string token, int id, string content = null, string attachFile = null)
        //{
        //    try
        //    {
        //        JArray objParr = null;

        //        // Giải mã token để lấy requestId từ token
        //        if (!CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:b2b"]))
        //        {
        //            return Unauthorized(new { status = (int)ResponseType.ERROR, msg = "Token không hợp lệ" });
        //        }

        //        var requestId = Convert.ToInt32(objParr[0]["request_id"]);
        //        var updatedBy = DateTime.UtcNow; // Giả định rằng UpdatedBy là thời điểm hiện tại

        //        // Thực hiện cập nhật comment trong database
        //        var updatedCommentId = await _commentRepository.UpdateComment(id, requestId, content, attachFile, updatedBy);

        //        if (updatedCommentId > 0)
        //        {
        //            string cacheName = "COMMENT_" + requestId;
        //            var updatedComment = new CommentViewModel
        //            {
        //                Id = updatedCommentId,
        //                RequestId = requestId,
        //                Content = content,
        //                AttachFile = attachFile,
        //                UpdatedBy = updatedBy,
        //                UpdatedDate = DateTime.UtcNow
        //            };

        //            // Publish qua Redis PUB/SUB
        //            _subscriber.Publish(cacheName, JsonConvert.SerializeObject(updatedComment));

        //            return Ok(new
        //            {
        //                status = (int)ResponseType.SUCCESS,
        //                msg = "Cập nhật comment thành công!",
        //                data = updatedComment
        //            });
        //        }

        //        return Ok(new
        //        {
        //            status = (int)ResponseType.FAILED,
        //            msg = "Không thể cập nhật comment!"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.InsertLogTelegram("comment/update - Error: " + ex.ToString() + " token = " + token);
        //        return Ok(new
        //        {
        //            status = (int)ResponseType.ERROR,
        //            msg = "Error while updating comment!"
        //        });
        //    }
        //}



    }
}
