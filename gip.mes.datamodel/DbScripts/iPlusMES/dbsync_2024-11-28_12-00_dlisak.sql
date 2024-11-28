CREATE TABLE [dbo].[PlanningMRCons](
	[PlanningMRConsID] [uniqueidentifier] NOT NULL,
	[PlanningMRID] [uniqueidentifier] NOT NULL,
	[MaterialID] [uniqueidentifier] NULL,
	[ConsumptionDate] [datetime] NOT NULL,
	[EstimatedQuantityUOM] [float] NOT NULL,
	[ReqCorrectionQuantityUOM] [float] NOT NULL,
	[RequiredQuantityUOM] [float] NOT NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PlanningMRCons] PRIMARY KEY CLUSTERED 
(
	[PlanningMRConsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PlanningMRCons] WITH CHECK ADD CONSTRAINT [FK_PlanningMRCons_PlanningMR] FOREIGN KEY([PlanningMRID]) 
REFERENCES [dbo].[PlanningMR] ([PlanningMRID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PlanningMRCons] CHECK CONSTRAINT [FK_PlanningMRCons_PlanningMR]
GO

ALTER TABLE [dbo].[PlanningMRCons]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRCons_Material] FOREIGN KEY([MaterialID])
REFERENCES [dbo].[Material] ([MaterialID])
GO

ALTER TABLE [dbo].[PlanningMRCons] CHECK CONSTRAINT [FK_PlanningMRCons_Material]
GO



CREATE TABLE [dbo].[PlanningMRPos](
	[PlanningMRPosID] [uniqueidentifier] NOT NULL,
	[PlanningMRID] [uniqueidentifier] NOT NULL,
	[PlanningMRProposalID] [uniqueidentifier] NOT NULL,
	[MaterialID] [uniqueidentifier] NULL,
	[OutOrderPosID] [uniqueidentifier] NULL,
	[ProdOrderPartslistPosID] [uniqueidentifier] NULL,
	[InOrderPosID] [uniqueidentifier] NULL,
	[ProdOrderPartslistID] [uniqueidentifier] NULL,
	[StoreQuantityUOM] [float] NOT NULL,
	[ExpectedPostingDate] [datetime] NOT NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PlanningMRPos] PRIMARY KEY CLUSTERED 
(
	[PlanningMRPosID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PlanningMRPos] WITH CHECK ADD CONSTRAINT [FK_PlanningMRPos_PlanningMR] FOREIGN KEY([PlanningMRID]) 
REFERENCES [dbo].[PlanningMR] ([PlanningMRID])
GO

ALTER TABLE [dbo].[PlanningMRPos] CHECK CONSTRAINT [FK_PlanningMRPos_PlanningMR]
GO

ALTER TABLE [dbo].[PlanningMRPos] WITH CHECK ADD CONSTRAINT [FK_PlanningMRPos_PlanningMRProposal] FOREIGN KEY([PlanningMRProposalID]) 
REFERENCES [dbo].[PlanningMRProposal] ([PlanningMRProposalID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PlanningMRPos] CHECK CONSTRAINT [FK_PlanningMRPos_PlanningMRProposal]
GO

ALTER TABLE [dbo].[PlanningMRPos]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRPos_Material] FOREIGN KEY([MaterialID])
REFERENCES [dbo].[Material] ([MaterialID])
GO

ALTER TABLE [dbo].[PlanningMRPos] CHECK CONSTRAINT [FK_PlanningMRPos_Material]
GO

ALTER TABLE [dbo].[PlanningMRPos]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRPos_OutOrderPos] FOREIGN KEY([OutOrderPosID])
REFERENCES [dbo].[OutOrderPos] ([OutOrderPosID])
GO

ALTER TABLE [dbo].[PlanningMRPos] CHECK CONSTRAINT [FK_PlanningMRPos_OutOrderPos]
GO

ALTER TABLE [dbo].[PlanningMRPos]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRPos_ProdOrderPartslistPos] FOREIGN KEY([ProdOrderPartslistPosID])
REFERENCES [dbo].[ProdOrderPartslistPos] ([ProdOrderPartslistPosID])
GO

ALTER TABLE [dbo].[PlanningMRPos] CHECK CONSTRAINT [FK_PlanningMRPos_ProdOrderPartslistPos]
GO

ALTER TABLE [dbo].[PlanningMRPos]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRPos_InOrderPos] FOREIGN KEY([InOrderPosID])
REFERENCES [dbo].[InOrderPos] ([InOrderPosID])
GO

ALTER TABLE [dbo].[PlanningMRPos] CHECK CONSTRAINT [FK_PlanningMRPos_InOrderPos]
GO

ALTER TABLE [dbo].[PlanningMRPos]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRPos_ProdOrderPartslist] FOREIGN KEY([ProdOrderPartslistID])
REFERENCES [dbo].[ProdOrderPartslist] ([ProdOrderPartslistID])
GO

ALTER TABLE [dbo].[PlanningMRPos] CHECK CONSTRAINT [FK_PlanningMRPos_ProdOrderPartslist]
GO

ALTER TABLE [dbo].[PlanningMRProposal]  DROP CONSTRAINT [FK_PlanningMRProposal_PlanningMR]
GO

ALTER TABLE [dbo].[PlanningMRProposal]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRProposal_PlanningMR] FOREIGN KEY([PlanningMRID]) 
REFERENCES [dbo].[PlanningMR] ([PlanningMRID])
ON DELETE CASCADE

ALTER TABLE [dbo].[Material] add [MRPProcedureIndex] smallint null
GO

update [dbo].[Material] set [MRPProcedureIndex] = 0
GO

alter table [dbo].[Material]  alter column [MRPProcedureIndex] smallint not null