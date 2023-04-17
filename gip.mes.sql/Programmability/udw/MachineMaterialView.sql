IF EXISTS (SELECT * FROM sysobjects WHERE type = 'V' AND name = 'MachineMaterialView')
	BEGIN
		DROP  view  dbo.[MachineMaterialView]
	END
GO
CREATE VIEW [dbo].[MachineMaterialView]
	AS 
select
		ROW_NUMBER() over (order by t.ProgramNo, t.PartslistSequence, t.MachineName) as Nr,
		t.ProgramNo,
		t.MaterialNo,
		t.MaterialName1,
		t.ACClassID,
		t.MachineName,
		t.BasedOnACClassID,
		t.BasedOnMachineName,
		t.InwardTargetQuantityUOM,
		t.InwardActualQuantityUOM,
		t.OutwardTargetQuantityUOM,
		t.OutwardActualQuantityUOM
	from
	(
		select 
			mmvPos.ProgramNo,
			mmvPos.Sequence as PartslistSequence,
			mmvPos.MaterialNo,
			mmvPos.MaterialName1,
			mmvPos.ACClassID,
			mmvPos.MachineName,
			mmvPos.BasedOnACClassID,
			mmvPos.BasedOnMachineName,
			0 as OutwardTargetQuantityUOM,
			0 as OutwardActualQuantityUOM,
			mmvPos.InwardTargetQuantityUOM,
			mmvPos.InwardActualQuantityUOM
		from MachineMaterialPosView mmvPos
		union
		select 
			mmvRel.ProgramNo,
			mmvRel.Sequence as PartslistSequence,
			mmvRel.MaterialNo,
			mmvRel.MaterialName1,
			mmvRel.ACClassID,
			mmvRel.MachineName,
			mmvRel.BasedOnACClassID,
			mmvRel.BasedOnMachineName,
			mmvRel.OutwardTargetQuantityUOM,
			mmvRel.OutwardActualQuantityUOM,
			0 as InwardTargetQuantityUOM,
			0 as InwardActualQuantityUOM
		from MachineMaterialRelView mmvRel
	) t