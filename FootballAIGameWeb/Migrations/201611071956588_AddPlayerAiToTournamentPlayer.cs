namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlayerAiToTournamentPlayer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TournamentPlayers", "PlayerAi", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TournamentPlayers", "PlayerAi");
        }
    }
}
