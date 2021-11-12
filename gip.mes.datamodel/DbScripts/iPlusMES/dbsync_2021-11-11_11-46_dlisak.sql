delete ACClassProperty from ACClassProperty p 
inner join ACClass c on c.ACClassID = p.ACClassID
where p.ACIdentifier like 'Error5%' and (c.ACKindIndex = 5310 or c.ACKindIndex = 6320);