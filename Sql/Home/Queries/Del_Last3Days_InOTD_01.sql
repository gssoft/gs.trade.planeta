use DbTrade2

-- ORDERS
select top 3 Cast( o.Created as Date) dt, Count(*) OrdersCount
from Orders o
Group by Cast( o.Created as Date)
Order by Cast( o.Created as Date) DESC
GO

DELETE from Orders
where Cast( Created as Date) NOT IN
(
select top 3 Cast( o.Created as Date) dt
from Orders o
Group by Cast( o.Created as Date)
Order by Cast( o.Created as Date) DESC
)
GO
select top 3 Cast( o.Created as Date) dt, Count(*) OrdersCount
from Orders o
Group by Cast( o.Created as Date)
Order by Cast( o.Created as Date) DESC
GO
-- TRADES
select top 3 Cast( DT as Date) dt, Count(*) TradesCount
from Trades o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
GO
DELETE from Trades
where Cast( DT as Date) NOT IN
(
select top 3 Cast( DT as Date) dt
from Trades o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
)
GO
select top 3 Cast( DT as Date) dt, Count(*) TradesCount
from Trades o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
GO

-- DEALS

select top 3 Cast( DT as Date) dt, Count(*) DealsCount
from Deals o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
GO

DELETE from Deals
where Cast( DT as Date) NOT IN
(
select top 3 Cast( DT as Date) dt
from Deals o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
)
GO
select top 3 Cast( DT as Date) dt, Count(*) DealsCount
from Deals o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
GO

-- EventLogs --

Use EventLog

select top 3 Cast( DT as Date) dt, Count(*) EvlisCount
from EventLogItems o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
GO

DELETE from EventLogItems
where Cast( DT as Date) NOT IN
(
select top 3 Cast( DT as Date) dt
from EventLogItems o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
)
GO
select top 3 Cast( DT as Date) dt, Count(*) EvlisCount
from EventLogItems o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
GO
