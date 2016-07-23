namespace Vertragsmanagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        describtion = c.String(),
                        contractID = c.Int(nullable: false),
                        userID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Contracts", t => t.contractID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.userID, cascadeDelete: true)
                .Index(t => t.contractID)
                .Index(t => t.userID);
            
            AddColumn("dbo.Contracts", "locality", c => c.String());
            AddColumn("dbo.Contracts", "partnerIs", c => c.Int());
            DropColumn("dbo.Contracts", "creditor");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contracts", "creditor", c => c.Boolean());
            DropForeignKey("dbo.Tasks", "userID", "dbo.Users");
            DropForeignKey("dbo.Tasks", "contractID", "dbo.Contracts");
            DropIndex("dbo.Tasks", new[] { "userID" });
            DropIndex("dbo.Tasks", new[] { "contractID" });
            DropColumn("dbo.Contracts", "partnerIs");
            DropColumn("dbo.Contracts", "locality");
            DropTable("dbo.Tasks");
        }
    }
}
