-- author:		@aagincic
-- name:		udpACClassDeleteVB
-- desc:		delete class and resources
-- created:		2020--06-03
-- updated:		2020--06-03
-- deployed:	-

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'udpACClassDeleteVB')
	BEGIN
		DROP  procedure  dbo.[udpACClassDeleteVB]
	END
GO

CREATE PROCEDURE [dbo].[udpACClassDeleteVB]
				
	@acClassID uniqueidentifier
	AS
begin
	begin tran

		--=======================================================================================
		--| TABLE_NAME                    | COLUMN_NAME                           | IS_NULLABLE |
		--=======================================================================================
		--| CompanyPersonRole             | VBiRoleACClassID                      | NO          |
		-----------------------------------------------------------------------------------------
		--| DemandOrderPos                | VBiProgramACClassMethodID             | NO          |
		-----------------------------------------------------------------------------------------
		--| Facility                      | VBiFacilityACClassID                  | YES         |
		-----------------------------------------------------------------------------------------
		--| Facility                      | VBiStackCalculatorACClassID           | YES         |
		-----------------------------------------------------------------------------------------
		--| FacilityBooking               | VBiStackCalculatorACClassID           | YES         |
		-----------------------------------------------------------------------------------------
		--| FacilityBookingCharge         | VBiStackCalculatorACClassID           | YES         |
		-----------------------------------------------------------------------------------------
		--| FacilityReservation           | VBiACClassID                          | YES         |
		-----------------------------------------------------------------------------------------
		--| HistoryConfig                 | VBiACClassID                          | YES         |
		-----------------------------------------------------------------------------------------
		--| HistoryConfig                 | VBiACClassPropertyRelationID          | YES         |
		-----------------------------------------------------------------------------------------
		--| HistoryConfig                 | VBiACClassWFID                        | YES         |
		-----------------------------------------------------------------------------------------
		--| HistoryConfig                 | VBiValueTypeACClassID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| InOrderConfig                 | VBiACClassID                          | YES         |
		-----------------------------------------------------------------------------------------
		--| InOrderConfig                 | VBiACClassPropertyRelationID          | YES         |
		-----------------------------------------------------------------------------------------
		--| InOrderConfig                 | VBiACClassWFID                        | YES         |
		-----------------------------------------------------------------------------------------
		--| InOrderConfig                 | VBiValueTypeACClassID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| InRequestConfig               | VBiACClassID                          | YES         |
		-----------------------------------------------------------------------------------------
		--| InRequestConfig               | VBiACClassPropertyRelationID          | YES         |
		-----------------------------------------------------------------------------------------
		--| InRequestConfig               | VBiValueTypeACClassID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| MachineMaterialPosView        | ACClassID                             | NO          |
		-----------------------------------------------------------------------------------------
		--| MachineMaterialPosView        | BasedOnACClassID                      | NO          |
		-----------------------------------------------------------------------------------------
		--| MachineMaterialRelView        | ACClassID                             | NO          |
		-----------------------------------------------------------------------------------------
		--| MachineMaterialRelView        | BasedOnACClassID                      | NO          |
		-----------------------------------------------------------------------------------------
		--| MachineMaterialView           | ACClassID                             | NO          |
		-----------------------------------------------------------------------------------------
		--| MachineMaterialView           | BasedOnACClassID                      | NO          |
		-----------------------------------------------------------------------------------------
		--| MaintACClass                  | MaintACClassID                        | NO          |
		-----------------------------------------------------------------------------------------
		--| MaintACClass                  | VBiACClassID                          | NO          |
		-----------------------------------------------------------------------------------------
		--| MaintACClassProperty          | MaintACClassID                        | NO          |
		-----------------------------------------------------------------------------------------
		--| MaintACClassProperty          | MaintACClassPropertyID                | NO          |
		-----------------------------------------------------------------------------------------
		--| MaintACClassProperty          | VBiACClassPropertyID                  | NO          |
		-----------------------------------------------------------------------------------------
		--| MaintACClassVBGroup           | MaintACClassID                        | YES         |
		-----------------------------------------------------------------------------------------
		--| MaintACClassVBGroup           | MaintACClassPropertyID                | YES         |
		-----------------------------------------------------------------------------------------
		--| MaintACClassVBGroup           | MaintACClassVBGroupID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| MaintOrder                    | MaintACClassID                        | NO          |
		-----------------------------------------------------------------------------------------
		--| MaintOrder                    | VBiPAACClassID                        | NO          |
		-----------------------------------------------------------------------------------------
		--| MaintOrderProperty            | MaintACClassPropertyID                | NO          |
		-----------------------------------------------------------------------------------------
		--| MaintTask                     | MaintACClassVBGroupID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| Material                      | VBiProgramACClassMethodID             | YES         |
		-----------------------------------------------------------------------------------------
		--| Material                      | VBiStackCalculatorACClassID           | YES         |
		-----------------------------------------------------------------------------------------
		--| MaterialConfig                | VBiACClassID                          | YES         |
		-----------------------------------------------------------------------------------------
		--| MaterialConfig                | VBiACClassPropertyRelationID          | YES         |
		-----------------------------------------------------------------------------------------
		--| MaterialConfig                | VBiACClassWFID                        | YES         |
		-----------------------------------------------------------------------------------------
		--| MaterialConfig                | VBiValueTypeACClassID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| MaterialWFACClassMethod       | ACClassMethodID                       | NO          |
		-----------------------------------------------------------------------------------------
		--| MaterialWFACClassMethod       | MaterialWFACClassMethodID             | NO          |
		-----------------------------------------------------------------------------------------
		--| MaterialWFACClassMethodConfig | MaterialWFACClassMethodConfigID       | NO          |
		-----------------------------------------------------------------------------------------
		--| MaterialWFACClassMethodConfig | MaterialWFACClassMethodID             | YES         |
		-----------------------------------------------------------------------------------------
		--| MaterialWFACClassMethodConfig | ParentMaterialWFACClassMethodConfigID | YES         |
		-----------------------------------------------------------------------------------------
		--| MaterialWFACClassMethodConfig | VBiACClassID                          | YES         |
		-----------------------------------------------------------------------------------------
		--| MaterialWFACClassMethodConfig | VBiACClassPropertyRelationID          | YES         |
		-----------------------------------------------------------------------------------------
		--| MaterialWFACClassMethodConfig | VBiACClassWFID                        | YES         |
		-----------------------------------------------------------------------------------------
		--| MaterialWFACClassMethodConfig | VBiValueTypeACClassID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| MaterialWFConnection          | ACClassWFID                           | NO          |
		-----------------------------------------------------------------------------------------
		--| MaterialWFConnection          | MaterialWFACClassMethodID             | NO          |
		-----------------------------------------------------------------------------------------
		--| MsgAlarmLog                   | ACClassID                             | YES         |
		-----------------------------------------------------------------------------------------
		--| OrderLogPosMachines           | ACClassID                             | NO          |
		-----------------------------------------------------------------------------------------
		--| OrderLogPosMachines           | BasedOnACClassID                      | YES         |
		-----------------------------------------------------------------------------------------
		--| OrderLogRelView               | ACClassID                             | NO          |
		-----------------------------------------------------------------------------------------
		--| OrderLogRelView               | BasedOnACClassID                      | YES         |
		-----------------------------------------------------------------------------------------
		--| OrderLogRelView               | RefACClassID                          | YES         |
		-----------------------------------------------------------------------------------------
		--| OutOfferingConfig             | VBiACClassID                          | YES         |
		-----------------------------------------------------------------------------------------
		--| OutOfferingConfig             | VBiACClassPropertyRelationID          | YES         |
		-----------------------------------------------------------------------------------------
		--| OutOfferingConfig             | VBiValueTypeACClassID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| OutOrderConfig                | VBiACClassID                          | YES         |
		-----------------------------------------------------------------------------------------
		--| OutOrderConfig                | VBiACClassPropertyRelationID          | YES         |
		-----------------------------------------------------------------------------------------
		--| OutOrderConfig                | VBiACClassWFID                        | YES         |
		-----------------------------------------------------------------------------------------
		--| OutOrderConfig                | VBiValueTypeACClassID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| PartslistACClassMethod        | MaterialWFACClassMethodID             | NO          |
		-----------------------------------------------------------------------------------------
		--| PartslistACClassMethod        | PartslistACClassMethodID              | NO          |
		-----------------------------------------------------------------------------------------
		--| PartslistConfig               | VBiACClassID                          | YES         |
		-----------------------------------------------------------------------------------------
		--| PartslistConfig               | VBiACClassPropertyRelationID          | YES         |
		-----------------------------------------------------------------------------------------
		--| PartslistConfig               | VBiACClassWFID                        | YES         |
		-----------------------------------------------------------------------------------------
		--| PartslistConfig               | VBiValueTypeACClassID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| Picking                       | ACClassMethodID                       | YES         |
		-----------------------------------------------------------------------------------------
		--| PickingConfig                 | VBiACClassID                          | YES         |
		-----------------------------------------------------------------------------------------
		--| PickingConfig                 | VBiACClassPropertyRelationID          | YES         |
		-----------------------------------------------------------------------------------------
		--| PickingConfig                 | VBiACClassWFID                        | YES         |
		-----------------------------------------------------------------------------------------
		--| PickingConfig                 | VBiValueTypeACClassID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| ProdOrderBatchPlan            | MaterialWFACClassMethodID             | YES         |
		-----------------------------------------------------------------------------------------
		--| ProdOrderBatchPlan            | VBiACClassWFID                        | YES         |
		-----------------------------------------------------------------------------------------
		--| ProdOrderPartslistConfig      | VBiACClassID                          | YES         |
		-----------------------------------------------------------------------------------------
		--| ProdOrderPartslistConfig      | VBiACClassPropertyRelationID          | YES         |
		-----------------------------------------------------------------------------------------
		--| ProdOrderPartslistConfig      | VBiACClassWFID                        | YES         |
		-----------------------------------------------------------------------------------------
		--| ProdOrderPartslistConfig      | VBiValueTypeACClassID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| ProdOrderPartslistPos         | ACClassTaskID                         | YES         |
		-----------------------------------------------------------------------------------------
		--| TandTv2StepItem               | ACClassID                             | YES         |
		-----------------------------------------------------------------------------------------
		--| TourplanConfig                | VBiACClassID                          | YES         |
		-----------------------------------------------------------------------------------------
		--| TourplanConfig                | VBiACClassPropertyRelationID          | YES         |
		-----------------------------------------------------------------------------------------
		--| TourplanConfig                | VBiValueTypeACClassID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| VBConfig                      | ACClassID                             | YES         |
		-----------------------------------------------------------------------------------------
		--| VBConfig                      | ACClassPropertyRelationID             | YES         |
		-----------------------------------------------------------------------------------------
		--| VBConfig                      | ValueTypeACClassID                    | NO          |
		-----------------------------------------------------------------------------------------
		--| VBGroupRight                  | ACClassDesignID                       | YES         |
		-----------------------------------------------------------------------------------------
		--| VBGroupRight                  | ACClassID                             | NO          |
		-----------------------------------------------------------------------------------------
		--| VBGroupRight                  | ACClassMethodID                       | YES         |
		-----------------------------------------------------------------------------------------
		--| VBGroupRight                  | ACClassPropertyID                     | YES         |
		-----------------------------------------------------------------------------------------
		--| VBUser                        | MenuACClassDesignID                   | YES         |
		-----------------------------------------------------------------------------------------
		--| VBUserACClassDesign           | ACClassDesignID                       | YES         |
		-----------------------------------------------------------------------------------------
		--| VBUserACClassDesign           | VBUserACClassDesignID                 | NO          |
		-----------------------------------------------------------------------------------------
		--| Weighing                      | VBiACClassID                          | YES         |
		-----------------------------------------------------------------------------------------

		delete from CompanyPersonRole where VBiRoleACClassID = @acClassID;
		delete from DemandOrderPos where VBiProgramACClassMethodID = @acClassID;
		update Facility set VBiFacilityACClassID = null where VBiFacilityACClassID = @acClassID;
		update Facility set VBiStackCalculatorACClassID = null where VBiStackCalculatorACClassID = @acClassID;
		update FacilityBooking set VBiStackCalculatorACClassID = null where VBiStackCalculatorACClassID = @acClassID;
		update FacilityBookingCharge set VBiStackCalculatorACClassID = null where VBiStackCalculatorACClassID = @acClassID;
		update FacilityReservation set VBiACClassID = null where VBiACClassID = @acClassID;
		update HistoryConfig set VBiACClassID = null where VBiACClassID = @acClassID;
		update HistoryConfig set VBiACClassPropertyRelationID = null where VBiACClassPropertyRelationID = @acClassID;
		update HistoryConfig set VBiACClassWFID = null where VBiACClassWFID = @acClassID;
		delete from HistoryConfig where VBiValueTypeACClassID = @acClassID;
		update InOrderConfig set VBiACClassID = null where VBiACClassID = @acClassID;
		update InOrderConfig set VBiACClassPropertyRelationID = null where VBiACClassPropertyRelationID = @acClassID;
		update InOrderConfig set VBiACClassWFID = null where VBiACClassWFID = @acClassID;
		delete from InOrderConfig where VBiValueTypeACClassID = @acClassID;
		update InRequestConfig set VBiACClassID = null where VBiACClassID = @acClassID;
		update InRequestConfig set VBiACClassPropertyRelationID = null where VBiACClassPropertyRelationID = @acClassID;
		delete from InRequestConfig where VBiValueTypeACClassID = @acClassID;
		delete from MachineMaterialPosView where ACClassID = @acClassID;
		delete from MachineMaterialPosView where BasedOnACClassID = @acClassID;
		delete from MachineMaterialRelView where ACClassID = @acClassID;
		delete from MachineMaterialRelView where BasedOnACClassID = @acClassID;
		delete from MachineMaterialView where ACClassID = @acClassID;
		delete from MachineMaterialView where BasedOnACClassID = @acClassID;
		delete from MaintACClass where MaintACClassID = @acClassID;
		delete from MaintACClass where VBiACClassID = @acClassID;
		delete from MaintACClassProperty where MaintACClassID = @acClassID;
		delete from MaintACClassProperty where MaintACClassPropertyID = @acClassID;
		delete from MaintACClassProperty where VBiACClassPropertyID = @acClassID;
		update MaintACClassVBGroup set MaintACClassID = null where MaintACClassID = @acClassID;
		update MaintACClassVBGroup set MaintACClassPropertyID = null where MaintACClassPropertyID = @acClassID;
		delete from MaintACClassVBGroup where MaintACClassVBGroupID = @acClassID;
		delete from MaintOrder where MaintACClassID = @acClassID;
		delete from MaintOrder where VBiPAACClassID = @acClassID;
		delete from MaintOrderProperty where MaintACClassPropertyID = @acClassID;
		delete from MaintTask where MaintACClassVBGroupID = @acClassID;
		update Material set VBiProgramACClassMethodID = null where VBiProgramACClassMethodID = @acClassID;
		update Material set VBiStackCalculatorACClassID = null where VBiStackCalculatorACClassID = @acClassID;
		update MaterialConfig set VBiACClassID = null where VBiACClassID = @acClassID;
		update MaterialConfig set VBiACClassPropertyRelationID = null where VBiACClassPropertyRelationID = @acClassID;
		update MaterialConfig set VBiACClassWFID = null where VBiACClassWFID = @acClassID;
		delete from MaterialConfig where VBiValueTypeACClassID = @acClassID;
		delete from MaterialWFACClassMethod where ACClassMethodID = @acClassID;
		delete from MaterialWFACClassMethod where MaterialWFACClassMethodID = @acClassID;
		delete from MaterialWFACClassMethodConfig where MaterialWFACClassMethodConfigID = @acClassID;
		update MaterialWFACClassMethodConfig set MaterialWFACClassMethodID = null where MaterialWFACClassMethodID = @acClassID;
		update MaterialWFACClassMethodConfig set ParentMaterialWFACClassMethodConfigID = null where ParentMaterialWFACClassMethodConfigID = @acClassID;
		update MaterialWFACClassMethodConfig set VBiACClassID = null where VBiACClassID = @acClassID;
		update MaterialWFACClassMethodConfig set VBiACClassPropertyRelationID = null where VBiACClassPropertyRelationID = @acClassID;
		update MaterialWFACClassMethodConfig set VBiACClassWFID = null where VBiACClassWFID = @acClassID;
		delete from MaterialWFACClassMethodConfig where VBiValueTypeACClassID = @acClassID;
		delete from MaterialWFConnection where ACClassWFID = @acClassID;
		delete from MaterialWFConnection where MaterialWFACClassMethodID = @acClassID;
		update MsgAlarmLog set ACClassID = null where ACClassID = @acClassID;
		delete from OrderLogPosMachines where ACClassID = @acClassID;
		update OrderLogPosMachines set BasedOnACClassID = null where BasedOnACClassID = @acClassID;
		delete from OrderLogRelView where ACClassID = @acClassID;
		update OrderLogRelView set BasedOnACClassID = null where BasedOnACClassID = @acClassID;
		update OrderLogRelView set RefACClassID = null where RefACClassID = @acClassID;
		update OutOfferingConfig set VBiACClassID = null where VBiACClassID = @acClassID;
		update OutOfferingConfig set VBiACClassPropertyRelationID = null where VBiACClassPropertyRelationID = @acClassID;
		delete from OutOfferingConfig where VBiValueTypeACClassID = @acClassID;
		update OutOrderConfig set VBiACClassID = null where VBiACClassID = @acClassID;
		update OutOrderConfig set VBiACClassPropertyRelationID = null where VBiACClassPropertyRelationID = @acClassID;
		update OutOrderConfig set VBiACClassWFID = null where VBiACClassWFID = @acClassID;
		delete from OutOrderConfig where VBiValueTypeACClassID = @acClassID;
		delete from PartslistACClassMethod where MaterialWFACClassMethodID = @acClassID;
		delete from PartslistACClassMethod where PartslistACClassMethodID = @acClassID;
		update PartslistConfig set VBiACClassID = null where VBiACClassID = @acClassID;
		update PartslistConfig set VBiACClassPropertyRelationID = null where VBiACClassPropertyRelationID = @acClassID;
		update PartslistConfig set VBiACClassWFID = null where VBiACClassWFID = @acClassID;
		delete from PartslistConfig where VBiValueTypeACClassID = @acClassID;
		update Picking set ACClassMethodID = null where ACClassMethodID = @acClassID;
		update PickingConfig set VBiACClassID = null where VBiACClassID = @acClassID;
		update PickingConfig set VBiACClassPropertyRelationID = null where VBiACClassPropertyRelationID = @acClassID;
		update PickingConfig set VBiACClassWFID = null where VBiACClassWFID = @acClassID;
		delete from PickingConfig where VBiValueTypeACClassID = @acClassID;
		update ProdOrderBatchPlan set MaterialWFACClassMethodID = null where MaterialWFACClassMethodID = @acClassID;
		update ProdOrderBatchPlan set VBiACClassWFID = null where VBiACClassWFID = @acClassID;
		update ProdOrderPartslistConfig set VBiACClassID = null where VBiACClassID = @acClassID;
		update ProdOrderPartslistConfig set VBiACClassPropertyRelationID = null where VBiACClassPropertyRelationID = @acClassID;
		update ProdOrderPartslistConfig set VBiACClassWFID = null where VBiACClassWFID = @acClassID;
		delete from ProdOrderPartslistConfig where VBiValueTypeACClassID = @acClassID;
		update ProdOrderPartslistPos set ACClassTaskID = null where ACClassTaskID = @acClassID;
		update TandTv2StepItem set ACClassID = null where ACClassID = @acClassID;
		update TourplanConfig set VBiACClassID = null where VBiACClassID = @acClassID;
		update TourplanConfig set VBiACClassPropertyRelationID = null where VBiACClassPropertyRelationID = @acClassID;
		delete from TourplanConfig where VBiValueTypeACClassID = @acClassID;
		update VBConfig set ACClassID = null where ACClassID = @acClassID;
		update VBConfig set ACClassPropertyRelationID = null where ACClassPropertyRelationID = @acClassID;
		delete from VBConfig where ValueTypeACClassID = @acClassID;
		update VBGroupRight set ACClassDesignID = null where ACClassDesignID = @acClassID;
		delete from VBGroupRight where ACClassID = @acClassID;
		update VBGroupRight set ACClassMethodID = null where ACClassMethodID = @acClassID;
		update VBGroupRight set ACClassPropertyID = null where ACClassPropertyID = @acClassID;
		update VBUser set MenuACClassDesignID = null where MenuACClassDesignID = @acClassID;
		update VBUserACClassDesign set ACClassDesignID = null where ACClassDesignID = @acClassID;
		delete from VBUserACClassDesign where VBUserACClassDesignID = @acClassID;
		update Weighing set VBiACClassID = null where VBiACClassID = @acClassID;

	commit tran;
end