ALTER TABLE dbo.OutOfferPos DROP CONSTRAINT FK_OutOfferPos_OutOfferID;
ALTER TABLE dbo.OutOfferPos DROP CONSTRAINT FK_OutOfferPos_MDUnitID;
ALTER TABLE dbo.OutOfferPos DROP CONSTRAINT FK_OutOfferPos_MDTimeRangeID;
ALTER TABLE dbo.OutOfferPos DROP CONSTRAINT FK_OutOfferPos_MDCountrySalesTaxID;
ALTER TABLE dbo.OutOfferPos DROP CONSTRAINT FK_OutOfferPos_MaterialID;
ALTER TABLE dbo.OutOfferPos DROP CONSTRAINT FK_OutOfferPos_ParentOutOfferPosID;
ALTER TABLE dbo.OutOfferPos DROP CONSTRAINT FK_OutOfferPos_GroupOutOfferPosID;

CREATE TABLE dbo.Tmp_OutOfferPos 
	( 
		OutOfferPosID uniqueidentifier NOT NULL,
		OutOfferID uniqueidentifier NOT NULL,
		ParentOutOfferPosID uniqueidentifier NULL,  
		GroupOutOfferPosID uniqueidentifier NULL,
		MDTimeRangeID uniqueidentifier NULL,
		MaterialPosTypeIndex smallint NOT NULL,
		
		MDCountrySalesTaxID uniqueidentifier NULL,  
		MDCountrySalesTaxMDMaterialGroupID uniqueidentifier NULL,
		MDCountrySalesTaxMaterialID uniqueidentifier NULL,
		
		Sequence int NOT NULL,
		MaterialID uniqueidentifier NOT NULL,
		MDUnitID uniqueidentifier NULL,
		TargetQuantityUOM float(53) NOT NULL,  
		TargetQuantity float(53) NOT NULL,
		PriceNet money NOT NULL,  
		PriceGross money NOT NULL,  
		SalesTax real null,

		TargetWeight float(53) NOT NULL,  
		TargetDeliveryDate datetime NOT NULL,  
		TargetDeliveryMaxDate datetime NULL,  
		TargetDeliveryPriority smallint NOT NULL,  
		GroupSum bit NOT NULL,
		Comment varchar(MAX) NULL,  
		Comment2 varchar(MAX) NULL,
		XMLDesign text NULL, 
		XMLConfig text NULL,

		InsertName varchar(20) NOT NULL,  
		InsertDate datetime NOT NULL,  
		UpdateName varchar(20) NOT NULL,  
		UpdateDate datetime NOT NULL
		 
		)  ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO
	INSERT INTO dbo.Tmp_OutOfferPos 
	(
		OutOfferPosID, 
		OutOfferID, 
		Sequence, 
		MaterialPosTypeIndex, 
		ParentOutOfferPosID, 
		GroupOutOfferPosID, 
		MaterialID, 
		TargetQuantityUOM, 
		MDUnitID, 
		TargetQuantity, 
		TargetWeight, 
		TargetDeliveryDate, 
		TargetDeliveryMaxDate, 
		TargetDeliveryPriority, 
		MDTimeRangeID, 
		PriceNet, 
		PriceGross, 
		MDCountrySalesTaxID, 
		Comment, 
		Comment2, 
		XMLConfig, 
		InsertName, 
		InsertDate, 
		UpdateName, 
		UpdateDate, 
		XMLDesign, 
		GroupSum
		) 
SELECT 
	OutOfferPosID, 
	OutOfferID, 
	Sequence, 
	MaterialPosTypeIndex, 
	ParentOutOfferPosID, 
	GroupOutOfferPosID, 
	MaterialID, 
	TargetQuantityUOM, 
	MDUnitID, 
	TargetQuantity, 
	TargetWeight, 
	TargetDeliveryDate, 
	TargetDeliveryMaxDate, 
	TargetDeliveryPriority, 
	MDTimeRangeID, 
	PriceNet, 
	PriceGross, 
	MDCountrySalesTaxID, 
	Comment, 
	Comment2, 
	XMLConfig, 
	InsertName, 
	InsertDate, 
	UpdateName, 
	UpdateDate, 
	XMLDesign, 
	GroupSum 
FROM dbo.OutOfferPos;

DROP TABLE dbo.OutOfferPos;
GO
EXECUTE sp_rename N'dbo.Tmp_OutOfferPos', N'OutOfferPos', 'OBJECT';
GO
	update dbo.OutOfferPos set SalesTax = 0;
	alter table dbo.OutOfferPos alter column SalesTax real not null;
GO
ALTER TABLE dbo.OutOfferPos ADD CONSTRAINT PK_OutOfferPos PRIMARY KEY CLUSTERED ( OutOfferPosID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ;
ALTER TABLE dbo.OutOfferPos ADD CONSTRAINT FK_OutOfferPos_MaterialID FOREIGN KEY ( MaterialID ) REFERENCES dbo.Material ( MaterialID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOfferPos ADD CONSTRAINT FK_OutOfferPos_MDCountrySalesTaxID FOREIGN KEY ( MDCountrySalesTaxID ) REFERENCES dbo.MDCountrySalesTax ( MDCountrySalesTaxID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOfferPos ADD CONSTRAINT FK_OutOfferPos_MDCountrySalesTaxMDMaterialGroupID FOREIGN KEY ( MDCountrySalesTaxMDMaterialGroupID ) REFERENCES dbo.MDCountrySalesTaxMDMaterialGroup ( MDCountrySalesTaxMDMaterialGroupID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOfferPos ADD CONSTRAINT FK_OutOfferPos_MDCountrySalesTaxMaterialID FOREIGN KEY ( MDCountrySalesTaxMaterialID ) REFERENCES dbo.MDCountrySalesTaxMaterial ( MDCountrySalesTaxMaterialID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOfferPos ADD CONSTRAINT FK_OutOfferPos_MDTimeRangeID FOREIGN KEY ( MDTimeRangeID ) REFERENCES dbo.MDTimeRange ( MDTimeRangeID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOfferPos ADD CONSTRAINT FK_OutOfferPos_MDUnitID FOREIGN KEY ( MDUnitID ) REFERENCES dbo.MDUnit ( MDUnitID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOfferPos ADD CONSTRAINT FK_OutOfferPos_OutOfferID FOREIGN KEY ( OutOfferID ) REFERENCES dbo.OutOffer ( OutOfferID ) ON UPDATE  NO ACTION ON DELETE  CASCADE ;
ALTER TABLE dbo.OutOfferPos ADD CONSTRAINT FK_OutOfferPos_ParentOutOfferPosID FOREIGN KEY ( ParentOutOfferPosID ) REFERENCES dbo.OutOfferPos ( OutOfferPosID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOfferPos ADD CONSTRAINT FK_OutOfferPos_GroupOutOfferPosID FOREIGN KEY ( GroupOutOfferPosID ) REFERENCES dbo.OutOfferPos ( OutOfferPosID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
GO
-- 3.0 Rebuild OutOrderPos
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_PickupCompanyMaterialID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_OutOrderID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_MDUnitID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_MDTransportModeID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_MDTourplanPosStateID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_MDToleranceStateID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_MDTimeRangeID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_MDOutOrderPosStateID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_MDOutOrderPlanStateID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_MDDelivPosStateID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_MDDelivPosLoadStateID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_MDCountrySalesTaxID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_MaterialID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_CompanyAddressUnloadingPointID
ALTER TABLE dbo.TandTv3MixPointOutOrderPos DROP CONSTRAINT FK_TandTv3MixPointOutOrderPos_OutOrderPosID
ALTER TABLE dbo.FacilityBookingCharge DROP CONSTRAINT FK_FacilityBookingCharge_OutOrderPosID
ALTER TABLE dbo.PickingPos DROP CONSTRAINT FK_PickingPos_OutOrderPosID
ALTER TABLE dbo.FacilityPreBooking DROP CONSTRAINT FK_FacilityPreBooking_OutOrderPosID
ALTER TABLE dbo.FacilityReservation DROP CONSTRAINT FK_FacilityReservation_OutOrderPosID
ALTER TABLE dbo.CompanyMaterialPickup DROP CONSTRAINT FK_CompanyMaterialPickup_OutOrderPosID
ALTER TABLE dbo.Weighing DROP CONSTRAINT FK_Weighing_OutOrderPosID
ALTER TABLE dbo.DeliveryNotePos DROP CONSTRAINT FK_DeliveryNotePos_OutOrderPosID
ALTER TABLE dbo.InvoicePos DROP CONSTRAINT FK_InvoicePos_OutOrderPosID
ALTER TABLE dbo.TandTv2StepItem DROP CONSTRAINT FK_TandTv2StepItem_OutOrderPosID
ALTER TABLE dbo.FacilityBooking DROP CONSTRAINT FK_FacilityBooking_OutOrderPosID
ALTER TABLE dbo.LabOrder DROP CONSTRAINT FK_LabOrder_OutOrderPosID
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_ParentOutOrderPosID
ALTER TABLE dbo.OutOrderPosSplit DROP CONSTRAINT FK_OutOrderPosSplit_OutOrderPosID
ALTER TABLE dbo.OutOrderPosUtilization DROP CONSTRAINT FK_OutOrderPosUtilization_OutOrderPosID

CREATE TABLE dbo.Tmp_OutOrderPos
	(
	OutOrderPosID uniqueidentifier NOT NULL,
	OutOrderID uniqueidentifier NOT NULL,
	ParentOutOrderPosID uniqueidentifier NULL,
	MDTimeRangeID uniqueidentifier NULL,
	MDOutOrderPosStateID uniqueidentifier NOT NULL,
	MDDelivPosStateID uniqueidentifier NOT NULL,
	MDDelivPosLoadStateID uniqueidentifier NULL,
	MDTransportModeID uniqueidentifier NULL,
	MDToleranceStateID uniqueidentifier NOT NULL,
	MDOutOrderPlanStateID uniqueidentifier NULL,
	MDTourplanPosStateID uniqueidentifier NULL,
	CompanyAddressUnloadingpointID uniqueidentifier NULL,
	PickupCompanyMaterialID uniqueidentifier NULL,
	LineNumber varchar(10) NOT NULL,
	MaterialPosTypeIndex smallint NOT NULL,

	MDCountrySalesTaxID uniqueidentifier NULL,
	MDCountrySalesTaxMDMaterialGroupID uniqueidentifier NULL,
	MDCountrySalesTaxMaterialID uniqueidentifier NULL,

	Sequence int NOT NULL,
	MaterialID uniqueidentifier NOT NULL,
	MDUnitID uniqueidentifier NULL,
	TargetQuantityUOM float(53) NOT NULL,
	TargetQuantity float(53) NOT NULL,
	PriceNet money NOT NULL,
	PriceGross money NOT NULL,
	SalesTax real NULL,

	ActualQuantityUOM float(53) NOT NULL,
	ActualQuantity float(53) NOT NULL,
	CalledUpQuantityUOM float(53) NOT NULL,
	CalledUpQuantity float(53) NOT NULL,
	ExternQuantityUOM float(53) NOT NULL,
	ExternQuantity float(53) NOT NULL,
	GroupSum bit NOT NULL,
	
	TargetDeliveryDate datetime NOT NULL,
	TargetDeliveryMaxDate datetime NULL,
	TargetDeliveryPriority smallint NOT NULL,
	TargetDeliveryDateConfirmed datetime NULL,

	Comment varchar(MAX) NULL,
	Comment2 varchar(MAX) NULL,
	XMLDesign text NULL,
	XMLConfig text NULL,
	
	InsertName varchar(20) NOT NULL,
	InsertDate datetime NOT NULL,
	UpdateName varchar(20) NOT NULL,
	UpdateDate datetime NOT NULL
	
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]

GO
INSERT INTO dbo.Tmp_OutOrderPos 
	(
		OutOrderPosID, 
		OutOrderID, 
		Sequence, 
		MaterialPosTypeIndex, 
		ParentOutOrderPosID, 
		MaterialID, 
		MDUnitID, 
		TargetQuantityUOM, 
		TargetQuantity, 
		ActualQuantityUOM, 
		ActualQuantity, 
		CalledUpQuantityUOM, 
		CalledUpQuantity, 
		ExternQuantityUOM, 
		ExternQuantity, 
		TargetDeliveryDate, 
		TargetDeliveryMaxDate, 
		TargetDeliveryPriority, 
		TargetDeliveryDateConfirmed, 
		MDTimeRangeID, 
		MDDelivPosStateID, 
		MDOutOrderPosStateID, 
		MDDelivPosLoadStateID, 
		PriceNet, 
		PriceGross, 
		MDCountrySalesTaxID, 
		MDToleranceStateID, 
		MDOutOrderPlanStateID, 
		MDTourplanPosStateID, 
		CompanyAddressUnloadingpointID, 
		Comment, 
		Comment2, 
		XMLConfig, 
		InsertName, 
		InsertDate, 
		UpdateName, 
		UpdateDate, 
		LineNumber, 
		PickupCompanyMaterialID, 
		MDTransportModeID, 
		XMLDesign, 
		GroupSum)
		SELECT 
			OutOrderPosID, 
			OutOrderID, 
			Sequence, 
			MaterialPosTypeIndex, 
			ParentOutOrderPosID, 
			MaterialID, 
			MDUnitID, 
			TargetQuantityUOM, 
			TargetQuantity, 
			ActualQuantityUOM, 
			ActualQuantity, 
			CalledUpQuantityUOM, 
			CalledUpQuantity, 
			ExternQuantityUOM, 
			ExternQuantity, 
			TargetDeliveryDate, 
			TargetDeliveryMaxDate, 
			TargetDeliveryPriority, 
			TargetDeliveryDateConfirmed, 
			MDTimeRangeID, 
			MDDelivPosStateID, 
			MDOutOrderPosStateID, 
			MDDelivPosLoadStateID, 
			PriceNet, 
			PriceGross, 
			MDCountrySalesTaxID, 
			MDToleranceStateID, 
			MDOutOrderPlanStateID, 
			MDTourplanPosStateID, 
			CompanyAddressUnloadingpointID, 
			Comment, 
			Comment2, 
			XMLConfig, 
			InsertName, 
			InsertDate, 
			UpdateName, 
			UpdateDate, 
			LineNumber, 
			PickupCompanyMaterialID, 
			MDTransportModeID, 
			XMLDesign, 
			GroupSum 
FROM dbo.OutOrderPos
DROP TABLE dbo.OutOrderPos;
GO
EXECUTE sp_rename N'dbo.Tmp_OutOrderPos', N'OutOrderPos', 'OBJECT' 
GO
	update dbo.OutOrderPos set SalesTax = 0;
	alter table dbo.OutOrderPos alter column SalesTax real not null;
GO
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT PK_OutOrderPos PRIMARY KEY CLUSTERED (OutOrderPosID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_CompanyAddressUnloadingpointID ON dbo.OutOrderPos (CompanyAddressUnloadingpointID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_MaterialID ON dbo.OutOrderPos (MaterialID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_MDCountrySalesTaxID ON dbo.OutOrderPos (MDCountrySalesTaxID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_MDDelivPosLoadStateID ON dbo.OutOrderPos (MDDelivPosLoadStateID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_MDDelivPosStateID ON dbo.OutOrderPos (MDDelivPosStateID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_MDOutOrderPlanStateID ON dbo.OutOrderPos (MDOutOrderPlanStateID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_MDOutOrderPosStateID ON dbo.OutOrderPos (MDOutOrderPosStateID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_MDTimeRangeID ON dbo.OutOrderPos (MDTimeRangeID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_MDToleranceStateID ON dbo.OutOrderPos (MDToleranceStateID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_MDTourplanPosStateID ON dbo.OutOrderPos (MDTourplanPosStateID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_MDTransportModeID ON dbo.OutOrderPos (MDTransportModeID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_MDUnitID ON dbo.OutOrderPos (MDUnitID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_OutOrderID ON dbo.OutOrderPos (OutOrderID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_ParentOutOrderPosID ON dbo.OutOrderPos (ParentOutOrderPosID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE NONCLUSTERED INDEX NCI_FK_OutOrderPos_PickupCompanyMaterialID ON dbo.OutOrderPos (PickupCompanyMaterialID) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
CREATE UNIQUE NONCLUSTERED INDEX UIX_OutOrderPos ON dbo.OutOrderPos (OutOrderPosID,Sequence) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_CompanyAddressUnloadingPointID FOREIGN KEY (CompanyAddressUnloadingpointID) REFERENCES dbo.CompanyAddressUnloadingpoint (CompanyAddressUnloadingpointID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_MaterialID FOREIGN KEY (MaterialID) REFERENCES dbo.Material (MaterialID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_MDCountrySalesTaxID FOREIGN KEY (MDCountrySalesTaxID) REFERENCES dbo.MDCountrySalesTax (MDCountrySalesTaxID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_MDCountrySalesTaxMDMaterialGroupID FOREIGN KEY (MDCountrySalesTaxMDMaterialGroupID) REFERENCES dbo.MDCountrySalesTaxMDMaterialGroup (MDCountrySalesTaxMDMaterialGroupID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_MDCountrySalesTaxMaterialID FOREIGN KEY (MDCountrySalesTaxMaterialID) REFERENCES dbo.MDCountrySalesTaxMaterial (MDCountrySalesTaxMaterialID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_MDDelivPosLoadStateID FOREIGN KEY (MDDelivPosLoadStateID) REFERENCES dbo.MDDelivPosLoadState (MDDelivPosLoadStateID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_MDDelivPosStateID FOREIGN KEY (MDDelivPosStateID) REFERENCES dbo.MDDelivPosState (MDDelivPosStateID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_MDOutOrderPlanStateID FOREIGN KEY (MDOutOrderPlanStateID) REFERENCES dbo.MDOutOrderPlanState (MDOutOrderPlanStateID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_MDOutOrderPosStateID FOREIGN KEY (MDOutOrderPosStateID) REFERENCES dbo.MDOutOrderPosState (MDOutOrderPosStateID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_MDTimeRangeID FOREIGN KEY (MDTimeRangeID) REFERENCES dbo.MDTimeRange (MDTimeRangeID) ON UPDATE  NO ACTION ON DELETE  NO ACTION
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_MDToleranceStateID FOREIGN KEY (MDToleranceStateID) REFERENCES dbo.MDToleranceState (MDToleranceStateID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_MDTourplanPosStateID FOREIGN KEY (MDTourplanPosStateID) REFERENCES dbo.MDTourplanPosState (MDTourplanPosStateID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_MDTransportModeID FOREIGN KEY (MDTransportModeID) REFERENCES dbo.MDTransportMode (MDTransportModeID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_MDUnitID FOREIGN KEY (MDUnitID) REFERENCES dbo.MDUnit (MDUnitID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_OutOrderID FOREIGN KEY (OutOrderID) REFERENCES dbo.OutOrder (OutOrderID) ON UPDATE  NO ACTION ON DELETE  CASCADE 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_ParentOutOrderPosID FOREIGN KEY (ParentOutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_PickupCompanyMaterialID FOREIGN KEY (PickupCompanyMaterialID) REFERENCES dbo.CompanyMaterial (CompanyMaterialID) ON UPDATE  NO ACTION ON DELETE  NO ACTION
ALTER TABLE dbo.OutOrderPosUtilization ADD CONSTRAINT FK_OutOrderPosUtilization_OutOrderPosID FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.OutOrderPosSplit ADD CONSTRAINT FK_OutOrderPosSplit_OutOrderPosID FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  CASCADE 
ALTER TABLE dbo.LabOrder ADD CONSTRAINT FK_LabOrder_OutOrderPosID FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION 
ALTER TABLE dbo.FacilityBooking ADD CONSTRAINT FK_FacilityBooking_OutOrderPosID FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.TandTv2StepItem ADD CONSTRAINT FK_TandTv2StepItem_OutOrderPosID FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.InvoicePos ADD CONSTRAINT FK_InvoicePos_OutOrderPos FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.DeliveryNotePos ADD CONSTRAINT FK_DeliveryNotePos_OutOrderPosID FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.Weighing ADD CONSTRAINT FK_Weighing_OutOrderPosID FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.CompanyMaterialPickup ADD CONSTRAINT FK_CompanyMaterialPickup_OutOrderPosID FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.FacilityReservation ADD CONSTRAINT FK_FacilityReservation_OutOrderPosID FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.FacilityPreBooking ADD CONSTRAINT FK_FacilityPreBooking_OutOrderPosID FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.PickingPos ADD CONSTRAINT FK_PickingPos_OutOrderPosID FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.FacilityBookingCharge ADD CONSTRAINT FK_FacilityBookingCharge_OutOrderPosID FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
ALTER TABLE dbo.TandTv3MixPointOutOrderPos ADD CONSTRAINT FK_TandTv3MixPointOutOrderPos_OutOrderPosID FOREIGN KEY (OutOrderPosID) REFERENCES dbo.OutOrderPos (OutOrderPosID) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
GO

-- 3.0 Rebuild InvoicePos
ALTER TABLE dbo.InvoicePos DROP CONSTRAINT FK_InvoicePos_OutOrderPos;
ALTER TABLE dbo.InvoicePos DROP CONSTRAINT FK_InvoicePos_MDCountrySalesTaxID
ALTER TABLE dbo.InvoicePos DROP CONSTRAINT FK_InvoicePos_MDCountrySalesTaxMDMaterialGroupID;
ALTER TABLE dbo.InvoicePos DROP CONSTRAINT FK_InvoicePos_MDCountrySalesTaxMaterialID;
ALTER TABLE dbo.InvoicePos DROP CONSTRAINT FK_InvoicePos_MDUnitID;
ALTER TABLE dbo.InvoicePos DROP CONSTRAINT FK_InvoicePos_MaterialID;
ALTER TABLE dbo.InvoicePos DROP CONSTRAINT FK_InvoicePos_InvoiceID;	

CREATE TABLE dbo.Tmp_InvoicePos 
( 
	InvoicePosID uniqueidentifier NOT NULL,  
	InvoiceID uniqueidentifier NOT NULL,  
	OutOrderPosID uniqueidentifier NULL, 

	MDCountrySalesTaxID uniqueidentifier NULL,
	MDCountrySalesTaxMDMaterialGroupID uniqueidentifier NULL,  
	MDCountrySalesTaxMaterialID uniqueidentifier NULL, 

	Sequence int NOT NULL,  
	MaterialID uniqueidentifier NOT NULL,  
	MDUnitID uniqueidentifier NULL,  
	TargetQuantityUOM float(53) NOT NULL,  
	TargetQuantity float(53) NOT NULL,  
	PriceNet money NOT NULL,  
	PriceGross money NOT NULL,  
	SalesTax real NULL,

	Comment varchar(MAX) NULL,
	XMLDesign text NULL,
	XMLConfig text NULL,

	InsertName varchar(20) NOT NULL,  
	InsertDate datetime NOT NULL,  
	UpdateName varchar(20) NOT NULL,  
	UpdateDate datetime NOT NULL
	
	)  
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

INSERT INTO dbo.Tmp_InvoicePos 
	(
		InvoicePosID, 
		InvoiceID, 
		Sequence, 
		MaterialID, 
		MDUnitID, 
		TargetQuantityUOM, 
		TargetQuantity, 
		MDCountrySalesTaxMDMaterialGroupID, 
		MDCountrySalesTaxMaterialID, 
		PriceNet, 
		PriceGross, 
		SalesTax, 
		OutOrderPosID, 
		Comment, 
		XMLConfig, 
		InsertName, 
		InsertDate, 
		UpdateName, 
		UpdateDate, 
		XMLDesign
	) 
SELECT 
	InvoicePosID, 
	InvoiceID, 
	Sequence, 
	MaterialID, 
	MDUnitID, 
	TargetQuantityUOM, 
	TargetQuantity, 
	MDCountrySalesTaxMDMaterialGroupID, 
	MDCountrySalesTaxMaterialID, 
	PriceNet, 
	PriceGross, 
	SalesTax, 
	OutOrderPosID, 
	Comment, 
	XMLConfig, 
	InsertName, 
	InsertDate, 
	UpdateName, 
	UpdateDate, 
	XMLDesign 
FROM dbo.InvoicePos
DROP TABLE dbo.InvoicePos;
GO
EXECUTE sp_rename N'dbo.Tmp_InvoicePos', N'InvoicePos', 'OBJECT';
GO
	update dbo.InvoicePos set SalesTax = 0;
	alter table dbo.InvoicePos alter column SalesTax real not null;
GO
ALTER TABLE dbo.InvoicePos ADD CONSTRAINT PK_InvoicePos PRIMARY KEY CLUSTERED ( InvoicePosID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ;
ALTER TABLE dbo.InvoicePos ADD CONSTRAINT FK_InvoicePos_InvoiceID FOREIGN KEY ( InvoiceID ) REFERENCES dbo.Invoice ( InvoiceID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InvoicePos ADD CONSTRAINT FK_InvoicePos_MaterialID FOREIGN KEY ( MaterialID ) REFERENCES dbo.Material ( MaterialID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InvoicePos ADD CONSTRAINT FK_InvoicePos_MDUnitID FOREIGN KEY ( MDUnitID ) REFERENCES dbo.MDUnit ( MDUnitID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InvoicePos ADD CONSTRAINT FK_InvoicePos_MDCountrySalesTaxID FOREIGN KEY ( MDCountrySalesTaxID ) REFERENCES dbo.MDCountrySalesTax ( MDCountrySalesTaxID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InvoicePos ADD CONSTRAINT FK_InvoicePos_MDCountrySalesTaxMaterialID FOREIGN KEY ( MDCountrySalesTaxMaterialID ) REFERENCES dbo.MDCountrySalesTaxMaterial ( MDCountrySalesTaxMaterialID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InvoicePos ADD CONSTRAINT FK_InvoicePos_MDCountrySalesTaxMDMaterialGroupID FOREIGN KEY ( MDCountrySalesTaxMDMaterialGroupID ) REFERENCES dbo.MDCountrySalesTaxMDMaterialGroup ( MDCountrySalesTaxMDMaterialGroupID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InvoicePos ADD CONSTRAINT FK_InvoicePos_OutOrderPosID FOREIGN KEY ( OutOrderPosID ) REFERENCES dbo.OutOrderPos ( OutOrderPosID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION 