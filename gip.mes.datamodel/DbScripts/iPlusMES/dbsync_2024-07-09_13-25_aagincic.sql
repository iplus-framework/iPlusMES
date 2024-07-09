ALTER TABLE [dbo].[Facility]
ADD 
	[VBiACClassMethodID] uniqueidentifier NULL
GO

ALTER TABLE [dbo].[Facility]  WITH CHECK ADD  CONSTRAINT [FK_Facility_ACClassMethodID] FOREIGN KEY([VBiACClassMethodID]) REFERENCES [dbo].[ACClassMethod] ([ACClassMethodID])