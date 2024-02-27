alter table [dbo].[LabOrder] add PickingPosID uniqueidentifier null
GO

ALTER TABLE [dbo].[LabOrder]  WITH CHECK ADD  CONSTRAINT [FK_LabOrder_PickingPosID] FOREIGN KEY([PickingPosID])
REFERENCES [dbo].[PickingPos] ([PickingPosID])
GO

ALTER TABLE [dbo].[LabOrder] CHECK CONSTRAINT [FK_LabOrder_PickingPosID]