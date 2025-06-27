alter table [dbo].[PlanningMRCons]  add [DefaultPartslistID] uniqueidentifier null
GO
ALTER TABLE [dbo].[PlanningMRCons]  WITH CHECK 
ADD  CONSTRAINT [FK_PlanningMRCons_PartslistID] 
FOREIGN KEY([DefaultPartslistID]) 
REFERENCES [dbo].[Partslist] ([PartslistID]) ON DELETE SET NULL
