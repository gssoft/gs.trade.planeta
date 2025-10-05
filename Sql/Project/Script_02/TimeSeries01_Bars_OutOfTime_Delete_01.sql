USE TimeSeries01
Declare @dt1 as DateTime, @dt2 as DateTime

SET @dt1 = GETDATE()
Select @dt1 as 'Start DateTIme'
delete from Bars
where id In
(
Select b.Id
FROM dbo.Bars b
INNER JOIN TimeSeries ts on b.BarSeriesId = ts.Id
INNER JOIN Tickers t on ts.TickerId = t.Id
inner join TradeBoards tb on t.TradeBoardId = tb.Id
Where (tb.Id = 2 Or tb.Id = 3) AND 
(cast(Dt as Time) < '10:00:00' OR cast(DT as Time) > '23:50:00')
)

SET @dt2  = GETDATE()
Select @dt2 as 'Finish DateTIme'
Select cast((@dt2 - @dt1) as Time) as 'ElapsedTime'
go
