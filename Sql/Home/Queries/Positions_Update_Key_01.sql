use dbtrade2
Update Positions
SET [Key] = s.[Key]
from
(
Select Id, [Key]
from Strategies
) s
WHERE Positions.StrategyId = s.Id
go
Select p.StrategyId, p.[Key]
from Positions p
go
