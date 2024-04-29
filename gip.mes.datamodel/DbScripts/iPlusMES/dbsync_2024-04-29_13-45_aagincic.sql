 delete pr
 from ACClassProperty pr
 where 
     pr.ACIdentifier like 'InwardFacilityChargeExternLotNo%'
     or pr.ACIdentifier like 'OutwardFacilityChargeExternLotNo%'