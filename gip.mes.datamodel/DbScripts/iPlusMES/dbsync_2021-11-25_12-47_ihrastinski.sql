alter table Facility add DisabledForMobile bit null;
GO
update Facility set DisabledForMobile = 0;
GO
alter table Facility alter column DisabledForMobile bit not null;
GO
INSERT INTO [dbo].[MDPickingType] VALUES (NEWID(), 15, 'en{''Internal relocation''}de{''Interne Umlagerung''}', 7, NULL, 0, 'InternalRelocation','00', GETDATE(), '00', GETDATE())
INSERT INTO [dbo].[MDPickingType] VALUES (NEWID(), 0, 'en{''Unplanned receipt''}de{''Ungeplanter Eingang''}', 8, NULL, 0, 'Receipt','00', GETDATE(), '00', GETDATE())
INSERT INTO [dbo].[MDPickingType] VALUES (NEWID(), 0, 'en{''Return receipt''}de{''Rückschein''}', 9, NULL, 0, 'Receipt','00', GETDATE(), '00', GETDATE())
INSERT INTO [dbo].[MDPickingType] VALUES (NEWID(), 1, 'en{''Retail''}de{''Einzelhandel''}', 10, NULL, 0, 'Issue','00', GETDATE(), '00', GETDATE())
INSERT INTO [dbo].[MDPickingType] VALUES (NEWID(), 1, 'en{''Internal issue''}de{''Interne Ausgang''}', 11, NULL, 0, 'Issue','00', GETDATE(), '00', GETDATE())