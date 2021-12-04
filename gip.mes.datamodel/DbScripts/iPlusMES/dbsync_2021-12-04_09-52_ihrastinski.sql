update [dbo].[MDPickingType] set MDPickingTypeIndex = 0, MDKey = 'UnplannedReceipt' where MDNameTrans = 'en{''Unplanned receipt''}de{''Ungeplanter Eingang''}'
update [dbo].[MDPickingType] set MDPickingTypeIndex = 0, MDKey = 'ReturnReceipt', MDNameTrans = 'en{''Return receipt''}de{''Rückschein''}' where MDNameTrans = 'en{''Return receipt''}de{''R?ckschein''}' or MDNameTrans = 'en{''Return receipt''}de{''Rückschein''}'
update [dbo].[MDPickingType] set MDPickingTypeIndex = 1, MDKey = 'Retail' where MDNameTrans = 'en{''Retail''}de{''Einzelhandel''}'
update [dbo].[MDPickingType] set MDPickingTypeIndex = 1, MDKey = 'InternalIssue' where MDNameTrans = 'en{''Internal issue''}de{''Interne Ausgang''}'
GO
INSERT INTO [dbo].[MDPickingType] VALUES (NEWID(), 1, 'en{''Return issue''}de{''Rückgabe''}', 112, NULL, 0, 'ReturnIssue','00', GETDATE(), '00', GETDATE())