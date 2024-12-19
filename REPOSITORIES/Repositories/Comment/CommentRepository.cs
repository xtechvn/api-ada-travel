using DAL.MongoDB.Notify;
using DAL.Permission;
using Entities.ConfigModels;
using ENTITIES.ViewModels.Comment;
using ENTITIES.ViewModels.Notify;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.Comment;
using REPOSITORIES.IRepositories.Notify;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories.Comment
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public CommentRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration["DataBaseConfig:SqlServer:ConnectionString"];
        }

        public async Task<List<CommentViewModel>> GetListCommentsByRequestId(int requestId)
        {
            var comments = new List<CommentViewModel>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("[dbo].[SP_GetListCommentsByRequestId]", connection) { CommandType = CommandType.StoredProcedure })
            {
                command.Parameters.AddWithValue("@RequestId", requestId);
                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        comments.Add(new CommentViewModel
                        {
                            Id = reader.GetInt32("Id"),
                            RequestId = reader.GetInt32("RequestId"),
                            Content = reader.GetString("Content"),
                            Username = reader.GetString("UserName"),
                            CreatedBy = reader.GetInt32("CreatedBy"),
                            CreatedDate = reader.GetDateTime("CreatedDate"),

                        });
                    }
                }
            }
            return comments;
        }


        public async Task<int> InsertComment(int requestId, string content, int createdBy, int userType)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("[dbo].[sp_InsertComments]", connection) { CommandType = CommandType.StoredProcedure })
            {
                command.Parameters.AddWithValue("@RequestId", requestId);
                command.Parameters.AddWithValue("@Content", string.IsNullOrEmpty(content) ? (object)DBNull.Value : content);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);
                command.Parameters.AddWithValue("@CreatedDate", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")));

                command.Parameters.AddWithValue("@UserType", userType); // Thêm UserType
                command.Parameters.Add("@Identity", SqlDbType.Int).Direction = ParameterDirection.Output;


                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return (int)command.Parameters["@Identity"].Value;
            }


        }

        public async Task<CommentViewModel> GetCommentDetail(int commentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("[dbo].[SP_GetDetailCommentById]", connection) { CommandType = CommandType.StoredProcedure })
            {
                command.Parameters.AddWithValue("@CommentId", commentId);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new CommentViewModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CommentId")),
                            RequestId = reader.GetInt32(reader.GetOrdinal("RequestId")),
                            Content = reader.IsDBNull(reader.GetOrdinal("Content")) ? null : reader.GetString(reader.GetOrdinal("Content")),
                            CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            UserType = reader.GetInt32(reader.GetOrdinal("UserType")),
                            Username = reader.GetInt32("UserType") != 1 ? reader.GetString("UserName") : reader.GetString("FullName"),
                            AttachFiles = new List<AttachFileViewModel>()
                        };
                    }
                }

            }
            return null;

        }


        public async Task<int> InsertAttachFiles(int requestId, int userId, int type, string path, string ext, float capacity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("[dbo].[SP_InsertAttachFile]", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@DataId", requestId);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Type", type);
                command.Parameters.AddWithValue("@Path", path);
                command.Parameters.AddWithValue("@Ext", ext);
                command.Parameters.AddWithValue("@Capacity", capacity);
                command.Parameters.Add("@Identity", SqlDbType.Int).Direction = ParameterDirection.Output;

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return (int)command.Parameters["@Identity"].Value;
            }
        }




        //public async Task<int> UpdateComment(int id, int requestId, string content, string attachFile, DateTime updatedBy)
        //{
        //    int updatedCommentId;
        //    using (var connection = new SqlConnection(_connectionString))
        //    using (var command = new SqlCommand("[dbo].[sp_UpdateComments]", connection) { CommandType = CommandType.StoredProcedure })
        //    {
        //        command.Parameters.AddWithValue("@Id", id);
        //        command.Parameters.AddWithValue("@RequestId", requestId);
        //        command.Parameters.AddWithValue("@Content", content ?? (object)DBNull.Value);
        //        command.Parameters.AddWithValue("@AttachFile", attachFile ?? (object)DBNull.Value);
        //        command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
        //        command.Parameters.Add("@Identity", SqlDbType.Int).Direction = ParameterDirection.Output;

        //        connection.Open();
        //        await command.ExecuteNonQueryAsync();
        //        updatedCommentId = (int)command.Parameters["@Identity"].Value;
        //    }
        //    return updatedCommentId;
        //}


    }

}
