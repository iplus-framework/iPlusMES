alter table Partslist add LastFormulaChange datetime null;
alter table ProdOrderPartslist add LastFormulaChange datetime null;
GO
update Partslist
set LastFormulaChange = UpdateDate

update prpl
set prpl.LastFormulaChange = pl.LastFormulaChange
from ProdOrderPartslist prpl
inner join Partslist pl on prpl.PartslistID = pl.PartslistID
GO
alter table Partslist alter column LastFormulaChange datetime not null;
alter table ProdOrderPartslist alter column LastFormulaChange datetime not null;