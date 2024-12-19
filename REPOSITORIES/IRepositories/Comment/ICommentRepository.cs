using ENTITIES.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories.Comment
{
    public interface ICommentRepository
    {
        Task<List<CommentViewModel>> GetListCommentsByRequestId(int requestId);

        Task<int> InsertComment(int requestId, string content, int createdBy, int userType);
        Task<int> InsertAttachFiles(int requestId, int userId, int type, string path, string ext, float capacity);
        Task<CommentViewModel> GetCommentDetail(int commentId);
        //Task<int> UpdateComment(int id, int requestId, string content, string attachFile, DateTime updatedBy);
    }
}
