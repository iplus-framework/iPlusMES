alter table Material add SpecHeatCapacity float null
GO
update Material set SpecHeatCapacity = 0
GO
alter table Material alter column SpecHeatCapacity float not null