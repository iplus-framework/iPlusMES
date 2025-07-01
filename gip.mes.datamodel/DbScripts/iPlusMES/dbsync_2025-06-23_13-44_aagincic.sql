IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PlanningMRPos]') AND type in (N'U'))
DROP TABLE [dbo].[PlanningMRPos]
GO
CREATE TABLE [dbo].[PlanningMRPos](
	[PlanningMRPosID] [uniqueidentifier] NOT NULL,
	[PlanningMRConsID] [uniqueidentifier] NOT NULL,
	[OutOrderPosID] [uniqueidentifier] NULL,
	[ProdOrderPartslistPosID] [uniqueidentifier] NULL,
	[StoreQuantityUOM] [float] NOT NULL,
	[PlanningMRProposalID] [uniqueidentifier] NOT NULL,
	[InOrderPosID] [uniqueidentifier] NULL,
	[ProdOrderPartslistID] [uniqueidentifier] NULL,
	[ExpectedBookingDate] [datetime] NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PlanningMRPos] PRIMARY KEY CLUSTERED 
(
	[PlanningMRPosID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PlanningMRPos]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRPos_PlanningMRCons] FOREIGN KEY([PlanningMRConsID]) REFERENCES [dbo].[PlanningMRCons] ([PlanningMRConsID])
ALTER TABLE [dbo].[PlanningMRPos]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRPos_OutOrderPos] FOREIGN KEY([OutOrderPosID]) REFERENCES [dbo].[OutOrderPos] ([OutOrderPosID])
ALTER TABLE [dbo].[PlanningMRPos]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRPos_ProdOrderPartslistPos] FOREIGN KEY([ProdOrderPartslistPosID]) REFERENCES [dbo].[ProdOrderPartslistPos] ([ProdOrderPartslistPosID])
ALTER TABLE [dbo].[PlanningMRPos]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRPos_PlanningMRProposal] FOREIGN KEY([PlanningMRProposalID]) REFERENCES [dbo].[PlanningMRProposal] ([PlanningMRProposalID]) ON DELETE CASCADE
ALTER TABLE [dbo].[PlanningMRPos]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRPos_InOrderPos] FOREIGN KEY([InOrderPosID]) REFERENCES [dbo].[InOrderPos] ([InOrderPosID])
ALTER TABLE [dbo].[PlanningMRPos]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRPos_ProdOrderPartslist] FOREIGN KEY([ProdOrderPartslistID]) REFERENCES [dbo].[ProdOrderPartslist] ([ProdOrderPartslistID])
GO
ALTER TABLE [dbo].[PlanningMRProposal] ADD [IsPublished] bit NULL
GO
update [dbo].[PlanningMRProposal] set [IsPublished] = 0
GO
alter table [dbo].[PlanningMRProposal]  alter column [IsPublished] bit not null