namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewTournamentProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "TournamentState", c => c.Int(nullable: false));
            AddColumn("dbo.Tournaments", "MinimumNumberOfPlayers", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tournaments", "MinimumNumberOfPlayers");
            DropColumn("dbo.Tournaments", "TournamentState");
        }
    }
}
