using System.Data.Entity.Migrations;

namespace FootballAIGame.Web.Migrations
{
    public partial class UpdateChallenges : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Challenges");
            AddColumn("dbo.Challenges", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Challenges", "Id");
            DropColumn("dbo.Challenges", "ChallengingPlayerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Challenges", "ChallengingPlayerId", c => c.String(nullable: false, maxLength: 128));
            DropPrimaryKey("dbo.Challenges");
            DropColumn("dbo.Challenges", "Id");
            AddPrimaryKey("dbo.Challenges", "ChallengingPlayerId");
        }
    }
}
