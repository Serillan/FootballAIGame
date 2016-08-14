namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlayerStatisticsProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "Score", c => c.Int(nullable: false));
            AddColumn("dbo.Players", "WonGames", c => c.Int(nullable: false));
            AddColumn("dbo.Players", "WonTournaments", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "WonTournaments");
            DropColumn("dbo.Players", "WonGames");
            DropColumn("dbo.Players", "Score");
        }
    }
}
