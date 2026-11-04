/* =====================================================================================
   DbSync repair skripta (aagincic, 2025-01-05)
   -------------------------------------------------------------------------------------
   fix mrp part
     - dbsync_2025-06-23_10-26 -> ALTER COLUMN Material.MRPProcedureIndex
     - dbsync_2025-06-23_13-44 -> CREATE PlanningMRPos s FK na PlanningMRCons / PlanningMRProposal
     - dbsync_2025-06-25_12-47 -> ALTER COLUMN PlanningMR.PlanningMRPhaseIndex
     - dbsync_2025-06-27_09-53 -> ALTER TABLE PlanningMRCons ADD DefaultPartslistID
   (PlanningMRPhaseIndex, IsPublished, DefaultPartslistID, PlanningMRPos.*) 
   ===================================================================================== */

/* --- 1) Material.MRPProcedureIndex ------------------------------------------------ */
IF COL_LENGTH('dbo.Material', 'MRPProcedureIndex') IS NULL
BEGIN
    ALTER TABLE [dbo].[Material] ADD [MRPProcedureIndex] smallint NULL;
END
GO
IF COL_LENGTH('dbo.Material', 'MRPProcedureIndex') IS NOT NULL
    UPDATE [dbo].[Material] SET [MRPProcedureIndex] = 0 WHERE [MRPProcedureIndex] IS NULL;
GO

/* --- 2) PlanningMR ---------------------------------------------------------------- */
IF OBJECT_ID('dbo.PlanningMR', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[PlanningMR](
        [PlanningMRID] [uniqueidentifier] NOT NULL,
        [PlanningMRNo] [varchar](20) NOT NULL,
        [PlanningName] [varchar](350) NOT NULL,
        [RangeFrom] [datetime] NULL,
        [RangeTo] [datetime] NULL,
        [Template] [bit] NOT NULL,
        [Comment] [varchar](max) NULL,
        [XMLConfig] [text] NULL,
        [InsertName] [varchar](20) NOT NULL,
        [InsertDate] [datetime] NOT NULL,
        [UpdateName] [varchar](20) NOT NULL,
        [UpdateDate] [datetime] NOT NULL,
     CONSTRAINT [PK_PlanningMR] PRIMARY KEY CLUSTERED ([PlanningMRID] ASC)
        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
              ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

/* --- 3) PlanningMRProposal -------------------------------------------------------- */
IF OBJECT_ID('dbo.PlanningMRProposal', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[PlanningMRProposal](
        [PlanningMRProposalID] [uniqueidentifier] NOT NULL,
        [PlanningMRID] [uniqueidentifier] NOT NULL,
        [InOrderID] [uniqueidentifier] NULL,
        [ProdOrderID] [uniqueidentifier] NULL,
        [ProdOrderPartslistID] [uniqueidentifier] NULL,
        [InsertName] [varchar](20) NOT NULL,
        [InsertDate] [datetime] NOT NULL,
        [UpdateName] [varchar](20) NOT NULL,
        [UpdateDate] [datetime] NOT NULL,
     CONSTRAINT [PK_PlanningMRProposal] PRIMARY KEY CLUSTERED ([PlanningMRProposalID] ASC)
        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
              ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    ALTER TABLE [dbo].[PlanningMRProposal] WITH CHECK ADD CONSTRAINT [FK_PlanningMRProposal_PlanningMR]
        FOREIGN KEY([PlanningMRID]) REFERENCES [dbo].[PlanningMR]([PlanningMRID]) ON DELETE CASCADE
    ALTER TABLE [dbo].[PlanningMRProposal] WITH CHECK ADD CONSTRAINT [FK_PlanningMRProposal_InOrderID]
        FOREIGN KEY([InOrderID]) REFERENCES [dbo].[InOrder]([InOrderID])
    ALTER TABLE [dbo].[PlanningMRProposal] WITH CHECK ADD CONSTRAINT [FK_PlanningMRProposal_ProdOrderID]
        FOREIGN KEY([ProdOrderID]) REFERENCES [dbo].[ProdOrder]([ProdOrderID])
    ALTER TABLE [dbo].[PlanningMRProposal] WITH CHECK ADD CONSTRAINT [FK_PlanningMRProposal_ProdOrderPartslistID]
        FOREIGN KEY([ProdOrderPartslistID]) REFERENCES [dbo].[ProdOrderPartslist]([ProdOrderPartslistID])
END
GO

/* --- 4) PlanningMRCons ------------------------------------------------------------ */
IF OBJECT_ID('dbo.PlanningMRCons', 'U') IS NULL
BEGIN
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
     CONSTRAINT [PK_PlanningMRCons] PRIMARY KEY CLUSTERED ([PlanningMRConsID] ASC)
        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
              ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    ALTER TABLE [dbo].[PlanningMRCons] WITH CHECK ADD CONSTRAINT [FK_PlanningMRCons_PlanningMR]
        FOREIGN KEY([PlanningMRID]) REFERENCES [dbo].[PlanningMR]([PlanningMRID]) ON DELETE CASCADE
    ALTER TABLE [dbo].[PlanningMRCons] WITH CHECK ADD CONSTRAINT [FK_PlanningMRCons_Material]
        FOREIGN KEY([MaterialID]) REFERENCES [dbo].[Material]([MaterialID])
END
GO
