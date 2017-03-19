namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlayerState : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "PlayerState", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "PlayerState");
        }
    }
}
