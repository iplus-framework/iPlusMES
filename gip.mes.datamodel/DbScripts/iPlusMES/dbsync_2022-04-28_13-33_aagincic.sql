IF COL_LENGTH('FacilityInventory','FacilityID') IS NULL
  BEGIN
    ALTER TABLE dbo.FacilityInventory ADD FacilityID uniqueidentifier NULL
  END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OutOfferingConfig]') AND type in (N'U'))
DROP TABLE [dbo].[OutOfferingConfig]
IF EXISTS (SELECT * 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'FK_FacilityInventory_FacilityID')
   AND parent_object_id = OBJECT_ID(N'dbo.FacilityInventory'))
   begin
		ALTER TABLE [dbo].[FacilityInventory] DROP CONSTRAINT [FK_FacilityInventory_FacilityID]
	
   end

IF EXISTS (SELECT * 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'FK_FacilityInventory_Facility')
   AND parent_object_id = OBJECT_ID(N'dbo.FacilityInventory'))
   begin
		ALTER TABLE [dbo].[FacilityInventory] DROP CONSTRAINT [FK_FacilityInventory_Facility]
   end
GO

ALTER TABLE dbo.FacilityInventory ADD CONSTRAINT FK_FacilityInventory_Facility FOREIGN KEY
(
	FacilityID
) REFERENCES dbo.Facility
(
	FacilityID
) 
ON UPDATE  NO ACTION ON DELETE  NO ACTION

