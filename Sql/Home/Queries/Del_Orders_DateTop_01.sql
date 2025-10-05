use [dbtrade2]
Delete from Orders
WHERE CAST(Created as Date) IN
(
select top 1 CAST(o.Created as Date) dt -- , Count(Created) cnt
from Orders o
group by CAST(o.Created as Date)
order by CAST(o.Created as Date)
)

go
