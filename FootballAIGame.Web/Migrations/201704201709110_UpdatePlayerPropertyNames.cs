namespace FootballAIGame.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePlayerPropertyNames : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Players", "SelectedAi", "SelectedAI");
            RenameColumn("dbo.Players", "ActiveAis", "ActiveAIs");
        }

        public override void Down()
        {
            RenameColumn("dbo.Players", "SelectedAI", "SelectedAi");
            RenameColumn("dbo.Players", "ActiveAIs", "ActiveAis");
        }
    }
}
