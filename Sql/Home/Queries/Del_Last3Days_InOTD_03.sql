use DbTrade2

DECLARE @DT DateTime
DECLARE @DT1 DateTime
DECLARE @DT2 DateTime

DECLARE @CUR_TABLE VARCHAR(100)
SET @CUR_TABLE = 'ORDERS'
-- ORDERS
SET @DT1 = GETDATE()
SELECT @DT1 [DateTime], DB_NAME()  [DB_NAME], @CUR_TABLE [TABLE]

SET @DT = GeTDATE()
Select @DT [DateTime], 'Orders To Delete' as Operation, Count(*) as Cnt, 
				Min(Cast( Modified as Date)) as Dt1, Max(Cast( Modified as Date)) as Dt2
From Orders
-- Go

DELETE from Orders
where Cast( Created as Date) NOT IN
(
select top 2 Cast( o.Created as Date) dt
from Orders o
Group by Cast( o.Created as Date)
Order by Cast( o.Created as Date) DESC
)
--GO
select  Cast( o.Created as Date) DT, Count(*) OrdersCount
from Orders o
Group by Cast( o.Created as Date)
Order by Cast( o.Created as Date) DESC
--GO

SET @DT = GeTDATE()
Select @DT [DateTime],'Orders After Delete' as Operation, Count(*) as Cnt, 
				Min(Cast( Modified as Date)) as Dt1, Max(Cast( Modified as Date)) as Dt2
From Orders
--Go

SET @DT2 = GeTDATE()

Select @DT2 [DateTime], DB_NAME() [DB_NAME], @CUR_TABLE [TABLE],
CAST((@dt2 - @dt1) AS Time) as 'Elapsed_Time'
GO

-- TRADES
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
DECLARE @DT DateTime
DECLARE @DT1 DateTime
DECLARE @DT2 DateTime

DECLARE @CUR_TABLE VARCHAR(100)

SET @CUR_TABLE = 'DEALS'
-- ORDERS
SET @DT1 = GETDATE()
SELECT @DT1 [DateTime], DB_NAME()  [DB_NAME], @CUR_TABLE [CUR_TABLE]

Select 'Deals Count' as Operation
--go
Select 'Deals Count' as Operation, Count(*) as Cnt,
			Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2
from Deals
-- go

SET @DT2 = GETDATE()
SELECT @DT2 [DateTime], DB_NAME()  [DB_NAME],  @CUR_TABLE [CUR_TABLE],
CAST((@dt2 - @dt1) AS Time) as 'Elapsed_Time'

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

-- EventLogs
--USE EventLog
--EXECUTE dbo.SP_EVLS_CLEAR_2_DAYS
--USE EventLog1
--EXECUTE dbo.SP_EVLS_CLEAR_2_DAYS
--USE EvlContext
--EXECUTE dbo.SP_EVLS_CLEAR_2_DAYS

/*
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
*/