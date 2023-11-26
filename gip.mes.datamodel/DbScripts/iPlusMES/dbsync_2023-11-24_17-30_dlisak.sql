alter table [dbo].[Weighing] add VisitorVoucherID uniqueidentifier null
GO

ALTER TABLE [dbo].[Weighing]  WITH CHECK ADD  CONSTRAINT [FK_Weighing_VisitorVoucherID] FOREIGN KEY([VisitorVoucherID])
REFERENCES [dbo].[VisitorVoucher] ([VisitorVoucherID])
GO

ALTER TABLE [dbo].[Weighing] CHECK CONSTRAINT [FK_Weighing_VisitorVoucherID]