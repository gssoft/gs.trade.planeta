namespace GS.Trade.TimeSeries.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bars",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BarSeriesId = c.Long(nullable: false),
                        Open = c.Double(nullable: false),
                        High = c.Double(nullable: false),
                        Low = c.Double(nullable: false),
                        Close = c.Double(nullable: false),
                        DT = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TimeSeries", t => t.BarSeriesId, cascadeDelete: true)
                .Index(t => t.BarSeriesId);
            
            CreateTable(
                "dbo.TimeSeries",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TickerId = c.Long(nullable: false),
                        TimeIntId = c.Long(nullable: false),
                        QuoteProviderId = c.Long(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedDT = c.DateTime(),
                        ModifiedDT = c.DateTime(),
                        TimeSeriesType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuoteProviders", t => t.QuoteProviderId, cascadeDelete: true)
                .ForeignKey("dbo.Tickers", t => t.TickerId, cascadeDelete: true)
                .ForeignKey("dbo.TimeInts", t => t.TimeIntId, cascadeDelete: true)
                .Index(t => t.TickerId)
                .Index(t => t.TimeIntId)
                .Index(t => t.QuoteProviderId);
            
            CreateTable(
                "dbo.QuoteProviders",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedDT = c.DateTime(),
                        ModifiedDT = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tickers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TradeBoardId = c.Long(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedDT = c.DateTime(),
                        ModifiedDT = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TradeBoards", t => t.TradeBoardId, cascadeDelete: true)
                .Index(t => t.TradeBoardId);
            
            CreateTable(
                "dbo.TradeBoards",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedDT = c.DateTime(),
                        ModifiedDT = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TimeInts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TimeInterval = c.Int(nullable: false),
                        TimeShift = c.Int(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedDT = c.DateTime(),
                        ModifiedDT = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Ticks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TickSeriesId = c.Long(nullable: false),
                        Last = c.Double(nullable: false),
                        DT = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TimeSeries", t => t.TickSeriesId, cascadeDelete: true)
                .Index(t => t.TickSeriesId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ticks", "TickSeriesId", "dbo.TimeSeries");
            DropForeignKey("dbo.Bars", "BarSeriesId", "dbo.TimeSeries");
            DropForeignKey("dbo.TimeSeries", "TimeIntId", "dbo.TimeInts");
            DropForeignKey("dbo.TimeSeries", "TickerId", "dbo.Tickers");
            DropForeignKey("dbo.Tickers", "TradeBoardId", "dbo.TradeBoards");
            DropForeignKey("dbo.TimeSeries", "QuoteProviderId", "dbo.QuoteProviders");
            DropIndex("dbo.Ticks", new[] { "TickSeriesId" });
            DropIndex("dbo.Tickers", new[] { "TradeBoardId" });
            DropIndex("dbo.TimeSeries", new[] { "QuoteProviderId" });
            DropIndex("dbo.TimeSeries", new[] { "TimeIntId" });
            DropIndex("dbo.TimeSeries", new[] { "TickerId" });
            DropIndex("dbo.Bars", new[] { "BarSeriesId" });
            DropTable("dbo.Ticks");
            DropTable("dbo.TimeInts");
            DropTable("dbo.TradeBoards");
            DropTable("dbo.Tickers");
            DropTable("dbo.QuoteProviders");
            DropTable("dbo.TimeSeries");
            DropTable("dbo.Bars");
        }
    }
}
