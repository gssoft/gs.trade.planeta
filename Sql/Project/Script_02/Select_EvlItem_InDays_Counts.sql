use eventlog
select Cast( DT as Date) dt, Count(*) as EvliCount
from EventLogItems o
Group by Cast( DT as Date)
Order by Cast( DT as Date) DESC
GO
Select 'EventLog After Delete' as Operation, Count(*) as Cnt, 
    Min(Cast( DT as Date)) as Dt1, Max(Cast( DT as Date)) as Dt2
From EventLogItems
Go