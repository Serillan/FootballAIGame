namespace FootballAIGame.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlayer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.AspNetUsers", "PlayerId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Players", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Players", new[] { "UserId" });
            DropColumn("dbo.AspNetUsers", "PlayerId");
            DropTable("dbo.Players");
        }
    }
}
