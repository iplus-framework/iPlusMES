ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_CompanyPerson;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_OutOrderID;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_MDInvoiceTypeID;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_MDInvoiceStateID;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_CompanyAddressID;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_CompanyAddressID1;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_CompanyAddress;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_CompanyID;
ALTER TABLE dbo.InvoicePos DROP CONSTRAINT FK_InvoicePos_InvoiceID;
CREATE TABLE dbo.Tmp_Invoice ( InvoiceID uniqueidentifier NOT NULL,  MDInvoiceTypeID uniqueidentifier NOT NULL,  MDInvoiceStateID uniqueidentifier NOT NULL,  InvoiceNo varchar(50) NOT NULL,  InvoiceDate datetime NOT NULL,  CustomerCompanyID uniqueidentifier NOT NULL,  CustRequestNo varchar(50) NULL,  DeliveryCompanyAddressID uniqueidentifier NULL,  BillingCompanyAddressID uniqueidentifier NULL,  IssuerCompanyAddressID uniqueidentifier NULL,  IssuerCompanyPersonID uniqueidentifier NULL,  OutOrderID uniqueidentifier NULL,  MDTermOfPaymentID uniqueidentifier NULL,  PriceNet money NOT NULL,  PriceGross money NOT NULL,  SalesTax real NOT NULL,  Comment varchar(MAX) NULL,  XMLDesignStart text NULL,  XMLDesignEnd text NULL,  XMLConfig text NULL,  InsertName varchar(20) NOT NULL,  InsertDate datetime NOT NULL,  UpdateName varchar(20) NOT NULL,  UpdateDate datetime NOT NULL )  ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
IF EXISTS(SELECT * FROM dbo.Invoice)
begin
	EXEC('INSERT INTO dbo.Tmp_Invoice (InvoiceID, MDInvoiceTypeID, MDInvoiceStateID, InvoiceNo, InvoiceDate, CustomerCompanyID, CustRequestNo, DeliveryCompanyAddressID, BillingCompanyAddressID, IssuerCompanyAddressID, IssuerCompanyPersonID, OutOrderID, PriceNet, PriceGross, SalesTax, Comment, XMLDesignStart, XMLDesignEnd, XMLConfig, InsertName, InsertDate, UpdateName, UpdateDate) SELECT InvoiceID, MDInvoiceTypeID, MDInvoiceStateID, InvoiceNo, InvoiceDate, CustomerCompanyID, CustRequestNo, DeliveryCompanyAddressID, BillingCompanyAddressID, IssuerCompanyAddressID, IssuerCompanyPersonID, OutOrderID, PriceNet, PriceGross, SalesTax, Comment, XMLDesignStart, XMLDesignEnd, XMLConfig, InsertName, InsertDate, UpdateName, UpdateDate FROM dbo.Invoice WITH (HOLDLOCK TABLOCKX)');
END
GO
DROP TABLE dbo.Invoice;
GO
EXECUTE sp_rename N'dbo.Tmp_Invoice', N'Invoice', 'OBJECT';
GO
ALTER TABLE dbo.Invoice ADD CONSTRAINT PK_Invoice PRIMARY KEY CLUSTERED ( InvoiceID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ;
CREATE UNIQUE NONCLUSTERED INDEX UX_InvoiceNo ON dbo.Invoice ( InvoiceNo ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
ALTER TABLE dbo.Invoice ADD CONSTRAINT FK_Invoice_CompanyID FOREIGN KEY ( CustomerCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Invoice ADD CONSTRAINT FK_Invoice_CompanyAddressID FOREIGN KEY ( DeliveryCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Invoice ADD CONSTRAINT FK_Invoice_CompanyAddressID1 FOREIGN KEY ( BillingCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Invoice ADD CONSTRAINT FK_Invoice_MDInvoiceStateID FOREIGN KEY ( MDInvoiceStateID ) REFERENCES dbo.MDInvoiceState ( MDInvoiceStateID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Invoice ADD CONSTRAINT FK_Invoice_MDInvoiceTypeID FOREIGN KEY ( MDInvoiceTypeID ) REFERENCES dbo.MDInvoiceType ( MDInvoiceTypeID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Invoice ADD CONSTRAINT FK_Invoice_OutOrderID FOREIGN KEY ( OutOrderID ) REFERENCES dbo.OutOrder ( OutOrderID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Invoice ADD CONSTRAINT FK_Invoice_CompanyAddress FOREIGN KEY ( IssuerCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Invoice ADD CONSTRAINT FK_Invoice_CompanyPerson FOREIGN KEY ( IssuerCompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Invoice ADD CONSTRAINT FK_Invoice_MDTermOfPayment FOREIGN KEY ( MDTermOfPaymentID ) REFERENCES dbo.MDTermOfPayment ( MDTermOfPaymentID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InvoicePos ADD CONSTRAINT FK_InvoicePos_InvoiceID FOREIGN KEY ( InvoiceID ) REFERENCES dbo.Invoice ( InvoiceID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION 