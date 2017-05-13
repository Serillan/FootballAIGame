using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class AddAiNamesToMatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "Player1Ai", c => c.String());
            AddColumn("dbo.Matches", "Player2Ai", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Matches", "Player2Ai");
            DropColumn("dbo.Matches", "Player1Ai");
        }
    }
}
