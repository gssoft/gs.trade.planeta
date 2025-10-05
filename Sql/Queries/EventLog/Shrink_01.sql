Select 'Shrink EventLog' as Operation
DBCC SHRINKDATABASE (EventLog, 10);
GO
Select 'Shrink EventLog1' as Operation
DBCC SHRINKDATABASE (EventLog1, 10);
GO
Select 'Shrink EvlContext' as Operation
DBCC SHRINKDATABASE (EvlContext, 10);
GO
