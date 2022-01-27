-- Creating lookup tables
-- TandT_MDTrackingDirection (former: TandT_TrackingStyle)
CREATE TABLE [dbo].[TandT_MDTrackingDirection](
	[MDTrackingDirectionID] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_TandT_MDTrackingDirection] PRIMARY KEY CLUSTERED 
(
	[MDTrackingDirectionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
insert into [TandT_MDTrackingDirection] ([MDTrackingDirectionID]) values('Backward');
insert into [TandT_MDTrackingDirection] ([MDTrackingDirectionID]) values('Forward');

GO

-- TandT_MDBookingDirection (former: TandT_Direction)
CREATE TABLE [dbo].[TandT_MDBookingDirection](
	[MDBookingDirectionID] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_TandT_MDBookingDirection] PRIMARY KEY CLUSTERED 
(
	[MDBookingDirectionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
insert into [TandT_MDBookingDirection] ([MDBookingDirectionID]) values('Inward');
insert into [TandT_MDBookingDirection] ([MDBookingDirectionID]) values('Outward');
GO


--TandT_MDTrackingStartItemType (former: TandT_ItemType)
CREATE TABLE [dbo].[TandT_MDTrackingStartItemType](
	[MDTrackingStartItemTypeID] [varchar](150) NOT NULL,
	[ACCaptionTranslation] [varchar](200) NOT NULL,
CONSTRAINT [PK_TandT_MDTrackingStartItemType] PRIMARY KEY CLUSTERED 
(
	[MDTrackingStartItemTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'ACClass', N'en{''Class''}de{''Klasse''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'DeliveryNote', N'en{''Delivery Note''}de{''Eingangslieferschein''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'DeliveryNotePos', N'en{''Indeliverynotepos''}de{''Eingangslieferscheinposition''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'Facility', N'en{''Facility''}de{''Lagerplatz''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'FacilityBooking', N'en{''Stock movement''}de{''Lagerbewegung''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'FacilityBookingCharge', N'en{''Stock movement of quantum''}de{''Lagerbewegung Quant''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'FacilityCharge', N'en{''Facilitycharge''}de{''Chargenplatz''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'FacilityLot', N'en{''Lot''}de{''Los''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'InOrder', N'en{''Purchase Order''}de{''Bestellung''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'InOrderPos', N'en{''Purchase Order Pos.''}de{''Bestellposition''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'OutOrder', N'en{''Sales Order''}de{''Verkaufsauftrag''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'OutOrderPos', N'en{''Sales Order Pos.''}de{''Auftragsposition''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'ProdOrder', N'en{''Production Order''}de{''Produktionsauftrag''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'ProdOrderPartslist', N'en{''ProductionorderBillOfMaterials''}de{''Produktionsauftragsstückliste''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'ProdOrderPartslistPos', N'en{''ProdOrderPartslistPos''}de{''ProdOrderPartslistPos''}')
INSERT [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID], [ACCaptionTranslation]) VALUES (N'ProdOrderPartslistPosRelation', N'en{''Production Order Pos. Status''}de{''Produktionsauftrag Pos.-Status''}')


-- Create Filtering tables
-- [TandT_FilterTracking] (former: TandT_Job)
CREATE TABLE [dbo].[TandT_FilterTracking](
	[FilterTrackingID] [uniqueidentifier] NOT NULL,
	[MDTrackingDirectionID] [nvarchar](20) NOT NULL,
	[MDTrackingStartItemTypeID] [varchar](150) NOT NULL,
	[FilterTrackingNo] [varchar](50) NOT NULL,
	[FilterDateFrom] [datetime] NULL,
	[FilterDateTo] [datetime] NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NULL,
	[ItemSystemNo] [varchar](50) NOT NULL,
	[PrimaryKeyID] [uniqueidentifier] NOT NULL,
	[InsertName] [varchar](5) NOT NULL,
CONSTRAINT [PK_TandT_FilterTracking] PRIMARY KEY CLUSTERED 
(
	[FilterTrackingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TandT_FilterTracking]  WITH CHECK ADD  CONSTRAINT [FK_TandT_FilterTracking_MDTrackingStartItemTypeID] FOREIGN KEY([MDTrackingStartItemTypeID]) REFERENCES [dbo].[TandT_MDTrackingStartItemType] ([MDTrackingStartItemTypeID])
ALTER TABLE [dbo].[TandT_FilterTracking]  WITH CHECK ADD  CONSTRAINT [FK_TandT_FilterTracking_MDTrackingDirectionID] FOREIGN KEY([MDTrackingDirectionID]) REFERENCES [dbo].[TandT_MDTrackingDirection] ([MDTrackingDirectionID])


-- TandT_FilterTrackingMaterial
CREATE TABLE [dbo].[TandT_FilterTrackingMaterial](
	[FilterTrackingMaterialID] [uniqueidentifier] NOT NULL,
	[FilterTrackingID] [uniqueidentifier] NOT NULL,
	[MaterialID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_FilterTrackingMaterial] PRIMARY KEY CLUSTERED 
(
	[FilterTrackingMaterialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TandT_FilterTrackingMaterial]  WITH CHECK ADD  CONSTRAINT [FK_TandT_FilterTrackingMaterial_MaterialID] FOREIGN KEY([MaterialID]) REFERENCES [dbo].[Material] ([MaterialID]) ON DELETE CASCADE
ALTER TABLE [dbo].[TandT_FilterTrackingMaterial]  WITH CHECK ADD  CONSTRAINT [FK_TandT_FilterTrackingMaterial_TandT_FilterTrackingID] FOREIGN KEY([FilterTrackingID]) REFERENCES [dbo].[TandT_FilterTracking] ([FilterTrackingID]) ON DELETE CASCADE

-- Create step table
-- TandT_Step
CREATE TABLE [dbo].[TandT_Step](
	[StepID] [uniqueidentifier] NOT NULL,
	[FilterTrackingID] [uniqueidentifier] NOT NULL,
	[StepNo] [int] NOT NULL,
	[StepName] [varchar](150) NULL,
CONSTRAINT [PK_TandT_Step] PRIMARY KEY CLUSTERED 
(
	[StepID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TandT_Step]  WITH CHECK ADD  CONSTRAINT [FK_TandT_Step_FilterTrackingID] FOREIGN KEY([FilterTrackingID]) REFERENCES [dbo].[TandT_FilterTracking] ([FilterTrackingID]) ON DELETE CASCADE


--TandT_MixPoint
CREATE TABLE [dbo].[TandT_MixPoint](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[StepID] [uniqueidentifier] NOT NULL,
	[IsProductionPoint] [bit] not null,
	[IsInputPoint] [bit] not null,
	[InwardLotID] [uniqueidentifier] NULL,
	[InwardMaterialID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPoint] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TandT_MixPoint]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPoint_FacilityLotID] FOREIGN KEY([InwardLotID]) REFERENCES [dbo].[FacilityLot] ([FacilityLotID])
ALTER TABLE [dbo].[TandT_MixPoint]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPoint_MaterialID] FOREIGN KEY([InwardMaterialID]) REFERENCES [dbo].[Material] ([MaterialID])
ALTER TABLE [dbo].[TandT_MixPoint]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPoint_TandT_StepID] FOREIGN KEY([StepID]) REFERENCES [dbo].[TandT_Step] ([StepID])


--TandT_MixPointACClass
CREATE TABLE [dbo].[TandT_MixPointACClass](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[ACClassID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointACClass] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[ACClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TandT_MixPointACClass]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointACClass_ACClassID] FOREIGN KEY([ACClassID]) REFERENCES [dbo].[ACClass] ([ACClassID])
ALTER TABLE [dbo].[TandT_MixPointACClass]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointACClass_TandT_MixPointID] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])

--TandT_MixPointDeliveryNotePos
CREATE TABLE [dbo].[TandT_MixPointDeliveryNotePos](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[DeliveryNotePosID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointDeliveryNotePos] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[DeliveryNotePosID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TandT_MixPointDeliveryNotePos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointDeliveryNotePos_DeliveryNotePosID] FOREIGN KEY([DeliveryNotePosID]) REFERENCES [dbo].[DeliveryNotePos] ([DeliveryNotePosID])
ALTER TABLE [dbo].[TandT_MixPointDeliveryNotePos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointDeliveryNotePos_TandT_MixPointID] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])

-- ### CheckPoint ###
--TandT_MixPointFacility
CREATE TABLE [dbo].[TandT_MixPointFacility](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[MDBookingDirectionID] [nvarchar](20) NOT NULL,
	[FacilityID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointFacility] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[MDBookingDirectionID] ASC,
	[FacilityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TandT_MixPointFacility]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacility_FacilityID] FOREIGN KEY([FacilityID]) REFERENCES [dbo].[Facility] ([FacilityID])
ALTER TABLE [dbo].[TandT_MixPointFacility]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacility_TandT_MDBookingDirectionID] FOREIGN KEY([MDBookingDirectionID]) REFERENCES [dbo].[TandT_MDBookingDirection] ([MDBookingDirectionID])
ALTER TABLE [dbo].[TandT_MixPointFacility]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacility_TandT_MixPointID] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])

--TandT_MixPointFacilityBookingCharge
CREATE TABLE [dbo].[TandT_MixPointFacilityBookingCharge](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[FacilityBookingChargeID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointFacilityBookingCharge] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[FacilityBookingChargeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TandT_MixPointFacilityBookingCharge]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacilityBookingCharge_FacilityBookingChargeID] FOREIGN KEY([FacilityBookingChargeID]) REFERENCES [dbo].[FacilityBookingCharge] ([FacilityBookingChargeID])
ALTER TABLE [dbo].[TandT_MixPointFacilityBookingCharge]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacilityBookingCharge_TandT_MixPoint] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])

--TandT_MixPointFacilityLot
CREATE TABLE [dbo].[TandT_MixPointFacilityLot](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[MDBookingDirectionID] [nvarchar](20) NOT NULL,
	[FacilityLotID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointFacilityLot] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[MDBookingDirectionID] ASC,
	[FacilityLotID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TandT_MixPointFacilityLot]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacilityLot_FacilityLotID] FOREIGN KEY([FacilityLotID]) REFERENCES [dbo].[FacilityLot] ([FacilityLotID])
ALTER TABLE [dbo].[TandT_MixPointFacilityLot]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacilityLot_TandT_MDBookingDirectionID] FOREIGN KEY([MDBookingDirectionID]) REFERENCES [dbo].[TandT_MDBookingDirection] ([MDBookingDirectionID])
ALTER TABLE [dbo].[TandT_MixPointFacilityLot]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacilityLot_TandT_MixPointID] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])

--TandT_MixPointInOrderPos
CREATE TABLE [dbo].[TandT_MixPointInOrderPos](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[InOrderPosID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointInOrderPos] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[InOrderPosID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TandT_MixPointInOrderPos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointInOrderPos_InOrderPosID] FOREIGN KEY([InOrderPosID]) REFERENCES [dbo].[InOrderPos] ([InOrderPosID])
ALTER TABLE [dbo].[TandT_MixPointInOrderPos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointInOrderPos_TandT_MixPointID] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])

--TandT_MixPointOutOrderPos
CREATE TABLE [dbo].[TandT_MixPointOutOrderPos](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[OutOrderPosID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointOutOrderPos] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[OutOrderPosID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TandT_MixPointOutOrderPos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointOutOrderPos_OutOrderPosID] FOREIGN KEY([OutOrderPosID]) REFERENCES [dbo].[OutOrderPos] ([OutOrderPosID])
ALTER TABLE [dbo].[TandT_MixPointOutOrderPos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointOutOrderPos_TandT_MixPointID] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])

--TandT_MixPointProdOrderPartslistPos
CREATE TABLE [dbo].[TandT_MixPointProdOrderPartslistPos](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[ProdOrderPartslistPosID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointProdOrderPartslistPos] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[ProdOrderPartslistPosID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TandT_MixPointProdOrderPartslistPos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointProdOrderPartslistPos_ProdOrderPartslistPosID] FOREIGN KEY([ProdOrderPartslistPosID]) REFERENCES [dbo].[ProdOrderPartslistPos] ([ProdOrderPartslistPosID])
ALTER TABLE [dbo].[TandT_MixPointProdOrderPartslistPos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointProdOrderPartslistPos_TandT_MixPointID] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])

--TandT_MixPointProdOrderPartslistPosRelation
CREATE TABLE [dbo].[TandT_MixPointProdOrderPartslistPosRelation](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[ProdOrderPartslistPosRelationID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointProdOrderPartslistPosRelation] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[ProdOrderPartslistPosRelationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TandT_MixPointProdOrderPartslistPosRelation]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointProdOrderPartslistPosRelation_ProdOrderPartslistPosRelationID] FOREIGN KEY([ProdOrderPartslistPosRelationID]) REFERENCES [dbo].[ProdOrderPartslistPosRelation] ([ProdOrderPartslistPosRelationID])
ALTER TABLE [dbo].[TandT_MixPointProdOrderPartslistPosRelation]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointProdOrderPartslistPosRelation_TandT_MixPointID] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])

--TandT_MixPointRelation
CREATE TABLE [dbo].[TandT_MixPointRelation](
	[SourceMixPointID] [uniqueidentifier] NOT NULL,
	[TargetMixPointID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointRelation] PRIMARY KEY CLUSTERED 
(
	[SourceMixPointID] ASC,
	[TargetMixPointID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[TandT_MixPointRelation]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointRelation_SourceMixPointID] FOREIGN KEY([SourceMixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])
ALTER TABLE [dbo].[TandT_MixPointRelation]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointRelation_TargetMixPointID] FOREIGN KEY([TargetMixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])
