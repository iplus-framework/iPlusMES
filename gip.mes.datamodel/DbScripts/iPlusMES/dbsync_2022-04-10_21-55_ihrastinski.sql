alter table Facility add SkipPrintQuestion bit null
GO
update Facility set SkipPrintQuestion = 0
GO
alter table Facility alter column SkipPrintQuestion bit not null