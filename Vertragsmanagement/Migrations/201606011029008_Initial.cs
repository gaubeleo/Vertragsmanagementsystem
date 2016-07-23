namespace Vertragsmanagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContractCategories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ContractPartners",
                c => new
                    {
                        accountNumbre = c.String(nullable: false, maxLength: 128),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.accountNumbre);
            
            CreateTable(
                "dbo.Contracts",
                c => new
                    {
                        intID = c.Int(nullable: false, identity: true),
                        extID = c.Int(),
                        titel = c.String(),
                        contractValue = c.Decimal(precision: 18, scale: 2),
                        contractCosts = c.Decimal(precision: 18, scale: 2),
                        paymentIntervall = c.Int(),
                        noticePeriod = c.Int(),
                        earliestNoticePeriod = c.Int(),
                        contractExtension = c.Int(),
                        valueTax = c.Decimal(precision: 18, scale: 2),
                        creditor = c.Boolean(),
                        remark = c.String(),
                        state = c.Int(nullable: false),
                        categoryID = c.Int(),
                        subcategoryID = c.Int(),
                        contractSpeciesID = c.Int(),
                        signerID = c.Int(),
                        personInChargeID = c.Int(),
                        mappedDepartmentID = c.Int(),
                        observingDepartmentID = c.Int(),
                        partnerID = c.String(maxLength: 128),
                        paymentBegin = c.DateTime(),
                        startDate = c.DateTime(),
                        creationDate = c.DateTime(),
                        noticeDate = c.DateTime(),
                        endDate = c.DateTime(),
                        remindingDate = c.DateTime(),
                        escalationDate = c.DateTime(),
                        escalated = c.Boolean(nullable: false),
                        reminded = c.Boolean(nullable: false),
                        noticeReminded = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.intID)
                .ForeignKey("dbo.ContractCategories", t => t.categoryID)
                .ForeignKey("dbo.Departments", t => t.mappedDepartmentID)
                .ForeignKey("dbo.Departments", t => t.observingDepartmentID)
                .ForeignKey("dbo.ContractPartners", t => t.partnerID)
                .ForeignKey("dbo.Users", t => t.personInChargeID)
                .ForeignKey("dbo.Users", t => t.signerID)
                .ForeignKey("dbo.ContractSpecies", t => t.contractSpeciesID)
                .ForeignKey("dbo.ContractSubcategories", t => t.subcategoryID)
                .Index(t => t.categoryID)
                .Index(t => t.subcategoryID)
                .Index(t => t.contractSpeciesID)
                .Index(t => t.signerID)
                .Index(t => t.personInChargeID)
                .Index(t => t.mappedDepartmentID)
                .Index(t => t.observingDepartmentID)
                .Index(t => t.partnerID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        surname = c.String(),
                        email = c.String(),
                        activeDirectoryName = c.String(),
                        departmentID = c.Int(),
                        administrator = c.Boolean(nullable: false),
                        dispatcher = c.Boolean(nullable: false),
                        coordinator = c.Boolean(nullable: false),
                        signer = c.Boolean(nullable: false),
                        personInCharge = c.Boolean(nullable: false),
                        blocked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Departments", t => t.departmentID)
                .Index(t => t.departmentID);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        ActiveDirectoryGroupName = c.String(),
                        mandantID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Mandants", t => t.mandantID)
                .Index(t => t.mandantID);
            
            CreateTable(
                "dbo.Mandants",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        title = c.String(),
                        remark = c.String(),
                        link = c.String(),
                        Contract_intID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Contracts", t => t.Contract_intID)
                .Index(t => t.Contract_intID);
            
            CreateTable(
                "dbo.OtherFeatures",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        title = c.String(),
                        describtion = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ContractSpecies",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ContractSubcategories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        categoryID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ContractCategories", t => t.categoryID)
                .Index(t => t.categoryID);
            
            CreateTable(
                "dbo.CostCentreDivides",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        contractID = c.Int(nullable: false),
                        costCentreID = c.Int(nullable: false),
                        percentage = c.Single(nullable: false),
                        contract_intID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Contracts", t => t.contract_intID)
                .ForeignKey("dbo.CostCentres", t => t.costCentreID, cascadeDelete: true)
                .Index(t => t.costCentreID)
                .Index(t => t.contract_intID);
            
            CreateTable(
                "dbo.CostCentres",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        describtion = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DepartmentCoordinators",
                c => new
                    {
                        DepartmentID = c.Int(nullable: false),
                        CoordinatorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DepartmentID, t.CoordinatorID })
                .ForeignKey("dbo.Departments", t => t.DepartmentID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CoordinatorID, cascadeDelete: true)
                .Index(t => t.DepartmentID)
                .Index(t => t.CoordinatorID);
            
            CreateTable(
                "dbo.DepartmentDispatchers",
                c => new
                    {
                        DepartmentID = c.Int(nullable: false),
                        DispatcherID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DepartmentID, t.DispatcherID })
                .ForeignKey("dbo.Departments", t => t.DepartmentID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.DispatcherID, cascadeDelete: true)
                .Index(t => t.DepartmentID)
                .Index(t => t.DispatcherID);
            
            CreateTable(
                "dbo.MandantCoordinators",
                c => new
                    {
                        MandantID = c.Int(nullable: false),
                        CoordinatorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MandantID, t.CoordinatorID })
                .ForeignKey("dbo.Mandants", t => t.MandantID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CoordinatorID, cascadeDelete: true)
                .Index(t => t.MandantID)
                .Index(t => t.CoordinatorID);
            
            CreateTable(
                "dbo.UserContractAuthorization",
                c => new
                    {
                        ContractID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ContractID, t.UserID })
                .ForeignKey("dbo.Contracts", t => t.ContractID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.ContractID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.ContractFeatures",
                c => new
                    {
                        ContractID = c.Int(nullable: false),
                        FeatureID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ContractID, t.FeatureID })
                .ForeignKey("dbo.Contracts", t => t.ContractID, cascadeDelete: true)
                .ForeignKey("dbo.OtherFeatures", t => t.FeatureID, cascadeDelete: true)
                .Index(t => t.ContractID)
                .Index(t => t.FeatureID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CostCentreDivides", "costCentreID", "dbo.CostCentres");
            DropForeignKey("dbo.CostCentreDivides", "contract_intID", "dbo.Contracts");
            DropForeignKey("dbo.Contracts", "subcategoryID", "dbo.ContractSubcategories");
            DropForeignKey("dbo.ContractSubcategories", "categoryID", "dbo.ContractCategories");
            DropForeignKey("dbo.Contracts", "contractSpeciesID", "dbo.ContractSpecies");
            DropForeignKey("dbo.Contracts", "signerID", "dbo.Users");
            DropForeignKey("dbo.Contracts", "personInChargeID", "dbo.Users");
            DropForeignKey("dbo.Contracts", "partnerID", "dbo.ContractPartners");
            DropForeignKey("dbo.ContractFeatures", "FeatureID", "dbo.OtherFeatures");
            DropForeignKey("dbo.ContractFeatures", "ContractID", "dbo.Contracts");
            DropForeignKey("dbo.Contracts", "observingDepartmentID", "dbo.Departments");
            DropForeignKey("dbo.Contracts", "mappedDepartmentID", "dbo.Departments");
            DropForeignKey("dbo.Documents", "Contract_intID", "dbo.Contracts");
            DropForeignKey("dbo.Contracts", "categoryID", "dbo.ContractCategories");
            DropForeignKey("dbo.UserContractAuthorization", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserContractAuthorization", "ContractID", "dbo.Contracts");
            DropForeignKey("dbo.Users", "departmentID", "dbo.Departments");
            DropForeignKey("dbo.Departments", "mandantID", "dbo.Mandants");
            DropForeignKey("dbo.MandantCoordinators", "CoordinatorID", "dbo.Users");
            DropForeignKey("dbo.MandantCoordinators", "MandantID", "dbo.Mandants");
            DropForeignKey("dbo.DepartmentDispatchers", "DispatcherID", "dbo.Users");
            DropForeignKey("dbo.DepartmentDispatchers", "DepartmentID", "dbo.Departments");
            DropForeignKey("dbo.DepartmentCoordinators", "CoordinatorID", "dbo.Users");
            DropForeignKey("dbo.DepartmentCoordinators", "DepartmentID", "dbo.Departments");
            DropIndex("dbo.ContractFeatures", new[] { "FeatureID" });
            DropIndex("dbo.ContractFeatures", new[] { "ContractID" });
            DropIndex("dbo.UserContractAuthorization", new[] { "UserID" });
            DropIndex("dbo.UserContractAuthorization", new[] { "ContractID" });
            DropIndex("dbo.MandantCoordinators", new[] { "CoordinatorID" });
            DropIndex("dbo.MandantCoordinators", new[] { "MandantID" });
            DropIndex("dbo.DepartmentDispatchers", new[] { "DispatcherID" });
            DropIndex("dbo.DepartmentDispatchers", new[] { "DepartmentID" });
            DropIndex("dbo.DepartmentCoordinators", new[] { "CoordinatorID" });
            DropIndex("dbo.DepartmentCoordinators", new[] { "DepartmentID" });
            DropIndex("dbo.CostCentreDivides", new[] { "contract_intID" });
            DropIndex("dbo.CostCentreDivides", new[] { "costCentreID" });
            DropIndex("dbo.ContractSubcategories", new[] { "categoryID" });
            DropIndex("dbo.Documents", new[] { "Contract_intID" });
            DropIndex("dbo.Departments", new[] { "mandantID" });
            DropIndex("dbo.Users", new[] { "departmentID" });
            DropIndex("dbo.Contracts", new[] { "partnerID" });
            DropIndex("dbo.Contracts", new[] { "observingDepartmentID" });
            DropIndex("dbo.Contracts", new[] { "mappedDepartmentID" });
            DropIndex("dbo.Contracts", new[] { "personInChargeID" });
            DropIndex("dbo.Contracts", new[] { "signerID" });
            DropIndex("dbo.Contracts", new[] { "contractSpeciesID" });
            DropIndex("dbo.Contracts", new[] { "subcategoryID" });
            DropIndex("dbo.Contracts", new[] { "categoryID" });
            DropTable("dbo.ContractFeatures");
            DropTable("dbo.UserContractAuthorization");
            DropTable("dbo.MandantCoordinators");
            DropTable("dbo.DepartmentDispatchers");
            DropTable("dbo.DepartmentCoordinators");
            DropTable("dbo.CostCentres");
            DropTable("dbo.CostCentreDivides");
            DropTable("dbo.ContractSubcategories");
            DropTable("dbo.ContractSpecies");
            DropTable("dbo.OtherFeatures");
            DropTable("dbo.Documents");
            DropTable("dbo.Mandants");
            DropTable("dbo.Departments");
            DropTable("dbo.Users");
            DropTable("dbo.Contracts");
            DropTable("dbo.ContractPartners");
            DropTable("dbo.ContractCategories");
        }
    }
}
