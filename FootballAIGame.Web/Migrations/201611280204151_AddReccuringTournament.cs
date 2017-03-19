using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class AddReccuringTournament : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReccuringTournaments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecurrenceInterval = c.Int(nullable: false),
                        NumberOfPresentTournaments = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        MaximumNumberOfPlayers = c.Int(nullable: false),
                        MinimumNumberOfPlayers = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Tournaments", "ReccuringTournament_Id", c => c.Int());
            CreateIndex("dbo.Tournaments", "ReccuringTournament_Id");
            AddForeignKey("dbo.Tournaments", "ReccuringTournament_Id", "dbo.ReccuringTournaments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tournaments", "ReccuringTournament_Id", "dbo.ReccuringTournaments");
            DropIndex("dbo.Tournaments", new[] { "ReccuringTournament_Id" });
            DropColumn("dbo.Tournaments", "ReccuringTournament_Id");
            DropTable("dbo.ReccuringTournaments");
        }
    }
}
