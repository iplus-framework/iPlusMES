ALTER TABLE [dbo].[OperationLog]  WITH CHECK ADD  CONSTRAINT [FK_OperationLog_OperationLogRefACClassID] FOREIGN KEY([RefACClassID])
REFERENCES [dbo].[ACClass] ([ACClassID])
GO

ALTER TABLE [dbo].[OperationLog] CHECK CONSTRAINT [FK_OperationLog_OperationLogRefACClassID]
GO
