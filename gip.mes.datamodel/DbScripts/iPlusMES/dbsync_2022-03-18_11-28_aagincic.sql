alter table dbo.Material add ExcludeFromSumCalc bit null;
GO
update dbo.Material set ExcludeFromSumCalc = 0;
GO
alter table dbo.Material alter column ExcludeFromSumCalc bit not null;
