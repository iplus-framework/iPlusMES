CREATE TABLE [dbo].[MDBatchPlanGroup](
	[MDBatchPlanGroupID] [uniqueidentifier] NOT NULL,
	[MDBatchPlanGroupIndex] [smallint] NOT NULL,
	[MDNameTrans] [varchar](max) NOT NULL,
	[SortIndex] [smallint] NOT NULL,
	[XMLConfig] [text] NULL,
	[IsDefault] [bit] NOT NULL,
	[MDKey] [varchar](40) NOT NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_MDBatchPlanGroup] PRIMARY KEY CLUSTERED 
(
	[MDBatchPlanGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
alter table [dbo].[ProdOrderBatchPlan] add MDBatchPlanGroupID uniqueidentifier null
GO
ALTER TABLE [dbo].[ProdOrderBatchPlan] ADD CONSTRAINT
FK_ProdOrderBatchPlan_MDBatchPlanGroupID FOREIGN KEY
(
	MDBatchPlanGroupID
) REFERENCES dbo.MDBatchPlanGroup
(
MDBatchPlanGroupID
) ON UPDATE  NO ACTION ON DELETE  NO ACTION