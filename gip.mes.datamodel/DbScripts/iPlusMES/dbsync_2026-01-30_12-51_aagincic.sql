alter table [dbo].[Invoice] add [ReferenceInvoiceID] uniqueidentifier null 
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_ReferenceInvoiceID] FOREIGN KEY([ReferenceInvoiceID])
REFERENCES [dbo].[Invoice] ([InvoiceID])