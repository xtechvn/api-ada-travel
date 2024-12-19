using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Tag
    {
        public Tag()
        {
            ArticleTag = new HashSet<ArticleTag>();
        }

        public long Id { get; set; }
        public string TagName { get; set; }
        public DateTime? CreatedOn { get; set; }

        public virtual ICollection<ArticleTag> ArticleTag { get; set; }
    }
}
