USE EventLog1

DECLARE @EVL_ID INT
DECLARE @EVL_CODE VARCHAR (100)
DECLARE @CURSOR CURSOR

SET @CURSOR = CURSOR SCROLL

FOR
SELECT EventLogID, Code
FROM dbo.EventLogs
OPEN @CURSOR

FETCH NEXT FROM @CURSOR INTO @EVL_ID, @EVL_CODE
WHILE @@FETCH_STATUS = 0
BEGIN

SELECT DB_NAME() [DB_NAME], [Code] [EVL_NAME], 'DELETE' [Operation] 
FROM EventLogs WHERE EventLogID = @EVL_ID

Select DB_NAME() [DB_NAME], @EVL_CODE [EVL_NAME], 'Before Delete' as Operation, Count(*) as Cnt, 
Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2
From EventLogItems
where	EventLogID = @EVL_ID

DELETE from EventLogItems 
where	EventLogID = @EVL_ID 
		AND Cast( DT as Date) NOT IN
(
select top 2 Cast( DT as Date) dt
from EventLogItems o
WHERE EventLogID = @EVL_ID
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
)

Select DB_NAME() [DB_NAME], @EVL_CODE [EVL_NAME], 'After Delete' as Operation, Count(*) as Cnt, 
Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2
From EventLogItems
where	EventLogID = @EVL_ID

FETCH NEXT FROM @CURSOR INTO @EVL_ID, @EVL_CODE
END
CLOSE @CURSOR
