namespace FootballAIGame.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAverageActionLatenciesToMatches : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "Player1AverageActionLatency", c => c.Int(nullable: false, defaultValue: 0));
            AddColumn("dbo.Matches", "Player2AverageActionLatency", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "Player2AverageActionLatency");
            DropColumn("dbo.Matches", "Player1AverageActionLatency");
        }
    }
}
