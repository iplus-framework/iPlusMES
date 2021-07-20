CREATE TABLE [dbo].[PlanningMR](
	[PlanningMRID] [uniqueidentifier] NOT NULL,
	
	[PlanningMRNo] [varchar](20) NOT NULL,
	[PlanningName] [varchar](350) NOT NULL,

	[RangeFrom] [datetime] NULL,
	[RangeTo] [datetime] NULL,
	[Template] [bit] not null,

	[Comment] [varchar](max) NULL,
	[XMLConfig] [text] NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL
 CONSTRAINT [PK_PlanningMR] PRIMARY KEY CLUSTERED 
(
	[PlanningMRID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[PlanningMRProposal](
	[PlanningMRProposalID] [uniqueidentifier] NOT NULL,
	[PlanningMRID] [uniqueidentifier] NOT NULL,
	[InOrderID] [uniqueidentifier] NULL,
	[ProdOrderID] [uniqueidentifier] NULL,
	[ProdOrderPartslistID] [uniqueidentifier] NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL
 CONSTRAINT [PK_PlanningMRProposal] PRIMARY KEY CLUSTERED 
(
	[PlanningMRProposalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PlanningMRProposal]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRProposal_PlanningMR] FOREIGN KEY([PlanningMRID]) REFERENCES [dbo].[PlanningMR] ([PlanningMRID])
ALTER TABLE [dbo].[PlanningMRProposal]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRProposal_InOrderID] FOREIGN KEY([InOrderID]) REFERENCES [dbo].[InOrder] ([InOrderID])
ALTER TABLE [dbo].[PlanningMRProposal]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRProposal_ProdOrderID] FOREIGN KEY([ProdOrderID]) REFERENCES [dbo].[ProdOrder] ([ProdOrderID])
ALTER TABLE [dbo].[PlanningMRProposal]  WITH CHECK ADD  CONSTRAINT [FK_PlanningMRProposal_ProdOrderPartslistID] FOREIGN KEY([ProdOrderPartslistID]) REFERENCES [dbo].[ProdOrderPartslist] ([ProdOrderPartslistID])
INSERT [dbo].[VBNoConfiguration] ([VBNoConfigurationID], [VBNoConfigurationName], [UsedPrefix], [UsedDelimiter], [UseDate], [MinCounter], [MaxCounter], [CurrentDate], [CurrentCounter], [XMLConfig], [UpdateDate], [InsertName], [InsertDate], [UpdateName]) VALUES (N'5a28923b-a2d4-4da8-80da-e705dd10bd7f', N'PlanningMRNo', N'', N'', 0, 10000000, 99999999, CAST(N'2021-07-20T14:20:54.217' AS DateTime), 10000000, NULL, CAST(N'2021-07-20T14:21:28.077' AS DateTime), N'SUP', CAST(N'2021-07-20T14:21:28.077' AS DateTime), N'SUP')