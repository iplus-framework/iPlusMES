ALTER TABLE dbo.CompanyAddress DROP CONSTRAINT FK_CompanyAddress_VBUser;
ALTER TABLE dbo.CompanyPerson DROP CONSTRAINT FK_CompanyPerson_VBUser;
ALTER TABLE dbo.CompanyPerson DROP COLUMN VBUserID;
ALTER TABLE dbo.CompanyAddress DROP COLUMN VBUserID;
CREATE TABLE dbo.UserSettings ( UserSettingsID uniqueidentifier NOT NULL,  VBUserID uniqueidentifier NOT NULL,  TenantCompanyID uniqueidentifier NOT NULL,  InvoiceCompanyAddressID uniqueidentifier NULL,  InvoiceCompanyPersonID uniqueidentifier NULL )  ON [PRIMARY];
GO
ALTER TABLE dbo.UserSettings ADD CONSTRAINT PK_UserSettings PRIMARY KEY CLUSTERED ( UserSettingsID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ;
ALTER TABLE dbo.UserSettings ADD CONSTRAINT FK_UserSettings_VBUser FOREIGN KEY ( VBUserID ) REFERENCES dbo.VBUser ( VBUserID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.UserSettings ADD CONSTRAINT FK_UserSettings_Company FOREIGN KEY ( TenantCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.UserSettings ADD CONSTRAINT FK_UserSettings_CompanyAddress FOREIGN KEY ( InvoiceCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.UserSettings ADD CONSTRAINT FK_UserSettings_CompanyPerson FOREIGN KEY ( InvoiceCompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION 