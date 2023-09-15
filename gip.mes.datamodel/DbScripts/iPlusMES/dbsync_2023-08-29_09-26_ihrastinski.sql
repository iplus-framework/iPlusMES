alter table MaintOrder
add VBiPAACClassID uniqueidentifier null
GO

ALTER TABLE dbo.MaintOrder ADD CONSTRAINT
	FK_MaintOrder_PAACClassID FOREIGN KEY
	(
	VBiPAACClassID
	) REFERENCES dbo.ACClass
	(
	ACClassID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

alter table MaintOrderTask
add TaskName varchar(50) null