namespace ShahadatApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateV100 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Area",
                c => new
                    {
                        MktbCode = c.String(nullable: false, maxLength: 3),
                        Name = c.String(nullable: false, maxLength: 128),
                        Talab_TalabNum = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MktbCode)
                .ForeignKey("dbo.Talab", t => t.Talab_TalabNum)
                .Index(t => t.Talab_TalabNum);
            
            CreateTable(
                "dbo.Talab",
                c => new
                    {
                        TalabNum = c.String(nullable: false, maxLength: 128),
                        Kawmy = c.String(maxLength: 14),
                        TalabStatus = c.String(),
                        PrintAreaID = c.String(maxLength: 3),
                        RecievedAreaID = c.String(maxLength: 3),
                        ServicePostion = c.String(),
                        ServiceType = c.String(),
                        PrintDate = c.DateTime(nullable: false),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.TalabNum)
                .ForeignKey("dbo.Area", t => t.RecievedAreaID)
                .ForeignKey("dbo.Citizen", t => t.Kawmy)
                .ForeignKey("dbo.Area", t => t.PrintAreaID)
                .Index(t => t.Kawmy)
                .Index(t => t.PrintAreaID)
                .Index(t => t.RecievedAreaID);
            
            CreateTable(
                "dbo.Citizen",
                c => new
                    {
                        Kawmy = c.String(nullable: false, maxLength: 14),
                        FullName = c.String(nullable: false),
                        Phone = c.String(maxLength: 13),
                        WhatsAppUser = c.Boolean(nullable: false),
                        Milad = c.String(maxLength: 4),
                        MktbCode = c.String(maxLength: 3),
                        Mosalsal = c.String(maxLength: 8),
                        IDImage = c.String(),
                        PersonalPhoto = c.String(),
                    })
                .PrimaryKey(t => t.Kawmy)
                .ForeignKey("dbo.Area", t => t.MktbCode)
                .Index(t => t.MktbCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Area", "Talab_TalabNum", "dbo.Talab");
            DropForeignKey("dbo.Talab", "PrintAreaID", "dbo.Area");
            DropForeignKey("dbo.Talab", "Kawmy", "dbo.Citizen");
            DropForeignKey("dbo.Citizen", "MktbCode", "dbo.Area");
            DropForeignKey("dbo.Talab", "RecievedAreaID", "dbo.Area");
            DropIndex("dbo.Citizen", new[] { "MktbCode" });
            DropIndex("dbo.Talab", new[] { "RecievedAreaID" });
            DropIndex("dbo.Talab", new[] { "PrintAreaID" });
            DropIndex("dbo.Talab", new[] { "Kawmy" });
            DropIndex("dbo.Area", new[] { "Talab_TalabNum" });
            DropTable("dbo.Citizen");
            DropTable("dbo.Talab");
            DropTable("dbo.Area");
        }
    }
}
