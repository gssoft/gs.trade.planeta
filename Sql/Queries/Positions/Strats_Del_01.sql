USE DbTrade2
/*
SELECT s.Code, s.[Key], t.Code, s.TimeInt
FROM Strategies s
INNER JOIN Tickers t on s.TickerId = t.Id
WHERE s.Code = 'Z0073521.FEN.1.19.25.110.230.301.403.803' 
			AND TimeInt Between 5 AND 20
			AND t.Code Like'RI%'
GO
*/
DELETE FROM Strategies
WHERE ID IN 
(
SELECT s.Id
FROM Strategies s
INNER JOIN Tickers t on s.TickerId = t.Id
WHERE	(	s.Code = 'Z0073521.FEN.1.19.25.110.230.301.403.803'
		OR	s.Code = 'Z0073521.FEN.2.19.25.110.230.301.403.803'
		OR	s.Code = 'Z0073521.FEN.5.19.25.110.230.301.403.803'
		OR	s.Code = 'Z0073521.FEN.6.19.25.110.230.301.403.803') 
			AND TimeInt Between 5 AND 20
			-- AND t.Code Like'RI%'
			)