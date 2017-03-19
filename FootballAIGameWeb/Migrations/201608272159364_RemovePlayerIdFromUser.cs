namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePlayerIdFromUser : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "PlayerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "PlayerId", c => c.Int(nullable: false));
        }
    }
}
