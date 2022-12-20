IF (SELECT COUNT(*) FROM sys.indexes indexes INNER JOIN sys.objects 
objects ON indexes.object_id = objects.object_id WHERE indexes.name 
='NCI_FK_ProdOrderPartslistPos_ACClassTaskID' AND objects.name = 'ProdOrderPartslistPos') = 0
	BEGIN
	CREATE NONCLUSTERED INDEX [NCI_FK_ProdOrderPartslistPos_ACClassTaskID] ON [dbo].[ProdOrderPartslistPos]
	(
		[ACClassTaskID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END