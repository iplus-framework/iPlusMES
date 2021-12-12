update dbo.Facility set PostingBehaviourIndex = 0;
GO
alter table dbo.Facility alter column PostingBehaviourIndex int not null;
