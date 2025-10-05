use dbtrade2
Select 'DbTrade2 SP_UpdateStats' as Operation
exec sp_updatestats
go

use EventLog
Select 'EventLog' as Operation
exec sp_updatestats
go

use EventLogFinReal
Select 'EventLogFinReal' as Operation
exec sp_updatestats
go

use EventLogFinTr
Select 'EventLogFinTr' as Operation
exec sp_updatestats
go

use EventLogOpTr
Select 'EventLogOpTr' as Operation
exec sp_updatestats
go
