namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requiredCommentr : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ArticleComments", "Comment", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ArticleComments", "Comment", c => c.String());
        }
    }
}
