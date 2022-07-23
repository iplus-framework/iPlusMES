use PanPekZGV4

declare @programNo varchar(50);
set @programNo = '2022-001-1982415';

select * from ProdOrder where ProgramNo = @programNo

exec udpRecalcActualQuantity @programNo,null


select
po.ProgramNo,
pos.Sequence,
mt.MaterialNo,
mt.MaterialName1,
pos.TargetQuantityUOM,
pos.MDUnitID,
mt.BaseMDUnitID,
baseUnit.TechnicalSymbol,
pos.CalledUpQuantityUOM,
pos.MDUnitID,
unit.TechnicalSymbol,
isnull(Pos.MDUnitID, mt.BaseMDUnitID) as test,
dbo.udfConvertToUnit(pos.CalledUpQuantityUOM, Mt.BaseMDUnitID, isnull(Pos.MDUnitID, mt.BaseMDUnitID),mt.BaseMDUnitID)
from ProdOrder po
inner join ProdOrderPartslist pPl on pPl.ProdOrderID = po.ProdOrderID
inner join ProdOrderPartslistPos pos on pos.ProdOrderPartslistID = pPl.ProdOrderPartslistID 
inner join Material mt on mt.MaterialID=  pos.MaterialID

inner join MDUnit baseUnit on baseUnit.MDUnitID = mt.BaseMDUnitID
inner join MDUnit unit on unit.MDUnitID = pos.MDUnitID

where po.ProgramNo = @programNo

declare @kgMDUnitID uniqueidentifier;
declare @komMDUnitID uniqueidentifier;

set @kgMDUnitID = (select top 1 MDUnitID from MDUnit where TechnicalSymbol = 'KG');
set @komMDUnitID = (select top 1 MDUnitID from MDUnit where TechnicalSymbol = 'KOM');


select dbo.udfConvertToUnit(100, @kgMDUnitID,@komMDUnitID, @kgMDUnitID);

SELECT * FROM sysobjects WHERE type = 'FN' AND name = 'udfConvertToUnit'