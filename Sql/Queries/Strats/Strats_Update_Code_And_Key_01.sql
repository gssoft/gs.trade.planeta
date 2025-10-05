USE DbTrade2

--SELECT Code, SUBSTRING(Code, 0, 9) + SUBSTRING(Code, 21,2) + REPLACE((SUBSTRING(Code, 9, LEN(Code)-8)), '.2.', '.')

SELECT Id, Code, SUBSTRING(code, 0, 9) + SUBSTRING(code, 21,2) + SUBSTRING(Code,9,12) + SUBSTRING(Code, 23,25) [Update]
,LEN(Code) [LenOfCode]
,LEN(SUBSTRING(code, 0, 9) + SUBSTRING(code, 21,2) + SUBSTRING(Code,9,12) +SUBSTRING(Code, 23,25)) [LenOFUpdate]
FROM Strategies s
WHERE [Key] Like '%736S.PN1.%'

UPDATE STRATEGIES
SET CODE = a.Updated
FROM
(
SELECT Id,Code,
 SUBSTRING(code, 0, 9) + SUBSTRING(code, 21,2) + SUBSTRING(Code,9,12) + SUBSTRING(Code, 23,25) [Updated]
,LEN(Code) [LenOfCode]
,LEN(SUBSTRING(code, 0, 9) + SUBSTRING(code, 21,2) + SUBSTRING(Code,9,12) +SUBSTRING(Code, 23,25)) [LenOFUpdate]
FROM Strategies s
WHERE Code Like '%736S.PN1.%'
) a
where Strategies.Id = a.Id

SELECT Id, Code, [KEY] , STUFF([KEY], 53, LEN(Code), Code) [KeyNew]
FROM Strategies s
WHERE [KEY] Like '%736S.PN1.%'

UPDATE STRATEGIES
SET [Key] = a.KeyNew
FROM
(
SELECT Id, Code, [KEY] , STUFF([KEY], 53, LEN(Code), Code) [KeyNew] 
FROM Strategies s
WHERE [KEY] Like '%736S.PN1.%'
) a
where Strategies.Id = a.Id

SELECT Code, [Key] , CodeInKey [CodeInKey], SOUNDEX(Code) SND_CODE, SOUNDEX(CodeInKey) SND_KEY, DIFFERENCE(Code,CodeInKEy) DIFFS
FROM
(
select Code, [Key], SUBSTRING([Key],CHARINDEX('Z00736S.', [Key]), LEN(Code)) [CodeInKey]
FROM Strategies
WHERE [KEY] Like '%736S.2.PN1.%' OR [Key] Like '%736S.1.PN1.%' 
) a




