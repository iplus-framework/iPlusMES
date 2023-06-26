drop table MaintTask
GO

drop table MaintOrderProperty
GO

drop table MaintOrder
GO

drop table MaintACClassVBGroup
GO

delete from MaintACClassProperty;
GO
delete from MaintACClass
GO


ALTER TABLE dbo.MaintACClass
	DROP CONSTRAINT FK_MaintACClass_MDMaintModeID
GO

ALTER TABLE dbo.MaintACClass
	DROP COLUMN MDMaintModeID, IsActive, MaintInterval, LastMaintTerm, IsWarningActive, WarningDiff
GO

CREATE TABLE dbo.MaintOrder
	(
	MaintOrderID uniqueidentifier NOT NULL,
	BasedOnMaintOrderID uniqueidentifier NULL,
	MaintOrderNo varchar(20) NOT NULL,
	MDMaintOrderStateID uniqueidentifier NULL,
	MaintACClassID uniqueidentifier NULL,
	FacilityID uniqueidentifier NULL,
	PickingID uniqueidentifier NULL,
	MaintModeIndex int NOT NULL,
	IsActive bit NOT NULL,
	MaintInterval int NULL,
	LastMaintTerm datetime NULL,
	WarningDiff int NULL,
	PlannedStartDate datetime NULL,
	PlannedDuration int NULL,
	StartDate datetime NULL,
	EndDate datetime NULL,
	Comment varchar(MAX) NULL,
	XMLConfig text NULL,
	InsertName varchar(20) NOT NULL,
	InsertDate datetime NOT NULL,
	UpdateName varchar(20) NOT NULL,
	UpdateDate datetime NOT NULL
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.MaintOrderProperty
	(
	MaintOrderPropertyID uniqueidentifier NOT NULL,
	MaintOrderID uniqueidentifier NOT NULL,
	MaintACClassPropertyID uniqueidentifier NOT NULL,
	SetValue varchar(max) NOT NULL,
	ActValue varchar(max) NOT NULL,
	InsertName varchar(20) NOT NULL,
	InsertDate datetime NOT NULL,
	UpdateName varchar(20) NOT NULL,
	UpdateDate datetime NOT NULL
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.MaintOrderTask
	(
	MaintOrderTaskID uniqueidentifier NOT NULL,
	MaintOrderID uniqueidentifier NOT NULL,
	MDMaintTaskStateID uniqueidentifier NOT NULL,
	TaskDescription varchar(max) NULL,
	Comment varchar(MAX) NULL,
	PlannedStartDate datetime NULL,
	PlannedDuration int NULL,
	StartDate datetime NULL,
	EndDate datetime NULL,
	InsertName varchar(20) NOT NULL,
	InsertDate datetime NOT NULL,
	UpdateName varchar(20) NOT NULL,
	UpdateDate datetime NOT NULL
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.MaintOrderPos
	(
	MaintOrderPosID uniqueidentifier NOT NULL,
	MaintOrderID uniqueidentifier NOT NULL,
	ParentMaintOrderPosID uniqueidentifier NOT NULL,
	MaterialID uniqueidentifier NOT NULL,
	Quantity float NOT NULL,
	InsertName varchar(20) NOT NULL,
	InsertDate datetime NOT NULL,
	UpdateName varchar(20) NOT NULL,
	UpdateDate datetime NOT NULL
	)  ON [PRIMARY]
GO

CREATE TABLE dbo.MaintOrderAssignment
	(
	MaintOrderAssignmentID uniqueidentifier NOT NULL,
	MaintOrderID uniqueidentifier NOT NULL,
	VBUserID uniqueidentifier NULL,
	VBGroupID uniqueidentifier NULL,
	CompanyID uniqueidentifier NULL,
	IsDefault bit NOT NULL,
	IsActive bit NOT NULL,
	Comment varchar(max) NULL,
	InsertName varchar(20) NOT NULL,
	InsertDate datetime NOT NULL,
	UpdateName varchar(20) NOT NULL,
	UpdateDate datetime NOT NULL
	)  ON [PRIMARY]
GO

ALTER TABLE dbo.MaintOrder ADD CONSTRAINT
	PK_MaintOrder PRIMARY KEY CLUSTERED 
	(
	MaintOrderID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.MaintOrder ADD CONSTRAINT
	FK_MaintOrder_BasedOnMaintOrderID FOREIGN KEY
	(
	BasedOnMaintOrderID
	) REFERENCES dbo.MaintOrder
	(
	MaintOrderID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.MaintOrder ADD CONSTRAINT
	FK_MaintOrder_MDMaintOrderStateID FOREIGN KEY
	(
	MDMaintOrderStateID
	) REFERENCES dbo.MDMaintOrderState
	(
	MDMaintOrderStateID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.MaintOrder ADD CONSTRAINT
	FK_MaintOrder_MaintACClassID FOREIGN KEY
	(
	MaintACClassID
	) REFERENCES dbo.MaintACClass
	(
	MaintACClassID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.MaintOrder ADD CONSTRAINT
	FK_MaintOrder_FacilityID FOREIGN KEY
	(
	FacilityID
	) REFERENCES dbo.Facility
	(
	FacilityID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.MaintOrder ADD CONSTRAINT
	FK_MaintOrder_PickingID FOREIGN KEY
	(
	PickingID
	) REFERENCES dbo.Picking
	(
	PickingID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.MaintOrderProperty ADD CONSTRAINT
	PK_MaintOrderProperty PRIMARY KEY CLUSTERED 
	(
	MaintOrderPropertyID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.MaintOrderProperty ADD CONSTRAINT
	FK_MaintOrderProperty_MaintOrderID FOREIGN KEY
	(
	MaintOrderID
	) REFERENCES dbo.MaintOrder
	(
	MaintOrderID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.MaintOrderProperty ADD CONSTRAINT
	FK_MaintOrderProperty_MaintACClassPropertyID FOREIGN KEY
	(
	MaintACClassPropertyID
	) REFERENCES dbo.MaintACClassProperty
	(
	MaintACClassPropertyID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.MaintOrderPos ADD CONSTRAINT
	PK_MaintOrderPos PRIMARY KEY CLUSTERED 
	(
	MaintOrderPosID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.MaintOrderPos ADD CONSTRAINT
	FK_MaintOrderPos_MaintOrderID FOREIGN KEY
	(
	MaintOrderID
	) REFERENCES dbo.MaintOrder
	(
	MaintOrderID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.MaintOrderPos ADD CONSTRAINT
	FK_MaintOrderPos_MaintOrderPos FOREIGN KEY
	(
	MaintOrderPosID
	) REFERENCES dbo.MaintOrderPos
	(
	MaintOrderPosID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.MaintOrderPos ADD CONSTRAINT
	FK_MaintOrderPos_ParentMaintOrderPosID FOREIGN KEY
	(
	MaterialID
	) REFERENCES dbo.Material
	(
	MaterialID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.MaintOrderTask ADD CONSTRAINT
	PK_MaintOrderTask PRIMARY KEY CLUSTERED 
	(
	MaintOrderTaskID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.MaintOrderTask ADD CONSTRAINT
	FK_MaintOrderTask_MaintOrderID FOREIGN KEY
	(
	MaintOrderID
	) REFERENCES dbo.MaintOrder
	(
	MaintOrderID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.MaintOrderTask ADD CONSTRAINT
	FK_MaintOrderTask_MDMaintTaskStateID FOREIGN KEY
	(
	MDMaintTaskStateID
	) REFERENCES dbo.MDMaintTaskState
	(
	MDMaintTaskStateID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.MaintOrderAssignment ADD CONSTRAINT
	PK_MaintOrderAssignment PRIMARY KEY CLUSTERED 
	(
	MaintOrderAssignmentID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.MaintOrderAssignment ADD CONSTRAINT
	FK_MaintOrderAssignment_MaintOrder FOREIGN KEY
	(
	MaintOrderID
	) REFERENCES dbo.MaintOrder
	(
	MaintOrderID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.MaintOrderAssignment ADD CONSTRAINT
	FK_MaintOrderAssignment_VBUserID FOREIGN KEY
	(
	VBUserID
	) REFERENCES dbo.VBUser
	(
	VBUserID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.MaintOrderAssignment ADD CONSTRAINT
	FK_MaintOrderAssignment_Company FOREIGN KEY
	(
	CompanyID
	) REFERENCES dbo.Company
	(
	CompanyID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.MaintOrderAssignment ADD CONSTRAINT
	FK_MaintOrderAssignment_VBGroup FOREIGN KEY
	(
	VBGroupID
	) REFERENCES dbo.VBGroup
	(
	VBGroupID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 