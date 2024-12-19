using ENTITIES.ViewModels.AttachFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Comment
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string Content { get; set; }

        // Danh sách file đính kèm
        public List<AttachFileViewModel> AttachFiles { get; set; } = new List<AttachFileViewModel>();

        public string Username { get; set; }

        public int UserType { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }


}

