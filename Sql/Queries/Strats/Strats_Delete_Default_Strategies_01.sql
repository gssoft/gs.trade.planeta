use dbtrade2
SELECT s.Name, t.NAme, t.Code FROM STRATEGIES s
INNER JOIN Tickers t on t.ID =  s.TickerID
WHERE s.NAme = 'Default'
GO

DELETE FROM STRATEGIES
WHERE ID IN
(
SELECT s.ID FROM STRATEGIES s
WHERE s.NAme = 'Default'
)
GO

SELECT 'Strategies After DELETE'
SELECT s.Name, t.NAme, t.Code FROM STRATEGIES s
INNER JOIN Tickers t on t.ID =  s.TickerID
WHERE s.NAme = 'Default'
GO

