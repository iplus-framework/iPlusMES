ALTER TABLE dbo.OutOrderPos ADD
	GroupOutOrderPosID uniqueidentifier NULL
GO
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT
	FK_OutOrderPos_GroupOutOrderPosID FOREIGN KEY
	(
	GroupOutOrderPosID
	) REFERENCES dbo.OutOrderPos
	(
	OutOrderPosID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 