alter table dbo.OutOrder add XMLDesign text null;
alter table dbo.OutOrderPos add XMLDesign text null;
alter table dbo.OutOfferPos add GroupSum bit null;
alter table dbo.OutOrderPos add GroupSum bit null;
GO
update dbo.OutOfferPos set GroupSum = 0;
update dbo.OutOrderPos set GroupSum = 0;
GO
alter table  dbo.OutOfferPos alter column GroupSum bit not null;
alter table  dbo.OutOrderPos alter column GroupSum bit not null;
GO
USE [OutOrderPosModelV4]
GO
CREATE TABLE [dbo].[Invoice](
	[InvoiceID] [uniqueidentifier] NOT NULL,
	[MDInvoiceTypeID] [uniqueidentifier] NOT NULL,
	[MDInvoiceStateID] [uniqueidentifier] NOT NULL,
	[InvoiceNo] [varchar](50) NOT NULL,
	[InvoiceDate] [datetime] NOT NULL,
	[CustomerCompanyID] [uniqueidentifier] NOT NULL,
	[CustRequestNo] [varchar](50) NULL,
	[DeliveryCompanyAddressID] [uniqueidentifier] NULL,
	[BillingCompanyAddressID] [uniqueidentifier] NULL,
	[OutOrderOrderID] [uniqueidentifier] NULL,
	[MDCountrySalesTaxID] [uniqueidentifier] NOT NULL,
	[Comment] [varchar](max) NULL,
	[XMLConfig] [text] NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[XMLDesign] [text] NULL,
 CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
(
	[InvoiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[InvoicePos](
	[InvoicePosID] [uniqueidentifier] NOT NULL,
	[InvoiceID] [uniqueidentifier] NOT NULL,
	[Sequence] [int] NOT NULL,
	[MaterialID] [uniqueidentifier] NOT NULL,
	[MDCountrySalesTaxMDMaterialGroupID] [uniqueidentifier] NULL,
	[MDCountrySalesTaxMaterialID] [uniqueidentifier] NULL,
	[Price] [money] NOT NULL,
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

CREATE TABLE [dbo].[MDInvoiceState](
	[MDInvoiceStateID] [uniqueidentifier] NOT NULL,
	[MDInvoiceStateIndex] [smallint] NOT NULL,
	[MDNameTrans] [varchar](max) NOT NULL,
	[SortIndex] [smallint] NOT NULL,
	[XMLConfig] [text] NULL,
	[IsDefault] [bit] NOT NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[MDKey] [varchar](40) NOT NULL,
 CONSTRAINT [PK_MDInvoiceState] PRIMARY KEY CLUSTERED 
(
	[MDInvoiceStateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[MDInvoiceType](
	[MDInvoiceTypeID] [uniqueidentifier] NOT NULL,
	[MDInvoiceTypeIndex] [smallint] NOT NULL,
	[MDNameTrans] [varchar](max) NOT NULL,
	[SortIndex] [smallint] NOT NULL,
	[XMLConfig] [text] NULL,
	[IsDefault] [bit] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[InsertName] [varchar](20) NOT NULL,
	[MDKey] [varchar](40) NOT NULL,
 CONSTRAINT [PK_MDInvoiceType] PRIMARY KEY CLUSTERED 
(
	[MDInvoiceTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_InvoiceNo] ON [dbo].[Invoice]
(
	[InvoiceNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE UNIQUE NONCLUSTERED INDEX [UX_MDInvoiceState_MDKey] ON [dbo].[MDInvoiceState]
(
	[MDKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE UNIQUE NONCLUSTERED INDEX [UX_MDInvoiceType_MDKey] ON [dbo].[MDInvoiceType]
(
	[MDKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_Company] FOREIGN KEY([CustomerCompanyID]) REFERENCES [dbo].[Company] ([CompanyID])
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_CompanyAddress] FOREIGN KEY([DeliveryCompanyAddressID]) REFERENCES [dbo].[CompanyAddress] ([CompanyAddressID])
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_CompanyAddress1] FOREIGN KEY([BillingCompanyAddressID]) REFERENCES [dbo].[CompanyAddress] ([CompanyAddressID])
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_MDCountrySalesTax] FOREIGN KEY([MDCountrySalesTaxID]) REFERENCES [dbo].[MDCountrySalesTax] ([MDCountrySalesTaxID])
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_MDInvoiceState] FOREIGN KEY([MDInvoiceStateID]) REFERENCES [dbo].[MDInvoiceState] ([MDInvoiceStateID])
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_MDInvoiceType] FOREIGN KEY([MDInvoiceTypeID]) REFERENCES [dbo].[MDInvoiceType] ([MDInvoiceTypeID])
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_OutOrder] FOREIGN KEY([OutOrderOrderID]) REFERENCES [dbo].[OutOrder] ([OutOrderID])
ALTER TABLE [dbo].[InvoicePos]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePos_Invoice] FOREIGN KEY([InvoiceID]) REFERENCES [dbo].[Invoice] ([InvoiceID])
ALTER TABLE [dbo].[InvoicePos]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePos_Material] FOREIGN KEY([MaterialID]) REFERENCES [dbo].[Material] ([MaterialID])
ALTER TABLE [dbo].[InvoicePos]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePos_MDCountrySalesTaxMaterial] FOREIGN KEY([MDCountrySalesTaxMaterialID]) REFERENCES [dbo].[MDCountrySalesTaxMaterial] ([MDCountrySalesTaxMaterialID])
ALTER TABLE [dbo].[InvoicePos]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePos_MDCountrySalesTaxMDMaterialGroup] FOREIGN KEY([MDCountrySalesTaxMDMaterialGroupID]) REFERENCES [dbo].[MDCountrySalesTaxMDMaterialGroup] ([MDCountrySalesTaxMDMaterialGroupID])
ALTER TABLE [dbo].[InvoicePos]  WITH CHECK ADD  CONSTRAINT [FK_InvoicePos_OutOrderPos] FOREIGN KEY([OutOrderPosID]) REFERENCES [dbo].[OutOrderPos] ([OutOrderPosID])