using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class RunningScheduleService
    {
        public int Id { get; set; }
        public int? PriceId { get; set; }
        public DateTime? LogDate { get; set; }

        public virtual Campaign Price { get; set; }
    }
}
