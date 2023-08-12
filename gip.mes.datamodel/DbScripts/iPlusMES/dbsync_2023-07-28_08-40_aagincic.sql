alter table PickingPos add ACClassTaskID uniqueidentifier null;
GO
ALTER TABLE dbo.PickingPos ADD CONSTRAINT
	FK_PickingPos_ACClassTaskID FOREIGN KEY
	(
	ACClassTaskID
	) REFERENCES dbo.ACClassTask
	(
	ACClassTaskID
	) ON UPDATE  NO ACTION 
	 ON DELETE  SET NULL
GO
 update pp
     set pp.ACClassTaskID = tmp.ACClassTaskID
 from PickingPos pp
 inner join 
 (
     select 
     DISTINCT
     clT.ACClassTaskID,
     ol.PickingPosID
     from ACClassTask clT
     inner join ACProgram pr on pr.ACProgramID = clT.ACProgramID
     inner join ACProgramLog prLog on prLog.ACProgramID = pr.ACProgramID
     inner join OrderLog ol on ol.VBiACProgramLogID = prLog.ACProgramLogID
     where 
         ol.PickingPosID is not null
 ) tmp on tmp.PickingPosID = pp.PickingPosID