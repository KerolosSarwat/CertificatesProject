namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV106 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Talab", "CreateDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Talab", "CreateDate");
        }
    }
}
