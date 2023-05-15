alter table MDSchedulingGroupWF add [InsertName] [varchar](20) null
alter table MDSchedulingGroupWF add [InsertDate] [datetime] null
alter table MDSchedulingGroupWF add [UpdateName] [varchar](20) null
alter table MDSchedulingGroupWF add [UpdateDate] [datetime] null
GO
update MDSchedulingGroupWF 
set
	[InsertName] = 'SUP',
	[InsertDate] = getdate(),
	[UpdateName] = 'SUP',
	[UpdateDate] = getdate()
GO
alter table MDSchedulingGroupWF alter column [InsertName] [varchar](20) not null
alter table MDSchedulingGroupWF alter column [InsertDate] [datetime]  not null
alter table MDSchedulingGroupWF alter column [UpdateName] [varchar](20) not null
alter table MDSchedulingGroupWF alter column [UpdateDate] [datetime] not null