namespace Vertragsmanagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _9 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tasks", "userID", "dbo.Users");
            DropIndex("dbo.Tasks", new[] { "userID" });
            CreateTable(
                "dbo.UserMappedToTask",
                c => new
                    {
                        TaskID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TaskID, t.UserID })
                .ForeignKey("dbo.Tasks", t => t.TaskID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.TaskID)
                .Index(t => t.UserID);
            
            AddColumn("dbo.Tasks", "deadline", c => c.DateTime());
            DropColumn("dbo.Departments", "ActiveDirectoryGroupName");
            DropColumn("dbo.Tasks", "userID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tasks", "userID", c => c.Int(nullable: false));
            AddColumn("dbo.Departments", "ActiveDirectoryGroupName", c => c.String());
            DropForeignKey("dbo.UserMappedToTask", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserMappedToTask", "TaskID", "dbo.Tasks");
            DropIndex("dbo.UserMappedToTask", new[] { "UserID" });
            DropIndex("dbo.UserMappedToTask", new[] { "TaskID" });
            DropColumn("dbo.Tasks", "deadline");
            DropTable("dbo.UserMappedToTask");
            CreateIndex("dbo.Tasks", "userID");
            AddForeignKey("dbo.Tasks", "userID", "dbo.Users", "ID", cascadeDelete: true);
        }
    }
}
