-- droping TandTv2 resources
/****** Object:  StoredProcedure [dbo].[udpTandTv2JobDelete]    Script Date: 2.5.2023. 9:39:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[udpTandTv2JobDelete]') AND type in (N'P'))
DROP PROCEDURE [dbo].[udpTandTv2JobDelete]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2TempPos_TandTv2StepID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2TempPos]'))
ALTER TABLE [dbo].[TandTv2TempPos] DROP CONSTRAINT [FK_TandTv2TempPos_TandTv2StepID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepLot_TandTv2StepID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepLot]'))
ALTER TABLE [dbo].[TandTv2StepLot] DROP CONSTRAINT [FK_TandTv2StepLot_TandTv2StepID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItemRelation_TargetTandTv2StepItemID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItemRelation]'))
ALTER TABLE [dbo].[TandTv2StepItemRelation] DROP CONSTRAINT [FK_TandTv2StepItemRelation_TargetTandTv2StepItemID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItemRelation_TandTv2RelationTypeID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItemRelation]'))
ALTER TABLE [dbo].[TandTv2StepItemRelation] DROP CONSTRAINT [FK_TandTv2StepItemRelation_TandTv2RelationTypeID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItemRelation_SourceTandTv2StepItemID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItemRelation]'))
ALTER TABLE [dbo].[TandTv2StepItemRelation] DROP CONSTRAINT [FK_TandTv2StepItemRelation_SourceTandTv2StepItemID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_TandTv2StepID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_TandTv2StepID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_TandTv2OperationID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_TandTv2OperationID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_TandTv2ItemTypeID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_TandTv2ItemTypeID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_ProdOrderPartslistPosRelationID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_ProdOrderPartslistPosRelationID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_ProdOrderPartslistPosID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_ProdOrderPartslistPosID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_ProdOrderPartslistID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_ProdOrderPartslistID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_ProdOrderID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_ProdOrderID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_OutOrderPosID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_OutOrderPosID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_OutOrderID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_OutOrderID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_InOrderPosID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_InOrderPosID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_InOrderID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_InOrderID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_FacilityPreBooking]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_FacilityPreBooking]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_FacilityLotID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_FacilityLotID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_FacilityID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_FacilityID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_FacilityChargeID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_FacilityChargeID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_FacilityBookingID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_FacilityBookingID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_FacilityBookingChargeID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_FacilityBookingChargeID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_DeliveryNotePosID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_DeliveryNotePosID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_DeliveryNoteID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_DeliveryNoteID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2StepItem_ACClassID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT [FK_TandTv2StepItem_ACClassID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2Step_TandTv2JobID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2Step]'))
ALTER TABLE [dbo].[TandTv2Step] DROP CONSTRAINT [FK_TandTv2Step_TandTv2JobID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2JobMaterial_TandTv2Job]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2JobMaterial]'))
ALTER TABLE [dbo].[TandTv2JobMaterial] DROP CONSTRAINT [FK_TandTv2JobMaterial_TandTv2Job]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2JobMaterial_Material]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2JobMaterial]'))
ALTER TABLE [dbo].[TandTv2JobMaterial] DROP CONSTRAINT [FK_TandTv2JobMaterial_Material]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2Job_TandTv2ItemTypeID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2Job]'))
ALTER TABLE [dbo].[TandTv2Job] DROP CONSTRAINT [FK_TandTv2Job_TandTv2ItemTypeID]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TandTv2_TrackingStyleID]') AND parent_object_id = OBJECT_ID(N'[dbo].[TandTv2Job]'))
ALTER TABLE [dbo].[TandTv2Job] DROP CONSTRAINT [FK_TandTv2_TrackingStyleID]
GO
/****** Object:  Table [dbo].[TandTv2TrackingStyle]    Script Date: 4.5.2023. 14:52:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2TrackingStyle]') AND type in (N'U'))
DROP TABLE [dbo].[TandTv2TrackingStyle]
GO
/****** Object:  Table [dbo].[TandTv2TempPos]    Script Date: 4.5.2023. 14:52:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2TempPos]') AND type in (N'U'))
DROP TABLE [dbo].[TandTv2TempPos]
GO
/****** Object:  Table [dbo].[TandTv2StepLot]    Script Date: 4.5.2023. 14:52:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepLot]') AND type in (N'U'))
DROP TABLE [dbo].[TandTv2StepLot]
GO
/****** Object:  Table [dbo].[TandTv2StepItemRelation]    Script Date: 4.5.2023. 14:52:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItemRelation]') AND type in (N'U'))
DROP TABLE [dbo].[TandTv2StepItemRelation]
GO
/****** Object:  Table [dbo].[TandTv2StepItem]    Script Date: 4.5.2023. 14:52:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
DROP TABLE [dbo].[TandTv2StepItem]
GO
/****** Object:  Table [dbo].[TandTv2Step]    Script Date: 4.5.2023. 14:52:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2Step]') AND type in (N'U'))
DROP TABLE [dbo].[TandTv2Step]
GO
/****** Object:  Table [dbo].[TandTv2RelationType]    Script Date: 4.5.2023. 14:52:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2RelationType]') AND type in (N'U'))
DROP TABLE [dbo].[TandTv2RelationType]
GO
/****** Object:  Table [dbo].[TandTv2Operation]    Script Date: 4.5.2023. 14:52:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2Operation]') AND type in (N'U'))
DROP TABLE [dbo].[TandTv2Operation]
GO
/****** Object:  Table [dbo].[TandTv2MaterialPosType]    Script Date: 4.5.2023. 14:52:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2MaterialPosType]') AND type in (N'U'))
DROP TABLE [dbo].[TandTv2MaterialPosType]
GO
/****** Object:  Table [dbo].[TandTv2JobMaterial]    Script Date: 4.5.2023. 14:52:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2JobMaterial]') AND type in (N'U'))
DROP TABLE [dbo].[TandTv2JobMaterial]
GO
/****** Object:  Table [dbo].[TandTv2Job]    Script Date: 4.5.2023. 14:52:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2Job]') AND type in (N'U'))
DROP TABLE [dbo].[TandTv2Job]
GO
/****** Object:  Table [dbo].[TandTv2ItemType]    Script Date: 4.5.2023. 14:52:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2ItemType]') AND type in (N'U'))
DROP TABLE [dbo].[TandTv2ItemType]