using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Programs
{
   public class ProgramsExpiredViewModel
    {
        public long ProgramId { get; set; }
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public string EndDate { get; set; }
        public long TotalRow { get; set; }
    }
}
