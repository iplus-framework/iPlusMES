ALTER TABLE [dbo].[Company] ALTER COLUMN [CompanyName] varchar(250) not null;
ALTER TABLE [dbo].[CompanyAddress] ALTER COLUMN [Name1] varchar(250) not null;
ALTER TABLE [dbo].[CompanyAddress] ALTER COLUMN [Name2] varchar(250) not null;
ALTER TABLE [dbo].[CompanyAddress] ALTER COLUMN [Name3] varchar(250) not null;
ALTER TABLE [dbo].[CompanyAddress] ALTER COLUMN [Street] varchar(250) not null;
ALTER TABLE [dbo].[CompanyAddress] ALTER COLUMN [City] varchar(250) not null;

ALTER TABLE [dbo].[CompanyPerson] ALTER COLUMN [Name1] varchar(250) not null;
ALTER TABLE [dbo].[CompanyPerson] ALTER COLUMN [Name2] varchar(250) null;
ALTER TABLE [dbo].[CompanyPerson] ALTER COLUMN [Name3] varchar(250) null;
ALTER TABLE [dbo].[CompanyPerson] ALTER COLUMN [Street] varchar(250) not null;
ALTER TABLE [dbo].[CompanyPerson] ALTER COLUMN [City] varchar(250) not null;