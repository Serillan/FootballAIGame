namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveAiToPlayer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "ActiveAi", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "ActiveAi");
        }
    }
}
