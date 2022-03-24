declare @oldString varchar(250);
declare @newString varchar(250);
declare @updateUser varchar(20);
declare @updateDate datetime;

set @updateUser = 'aagincic';
set @updateDate = getdate();

set @oldString = N'>gip.mes.datamodel.GlobalApp+BatchPlanMode<';
set @newString = N'>gip.mes.datamodel.BatchPlanMode<';


--PartslistConfig
UPDATE PartslistConfig set 
	XMLConfig = REPLACE(SUBSTRING(XMLConfig,1,DATALENGTH(XMLConfig)),@oldString,@newString),
	UpdateName = @updateUser,
	UpdateDate =@updateDate
WHERE XMLConfig like '%' + @oldString + '%'


--PickingConfig
UPDATE PickingConfig set 
	XMLConfig =  REPLACE(SUBSTRING(XMLConfig,1,DATALENGTH(XMLConfig)),@oldString,@newString),
	UpdateName = @updateUser,
	UpdateDate =@updateDate
WHERE XMLConfig like '%' + @oldString + '%'

--ProdOrderPartslistConfig
UPDATE ProdOrderPartslistConfig set 
	XMLConfig =  REPLACE(SUBSTRING(XMLConfig,1,DATALENGTH(XMLConfig)),@oldString,@newString),
	UpdateName = @updateUser,
	UpdateDate =@updateDate
WHERE XMLConfig like '%' + @oldString + '%'


--ACClassMethodConfig
UPDATE ACClassMethodConfig set 
	XMLConfig =  REPLACE(SUBSTRING(XMLConfig,1,DATALENGTH(XMLConfig)),@oldString,@newString),
	UpdateName = @updateUser,
	UpdateDate =@updateDate
WHERE XMLConfig like '%' + @oldString + '%'

--MaterialWFACClassMethodConfig
UPDATE MaterialWFACClassMethodConfig set 
	XMLConfig =  REPLACE(SUBSTRING(XMLConfig,1,DATALENGTH(XMLConfig)),@oldString,@newString),
	UpdateName = @updateUser,
	UpdateDate =@updateDate
WHERE XMLConfig like '%' + @oldString + '%'

set @oldString = N'>gip.mes.datamodel.GlobalApp+BatchPlanStartModeEnum<';
set @newString = N'>gip.mes.datamodel.BatchPlanStartModeEnum<';

--PartslistConfig
UPDATE PartslistConfig set 
	XMLConfig = REPLACE(SUBSTRING(XMLConfig,1,DATALENGTH(XMLConfig)),@oldString,@newString),
	UpdateName = @updateUser,
	UpdateDate =@updateDate
WHERE XMLConfig like '%' + @oldString + '%'


--PickingConfig
UPDATE PickingConfig set 
	XMLConfig =  REPLACE(SUBSTRING(XMLConfig,1,DATALENGTH(XMLConfig)),@oldString,@newString),
	UpdateName = @updateUser,
	UpdateDate =@updateDate
WHERE XMLConfig like '%' + @oldString + '%'

--ProdOrderPartslistConfig
UPDATE ProdOrderPartslistConfig set 
	XMLConfig =  REPLACE(SUBSTRING(XMLConfig,1,DATALENGTH(XMLConfig)),@oldString,@newString),
	UpdateName = @updateUser,
	UpdateDate =@updateDate
WHERE XMLConfig like '%' + @oldString + '%'


--ACClassMethodConfig
UPDATE ACClassMethodConfig set 
	XMLConfig =  REPLACE(SUBSTRING(XMLConfig,1,DATALENGTH(XMLConfig)),@oldString,@newString),
	UpdateName = @updateUser,
	UpdateDate =@updateDate
WHERE XMLConfig like '%' + @oldString + '%'

--MaterialWFACClassMethodConfig
UPDATE MaterialWFACClassMethodConfig set 
	XMLConfig =  REPLACE(SUBSTRING(XMLConfig,1,DATALENGTH(XMLConfig)),@oldString,@newString),
	UpdateName = @updateUser,
	UpdateDate =@updateDate
WHERE XMLConfig like '%' + @oldString + '%'