using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class AccountClient
    {
        public int Id { get; set; }
        public long? ClientId { get; set; }
        public int? ClientType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordBackup { get; set; }
        public string ForgotPasswordToken { get; set; }
        public byte? Status { get; set; }
        public int? GroupPermission { get; set; }
    }
}
