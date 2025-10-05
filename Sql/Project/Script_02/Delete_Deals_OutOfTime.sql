use DbTrade2
Delete from deals
Where ID In
(
Select Id
from Deals
where cast(LastTradeDT as time) > '00:00:00' AND cast(LastTradeDT as time) <= '10:00:00'
)
