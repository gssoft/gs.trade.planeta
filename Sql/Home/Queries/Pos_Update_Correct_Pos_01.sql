use dbtrade2
--Update dbo.Positions 
--Set Operation = -1
--where [Key] IN
(
select p.[Key] , p.Operation,  s.sCode, s.tCode, s.aCode
from Positions p
inner join 
(
select id, sCode, tCode, aCode from dbo.fStrategy()
where 
--tCode = 'SRM4' 
--AND aCode='spbfut00125'
Right(sCode,1) = 'S'
) s on p.StrategyId = s.id
where p.Discriminator = 'Position'
AND p.Quantity * p.Operation > 0
)

go

--Update dbo.Positions 
--Set Operation = 1
--where [Key] IN
(
select p.[Key] , p.Operation,  s.sCode, s.tCode, s.aCode
from Positions p
inner join 
(
select id, sCode, tCode, aCode from dbo.fStrategy()
where 
--tCode = 'SRM4' 
--AND aCode='spbfut00125'
Right(sCode,1) = 'L'
) s on p.StrategyId = s.id
where p.Discriminator = 'Position'
AND p.Quantity * p.Operation < 0
)

go