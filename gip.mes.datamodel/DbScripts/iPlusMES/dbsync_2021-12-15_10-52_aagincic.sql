alter table dbo.Picking add DeliveryCompanyAddressID uniqueidentifier NULL;
GO
ALTER TABLE [dbo].[Picking]  
WITH CHECK ADD  CONSTRAINT [FK_Picking_DeliveryCompanyAddressID] FOREIGN KEY([DeliveryCompanyAddressID])
REFERENCES [dbo].[CompanyAddress] ([CompanyAddressID])
