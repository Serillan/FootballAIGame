namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMatchStatisticsProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "Goals", c => c.String());
            AddColumn("dbo.Matches", "Shots1", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "Shots2", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "ShotsOnTarget1", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "ShotsOnTarget2", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "ShotsOnTarget2");
            DropColumn("dbo.Matches", "ShotsOnTarget1");
            DropColumn("dbo.Matches", "Shots2");
            DropColumn("dbo.Matches", "Shots1");
            DropColumn("dbo.Matches", "Goals");
        }
    }
}
