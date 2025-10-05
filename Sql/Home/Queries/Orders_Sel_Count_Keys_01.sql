Use [DbTrade2]
Select Count(a.[Key])
From
(
Select o.[Key]
From Orders o
) a;
GO
Select Count(b.[Key])
From
(
Select o.[Key]
From Orders o
Group by o.[Key]
) b;
GO