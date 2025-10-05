namespace GS.Trade.TimeSeries.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BarVolume : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bars", "Volume", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bars", "Volume");
        }
    }
}
