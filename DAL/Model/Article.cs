using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Article
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Required]
        [Display(Name ="Content")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public string AuthorId { get; set; }
        [Display(AutoGenerateField =false)]
        public DateTime CreationDate { get; set; }
        [Display(AutoGenerateField = false)]
        public DateTime? LastUpdateDate { get; set; }
        public virtual ICollection<ArticleComments> Comments { get; set; }
        public virtual ICollection<ArticleLike> Likes { get; set; }

        [NotMapped]
        public int LikeCount { get; set; }

        [NotMapped]
        public int Percentage { get; set; }
    }
}
