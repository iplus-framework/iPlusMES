CREATE TABLE [dbo].[PriceList](
	[PriceListID] [uniqueidentifier] NOT NULL,
	[PriceListNo] [varchar](50) NOT NULL,
	[DateFrom] [datetime] NOT NULL,
	[DateTo] [datetime] NULL,
	[Comment] [text] NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PriceList] PRIMARY KEY CLUSTERED 
(
	[PriceListID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[PriceListMaterial](
	[PriceListMaterialID] [uniqueidentifier] NOT NULL,
	[PriceListID] [uniqueidentifier] NOT NULL,
	[MaterialID] [uniqueidentifier] NOT NULL,
	[Price] [money] NOT NULL,
 CONSTRAINT [PK_PriceListMaterial] PRIMARY KEY CLUSTERED 
(
	[PriceListMaterialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Tax](
	[TaxID] [uniqueidentifier] NOT NULL,
	[TaxNo] [varchar](50) NOT NULL,
	[DateFrom] [datetime] NOT NULL,
	[DateTo] [datetime] NULL,
	[DefaultTaxValue] [money] NOT NULL,
	[InsertName] [varchar](20) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[UpdateName] [varchar](20) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Tax] PRIMARY KEY CLUSTERED 
(
	[TaxID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TaxMaterial](
	[TaxMaterialID] [uniqueidentifier] NOT NULL,
	[TaxID] [uniqueidentifier] NOT NULL,
	[MaterialID] [uniqueidentifier] NOT NULL,
	[TaxValue] [money] NOT NULL,
 CONSTRAINT [PK_TaxMaterial] PRIMARY KEY CLUSTERED 
(
	[TaxMaterialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TaxMDMaterialGroup](
	[TaxMDMaterialGroupID] [uniqueidentifier] NOT NULL,
	[TaxID] [uniqueidentifier] NOT NULL,
	[MDMaterialGroupID] [uniqueidentifier] NOT NULL,
	[TaxValue] [money] NOT NULL,
 CONSTRAINT [PK_TaxMDMaterialGroup] PRIMARY KEY CLUSTERED 
(
	[TaxMDMaterialGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_PriceListNo] ON [dbo].[PriceList]
(
	[PriceListNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE UNIQUE NONCLUSTERED INDEX [UX_PriceList_Material] ON [dbo].[PriceListMaterial]
(
	[PriceListID] ASC,
	[MaterialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE UNIQUE NONCLUSTERED INDEX [UX_TaxNo] ON [dbo].[Tax]
(
	[TaxNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE UNIQUE NONCLUSTERED INDEX [UX_Tax_Material] ON [dbo].[TaxMaterial]
(
	[TaxID] ASC,
	[MaterialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE UNIQUE NONCLUSTERED INDEX [UX_Tax_MDMaterialGroup] ON [dbo].[TaxMDMaterialGroup]
(
	[TaxID] ASC,
	[MDMaterialGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

ALTER TABLE [dbo].[PriceListMaterial]  WITH CHECK ADD  CONSTRAINT [FK_PriceListMaterial_Material] FOREIGN KEY([MaterialID]) REFERENCES [dbo].[Material] ([MaterialID])
ALTER TABLE [dbo].[PriceListMaterial]  WITH CHECK ADD  CONSTRAINT [FK_PriceListMaterial_PriceList] FOREIGN KEY([PriceListID]) REFERENCES [dbo].[PriceList] ([PriceListID])
ALTER TABLE [dbo].[TaxMaterial]  WITH CHECK ADD  CONSTRAINT [FK_TaxMaterial_Material] FOREIGN KEY([MaterialID]) REFERENCES [dbo].[Material] ([MaterialID])
ALTER TABLE [dbo].[TaxMaterial]  WITH CHECK ADD  CONSTRAINT [FK_TaxMaterial_Tax] FOREIGN KEY([TaxID]) REFERENCES [dbo].[Tax] ([TaxID])
ALTER TABLE [dbo].[TaxMDMaterialGroup]  WITH CHECK ADD  CONSTRAINT [FK_TaxMDMaterialGroup_MDMaterialGroup] FOREIGN KEY([MDMaterialGroupID]) REFERENCES [dbo].[MDMaterialGroup] ([MDMaterialGroupID])
ALTER TABLE [dbo].[TaxMDMaterialGroup]  WITH CHECK ADD  CONSTRAINT [FK_TaxMDMaterialGroup_Tax] FOREIGN KEY([TaxID]) REFERENCES [dbo].[Tax] ([TaxID])
GO
INSERT [dbo].[VBNoConfiguration] 
    (
        [VBNoConfigurationID], 
        [VBNoConfigurationName], 
        [UsedPrefix], 
        [UsedDelimiter], 
        [UseDate], 
        [MinCounter], 
        [MaxCounter], 
        [CurrentDate], 
        [CurrentCounter], 
        [XMLConfig], 
        [UpdateDate], 
        [InsertName], 
        [InsertDate], 
        [UpdateName]
    ) 
    VALUES 
    (
        newid(), 
        N'Tax', 
        NULL, 
        N'', 
        0, 
        10000000, 
        99999999, 
        getdate(), 
        1, 
        NULL, 
        getdate(),  
        N'SUP', 
        getdate(), 
        N'SUP'    
    );
    INSERT [dbo].[VBNoConfiguration] 
    (
        [VBNoConfigurationID], 
        [VBNoConfigurationName], 
        [UsedPrefix], 
        [UsedDelimiter], 
        [UseDate], 
        [MinCounter], 
        [MaxCounter], 
        [CurrentDate], 
        [CurrentCounter], 
        [XMLConfig], 
        [UpdateDate], 
        [InsertName], 
        [InsertDate], 
        [UpdateName]
    ) 
    VALUES 
    (
        newid(), 
        N'PriceList', 
        NULL, 
        N'', 
        0, 
        10000000, 
        99999999, 
        getdate(), 
        1, 
        NULL, 
        getdate(),  
        N'SUP', 
        getdate(), 
        N'SUP'    
    );