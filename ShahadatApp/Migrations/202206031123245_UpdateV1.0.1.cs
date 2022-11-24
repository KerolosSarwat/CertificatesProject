namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV101 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Area", "Talab_TalabNum", "dbo.Talab");
            DropIndex("dbo.Area", new[] { "Talab_TalabNum" });
            AddColumn("dbo.Area", "Location", c => c.String());
            DropColumn("dbo.Area", "Talab_TalabNum");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Area", "Talab_TalabNum", c => c.String(maxLength: 128));
            DropColumn("dbo.Area", "Location");
            CreateIndex("dbo.Area", "Talab_TalabNum");
            AddForeignKey("dbo.Area", "Talab_TalabNum", "dbo.Talab", "TalabNum");
        }
    }
}
