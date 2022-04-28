ALTER TABLE dbo.FacilityInventory ADD FacilityID uniqueidentifier NULL
GO
ALTER TABLE dbo.FacilityInventory ADD CONSTRAINT FK_FacilityInventory_Facility FOREIGN KEY
(
	FacilityID
) REFERENCES dbo.Facility
(
	FacilityID
) 
ON UPDATE  NO ACTION ON DELETE  NO ACTION
