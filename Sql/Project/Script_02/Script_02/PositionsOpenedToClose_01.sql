use dbtrade2
Update dbo.Positions 
Set Operation = 0, [Status] = 0, Quantity = 0
From Positions p
WHERE p.[Status] <> 0 
