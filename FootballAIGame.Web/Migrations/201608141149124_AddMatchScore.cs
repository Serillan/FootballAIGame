using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class AddMatchScore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "Score", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Matches", "Score");
        }
    }
}
