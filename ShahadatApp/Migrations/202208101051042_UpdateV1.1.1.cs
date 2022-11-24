namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV111 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Talab", "isCanceled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Talab", "isCanceled");
        }
    }
}
