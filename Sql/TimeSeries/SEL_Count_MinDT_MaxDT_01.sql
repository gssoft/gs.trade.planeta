USE TimeSeries01
SELECT ts.Code [TICKER], tbs.Code [BOARD],  Count(*) [COUNT], Min(bs.DT) [DT1], Max(bs.DT) [DT2]
FROM Bars bs
INNER JOIN TimeSeries tms ON bs.BarSeriesId = tms.Id
INNER JOIN Tickers ts ON tms.TickerId = ts.Id
INNER JOIN TradeBoards tbs ON ts.TradeBoardId = tbs.Id
GROUP BY ts.Code, tbs.Code
ORDER BY ts.Code
GO



