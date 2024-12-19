using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class ClientLinkAff
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string LinkAff { get; set; }
        public long ClientId { get; set; }
    }
}
