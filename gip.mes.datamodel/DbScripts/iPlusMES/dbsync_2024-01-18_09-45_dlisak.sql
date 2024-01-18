alter table dbo.Facility add ClassCode smallint null;
GO
update dbo.Facility set ClassCode = 0;
GO
alter table dbo.Facility alter column ClassCode smallint not null;
