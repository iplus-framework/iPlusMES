ALTER TABLE dbo.ProdOrderBatchPlan ADD
	OutOrderPosID uniqueidentifier NULL
GO
ALTER TABLE dbo.ProdOrderBatchPlan ADD CONSTRAINT
	FK_ProdOrderBatchPlan_OutOrderPos FOREIGN KEY
	(
	OutOrderPosID
	) REFERENCES dbo.OutOrderPos
	(
	OutOrderPosID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 