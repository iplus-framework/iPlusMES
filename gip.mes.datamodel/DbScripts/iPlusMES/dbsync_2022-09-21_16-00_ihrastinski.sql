CREATE TABLE dbo.OperationLog
	(
	OperationLogID uniqueidentifier NOT NULL,
	RefACClassID uniqueidentifier NULL,
	ACProgramLogID uniqueidentifier NULL,
	FacilityChargeID uniqueidentifier NULL,
	OperationTime datetime NOT NULL,
	Operation smallint NOT NULL,
	OperationState smallint NOT NULL,
	InsertName varchar(20) NOT NULL,
	InsertDate datetime NOT NULL,
	UpdateName varchar(20) NOT NULL,
	UpdateDate datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.OperationLog ADD CONSTRAINT
	PK_OperationLog PRIMARY KEY CLUSTERED 
	(
	OperationLogID
	)
GO
ALTER TABLE dbo.OperationLog ADD CONSTRAINT
	FK_OperationLog_ACProgramLog FOREIGN KEY
	(
	ACProgramLogID
	) REFERENCES dbo.ACProgramLog
	(
	ACProgramLogID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO
ALTER TABLE dbo.OperationLog ADD CONSTRAINT
	FK_OperationLog_FacilityCharge FOREIGN KEY
	(
	FacilityChargeID
	) REFERENCES dbo.FacilityCharge
	(
	FacilityChargeID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO

