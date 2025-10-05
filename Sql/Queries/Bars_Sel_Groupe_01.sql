use timeseries03

select bs.* from TimeSeries bs
go
Select Count(*) Cnt from Bars
Go

select bs.Id, [Key],TimeInterval, TimeShift, FirstDT, LastDT, a.Cnt 
from TimeSeries bs
inner join 
(
Select BarSeriesId, Min(DT) FirstDT, Max(DT) LastDT, Count(*) Cnt from Bars
Group by BarSeriesId
) a
On bs.Id = a.BarSeriesID
