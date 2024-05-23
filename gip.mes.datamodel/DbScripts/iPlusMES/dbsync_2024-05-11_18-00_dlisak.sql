declare @coffePropertyID UNIQUEIDENTIFIER;
declare @pamPropertyID UNIQUEIDENTIFIER;

set  @coffePropertyID = 
(
    select top 1 p.ACClassPropertyID 
    from ACClassProperty p 
    inner join ACClass c on c.ACClassID = p.ACClassID
    where p.ACIdentifier = 'FillLevelScale' and c.ACIdentifier = 'CoffeeSilo'
);

set  @pamPropertyID = 
(
    select top 1 p.ACClassPropertyID 
    from ACClassProperty p 
    inner join ACClass c on c.ACClassID = p.ACClassID
    where p.ACIdentifier = 'FillLevelScale' and c.ACIdentifier = 'PAMSilo'
);

update ACClassPropertyRelation
set TargetACClassPropertyID = @pamPropertyID
where TargetACClassPropertyID = @coffePropertyID;

delete ACClassProperty
from ACClassProperty p 
inner join ACClass c on c.ACClassID = p.ACClassID
where p.ACIdentifier = 'FillLevelScale' and c.ACIdentifier = 'CoffeeSilo';