IF NOT EXISTS (SELECT * FROM MDZeroStockState WHERE MDKey = 'ResetIfNotAvailableFacility')
	INSERT INTO MDZeroStockState ([MDZeroStockStateID]
           ,[MDZeroStockStateIndex]
           ,[MDNameTrans]
           ,[SortIndex]
           ,[XMLConfig]
           ,[IsDefault]
           ,[InsertName]
           ,[InsertDate]
           ,[UpdateName]
           ,[UpdateDate]
           ,[MDKey]) VALUES (NEWID(), 4, 'en{''Reset If Not Available on facility''}de{''Zur�cksetzen, wenn nicht verf�gbar auf Anlage''}', 4, NULL, 0, 'SUP', SYSDATETIME(), 'SUP', SYSDATETIME(), 'ResetIfNotAvailableFacility')

GO

IF NOT EXISTS (SELECT * FROM MDZeroStockState WHERE MDKey = 'RestoreQuantityIfNotAvailable')
	INSERT INTO MDZeroStockState ([MDZeroStockStateID]
           ,[MDZeroStockStateIndex]
           ,[MDNameTrans]
           ,[SortIndex]
           ,[XMLConfig]
           ,[IsDefault]
           ,[InsertName]
           ,[InsertDate]
           ,[UpdateName]
           ,[UpdateDate]
           ,[MDKey]) VALUES (NEWID(), 5, 'en{''Restore quantity if not available''}de{''Menge wiederherstellen, wenn nicht verf�gbar''}', 5, NULL, 0, 'SUP', SYSDATETIME(), 'SUP', SYSDATETIME(), 'RestoreQuantityIfNotAvailable')