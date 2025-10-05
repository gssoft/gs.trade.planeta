use EventLogFinReal
SELECT Count(*) As Count
FROM [dbo].[EventLogItems]
go
SELECT TOP 10000 [EventLogItemID]
      ,[DT]
      ,[ResultCode]
      ,[Subject]
      ,[Source]
      ,[Entity]
      ,[Operation]
      ,[Description]
      ,[Object]
      ,[Index]
      ,[EventLogID]
  FROM [dbo].[EventLogItems]
  order by [EventLogItemID] desc
  go