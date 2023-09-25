alter table MaintOrderTask
add Sequence int null
GO

update MaintOrderTask set Sequence = 1
GO

alter table MaintOrderTask
alter column Sequence int not null