ALTER TABLE FacilityMaterial ADD Throughput float null
GO
ALTER TABLE FacilityMaterial ADD ThroughputMax float null
GO
ALTER TABLE FacilityMaterial ADD ThroughputMin float null
GO
ALTER TABLE FacilityMaterial ADD ThroughputAuto smallint not null
GO

CREATE TABLE [dbo].[FacilityMaterialOEE](
	[FacilityMaterialOEEID] [uniqueidentifier] NOT NULL,
	[FacilityMaterialID] [uniqueidentifier] NOT NULL,
	[ACProgramLogID] [uniqueidentifier] NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[IdleTimeSec] [int] NOT NULL,
	[StandByTimeSec] [int] NOT NULL,
	[OperationTimeSec] [int] NOT NULL,
	[ScheduledBreakTimeSec] [int] NOT NULL,
	[UnscheduledBreakTimeSec] [int] NOT NULL,
	[RetoolingTimeSec] [int] NOT NULL,
	[MaintenanceTimeSec] [int] NOT NULL,
	[AvailabilityOEE] [float] NOT NULL,
	[Quantity] [float] NOT NULL,
	[Throughput] [float] NOT NULL,
	[PerformanceOEE] [float] NOT NULL,
	[QuantityScrap] [float] NOT NULL,
	[QualityOEE] [float] NOT NULL,
	[TotalOEE] [float] NOT NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_FacilityMaterialOEE] PRIMARY KEY CLUSTERED 
(
	[FacilityMaterialOEEID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FacilityMaterialOEE]  WITH CHECK ADD  CONSTRAINT [FK_FacilityMaterialOEE_FacilityMaterial] FOREIGN KEY([FacilityMaterialID])
REFERENCES [dbo].[FacilityMaterial] ([FacilityMaterialID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[FacilityMaterialOEE] CHECK CONSTRAINT [FK_FacilityMaterialOEE_FacilityMaterial]
GO
