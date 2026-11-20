ALTER TABLE [dbo].[PlanningMRPos] ADD [AllocatedQuantityUOM] float NULL
GO
update [dbo].[PlanningMRPos] set [AllocatedQuantityUOM] = 0
GO
alter table [dbo].[PlanningMRPos] alter column AllocatedQuantityUOM float not null
GO
