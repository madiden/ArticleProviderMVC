using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class ArticleComments
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set;}
        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
    }
}
