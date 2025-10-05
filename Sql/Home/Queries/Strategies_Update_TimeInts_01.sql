use dbtrade2
update Strategies
Set Code = cd, [Key] = ky
from
( 
select s.Id, s.Code, replace(s.Code, '.' + RTrim(LTrim(str(timeInt))) + '.','.') cd, 
s.[Key], replace(s.[Key], '.' + RTrim(LTrim(str(timeInt))) + '.','.') ky,
RTrim(LTrim(str(timeInt))) tm
from Strategies s
) a
where Strategies.Id = a.Id
go
