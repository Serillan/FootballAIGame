using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class RenamePlayerProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "SelectedAi", c => c.String());
            DropColumn("dbo.Players", "ActiveAi");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Players", "ActiveAi", c => c.String());
            DropColumn("dbo.Players", "SelectedAi");
        }
    }
}
