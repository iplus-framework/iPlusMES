CREATE TABLE dbo.MDPickingType
	(
	MDPickingTypeID uniqueidentifier NOT NULL,
	MDPickingTypeIndex smallint NOT NULL,
	MDNameTrans varchar(MAX) NOT NULL,
	SortIndex smallint NOT NULL,
	XMLConfig text NULL,
	IsDefault bit NOT NULL,
	MDKey varchar(40) NOT NULL,
	InsertName varchar(20) NOT NULL,
	InsertDate datetime NOT NULL,
	UpdateName varchar(20) NOT NULL,
	UpdateDate datetime NOT NULL
	)  
GO

ALTER TABLE dbo.MDPickingType ADD CONSTRAINT
	PK_MDPickingType PRIMARY KEY CLUSTERED 
	(
	MDPickingTypeID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

INSERT INTO [dbo].[MDPickingType] VALUES (NEWID(), 0, 'en{''Receipt''}de{''Eingang''}', 1, NULL, 0, 'Receipt', '00', GETDATE(), '00', GETDATE())
INSERT INTO [dbo].[MDPickingType] VALUES (NEWID(), 1, 'en{''Issue''}de{''Ausgang''}', 2, NULL, 0, 'Issue','00', GETDATE(), '00', GETDATE())
INSERT INTO [dbo].[MDPickingType] VALUES (NEWID(), 2, 'en{''Production''}de{''Produktion''}', 3, NULL, 0, 'Production','00', GETDATE(), '00', GETDATE())
INSERT INTO [dbo].[MDPickingType] VALUES (NEWID(), 3, 'en{''Receipt with vehicle''}de{''Eingang mit Fahrzeug''}', 4, NULL, 0, 'ReceiptVehicle','00', GETDATE(), '00', GETDATE())
INSERT INTO [dbo].[MDPickingType] VALUES (NEWID(), 4, 'en{''Issue with vehicle''}de{''Ausgang mit Fahrzeug''}', 5, NULL, 0, 'IssueVehicle','00', GETDATE(), '00', GETDATE())
INSERT INTO [dbo].[MDPickingType] VALUES (NEWID(), 10, 'en{''Automatic relocation''}de{''Automatische Umlagerung''}', 6, NULL, 1, 'AutomaticRelocation','00', GETDATE(), '00', GETDATE())
GO

ALTER TABLE dbo.Picking ADD
	MDPickingTypeID uniqueidentifier NULL
GO
ALTER TABLE dbo.Picking ADD CONSTRAINT
	FK_Picking_MDPickingType FOREIGN KEY
	(
	MDPickingTypeID
	) REFERENCES dbo.MDPickingType
	(
	MDPickingTypeID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION
GO

UPDATE Picking SET MDPickingTypeID = t.MDPickingTypeID FROM Picking p INNER JOIN MDPickingType t ON p.PickingTypeIndex = t.MDPickingTypeIndex
GO

ALTER TABLE Picking ALTER COLUMN MDPickingTypeID uniqueidentifier NOT NULL
GO

ALTER TABLE Picking DROP COLUMN PickingTypeIndex
