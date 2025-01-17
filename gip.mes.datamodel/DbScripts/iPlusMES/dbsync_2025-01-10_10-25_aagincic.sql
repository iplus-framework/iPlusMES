ALTER TABLE [dbo].[PartslistPos] ADD [KeepBatchCount] bit NULL
GO
update [dbo].[PartslistPos] set [KeepBatchCount] = 0
alter table  [dbo].[PartslistPos] alter column KeepBatchCount bit not null