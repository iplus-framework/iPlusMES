alter table FacilityInventory add SuggestStockQuantity bit null 
GO
update FacilityInventory set SuggestStockQuantity=1
GO
alter table FacilityInventory alter column SuggestStockQuantity bit not null