using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class UserAgent
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public long ClientId { get; set; }
        public short? MainFollow { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateLast { get; set; }
        public DateTime? VerifyDate { get; set; }
        public int? VerifyStatus { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Client Client { get; set; }
        public virtual User User { get; set; }
    }
}
