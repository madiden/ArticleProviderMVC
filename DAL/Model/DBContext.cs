using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.Log = (x) => System.Diagnostics.Debug.WriteLine(x);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class ArticleContext : DbContext
    {
        public ArticleContext() : base("DefaultConnection") {
        }


        public virtual DbSet<Article> Articles { get; set; }

        public virtual DbSet<ArticleComments> Comments { get; set; }

        public virtual DbSet<ArticleLike> Likes { get; set; }
    }
}
