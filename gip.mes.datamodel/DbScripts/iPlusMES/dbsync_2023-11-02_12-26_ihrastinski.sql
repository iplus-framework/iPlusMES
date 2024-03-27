alter table PartslistPosRelation add InsertName varchar(20) null
alter table PartslistPosRelation add InsertDate datetime null
alter table PartslistPosRelation add UpdateName varchar(20) null
alter table PartslistPosRelation add UpdateDate datetime null
GO

update PartslistPosRelation set InsertName = '00'
update PartslistPosRelation set UpdateName = '00'
update PartslistPosRelation set InsertDate = '2023-11-02 00:00:00'
update PartslistPosRelation set UpdateDate = '2023-11-02 00:00:00'
GO


alter table PartslistPosRelation alter column InsertName varchar(20) not null
alter table PartslistPosRelation alter column InsertDate datetime not null
alter table PartslistPosRelation alter column UpdateName varchar(20) not null
alter table PartslistPosRelation alter column UpdateDate datetime not null
