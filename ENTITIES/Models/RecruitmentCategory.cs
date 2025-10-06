using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.Models
{
    public partial class  RecruitmentCategory
    {
        public long Id { get; set; }
        public int? CategoryId { get; set; }
        public long? ArticleId { get; set; }
    }
}
