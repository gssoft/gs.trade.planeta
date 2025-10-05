USE [dbtrade2]

Update Positions
SET LastTradeNumber = 0
WHERE Discriminator = 'Position'

