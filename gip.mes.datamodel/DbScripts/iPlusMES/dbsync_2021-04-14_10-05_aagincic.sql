-- CompanyPerson -- add VBUserID
ALTER TABLE dbo.CompanyPerson DROP CONSTRAINT FK_CompanyPerson_MDTimeRangeID;
ALTER TABLE dbo.CompanyPerson DROP CONSTRAINT FK_CompanyPerson_MDCountryID;
ALTER TABLE dbo.CompanyPerson DROP CONSTRAINT FK_CompanyPerson_CompanyID;
ALTER TABLE dbo.CalendarShiftPerson DROP CONSTRAINT FK_CalendarShiftPerson_CompanyPersonID;
ALTER TABLE dbo.Visitor DROP CONSTRAINT FK_Visitor_VisitorCompanyPersonID;
ALTER TABLE dbo.VisitorVoucher DROP CONSTRAINT FK_VisitorVoucher_VisitorCompanyPersonID;
ALTER TABLE dbo.OutOffer DROP CONSTRAINT FK_OutOffer_CompanyPerson;
ALTER TABLE dbo.CompanyPersonRole DROP CONSTRAINT FK_CompanyPersonRole_CompanyPersonID;
ALTER TABLE dbo.OutOrder DROP CONSTRAINT FK_OutOrder_CompanyPerson;
ALTER TABLE dbo.Rating DROP CONSTRAINT FK_Rating_CompanyPerson;
ALTER TABLE dbo.Facility DROP CONSTRAINT FK_Facility_CompanyPersonID;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_CompanyPerson;
CREATE TABLE dbo.Tmp_CompanyPerson ( CompanyPersonID uniqueidentifier NOT NULL,  CompanyID uniqueidentifier NOT NULL,  VBUserID uniqueidentifier NULL,  Name1 varchar(40) NOT NULL,  Name2 varchar(40) NULL,  Name3 varchar(40) NULL,  Street varchar(40) NOT NULL,  City varchar(40) NOT NULL,  Postcode varchar(10) NOT NULL,  PostOfficeBox varchar(10) NULL,  Phone varchar(20) NULL,  Fax varchar(20) NULL,  Mobile varchar(20) NULL,  MDCountryID uniqueidentifier NULL,  MDTimeRangeID uniqueidentifier NULL,  XMLConfig text NULL,  UpdateDate datetime NOT NULL,  UpdateName varchar(20) NOT NULL,  InsertDate datetime NOT NULL,  InsertName varchar(20) NOT NULL,  CompanyPersonNo varchar(20) NOT NULL )  ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
IF EXISTS(SELECT * FROM dbo.CompanyPerson)
begin
	EXEC('INSERT INTO dbo.Tmp_CompanyPerson (CompanyPersonID, CompanyID, Name1, Name2, Name3, Street, City, Postcode, PostOfficeBox, Phone, Fax, Mobile, MDCountryID, MDTimeRangeID, XMLConfig, UpdateDate, UpdateName, InsertDate, InsertName, CompanyPersonNo) SELECT CompanyPersonID, CompanyID, Name1, Name2, Name3, Street, City, Postcode, PostOfficeBox, Phone, Fax, Mobile, MDCountryID, MDTimeRangeID, XMLConfig, UpdateDate, UpdateName, InsertDate, InsertName, CompanyPersonNo FROM dbo.CompanyPerson WITH (HOLDLOCK TABLOCKX)');
end
DROP TABLE dbo.CompanyPerson;
GO
EXECUTE sp_rename N'dbo.Tmp_CompanyPerson', N'CompanyPerson', 'OBJECT';
GO
ALTER TABLE dbo.CompanyPerson ADD CONSTRAINT PK_CompanyPerson PRIMARY KEY CLUSTERED ( CompanyPersonID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ;
CREATE NONCLUSTERED INDEX NCI_FK_CompanyPerson_CompanyID ON dbo.CompanyPerson ( CompanyID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_CompanyPerson_MDCountryID ON dbo.CompanyPerson ( MDCountryID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_CompanyPerson_MDTimeRangeID ON dbo.CompanyPerson ( MDTimeRangeID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE UNIQUE NONCLUSTERED INDEX UIX_CompanyPerson_CompanyPersonNo ON dbo.CompanyPerson ( CompanyPersonNo ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
ALTER TABLE dbo.CompanyPerson ADD CONSTRAINT FK_CompanyPerson_CompanyID FOREIGN KEY ( CompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  CASCADE ;
ALTER TABLE dbo.CompanyPerson ADD CONSTRAINT FK_CompanyPerson_MDCountryID FOREIGN KEY ( MDCountryID ) REFERENCES dbo.MDCountry ( MDCountryID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.CompanyPerson ADD CONSTRAINT FK_CompanyPerson_MDTimeRangeID FOREIGN KEY ( MDTimeRangeID ) REFERENCES dbo.MDTimeRange ( MDTimeRangeID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.CompanyPerson ADD CONSTRAINT FK_CompanyPerson_VBUser FOREIGN KEY ( VBUserID ) REFERENCES dbo.VBUser ( VBUserID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Invoice ADD CONSTRAINT FK_Invoice_CompanyPerson FOREIGN KEY ( IssuerCompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Facility ADD CONSTRAINT FK_Facility_CompanyPersonID FOREIGN KEY ( CompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Rating ADD CONSTRAINT FK_Rating_CompanyPerson FOREIGN KEY ( CompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOrder ADD CONSTRAINT FK_OutOrder_CompanyPerson FOREIGN KEY ( IssuerCompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.CompanyPersonRole ADD CONSTRAINT FK_CompanyPersonRole_CompanyPersonID FOREIGN KEY ( CompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  CASCADE ;
ALTER TABLE dbo.OutOffer ADD CONSTRAINT FK_OutOffer_CompanyPerson FOREIGN KEY ( IssuerCompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.VisitorVoucher ADD CONSTRAINT FK_VisitorVoucher_VisitorCompanyPersonID FOREIGN KEY ( VisitorCompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Visitor ADD CONSTRAINT FK_Visitor_VisitorCompanyPersonID FOREIGN KEY ( VisitorCompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.CalendarShiftPerson ADD CONSTRAINT FK_CalendarShiftPerson_CompanyPersonID FOREIGN KEY ( CompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  CASCADE 