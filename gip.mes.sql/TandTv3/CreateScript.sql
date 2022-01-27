CREATE TABLE [dbo].[TandT_Direction](
	[DirectionID] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_TandT_Direction] PRIMARY KEY CLUSTERED 
(
	[DirectionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

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

CREATE TABLE [dbo].[TandT_MixPointACClass](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[ACClassID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointACClass] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[ACClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TandT_MixPointDeliveryNotePos](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[DeliveryNotePosID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointDeliveryNotePos] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[DeliveryNotePosID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TandT_MixPointFacility](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[DirectionID] [nvarchar](20) NOT NULL,
	[FacilityID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointFacility] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[DirectionID] ASC,
	[FacilityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TandT_MixPointFacilityBookingCharge](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[FacilityBookingChargeID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointFacilityBookingCharge] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[FacilityBookingChargeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TandT_MixPointFacilityLot](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[DirectionID] [nvarchar](20) NOT NULL,
	[FacilityLotID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointFacilityLot] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[DirectionID] ASC,
	[FacilityLotID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TandT_MixPointProdOrderPartslistPosRelation](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[ProdOrderPartslistPosRelationID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointProdOrderPartslistPosRelation] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[ProdOrderPartslistPosRelationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TandT_MixPointRelation](
	[SourceMixPointID] [uniqueidentifier] NOT NULL,
	[TargetMixPointID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointRelation] PRIMARY KEY CLUSTERED 
(
	[SourceMixPointID] ASC,
	[TargetMixPointID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TandT_MixPointProdOrderPartslistPos](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[ProdOrderPartslistPosID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointProdOrderPartslistPos] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[ProdOrderPartslistPosID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TandT_MixPointInOrderPos](
	[MixPointID] [uniqueidentifier] NOT NULL,
	[InOrderPosID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandT_MixPointInOrderPos] PRIMARY KEY CLUSTERED 
(
	[MixPointID] ASC,
	[InOrderPosID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

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
ALTER TABLE [dbo].[TandT_MixPoint]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPoint_FacilityLot] FOREIGN KEY([InwardLotID]) REFERENCES [dbo].[FacilityLot] ([FacilityLotID])
ALTER TABLE [dbo].[TandT_MixPoint]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPoint_Material] FOREIGN KEY([InwardMaterialID]) REFERENCES [dbo].[Material] ([MaterialID])
ALTER TABLE [dbo].[TandT_MixPoint]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPoint_ProdOrderBatch] FOREIGN KEY([ProdOrderBatchID]) REFERENCES [dbo].[ProdOrderBatch] ([ProdOrderBatchID])
ALTER TABLE [dbo].[TandT_MixPoint]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPoint_ProdOrderPartslistPos] FOREIGN KEY([ProdOrderPartslistPosID]) REFERENCES [dbo].[ProdOrderPartslistPos] ([ProdOrderPartslistPosID])
ALTER TABLE [dbo].[TandT_MixPoint]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPoint_TandT_Step] FOREIGN KEY([StepID]) REFERENCES [dbo].[TandT_Step] ([StepID])
ALTER TABLE [dbo].[TandT_MixPoint]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPoint_InOrderPos] FOREIGN KEY([InOrderPosID]) REFERENCES [dbo].[InOrderPos] ([InOrderPosID])
ALTER TABLE [dbo].[TandT_MixPointACClass]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointACClass_ACClass] FOREIGN KEY([ACClassID]) REFERENCES [dbo].[ACClass] ([ACClassID])
ALTER TABLE [dbo].[TandT_MixPointACClass]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointACClass_TandT_MixPoint] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])
ALTER TABLE [dbo].[TandT_MixPointDeliveryNotePos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointDeliveryNotePos_DeliveryNotePos] FOREIGN KEY([DeliveryNotePosID]) REFERENCES [dbo].[DeliveryNotePos] ([DeliveryNotePosID])
ALTER TABLE [dbo].[TandT_MixPointDeliveryNotePos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointDeliveryNotePos_TandT_MixPoint] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])
ALTER TABLE [dbo].[TandT_MixPointFacility]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacility_Facility] FOREIGN KEY([FacilityID]) REFERENCES [dbo].[Facility] ([FacilityID])
ALTER TABLE [dbo].[TandT_MixPointFacility]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacility_TandT_Direction] FOREIGN KEY([DirectionID]) REFERENCES [dbo].[TandT_Direction] ([DirectionID])
ALTER TABLE [dbo].[TandT_MixPointFacility]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacility_TandT_MixPoint] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])
ALTER TABLE [dbo].[TandT_MixPointFacilityBookingCharge]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacilityBookingCharge_FacilityBookingCharge] FOREIGN KEY([FacilityBookingChargeID]) REFERENCES [dbo].[FacilityBookingCharge] ([FacilityBookingChargeID])
ALTER TABLE [dbo].[TandT_MixPointFacilityBookingCharge]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacilityBookingCharge_TandT_MixPoint] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])
ALTER TABLE [dbo].[TandT_MixPointFacilityLot]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacilityLot_FacilityLot] FOREIGN KEY([FacilityLotID]) REFERENCES [dbo].[FacilityLot] ([FacilityLotID])
ALTER TABLE [dbo].[TandT_MixPointFacilityLot]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacilityLot_TandT_Direction] FOREIGN KEY([DirectionID]) REFERENCES [dbo].[TandT_Direction] ([DirectionID])
ALTER TABLE [dbo].[TandT_MixPointFacilityLot]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointFacilityLot_TandT_MixPoint] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])
ALTER TABLE [dbo].[TandT_MixPointProdOrderPartslistPosRelation]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointProdOrderPartslistPosRelation_ProdOrderPartslistPosRelation] FOREIGN KEY([ProdOrderPartslistPosRelationID]) REFERENCES [dbo].[ProdOrderPartslistPosRelation] ([ProdOrderPartslistPosRelationID])
ALTER TABLE [dbo].[TandT_MixPointProdOrderPartslistPosRelation]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointProdOrderPartslistPosRelation_TandT_MixPoint] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])
ALTER TABLE [dbo].[TandT_MixPointRelation]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointRelation_SourceMixPoint] FOREIGN KEY([SourceMixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])
ALTER TABLE [dbo].[TandT_MixPointRelation]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointRelation_TargetMixPoint] FOREIGN KEY([TargetMixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])
ALTER TABLE [dbo].[TandT_MixPointProdOrderPartslistPos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointProdOrderPartslistPos_ProdOrderPartslistPos] FOREIGN KEY([ProdOrderPartslistPosID]) REFERENCES [dbo].[ProdOrderPartslistPos] ([ProdOrderPartslistPosID])
ALTER TABLE [dbo].[TandT_MixPointProdOrderPartslistPos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointProdOrderPartslistPos_TandT_MixPoint] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])
ALTER TABLE [dbo].[TandT_MixPointInOrderPos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointInOrderPos_InOrderPos] FOREIGN KEY([InOrderPosID]) REFERENCES [dbo].[InOrderPos] ([InOrderPosID])
ALTER TABLE [dbo].[TandT_MixPointInOrderPos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointInOrderPos_TandT_MixPoint] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])
ALTER TABLE [dbo].[TandT_MixPointOutOrderPos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointOutOrderPos_OutOrderPos] FOREIGN KEY([OutOrderPosID]) REFERENCES [dbo].[OutOrderPos] ([OutOrderPosID])
ALTER TABLE [dbo].[TandT_MixPointOutOrderPos]  WITH CHECK ADD  CONSTRAINT [FK_TandT_MixPointOutOrderPos_TandT_MixPoint] FOREIGN KEY([MixPointID]) REFERENCES [dbo].[TandT_MixPoint] ([MixPointID])

insert into [dbo].[TandT_Direction] (DirectionID) values ('Inward');
insert into [dbo].[TandT_Direction] (DirectionID) values ('Outward');