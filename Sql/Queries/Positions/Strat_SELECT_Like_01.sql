use DbTrade2
-- /*
SELECT 
 Code,
 Replace(Code,'736S.PN1.S', '736S.PN1.M0.S') AS S,
 Replace(Code,'736S.PN1.L', '736S.PN1.M0.L') AS L,
 Replace([Key],'736S.PN1.S', '736S.PN1.M0.S') AS KeyS,
 Replace([Key],'736S.PN1.L', '736S.PN1.M0.L') AS KeyL

FROM Strategies
WHERE Code Like '%S.PN1.S%' OR Code LIKE '%S.PN1.L%'
go
-- */

UPDATE Strategies
SET 
	Code =  Replace(Code,'736S.PN1.M.L', '736S.PN1.M5.L'),	
	[Key] = Replace([Key],'736S.PN1.M.L', '736S.PN1.M5.L')

WHERE Code Like '%S.PN1.M.L%' 
GO

UPDATE Strategies
SET 
	Code =  Replace(Code,'736S.PN1.M.S', '736S.PN1.M5.S'),
	[Key] = Replace([Key],'736S.PN1.M.S', '736S.PN1.M5.S')
 
WHERE Code Like '%S.PN1.M.S%' 
GO

SELECT Code, [Key]
FROM Strategies
WHERE Code Like '%S.PN1.%' OR Code LIKE '%S.PN1.%'
GO





