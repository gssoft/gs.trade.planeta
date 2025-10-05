USE DBtrade2

UPDATE Positions
SET [Key] = StratKey
FROM
(
SELECT p.Id [PosId], s.[Key] [StratKey], p.[Key] [PosKey]
FROM Strategies s
INNER Join POSITIONS p ON s.Id = p.StrategyId
WHERE s.[Key] <> p.[Key]
) a
WHERE a.PosId = Id
