Use DbTrade2

SELECT 'Strategies To Delete'

SELECT * FROM STRATEGIES
WHERE		Code Like 'Z0073521.%'
		AND (TimeInt = 5 OR TImeInt = 10)
SELECT 'Deals Before DELETE', Count(*) FROM Deals

go

Delete FROM STRATEGIES
WHERE		Code Like 'Z0073521.%'
		AND (TimeInt = 5 OR TImeInt = 10)
GO


/*
SELECT Count(*) FROM Deals
WHERE StrategyId IN
(SELECT StrategyId FROM STRATEGIES
WHERE		Code Like 'Z0073521.%'
		AND (TimeInt = 5 OR TimeInt = 10)
)
*/

SELECT 'Strategies  After Delete'

SELECT * FROM STRATEGIES
WHERE		Code Like 'Z0073521.%'
		AND (TimeInt = 5 OR TImeInt = 10)

SELECT 'Deals After DELETE', Count(*) FROM Deals
go
