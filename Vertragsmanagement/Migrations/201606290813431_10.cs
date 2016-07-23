namespace Vertragsmanagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SpecialAuthorization",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        ContractID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserID, t.ContractID })
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.Contracts", t => t.ContractID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.ContractID);
            
            AddColumn("dbo.Contracts", "frameworkContractID", c => c.Int());
            AddColumn("dbo.Contracts", "isframeworkContract", c => c.Boolean(nullable: false, defaultValue: false));
            CreateIndex("dbo.Contracts", "frameworkContractID");
            AddForeignKey("dbo.Contracts", "frameworkContractID", "dbo.Contracts", "intID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contracts", "frameworkContractID", "dbo.Contracts");
            DropForeignKey("dbo.SpecialAuthorization", "ContractID", "dbo.Contracts");
            DropForeignKey("dbo.SpecialAuthorization", "UserID", "dbo.Users");
            DropIndex("dbo.SpecialAuthorization", new[] { "ContractID" });
            DropIndex("dbo.SpecialAuthorization", new[] { "UserID" });
            DropIndex("dbo.Contracts", new[] { "frameworkContractID" });
            DropColumn("dbo.Contracts", "isframeworkContract");
            DropColumn("dbo.Contracts", "frameworkContractID");
            DropTable("dbo.SpecialAuthorization");
        }
    }
}
