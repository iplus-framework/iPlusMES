-- ProdOrderPartslistPos
alter table ProdOrderPartslistPos add InputQForActualOutput float null;
alter table ProdOrderPartslistPos add InputQForGoodActualOutput float null;
alter table ProdOrderPartslistPos add InputQForScrapActualOutput float null;

alter table ProdOrderPartslistPos add InputQForFinalActualOutput float null;
alter table ProdOrderPartslistPos add InputQForFinalGoodActualOutput float null;
alter table ProdOrderPartslistPos add InputQForFinalScrapActualOutput float null;

-- ProdOrderPartslist
alter table ProdOrderPartslist add ActualQuantityScrapUOM float null;

alter table ProdOrderPartslist add InputQForActualOutputPer float null;
alter table ProdOrderPartslist add InputQForGoodActualOutputPer float null;
alter table ProdOrderPartslist add InputQForScrapActualOutputPer float null;

alter table ProdOrderPartslist add InputQForFinalActualOutputPer float null;
alter table ProdOrderPartslist add InputQForFinalGoodActualOutputPer float null;
alter table ProdOrderPartslist add InputQForFinalScrapActualOutputPer float null;
GO
update ProdOrderPartslist set ActualQuantityScrapUOM = 0
GO
alter table ProdOrderPartslist alter column ActualQuantityScrapUOM float not null;