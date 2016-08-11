namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPropertiesToTournament : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "Name", c => c.String(nullable: false));
            AddColumn("dbo.Tournaments", "StartTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Tournaments", "MaximumNumberOfPlayers", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tournaments", "MaximumNumberOfPlayers");
            DropColumn("dbo.Tournaments", "StartTime");
            DropColumn("dbo.Tournaments", "Name");
        }
    }
}
