use DbTrade2

-- ORDERS
Select 'Orders To Delete' as Operation, Count(*) as Cnt, 
    Min(Cast( Modified as Date)) as Dt1, Max(Cast( Modified as Date)) as Dt2
From Orders
Go

DELETE from Orders
where Cast( Created as Date) NOT IN
(
select top 2 Cast( o.Created as Date) dt
from Orders o
Group by Cast( o.Created as Date)
Order by Cast( o.Created as Date) DESC
)
GO
select  Cast( o.Created as Date) DT, Count(*) OrdersCount
from Orders o
Group by Cast( o.Created as Date)
Order by Cast( o.Created as Date) DESC
GO

Select 'Orders After Delete' as Operation, Count(*) as Cnt, 
    Min(Cast( Modified as Date)) as Dt1, Max(Cast( Modified as Date)) as Dt2
From Orders
Go

-- TRADES
Select 'Trades Delete' as Operation
go
Select 'Trades To Delete' as Operation, Count(*) as Cnt, 
    Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2
From Trades
Go

DELETE from Trades
where Cast( DT as Date) NOT IN
(
select top 2 Cast( DT as Date) DT
from Trades o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
)
GO
select  Cast( DT as Date) dt, Count(*) TradesCount
from Trades o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
GO

Select 'Trades After Delete' as Operation, Count(*) as Cnt, 
    Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2
From Trades
Go

-- DEALS
Select 'Deals Delete' as Operation
go
Select 'Deals Count' as Operation, Count(*) as Cnt,
   Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2
from Deals
go

--select  Cast( DT as Date) dt, Count(*) DealsCount
--FROM Deals
--Group by Cast( DT as Date)
--Order by Cast( DT as Date) DESC

--Select 'Deals Delete' as Operation

--DELETE from Deals
--where Cast( DT as Date) NOT IN
--(
--select top 3 Cast( DT as Date) dt
--from Deals o
--Group by Cast( DT as Date)
--Order by Cast( DT as Date) DESC
--)
--GO
--select  Cast( DT as Date) dt, Count(*) DealsCount
--from Deals o
--Group by Cast( DT as Date)
--Order by Cast( DT as Date) DESC
--GO

-- EventLogs --

Use EventLog
Select 'EventLog To Delete' as Operation, Count(*) as Cnt, 
    Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2
From EventLogItems
Go
DELETE from EventLogItems 
where Cast( DT as Date) NOT IN
(
select top 2 Cast( DT as Date) dt
from EventLogItems o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
)
GO
select Cast( DT as Date) dt, Count(*) as EvliCount
from EventLogItems o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
GO
Select 'EventLog After Delete' as Operation, Count(*) as Cnt, 
    Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2
From EventLogItems
Go