alter table Facility add OrderPostingOnEmptying bit null
GO
update Facility set OrderPostingOnEmptying = 0;
GO
alter table Facility alter column OrderPostingOnEmptying bit not null