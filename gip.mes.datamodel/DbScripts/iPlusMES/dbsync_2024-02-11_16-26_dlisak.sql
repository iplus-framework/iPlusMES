ALTER TABLE Weighing add UpdateName varchar(20) null
ALTER TABLE Weighing add UpdateDate datetime null
ALTER TABLE Weighing ADD StateIndex smallint null
GO

update Weighing set UpdateName = '00'
update Weighing set UpdateDate = '2023-11-02 00:00:00'
update Weighing set StateIndex = 0
GO

alter table Weighing alter column UpdateName varchar(20) not null
alter table Weighing alter column UpdateDate datetime not null
alter table Weighing alter column StateIndex smallint not null