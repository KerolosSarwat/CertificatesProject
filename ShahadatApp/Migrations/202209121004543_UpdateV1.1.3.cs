namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV113 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Talab", "CardServicePostion", c => c.String());
            AddColumn("dbo.Talab", "CardPrintDate", c => c.DateTime());
            AddColumn("dbo.Talab", "CardServiceReadyMsg", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Talab", "CardServiceReadyMsg");
            DropColumn("dbo.Talab", "CardPrintDate");
            DropColumn("dbo.Talab", "CardServicePostion");
        }
    }
}
