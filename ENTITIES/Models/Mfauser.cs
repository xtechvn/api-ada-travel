using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Mfauser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public short Status { get; set; }
        public string SecretKey { get; set; }
        public string BackupCode { get; set; }
        public string UserCreatedYear { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
