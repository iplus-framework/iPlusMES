ALTER TABLE [dbo].[Picking]
ADD 
    [ScheduledOrder] int NULL,
	[ScheduledStartDate] datetime NULL,
	[ScheduledEndDate] datetime NULL,
	[CalculatedStartDate] datetime NULL,
	[CalculatedEndDate] datetime NULL,
	[VBiACClassWFID] uniqueidentifier NULL
GO

ALTER TABLE [dbo].[Picking]  WITH CHECK ADD  CONSTRAINT [FK_Picking_ACClassWFID] FOREIGN KEY([VBiACClassWFID])
REFERENCES [dbo].[ACClassWF] ([ACClassWFID])
GO

ALTER TABLE [dbo].[Picking] CHECK CONSTRAINT [FK_Picking_ACClassWFID];