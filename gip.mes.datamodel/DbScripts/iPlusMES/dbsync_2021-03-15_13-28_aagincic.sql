drop table [dbo].[InvoicePos]
GO
EXEC sp_rename 'dbo.MDInvoiceState.MDInvoiceStateIndex', 'InvoiceStateIndex', 'COLUMN';
EXEC sp_rename 'dbo.MDInvoiceType.MDInvoiceTypeIndex', 'InvoiceTypeIndex', 'COLUMN';
GO
CREATE TABLE [dbo].[InvoicePos](
	[InvoicePosID] [uniqueidentifier] NOT NULL,
	[InvoiceID] [uniqueidentifier] NOT NULL,
	[Sequence] [int] NOT NULL,
	[MaterialID] [uniqueidentifier] NOT NULL,
	[MDUnitID] [uniqueidentifier] NULL,
	TargetQuantityUOM float not null,
	TargetQuantity	float not null,
	[MDCountrySalesTaxMDMaterialGroupID] [uniqueidentifier] NULL,
	[MDCountrySalesTaxMaterialID] [uniqueidentifier] NULL,
	[PriceNet] [money] NOT NULL,
	[PriceGross] [money] NOT NULL,
	[SalexTax] [real] NULL,
	[OutOrderPosID] [uniqueidentifier] NULL,
	[Comment] [varchar](max) NULL,
	[XMLConfig] [text] NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[XMLDesign] [text] NULL,
 CONSTRAINT [PK_InvoicePos] PRIMARY KEY CLUSTERED 
(
	[InvoicePosID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[InvoicePos]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePos_Invoice] FOREIGN KEY([InvoiceID]) REFERENCES [dbo].[Invoice] ([InvoiceID])
ALTER TABLE [dbo].[InvoicePos]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePos_Material] FOREIGN KEY([MaterialID]) REFERENCES [dbo].[Material] ([MaterialID])
ALTER TABLE [dbo].[InvoicePos]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePos_MDUnit] FOREIGN KEY([MDUnitID]) REFERENCES [dbo].[MDUnit] ([MDUnitID])
ALTER TABLE [dbo].[InvoicePos]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePos_MDCountrySalesTaxMaterial] FOREIGN KEY([MDCountrySalesTaxMaterialID]) REFERENCES [dbo].[MDCountrySalesTaxMaterial] ([MDCountrySalesTaxMaterialID])
ALTER TABLE [dbo].[InvoicePos]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePos_MDCountrySalesTaxMDMaterialGroup] FOREIGN KEY([MDCountrySalesTaxMDMaterialGroupID]) REFERENCES [dbo].[MDCountrySalesTaxMDMaterialGroup] ([MDCountrySalesTaxMDMaterialGroupID])
ALTER TABLE [dbo].[InvoicePos]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePos_OutOrderPos] FOREIGN KEY([OutOrderPosID]) REFERENCES [dbo].[OutOrderPos] ([OutOrderPosID])
GO
INSERT [dbo].[MDInvoiceType] ([MDInvoiceTypeID], [InvoiceTypeIndex], [MDNameTrans], [SortIndex], [XMLConfig], [IsDefault], [UpdateDate], [UpdateName], [InsertDate], [InsertName], [MDKey]) VALUES (N'311e168b-11ce-4776-af14-d9efa9c7c296', 1, N'en{''Invoice''}de{''Rechnung''}', 0, N'', 1, CAST(N'2020-12-10T15:12:46.780' AS DateTime), N'dl', CAST(N'2009-05-08T09:19:30.287' AS DateTime), N'XXX', N'Invoice')
INSERT [dbo].[MDInvoiceType] ([MDInvoiceTypeID], [InvoiceTypeIndex], [MDNameTrans], [SortIndex], [XMLConfig], [IsDefault], [UpdateDate], [UpdateName], [InsertDate], [InsertName], [MDKey]) VALUES (N'cb1a3c5a-b40c-46ad-b87c-defc23266fd2', 4, N'de{''Recnhungberuf''}en{''Release Invoice''}', 4, NULL, 0, CAST(N'2020-12-10T15:13:10.540' AS DateTime), N'dl', CAST(N'2012-05-14T16:02:07.753' AS DateTime), N'SUP', N'ReleaseInvoice')
INSERT [dbo].[MDInvoiceType] ([MDInvoiceTypeID], [InvoiceTypeIndex], [MDNameTrans], [SortIndex], [XMLConfig], [IsDefault], [UpdateDate], [UpdateName], [InsertDate], [InsertName], [MDKey]) VALUES (N'12365e44-2ed5-44f3-acc1-efd618758366', 3, N'de{''Interner Rechnung''}en{''Internal Invoice''}', 2, NULL, 0, CAST(N'2020-12-10T15:12:49.993' AS DateTime), N'dl', CAST(N'2012-05-14T16:01:46.307' AS DateTime), N'SUP', N'InternalInvoice')
INSERT [dbo].[MDInvoiceType] ([MDInvoiceTypeID], [InvoiceTypeIndex], [MDNameTrans], [SortIndex], [XMLConfig], [IsDefault], [UpdateDate], [UpdateName], [InsertDate], [InsertName], [MDKey]) VALUES (N'952b8e29-ee47-4278-9cba-1fe131d1de5e', 2, N'en{''Purchase agreement (Contract)''}de{''Rahmenvertrag (Kontrakt)''}', 1, NULL, 0, CAST(N'2020-12-10T15:12:32.860' AS DateTime), N'dl', CAST(N'2012-05-14T16:01:19.613' AS DateTime), N'SUP', N'Contract')

INSERT [dbo].[MDInvoiceState] ([MDInvoiceStateID], [InvoiceStateIndex], [MDNameTrans], [SortIndex], [XMLConfig], [IsDefault], [InsertName], [InsertDate], [UpdateName], [UpdateDate], [MDKey]) VALUES (N'8fe52576-e8fc-4f72-93fb-722bf9694009', 4, N'en{''Blocked''}de{''Gesperrt''}', 4, NULL, 0, N'SUP', CAST(N'2013-07-19T11:09:34.597' AS DateTime), N'SUP', CAST(N'2013-07-19T11:09:34.597' AS DateTime), N'Blocked')
INSERT [dbo].[MDInvoiceState] ([MDInvoiceStateID], [InvoiceStateIndex], [MDNameTrans], [SortIndex], [XMLConfig], [IsDefault], [InsertName], [InsertDate], [UpdateName], [UpdateDate], [MDKey]) VALUES (N'ec859d05-0d6c-491c-af37-d40174da3c05', 3, N'en{''Completed''}de{''Fertiggestellt''}', 3, NULL, 0, N'xxx', CAST(N'2008-11-21T08:23:20.027' AS DateTime), N'SUP', CAST(N'2013-07-19T11:09:11.413' AS DateTime), N'Completed')
INSERT [dbo].[MDInvoiceState] ([MDInvoiceStateID], [InvoiceStateIndex], [MDNameTrans], [SortIndex], [XMLConfig], [IsDefault], [InsertName], [InsertDate], [UpdateName], [UpdateDate], [MDKey]) VALUES (N'b9282912-51d0-48ee-b9e9-92de4d38dbff', 5, N'en{''Cancelled''}de{''Storniert''}', 5, NULL, 0, N'SUP', CAST(N'2013-07-19T11:09:51.763' AS DateTime), N'SUP', CAST(N'2013-07-19T11:09:51.763' AS DateTime), N'Cancelled')
INSERT [dbo].[MDInvoiceState] ([MDInvoiceStateID], [InvoiceStateIndex], [MDNameTrans], [SortIndex], [XMLConfig], [IsDefault], [InsertName], [InsertDate], [UpdateName], [UpdateDate], [MDKey]) VALUES (N'12183399-bf9d-4265-83c2-d8f9b2385c04', 1, N'en{''Created''}de{''Neu Angelegt''}', 1, NULL, 1, N'xxx', CAST(N'2008-11-21T08:23:20.010' AS DateTime), N'xxx', CAST(N'2008-11-21T08:23:20.010' AS DateTime), N'Neu Angelegt')
INSERT [dbo].[MDInvoiceState] ([MDInvoiceStateID], [InvoiceStateIndex], [MDNameTrans], [SortIndex], [XMLConfig], [IsDefault], [InsertName], [InsertDate], [UpdateName], [UpdateDate], [MDKey]) VALUES (N'ab7fe1d9-9e41-4af2-a420-75ce9325cb05', 2, N'en{''In Process''}de{''In Bearbeitung''}', 2, NULL, 0, N'xxx', CAST(N'2008-11-21T08:23:20.027' AS DateTime), N'SUP', CAST(N'2013-07-19T11:09:06.843' AS DateTime), N'InProcess')