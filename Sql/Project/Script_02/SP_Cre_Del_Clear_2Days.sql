
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE SP_Clear_DB_Last2Days 
AS
BEGIN
	SET NOCOUNT ON;

-- ORDERS

Select 'Orders To Delete' as Operation, Count(*) as Cnt, 
				Min(Cast( Modified as Date)) as Dt1, Max(Cast( Modified as Date)) as Dt2
From Orders

DELETE from Orders
where Cast( Created as Date) NOT IN
(
select top 2 Cast( o.Created as Date) dt
from Orders o
Group by Cast( o.Created as Date)
Order by Cast( o.Created as Date) DESC
)


select  Cast( o.Created as Date) DT, Count(*) OrdersCount
from Orders o
Group by Cast( o.Created as Date)
Order by Cast( o.Created as Date) DESC


Select 'Orders After Delete' as Operation, Count(*) as Cnt, 
				Min(Cast( Modified as Date)) as Dt1, Max(Cast( Modified as Date)) as Dt2
From Orders


-- TRADES
Select 'Trades To Delete' as Operation, Count(*) as Cnt, 
				Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2
From Trades


DELETE from Trades
where Cast( DT as Date) NOT IN
(
select top 2 Cast( DT as Date) DT
from Trades o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
)

select  Cast( DT as Date) dt, Count(*) TradesCount
from Trades o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC


Select 'Trades After Delete' as Operation, Count(*) as Cnt, 
				Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2
From Trades


-- DEALS
--Select 'Deals Count' as Operation

Select 'Deals Count' as Operation, Count(*) as Cnt,
			Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2
from Deals
END
GO
