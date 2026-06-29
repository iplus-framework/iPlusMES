IF COL_LENGTH('dbo.MDVisitorCard', 'VBUserID') IS NULL
BEGIN
	ALTER TABLE [dbo].[MDVisitorCard] ADD [VBUserID] [uniqueidentifier] NULL;
END
GO

IF NOT EXISTS (
	SELECT 1
	FROM sys.foreign_keys
	WHERE [name] = 'FK_MDVisitorCard_VBUserID'
)
BEGIN
	ALTER TABLE [dbo].[MDVisitorCard] WITH CHECK ADD CONSTRAINT [FK_MDVisitorCard_VBUserID] FOREIGN KEY([VBUserID])
	REFERENCES [dbo].[VBUser] ([VBUserID]) ON UPDATE NO ACTION ON DELETE NO ACTION;
END
GO
