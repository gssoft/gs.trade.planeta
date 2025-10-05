use dbtrade2
delete from strategies
where id in
(
--select id,code
select id
from Strategies
where code like '%.120.210.%'
)