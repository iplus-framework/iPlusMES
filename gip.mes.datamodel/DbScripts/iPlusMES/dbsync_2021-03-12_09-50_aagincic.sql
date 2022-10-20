alter table MDCountrySalesTax add DateFrom datetime null
alter table MDCountrySalesTax add DateTo datetime null
GO
update MDCountrySalesTax set DateFrom = getdate();
alter table MDCountrySalesTax alter column DateFrom datetime not null
EXEC sp_rename 'dbo.MDCountrySalesTax.MDCountrySalesTax', 'SalesTax', 'COLUMN';  
GO
CREATE TABLE [dbo].[MDCountrySalesTaxMaterial](
	[MDCountrySalesTaxMaterialID] [uniqueidentifier] NOT NULL,
	[MDCountrySalesTaxID] [uniqueidentifier] NOT NULL,
	[MaterialID] [uniqueidentifier] NOT NULL,
	[SalesTax] [real] NOT NULL,
 CONSTRAINT [PK_MDCountrySalesTaxMaterial] PRIMARY KEY CLUSTERED 
(
	[MDCountrySalesTaxMaterialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[MDCountrySalesTaxMDMaterialGroup](
	[MDCountrySalesTaxMDMaterialGroupID] [uniqueidentifier] NOT NULL,
	[MDCountrySalesTaxID] [uniqueidentifier] NOT NULL,
	[MDMaterialGroupID] [uniqueidentifier] NOT NULL,
	[SalesTax] [real] NOT NULL,
 CONSTRAINT [PK_MDCountrySalesTaxMDMaterialGroup] PRIMARY KEY CLUSTERED 
(
	[MDCountrySalesTaxMDMaterialGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_MDCountrySalesTax_Material] ON [dbo].[MDCountrySalesTaxMaterial]
(
	[MaterialID] ASC,
	[MDCountrySalesTaxID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_MDCountrySalesTax_MDMaterialGroup] ON [dbo].[MDCountrySalesTaxMDMaterialGroup]
(
	[MDCountrySalesTaxID] ASC,
	[MDMaterialGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[MDCountrySalesTaxMaterial]  WITH CHECK ADD  CONSTRAINT [FK_MDCountrySalesTaxMaterial_Material] FOREIGN KEY([MaterialID]) REFERENCES [dbo].[Material] ([MaterialID])
ALTER TABLE [dbo].[MDCountrySalesTaxMaterial]  WITH CHECK ADD  CONSTRAINT [FK_MDCountrySalesTaxMaterial_MDCountrySalesTax] FOREIGN KEY([MDCountrySalesTaxID]) REFERENCES [dbo].[MDCountrySalesTax] ([MDCountrySalesTaxID])
ALTER TABLE [dbo].[MDCountrySalesTaxMDMaterialGroup]  WITH CHECK ADD  CONSTRAINT [FK_MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax] FOREIGN KEY([MDCountrySalesTaxID]) REFERENCES [dbo].[MDCountrySalesTax] ([MDCountrySalesTaxID])
ALTER TABLE [dbo].[MDCountrySalesTaxMDMaterialGroup]  WITH CHECK ADD  CONSTRAINT [FK_MDCountrySalesTaxMDMaterialGroup_MDMaterialGroup] FOREIGN KEY([MDMaterialGroupID]) REFERENCES [dbo].[MDMaterialGroup] ([MDMaterialGroupID])
DROP TABLE [dbo].[TaxMDMaterialGroup]
DROP TABLE [dbo].[TaxMaterial]
DROP TABLE [dbo].[Tax]
delete from [dbo].[VBNoConfiguration]  where [VBNoConfigurationName] = 'Tax'