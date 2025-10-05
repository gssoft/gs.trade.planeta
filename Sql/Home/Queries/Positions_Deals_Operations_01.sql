use dbtrade2
select d.StrategyId, s.Code, t.Code, sum((d.Price2 - d.Price1)*d.Quantity*d.Operation), sum(d.Quantity)
from Deals d
inner join Strategies s on s.Id = d.StrategyId
inner join Tickers t on t.Id = s.TickerId
group by d.StrategyId, s.Code, t.Code
order by d.StrategyId
go
select  t.Code, sum((d.Price2 - d.Price1)*d.Quantity*d.Operation), sum(d.Quantity)
from Deals d
inner join Strategies s on s.Id = d.StrategyId
inner join Tickers t on t.Id = s.TickerId
group by  t.Code
go
select d.StrategyId as StratId,  sum((d.Price2 - d.Price1)*d.Quantity*d.Operation) as PnL, sum(d.Quantity) as q
from Deals d
inner join Strategies s on s.Id = d.StrategyId
inner join Tickers t on t.Id = s.TickerId
group by d.StrategyId
order by d.StrategyId
go


Select p.StrategyId, a.PnL, a.q, p.Discriminator
--update Positions
--Set PnL = a.PnL, Quantity = a.q 
from Positions p
inner join 
(select d.StrategyId as StratId,  sum((d.Price2 - d.Price1)*d.Quantity*d.Operation) as PnL, sum(d.Quantity) as q
from Deals d
group by d.StrategyId
) a on p.StrategyId = a.StratId
--where p.Discriminator = 'total'
go
/*
Delete
from Positions
where StrategyId NOT IN
(
select d.StrategyId from Deals d
group by d.StrategyId
)
*/
update Positions
SET Quantity = 0, PnL = 0
where Discriminator = 'Position'
go

Select p.StrategyId, p.[status], p.operation, p.Quantity, p.PnL, p.Discriminator
from Positions p
order by p.StrategyId


