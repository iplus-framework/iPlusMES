alter table [dbo].[PlanningMRPos] add [Sequence] int null 
GO
update [dbo].[PlanningMRPos] set [Sequence] = 0
GO
alter table PlanningMRPos alter column Sequence int not null
GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_PlanningMRCons_Sequence] ON [dbo].[PlanningMRPos]
(
	[PlanningMRConsID] ASC,
	[Sequence] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
