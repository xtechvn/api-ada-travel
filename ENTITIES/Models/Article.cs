using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Article
    {
        public Article()
        {
            ArticleRelated = new HashSet<ArticleRelated>();
            ArticleTag = new HashSet<ArticleTag>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Lead { get; set; }
        public string Body { get; set; }
        public int Status { get; set; }
        public int ArticleType { get; set; }
        public int? PageView { get; set; }
        public DateTime? PublishDate { get; set; }
        public int? AuthorId { get; set; }
        public string Image169 { get; set; }
        public string Image43 { get; set; }
        public string Image11 { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? DownTime { get; set; }
        public DateTime? UpTime { get; set; }
        public short? Position { get; set; }

        public virtual ICollection<ArticleRelated> ArticleRelated { get; set; }
        public virtual ICollection<ArticleTag> ArticleTag { get; set; }
    }
}
