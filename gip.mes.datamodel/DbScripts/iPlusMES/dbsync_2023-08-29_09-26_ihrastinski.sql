alter table MaintOrder
add VBiPAAClassID uniqueidentifier null
GO
alter table MaintOrderTask
add TaskName varchar(50) null