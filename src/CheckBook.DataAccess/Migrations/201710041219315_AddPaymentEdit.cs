namespace CheckBook.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPaymentEdit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentEdits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false),
                        PaymentId = c.Int(nullable: false),
                        EditorId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        AmountOriginal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountNew = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.EditorId, cascadeDelete: false)
                .ForeignKey("dbo.Payments", t => t.PaymentId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.PaymentId)
                .Index(t => t.EditorId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentEdits", "UserId", "dbo.Users");
            DropForeignKey("dbo.PaymentEdits", "PaymentId", "dbo.Payments");
            DropForeignKey("dbo.PaymentEdits", "EditorId", "dbo.Users");
            DropIndex("dbo.PaymentEdits", new[] { "UserId" });
            DropIndex("dbo.PaymentEdits", new[] { "EditorId" });
            DropIndex("dbo.PaymentEdits", new[] { "PaymentId" });
            DropTable("dbo.PaymentEdits");
        }
    }
}
