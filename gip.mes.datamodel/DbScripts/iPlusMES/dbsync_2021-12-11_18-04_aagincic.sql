alter table dbo.Facility add PostingBehaviourIndex smallint null;
GO
update dbo.Facility set PostingBehaviourIndex = 0;
GO
alter table dbo.Facility alter column PostingBehaviourIndex smallint not null;
