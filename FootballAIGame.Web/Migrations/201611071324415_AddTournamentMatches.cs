using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class AddTournamentMatches : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "Tournament_Id", c => c.Int());
            CreateIndex("dbo.Matches", "Tournament_Id");
            AddForeignKey("dbo.Matches", "Tournament_Id", "dbo.Tournaments", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Matches", "Tournament_Id", "dbo.Tournaments");
            DropIndex("dbo.Matches", new[] { "Tournament_Id" });
            DropColumn("dbo.Matches", "Tournament_Id");
        }
    }
}
