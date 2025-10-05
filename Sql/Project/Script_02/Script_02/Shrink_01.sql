Select 'Shrink EventLog' as Operation
DBCC SHRINKDATABASE (EventLog, 10);
GO
Select 'Shrink EvlContext' as Operation
DBCC SHRINKDATABASE (EvlContext, 10);
GO
Select 'Shrink DbTrade2' as Operation
DBCC SHRINKDATABASE (DbTrade2, 10);
GO