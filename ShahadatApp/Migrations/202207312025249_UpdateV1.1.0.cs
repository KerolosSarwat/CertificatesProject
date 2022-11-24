namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV110 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Talab", "isNegotiated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Talab", "isNegotiated");
        }
    }
}
