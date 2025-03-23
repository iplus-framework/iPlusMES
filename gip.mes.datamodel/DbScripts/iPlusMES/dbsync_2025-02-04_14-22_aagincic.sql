declare @pausedID uniqueidentifier;
declare @finishedID uniqueidentifier;

set @pausedID = (select top 1 MDFacilityInventoryPosStateID from MDFacilityInventoryPosState where MDFacilityInventoryPosStateIndex = 3);
set @finishedID = (select top 1 MDFacilityInventoryPosStateID from MDFacilityInventoryPosState where MDFacilityInventoryPosStateIndex = 4);

update [dbo].[FacilityInventoryPos] 
 set MDFacilityInventoryPosStateID = @finishedID
where MDFacilityInventoryPosStateID = @pausedID;

GO
delete from MDFacilityInventoryPosState where MDFacilityInventoryPosStateIndex = 3;

GO
update MDFacilityInventoryPosState set MDNameTrans = N'en{''Not counted''}de{''Ungezählt''}' where MDFacilityInventoryPosStateIndex = 1
update MDFacilityInventoryPosState set MDNameTrans = N'en{''In Counting''}de{''In Zählung''}' where MDFacilityInventoryPosStateIndex = 2
update MDFacilityInventoryPosState set MDNameTrans = N'en{''Counted''}de{''Gezählt''}' where MDFacilityInventoryPosStateIndex = 4
update MDFacilityInventoryPosState set MDNameTrans = N'en{''Posted''}de{''Gebucht''}' where MDFacilityInventoryPosStateIndex = 5
