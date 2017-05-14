namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentUserIdAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ArticleComments", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ArticleComments", "UserId");
        }
    }
}
