using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class FixMispelledWord : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ReccuringTournaments", newName: "RecurringTournaments");
            RenameColumn(table: "dbo.Tournaments", name: "ReccuringTournament_Id", newName: "RecurringTournament_Id");
            RenameIndex(table: "dbo.Tournaments", name: "IX_ReccuringTournament_Id", newName: "IX_RecurringTournament_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Tournaments", name: "IX_RecurringTournament_Id", newName: "IX_ReccuringTournament_Id");
            RenameColumn(table: "dbo.Tournaments", name: "RecurringTournament_Id", newName: "ReccuringTournament_Id");
            RenameTable(name: "dbo.RecurringTournaments", newName: "ReccuringTournaments");
        }
    }
}
