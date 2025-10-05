-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_EVLS_CLEAR_2_DAYS]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @EVL_ID INT
DECLARE @EVL_CODE VARCHAR (100)
DECLARE @MIN_DT DATE
DECLARE @MAX_DT DATE

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
Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2, DATEDIFF(day, Min(DT), Max(DT)) +1 [DAYS]
From EventLogItems
where	EventLogID = @EVL_ID

SELECT @MIN_DT = Min(dt), @MAX_DT = Max(dt)
FROM
(
select top 2 Cast( DT as Date) dt
from EventLogItems o
WHERE EventLogID = @EVL_ID
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
) a

PRINT(@MIN_DT)

if @MIN_DT IS NOT NULL
BEGIN

--SET @MIN_DT = CAST(@MIN_DT as Datetime)
--PRINT(@MIN_DT)

DELETE from EventLogItems 
WHERE	EventLogID = @EVL_ID 
AND		@MIN_DT IS NOT NULL
AND		DT < @MIN_DT 

END

Select DB_NAME() [DB_NAME], @EVL_CODE [EVL_NAME], 'After Delete' as Operation, Count(*) as Cnt, 
Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2, DATEDIFF(day, Min(DT), Max(DT)) +1 [DAYS]
From EventLogItems
where	EventLogID = @EVL_ID

FETCH NEXT FROM @CURSOR INTO @EVL_ID, @EVL_CODE
END
CLOSE @CURSOR

PRINT('OK')
END
GO
