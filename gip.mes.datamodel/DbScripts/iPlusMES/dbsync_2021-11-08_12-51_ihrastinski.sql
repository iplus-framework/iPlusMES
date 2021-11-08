update MaterialConfig set KeyACUrl = 'PWMethodNodeConfig' where KeyACUrl = 'SingleDosConfig'
GO
delete d from ACClassDesign d inner join ACClass c on d.ACClassID = c.ACClassID  where d.ACIdentifier = 'SingleDosingDialog' and c.ACIdentifier = 'BSOMaterial'