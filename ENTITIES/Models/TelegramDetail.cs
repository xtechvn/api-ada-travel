using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class TelegramDetail
    {
        public int Id { get; set; }
        public string GroupLog { get; set; }
        public string Token { get; set; }
        public string GroupChatId { get; set; }
        public DateTime CreateDate { get; set; }
        public int ProjectType { get; set; }
        public int Status { get; set; }
    }
}
