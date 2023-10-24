alter table [dbo].[FacilityReservation] add PickingPosID uniqueidentifier null
alter table [dbo].[FacilityReservation] add ReservedQuantityUOM float null
GO
ALTER TABLE [dbo].[FacilityReservation] ADD  CONSTRAINT [FK_FacilityReservation_PickingPos] FOREIGN KEY([PickingPosID]) REFERENCES [dbo].[PickingPos] ([PickingPosID])