namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMatchProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "Player1ErrorLog", c => c.String());
            AddColumn("dbo.Matches", "Player2ErrorLog", c => c.String());
            AddColumn("dbo.Matches", "MatchData", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "MatchData");
            DropColumn("dbo.Matches", "Player2ErrorLog");
            DropColumn("dbo.Matches", "Player1ErrorLog");
        }
    }
}
