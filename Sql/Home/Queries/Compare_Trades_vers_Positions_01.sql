use DbTrade2
Select s.Id, sCode, tCode, tr.s, (p.Operation*p.Quantity) as Pos, cnt as TrCnt
from fStrategy() s
Inner join
(
select t.StrategyId, Sum(t.Operation*t.Quantity) as s, Count(*) cnt
from Trades t
Group by t.StrategyId
) tr 
on s.Id = tr.StrategyId
inner join Positions p on p.StrategyId = s.Id
Where p.Discriminator = 'Position'
Order by sCode, tCode
go
