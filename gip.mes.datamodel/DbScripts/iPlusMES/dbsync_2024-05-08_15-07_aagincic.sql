alter table [dbo].[Facility] add LeaveMaterialOccupation bit null;
GO
declare @paramName varchar(20);
set @paramName = N'LeaveMaterialOccupation'
 update fc
 set fc.LeaveMaterialOccupation = 1
 from Facility fc
 inner join ACClass cl on cl.acclassid = fc.VBiFacilityACClassID
 inner join ACClassConfig cnf on cnf.ACClassID = cl.ACClassID
 where 
     cnf.LocalConfigACUrl like @paramName + '%'
     and cnf.XMLConfig like '%<XMLValue%>True</XMLValue>%'