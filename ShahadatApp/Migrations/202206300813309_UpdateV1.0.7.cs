namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV107 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Talab", "TmEbla8Msg", c => c.Boolean(nullable: false));
            AddColumn("dbo.Talab", "TmDaf3Msg", c => c.Boolean(nullable: false));
            AddColumn("dbo.Talab", "ServiceReadyMsg", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Talab", "ServiceReadyMsg");
            DropColumn("dbo.Talab", "TmDaf3Msg");
            DropColumn("dbo.Talab", "TmEbla8Msg");
        }
    }
}
