using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class UpdateMatchProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "Time", c => c.DateTime(nullable: false));
            AddColumn("dbo.Matches", "Winner", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Matches", "Winner");
            DropColumn("dbo.Matches", "Time");
        }
    }
}
