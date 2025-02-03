

declare @programNo varchar(20);

set @programNo = 'FA-24-100048';


SELECT
po.ProgramNo,
pl.[Sequence],
mt.MaterialNo,
mt.MaterialName1
from ProdOrder po 
inner join ProdOrderPartslist pl on pl.ProdOrderID= po.ProdOrderID
inner join Partslist pll on pll.PartslistID = pl.PartslistID
inner join Material mt on mt.MaterialID = pll.MaterialID
--left join ProdOrderPartslistPos pos on pos.ProdOrderPartslistID = pl.ProdOrderPartslistID
where 
    po.ProgramNo = @programNo
    --and pos.SourceProdOrderPartslistID is not null