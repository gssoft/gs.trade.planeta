-- EventLogs --
use EventLog
Select 'EventLog Delete' as Operation

select count(*) EvlItemCount
from eventlogitems
go

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

select count(*) EvlItemCount
from eventlogitems
go

use EventLogFinReal
Select 'EventLogFinReal Delete' as Operation

select count(*) EvlItemCount
from eventlogitems
go

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

select count(*) EvlItemCount
from eventlogitems
go

use EventLogFinTr
Select 'EventLogFinTr Delete' as Operation

select count(*) EvlItemCount
from eventlogitems
go

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

select count(*) EvlItemCount
from eventlogitems
go


use EventLogOpTr
Select 'EventLogOpTr Delete' as Operation

select count(*) EvlItemCount
from eventlogitems
go

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

select count(*) EvlItemCount
from eventlogitems
go

use EventLogVtbSim
Select 'EventLogVtbSim Delete' as Operation

select count(*) EvlItemCount
from eventlogitems
go

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

select count(*) EvlItemCount
from eventlogitems
go

