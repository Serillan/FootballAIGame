using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class AddTournamentReferenceToMatch : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Matches", name: "Tournament_Id", newName: "TournamentId");
            RenameIndex(table: "dbo.Matches", name: "IX_Tournament_Id", newName: "IX_TournamentId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Matches", name: "IX_TournamentId", newName: "IX_Tournament_Id");
            RenameColumn(table: "dbo.Matches", name: "TournamentId", newName: "Tournament_Id");
        }
    }
}
