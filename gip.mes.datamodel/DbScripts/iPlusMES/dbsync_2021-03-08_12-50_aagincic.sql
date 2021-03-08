-- author:		@aagincic
-- name:		Rename OutOffering into OutOffer
-- created:		2021-03-08
-- updated:		2021-03-08


-- P0 Setup external entities to null
EXEC sp_rename 'dbo.InRequest.DistributorOfferingNo', 'DistributorOfferNo', 'COLUMN';
update OutOrder set BasedOnOutOfferingID = null; 
GO
-- P1 Delete Offering Tables

-- p1.1 drop [OutOfferingConfig]
ALTER TABLE [dbo].[OutOfferingConfig] DROP CONSTRAINT [FK_OutOfferingConfig_ParentOutOfferingConfigID]
ALTER TABLE [dbo].[OutOfferingConfig] DROP CONSTRAINT [FK_OutOfferingConfig_OutOfferingID]
ALTER TABLE [dbo].[OutOfferingConfig] DROP CONSTRAINT [FK_OutOfferingConfig_MaterialID]
ALTER TABLE [dbo].[OutOfferingConfig] DROP CONSTRAINT [FK_OutOfferingConfig_DataTypeACClassID]
ALTER TABLE [dbo].[OutOfferingConfig] DROP CONSTRAINT [FK_OutOfferingConfig_ACClassPropertyRelationID]
ALTER TABLE [dbo].[OutOfferingConfig] DROP CONSTRAINT [FK_OutOfferingConfig_ACClassID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OutOfferingConfig]') AND type in (N'U'))
DROP TABLE [dbo].[OutOfferingConfig]
GO

-- p1.2 drop OutOfferingPos
ALTER TABLE [dbo].[OutOfferingPos] DROP CONSTRAINT [FK_OutOfferingPos_ParentOutOfferingPosID]
ALTER TABLE [dbo].[OutOfferingPos] DROP CONSTRAINT [FK_OutOfferingPos_OutOfferingID]
ALTER TABLE [dbo].[OutOfferingPos] DROP CONSTRAINT [FK_OutOfferingPos_MDUnitID]
ALTER TABLE [dbo].[OutOfferingPos] DROP CONSTRAINT [FK_OutOfferingPos_MDTimeRangeID]
ALTER TABLE [dbo].[OutOfferingPos] DROP CONSTRAINT [FK_OutOfferingPos_MDCountrySalesTaxID]
ALTER TABLE [dbo].[OutOfferingPos] DROP CONSTRAINT [FK_OutOfferingPos_MaterialID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OutOfferingPos]') AND type in (N'U'))
DROP TABLE [dbo].[OutOfferingPos]
GO

-- p1.3 drop OutOffering
ALTER TABLE [dbo].[OutOffering] DROP CONSTRAINT [FK_OutOffering_MDTimeRangeID]
ALTER TABLE [dbo].[OutOffering] DROP CONSTRAINT [FK_OutOffering_MDTermOfPaymentID]
ALTER TABLE [dbo].[OutOffering] DROP CONSTRAINT [FK_OutOffering_MDOutOrderTypeID]
ALTER TABLE [dbo].[OutOffering] DROP CONSTRAINT [FK_OutOffering_MDOutOfferingStateID]
ALTER TABLE [dbo].[OutOffering] DROP CONSTRAINT [FK_OutOffering_MDDelivTypeID]
ALTER TABLE [dbo].[OutOffering] DROP CONSTRAINT [FK_OutOffering_DeliveryCompanyAddressID]
ALTER TABLE [dbo].[OutOffering] DROP CONSTRAINT [FK_OutOffering_CompanyID]
ALTER TABLE [dbo].[OutOffering] DROP CONSTRAINT [FK_OutOffering_BillingCompanyAddressID]
ALTER TABLE [dbo].[OutOrder] DROP CONSTRAINT [FK_OutOrder_BasedOnOutOfferingID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OutOffering]') AND type in (N'U'))
DROP TABLE [dbo].[OutOffering]
GO



-- P2 Recreate / Rename tables now with Offer name

-- p2.1 Rename Table MDOutOfferingState into MDOutOfferState, [FK_OutOrder_BasedOnOutOfferingID]
EXEC sp_rename 'dbo.MDOutOfferingState', 'MDOutOfferState';
GO
EXEC sp_rename 'dbo.MDOutOfferState.MDOutOfferingStateID', 'MDOutOfferStateID', 'COLUMN';
EXEC sp_rename 'dbo.MDOutOfferState.MDOutOfferingStateIndex', 'MDOutOfferStateIndex', 'COLUMN';
EXEC sp_rename 'dbo.OutOrder.BasedOnOutOfferingID', 'BasedOnOutOfferID', 'COLUMN';
EXEC sp_rename 'dbo.MDOutOfferState.PK_MDOutOfferingState', 'PK_MDOutOfferState';
GO

-- p2.2 Create OutOffer Table
CREATE TABLE [dbo].[OutOffer](
	[OutOfferID] [uniqueidentifier] NOT NULL,
	[OutOfferNo] [varchar](20) NOT NULL,
	[OutOfferVersion] [int] NOT NULL,
	[OutOfferDate] [datetime] NULL,
	[MDOutOrderTypeID] [uniqueidentifier] NOT NULL,
	[MDOutOfferStateID] [uniqueidentifier] NOT NULL,
	[CustomerCompanyID] [uniqueidentifier] NOT NULL,
	[CustRequestNo] [varchar](20) NULL,
	[DeliveryCompanyAddressID] [uniqueidentifier] NULL,
	[BillingCompanyAddressID] [uniqueidentifier] NOT NULL,
	[TargetDeliveryDate] [datetime] NOT NULL,
	[TargetDeliveryMaxDate] [datetime] NULL,
	[MDTimeRangeID] [uniqueidentifier] NULL,
	[MDDelivTypeID] [uniqueidentifier] NOT NULL,
	[PriceNet] [money] NOT NULL,
	[PriceGross] [money] NOT NULL,
	[MDTermOfPaymentID] [uniqueidentifier] NULL,
	[Comment] [varchar](max) NULL,
	[XMLConfig] [text] NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[XMLDesign] [text] null
 CONSTRAINT [PK_OutOffer] PRIMARY KEY CLUSTERED 
(
	[OutOfferID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[OutOffer]  WITH CHECK ADD  CONSTRAINT [FK_OutOffer_BillingCompanyAddressID] FOREIGN KEY([BillingCompanyAddressID]) REFERENCES [dbo].[CompanyAddress] ([CompanyAddressID])
ALTER TABLE [dbo].[OutOffer]  WITH CHECK ADD  CONSTRAINT [FK_OutOffer_CompanyID] FOREIGN KEY([CustomerCompanyID]) REFERENCES [dbo].[Company] ([CompanyID])
ALTER TABLE [dbo].[OutOffer]  WITH CHECK ADD  CONSTRAINT [FK_OutOffer_DeliveryCompanyAddressID] FOREIGN KEY([DeliveryCompanyAddressID]) REFERENCES [dbo].[CompanyAddress] ([CompanyAddressID])
ALTER TABLE [dbo].[OutOffer]  WITH CHECK ADD  CONSTRAINT [FK_OutOffer_MDDelivTypeID] FOREIGN KEY([MDDelivTypeID]) REFERENCES [dbo].[MDDelivType] ([MDDelivTypeID])
ALTER TABLE [dbo].[OutOffer]  WITH CHECK ADD  CONSTRAINT [FK_OutOffer_MDOutOfferStateID] FOREIGN KEY([MDOutOfferStateID]) REFERENCES [dbo].[MDOutOfferState] ([MDOutOfferStateID])
ALTER TABLE [dbo].[OutOffer]  WITH CHECK ADD  CONSTRAINT [FK_OutOffer_MDOutOrderTypeID] FOREIGN KEY([MDOutOrderTypeID]) REFERENCES [dbo].[MDOutOrderType] ([MDOutOrderTypeID])
ALTER TABLE [dbo].[OutOffer]  WITH CHECK ADD  CONSTRAINT [FK_OutOffer_MDTermOfPaymentID] FOREIGN KEY([MDTermOfPaymentID]) REFERENCES [dbo].[MDTermOfPayment] ([MDTermOfPaymentID])
ALTER TABLE [dbo].[OutOffer]  WITH CHECK ADD  CONSTRAINT [FK_OutOffer_MDTimeRangeID] FOREIGN KEY([MDTimeRangeID]) REFERENCES [dbo].[MDTimeRange] ([MDTimeRangeID])
ALTER TABLE [dbo].[OutOrder]  WITH CHECK ADD  CONSTRAINT [FK_OutOrder_BasedOnOutOfferID] FOREIGN KEY([BasedOnOutOfferID]) REFERENCES [dbo].[OutOffer] ([OutOfferID])
GO

-- p2.3 Create [OutOfferConfig] Table
CREATE TABLE [dbo].[OutOfferConfig](
	[OutOfferConfigID] [uniqueidentifier] NOT NULL,
	[OutOfferID] [uniqueidentifier] NOT NULL,
	[VBiACClassID] [uniqueidentifier] NULL,
	[VBiACClassPropertyRelationID] [uniqueidentifier] NULL,
	[MaterialID] [uniqueidentifier] NULL,
	[ParentOutOfferConfigID] [uniqueidentifier] NULL,
	[VBiValueTypeACClassID] [uniqueidentifier] NOT NULL,
	[KeyACUrl] [varchar](max) NULL,
	[PreConfigACUrl] [varchar](max) NULL,
	[LocalConfigACUrl] [varchar](max) NULL,
	[Expression] [text] NULL,
	[Comment] [varchar](max) NULL,
	[XMLConfig] [text] NOT NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_OutOfferConfig] PRIMARY KEY CLUSTERED 
(
	[OutOfferConfigID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[OutOfferConfig]  WITH CHECK ADD  CONSTRAINT [FK_OutOfferConfig_ACClassID] FOREIGN KEY([VBiACClassID]) REFERENCES [dbo].[ACClass] ([ACClassID])
ALTER TABLE [dbo].[OutOfferConfig]  WITH CHECK ADD  CONSTRAINT [FK_OutOfferConfig_ACClassPropertyRelationID] FOREIGN KEY([VBiACClassPropertyRelationID]) REFERENCES [dbo].[ACClassPropertyRelation] ([ACClassPropertyRelationID])
ALTER TABLE [dbo].[OutOfferConfig]  WITH CHECK ADD  CONSTRAINT [FK_OutOfferConfig_DataTypeACClassID] FOREIGN KEY([VBiValueTypeACClassID]) REFERENCES [dbo].[ACClass] ([ACClassID])
ALTER TABLE [dbo].[OutOfferConfig]  WITH CHECK ADD  CONSTRAINT [FK_OutOfferConfig_MaterialID] FOREIGN KEY([MaterialID]) REFERENCES [dbo].[Material] ([MaterialID])
ALTER TABLE [dbo].[OutOfferConfig]  WITH CHECK ADD  CONSTRAINT [FK_OutOfferConfig_OutOfferID] FOREIGN KEY([OutOfferID]) REFERENCES [dbo].[OutOffer] ([OutOfferID]) ON DELETE CASCADE
ALTER TABLE [dbo].[OutOfferConfig]  WITH CHECK ADD  CONSTRAINT [FK_OutOfferConfig_ParentOutOfferConfigID] FOREIGN KEY([ParentOutOfferConfigID]) REFERENCES [dbo].[OutOfferConfig] ([OutOfferConfigID])
GO

-- p2.4 Create [OutOfferPos] Table

CREATE TABLE [dbo].[OutOfferPos](
	[OutOfferPosID] [uniqueidentifier] NOT NULL,
	[OutOfferID] [uniqueidentifier] NOT NULL,
	[Sequence] [int] NOT NULL,
	[MaterialPosTypeIndex] [smallint] NOT NULL,
	[ParentOutOfferPosID] [uniqueidentifier] NULL,
	[GroupOutOfferPosID] [uniqueidentifier] NULL,
	[MaterialID] [uniqueidentifier] NOT NULL,
	[TargetQuantityUOM] [float] NOT NULL,
	[MDUnitID] [uniqueidentifier] NULL,
	[TargetQuantity] [float] NOT NULL,
	[TargetWeight] [float] NOT NULL,
	[TargetDeliveryDate] [datetime] NOT NULL,
	[TargetDeliveryMaxDate] [datetime] NULL,
	[TargetDeliveryPriority] [smallint] NOT NULL,
	[MDTimeRangeID] [uniqueidentifier] NULL,
	[PriceNet] [money] NOT NULL,
	[PriceGross] [money] NOT NULL,
	[MDCountrySalesTaxID] [uniqueidentifier] NULL,
	[Comment] [varchar](max) NULL,
	[Comment2] [varchar](max) NULL,
	[XMLConfig] [text] NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[XMLDesign] [text] null
 CONSTRAINT [PK_OutOfferPos] PRIMARY KEY CLUSTERED 
(
	[OutOfferPosID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[OutOfferPos]  WITH CHECK ADD  CONSTRAINT [FK_OutOfferPos_MaterialID] FOREIGN KEY([MaterialID]) REFERENCES [dbo].[Material] ([MaterialID])
ALTER TABLE [dbo].[OutOfferPos]  WITH CHECK ADD  CONSTRAINT [FK_OutOfferPos_MDCountrySalesTaxID] FOREIGN KEY([MDCountrySalesTaxID]) REFERENCES [dbo].[MDCountrySalesTax] ([MDCountrySalesTaxID])
ALTER TABLE [dbo].[OutOfferPos]  WITH CHECK ADD  CONSTRAINT [FK_OutOfferPos_MDTimeRangeID] FOREIGN KEY([MDTimeRangeID]) REFERENCES [dbo].[MDTimeRange] ([MDTimeRangeID])
ALTER TABLE [dbo].[OutOfferPos]  WITH CHECK ADD  CONSTRAINT [FK_OutOfferPos_MDUnitID] FOREIGN KEY([MDUnitID]) REFERENCES [dbo].[MDUnit] ([MDUnitID]) 
ALTER TABLE [dbo].[OutOfferPos]  WITH CHECK ADD  CONSTRAINT [FK_OutOfferPos_OutOfferID] FOREIGN KEY([OutOfferID]) REFERENCES [dbo].[OutOffer] ([OutOfferID]) ON DELETE CASCADE
ALTER TABLE [dbo].[OutOfferPos]  WITH CHECK ADD  CONSTRAINT [FK_OutOfferPos_ParentOutOfferPosID] FOREIGN KEY([ParentOutOfferPosID]) REFERENCES [dbo].[OutOfferPos] ([OutOfferPosID])
ALTER TABLE [dbo].[OutOfferPos]  WITH CHECK ADD  CONSTRAINT [FK_OutOfferPos_GroupOutOfferPosID] FOREIGN KEY([GroupOutOfferPosID]) REFERENCES [dbo].[OutOfferPos] ([OutOfferPosID])