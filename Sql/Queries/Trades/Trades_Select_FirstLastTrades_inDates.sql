use dbtrade2
SELECT ID, Code FROM Strategies
WHERE Code Like '%P3.L.PE%'  
OR Code Like '%P3.S.PE%'  
Go

SELECT ID,Number,Price, StrategyId, Created, Operation, Quantity FROM Trades
WHERE StrategyId IN(6192,6193)
ORDER BY Created Asc
GO

SELECT StrategyId, Cast(Created As Date) As [Date],
Min(Created) as MinDAte, Max(Created) as MaxDAte, 
Count(*) As Cnt
FROM Trades
WHERE StrategyId IN(6192,6193)
GROUP BY StrategyId, Cast(Created As Date)
ORDER BY MinDate
GO
