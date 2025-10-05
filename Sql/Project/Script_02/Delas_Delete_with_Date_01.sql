use dbtrade2
Delete from deals
where Id In
(
select d.Id
-- d.*
from deals as d
inner join strategies s on d.StrategyId = s.Id
inner join Accounts a on s.AccountId = a.Id
where a.Id = 6 AND
Cast(d.DT as Date) = '2015-11-13'
)
go

