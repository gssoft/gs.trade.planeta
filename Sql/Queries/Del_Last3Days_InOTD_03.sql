use DbTrade2

-- ORDERS
Select 'Orders Delete'
go
DELETE from Orders
where Cast( Created as Date) NOT IN
(
select top 3 Cast( o.Created as Date) dt
from Orders o
Group by Cast( o.Created as Date)
Order by Cast( o.Created as Date) DESC
)
GO
select  Cast( o.Created as Date) dt, Count(*) OrdersCount
from Orders o
Group by Cast( o.Created as Date)
Order by Cast( o.Created as Date) DESC
GO
-- TRADES
Select 'Trades Delete'
go
DELETE from Trades
where Cast( DT as Date) NOT IN
(
select top 3 Cast( DT as Date) dt
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

-- DEALS
Select 'Deals Delete'
go
DELETE from Deals
where Cast( DT as Date) NOT IN
(
select top 3 Cast( DT as Date) dt
from Deals o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
)
GO
select  Cast( DT as Date) dt, Count(*) DealsCount
from Deals o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
GO

-- GSExceptions
Select 'GSExceptions Delete'
go
DELETE from GSExceptions
where Cast( [DateTime] as Date) NOT IN
(
select top 3 Cast( [DateTime] as Date) dt
from GSExceptions o
Group by Cast( [DateTime] as Date)
Order by Cast( [DateTime] as Date) DESC
)
GO
select  Cast( [DateTime] as Date) dt, Count(*) GSExceptionsCount
from GSExceptions o
Group by Cast( [DateTime] as Date)
Order by Cast( [DateTime] as Date) DESC
GO

-- EventLogs --
Select 'EventLog Delete'
Use EventLog

DELETE from EventLogItems
where Cast( DT as Date) NOT IN
(
select top 3 Cast( DT as Date) dt
from EventLogItems o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
)
GO
select Cast( DT as Date) dt, Count(*) EvlisCount
from EventLogItems o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
GO
