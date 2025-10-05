SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fStrategy]()

RETURNS TABLE 
AS
RETURN 
(
Select s.Id, s.Code sCode, s.[Key] as sKey, a.Code aCode, a.Id  aId, t.Code as tCode, t.Id tId,
Substring(s.[Key],CharIndex('TimeInt',s.[Key]), Len(s.[Key]) - CharIndex('TimeInt',s.[Key])) as ti
from Strategies s
inner join Accounts a on s.AccountId = a.Id
inner join Tickers t on s.TickerId = t.Id
)

GO