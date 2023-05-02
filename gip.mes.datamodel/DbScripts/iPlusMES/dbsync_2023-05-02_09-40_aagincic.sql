-- droping TandTv2 resources
/****** Object:  StoredProcedure [dbo].[udpTandTv2JobDelete]    Script Date: 2.5.2023. 9:39:24 ******/
DROP PROCEDURE IF EXISTS [dbo].[udpTandTv2JobDelete]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2TempPos]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2TempPos] DROP CONSTRAINT IF EXISTS [FK_TandTv2TempPos_TandTv2StepID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepLot]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepLot] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepLot_TandTv2StepID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItemRelation]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItemRelation] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItemRelation_TargetTandTv2StepItemID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItemRelation]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItemRelation] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItemRelation_TandTv2RelationTypeID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItemRelation]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItemRelation] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItemRelation_SourceTandTv2StepItemID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_TandTv2StepID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_TandTv2OperationID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_TandTv2ItemTypeID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_ProdOrderPartslistPosRelationID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_ProdOrderPartslistPosID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_ProdOrderPartslistID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_ProdOrderID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_OutOrderPosID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_OutOrderID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_InOrderPosID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_InOrderID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_FacilityPreBooking]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_FacilityLotID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_FacilityID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_FacilityChargeID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_FacilityBookingID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_FacilityBookingChargeID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_DeliveryNotePosID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_DeliveryNoteID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2StepItem]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2StepItem] DROP CONSTRAINT IF EXISTS [FK_TandTv2StepItem_ACClassID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2Step]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2Step] DROP CONSTRAINT IF EXISTS [FK_TandTv2Step_TandTv2JobID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2JobMaterial]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2JobMaterial] DROP CONSTRAINT IF EXISTS [FK_TandTv2JobMaterial_TandTv2Job]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2JobMaterial]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2JobMaterial] DROP CONSTRAINT IF EXISTS [FK_TandTv2JobMaterial_Material]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2Job]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2Job] DROP CONSTRAINT IF EXISTS [FK_TandTv2Job_TandTv2ItemTypeID]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TandTv2Job]') AND type in (N'U'))
ALTER TABLE [dbo].[TandTv2Job] DROP CONSTRAINT IF EXISTS [FK_TandTv2_TrackingStyleID]
GO
/****** Object:  Table [dbo].[TandTv2TrackingStyle]    Script Date: 2.5.2023. 9:39:25 ******/
DROP TABLE IF EXISTS [dbo].[TandTv2TrackingStyle]
GO
/****** Object:  Table [dbo].[TandTv2TempPos]    Script Date: 2.5.2023. 9:39:25 ******/
DROP TABLE IF EXISTS [dbo].[TandTv2TempPos]
GO
/****** Object:  Table [dbo].[TandTv2StepLot]    Script Date: 2.5.2023. 9:39:25 ******/
DROP TABLE IF EXISTS [dbo].[TandTv2StepLot]
GO
/****** Object:  Table [dbo].[TandTv2StepItemRelation]    Script Date: 2.5.2023. 9:39:25 ******/
DROP TABLE IF EXISTS [dbo].[TandTv2StepItemRelation]
GO
/****** Object:  Table [dbo].[TandTv2StepItem]    Script Date: 2.5.2023. 9:39:25 ******/
DROP TABLE IF EXISTS [dbo].[TandTv2StepItem]
GO
/****** Object:  Table [dbo].[TandTv2Step]    Script Date: 2.5.2023. 9:39:25 ******/
DROP TABLE IF EXISTS [dbo].[TandTv2Step]
GO
/****** Object:  Table [dbo].[TandTv2RelationType]    Script Date: 2.5.2023. 9:39:25 ******/
DROP TABLE IF EXISTS [dbo].[TandTv2RelationType]
GO
/****** Object:  Table [dbo].[TandTv2Operation]    Script Date: 2.5.2023. 9:39:25 ******/
DROP TABLE IF EXISTS [dbo].[TandTv2Operation]
GO
/****** Object:  Table [dbo].[TandTv2MaterialPosType]    Script Date: 2.5.2023. 9:39:25 ******/
DROP TABLE IF EXISTS [dbo].[TandTv2MaterialPosType]
GO
/****** Object:  Table [dbo].[TandTv2JobMaterial]    Script Date: 2.5.2023. 9:39:25 ******/
DROP TABLE IF EXISTS [dbo].[TandTv2JobMaterial]
GO
/****** Object:  Table [dbo].[TandTv2Job]    Script Date: 2.5.2023. 9:39:25 ******/
DROP TABLE IF EXISTS [dbo].[TandTv2Job]
GO
/****** Object:  Table [dbo].[TandTv2ItemType]    Script Date: 2.5.2023. 9:39:25 ******/
DROP TABLE IF EXISTS [dbo].[TandTv2ItemType]