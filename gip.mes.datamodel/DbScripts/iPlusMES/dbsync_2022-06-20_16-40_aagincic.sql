CREATE TABLE [dbo].[FacilityMaterial](
	[FacilityMaterialID] [uniqueidentifier] NOT NULL,
	[FacilityID] [uniqueidentifier] NOT NULL,
	[MaterialID] [uniqueidentifier] NOT NULL,
	[MinStockQuantity] [float] NULL,
	[OptStockQuantity] [float] NULL,
	[MaxStockQuantity] [float] NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_FacilityMaterial] PRIMARY KEY CLUSTERED 
(
	[FacilityMaterialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FacilityMaterial] 
ADD  CONSTRAINT [FK_FacilityMaterial_Facility] 
FOREIGN KEY([FacilityID]) REFERENCES [dbo].[Facility] ([FacilityID])


ALTER TABLE [dbo].[FacilityMaterial]  
ADD  CONSTRAINT [FK_FacilityMaterial_Material] 
FOREIGN KEY([MaterialID]) REFERENCES [dbo].[Material] ([MaterialID])

CREATE UNIQUE NONCLUSTERED INDEX [UIX_FacilityMaterial] ON [dbo].[FacilityMaterial]
(
	[FacilityID] ASC,
	[MaterialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

CREATE TABLE [dbo].[FacilityMDSchedulingGroup](
	[FacilityMDSchedulingGroupID] [uniqueidentifier] NOT NULL,
	[FacilityID] [uniqueidentifier] NOT NULL,
	[MDSchedulingGroupID] [uniqueidentifier] NOT NULL,
	[MDPickingTypeID] [uniqueidentifier] NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_FacilityMDSchedulingGroup] PRIMARY KEY CLUSTERED 
(
	[FacilityMDSchedulingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FacilityMDSchedulingGroup]  
ADD  CONSTRAINT [FK_FacilityMDSchedulingGroup_Facility] 
FOREIGN KEY([FacilityID]) REFERENCES [dbo].[Facility] ([FacilityID])

ALTER TABLE [dbo].[FacilityMDSchedulingGroup] 
ADD  CONSTRAINT [FK_FacilityMDSchedulingGroup_MDPickingType] 
FOREIGN KEY([MDPickingTypeID]) REFERENCES [dbo].[MDPickingType] ([MDPickingTypeID])

ALTER TABLE [dbo].[FacilityMDSchedulingGroup]  
ADD  CONSTRAINT [FK_FacilityMDSchedulingGroup_MDSchedulingGroup] 
FOREIGN KEY([MDSchedulingGroupID]) REFERENCES [dbo].[MDSchedulingGroup] ([MDSchedulingGroupID])

CREATE UNIQUE NONCLUSTERED INDEX [UIX_FacilityMDSchedulingGroup] ON [dbo].[FacilityMDSchedulingGroup]
(
	[FacilityID] ASC,
	[MDSchedulingGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

