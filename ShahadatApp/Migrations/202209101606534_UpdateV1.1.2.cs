namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV112 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Citizen", "MorfkatRecieved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Citizen", "MorfkatRecieved");
        }
    }
}
