-- VBUserID to CompanyPerson and CompanyAddress
-- IssuerCompanyAddressID (mandant), IssuerCompanyPersonID (operator) add to: Invoice, OutOffer, OutOurder
ALTER TABLE dbo.CompanyAddress DROP CONSTRAINT FK_CompanyAddress_MDCountryLandID;
ALTER TABLE dbo.CompanyAddress DROP CONSTRAINT FK_CompanyAddress_MDCountryID;
ALTER TABLE dbo.Company DROP CONSTRAINT FK_Company_MDCurrencyID;
ALTER TABLE dbo.OutOrder DROP CONSTRAINT FK_OutOrder_MDOutOrderStateID;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_MDInvoiceTypeID;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_MDInvoiceStateID;
ALTER TABLE dbo.OutOffer DROP CONSTRAINT FK_OutOffer_MDTimeRangeID;
ALTER TABLE dbo.OutOrder DROP CONSTRAINT FK_OutOrder_MDTimeRangeID;
ALTER TABLE dbo.OutOffer DROP CONSTRAINT FK_OutOffer_MDTermOfPaymentID;
ALTER TABLE dbo.Company DROP CONSTRAINT FK_Company_BillingMDTermOfPaymentID;
ALTER TABLE dbo.Company DROP CONSTRAINT FK_Company_ShippingMDTermOfPaymentID;
ALTER TABLE dbo.OutOrder DROP CONSTRAINT FK_OutOrder_MDTermOfPaymentID;
ALTER TABLE dbo.OutOffer DROP CONSTRAINT FK_OutOffer_MDOutOrderTypeID;
ALTER TABLE dbo.OutOrder DROP CONSTRAINT FK_OutOrder_MDOutOrderTypeID;
ALTER TABLE dbo.OutOffer DROP CONSTRAINT FK_OutOffer_MDOutOfferStateID;
ALTER TABLE dbo.OutOffer DROP CONSTRAINT FK_OutOffer_MDDelivTypeID;
ALTER TABLE dbo.CompanyAddress DROP CONSTRAINT FK_CompanyAddress_MDDelivTypeID;
ALTER TABLE dbo.OutOrder DROP CONSTRAINT FK_OutOrder_MDDelivTypeID;
ALTER TABLE dbo.Company DROP CONSTRAINT DF__Company__IsTenan__58671BC9;
ALTER TABLE dbo.Tourplan DROP CONSTRAINT FK_Tourplan_CompanyID;
ALTER TABLE dbo.OutOffer DROP CONSTRAINT FK_OutOffer_CompanyID;
ALTER TABLE dbo.TourplanPos DROP CONSTRAINT FK_TourplanPos_CompanyID;
ALTER TABLE dbo.ProdOrder DROP CONSTRAINT FK_ProdOrder_CPartnerCompanyID;
ALTER TABLE dbo.Company DROP CONSTRAINT FK_Company_ParentCompanyID;
ALTER TABLE dbo.CompanyAddress DROP CONSTRAINT FK_CompanyAddress_CompanyID;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_CompanyID;
ALTER TABLE dbo.CompanyMaterial DROP CONSTRAINT FK_CompanyMaterial_CompanyID;
ALTER TABLE dbo.Visitor DROP CONSTRAINT FK_Visitor_VisitedCompanyID;
ALTER TABLE dbo.Visitor DROP CONSTRAINT FK_Visitor_VisitorCompanyID;
ALTER TABLE dbo.VisitorVoucher DROP CONSTRAINT FK_VisitorVoucher_VisitorCompanyID;
ALTER TABLE dbo.CompanyPerson DROP CONSTRAINT FK_CompanyPerson_CompanyID;
ALTER TABLE dbo.InOrder DROP CONSTRAINT FK_InOrder_CompanyID;
ALTER TABLE dbo.InOrder DROP CONSTRAINT FK_InOrder_CPartnerCompanyID;
ALTER TABLE dbo.Rating DROP CONSTRAINT FK_Rating_Company;
ALTER TABLE dbo.Facility DROP CONSTRAINT FK_Facility_CompanyID;
ALTER TABLE dbo.FacilityBooking DROP CONSTRAINT FK_FacilityBooking_CompanyID;
ALTER TABLE dbo.InRequest DROP CONSTRAINT FK_InRequest_CompanyID;
ALTER TABLE dbo.OutOrder DROP CONSTRAINT FK_OutOrder_CompanyID;
ALTER TABLE dbo.OutOrder DROP CONSTRAINT FK_OutOrder_CPartnerCompanyID;
CREATE TABLE dbo.Tmp_Company ( CompanyID uniqueidentifier NOT NULL,  ParentCompanyID uniqueidentifier NULL,  CompanyNo varchar(20) NOT NULL,  CompanyName varchar(50) NOT NULL,  BillingMDTermOfPaymentID uniqueidentifier NULL,  ShippingMDTermOfPaymentID uniqueidentifier NULL,  VBUserID uniqueidentifier NULL,  UseBillingAccountNo bit NOT NULL,  UseShippingAccountNo bit NOT NULL,  BillingAccountNo varchar(20) NOT NULL,  ShippingAccountNo varchar(20) NOT NULL,  NoteInternal varchar(100) NOT NULL,  NoteExternal varchar(100) NOT NULL,  IsCustomer bit NOT NULL,  IsDistributor bit NOT NULL,  IsSalesLead bit NOT NULL,  IsDistributorLead bit NOT NULL,  IsOwnCompany bit NOT NULL,  IsShipper bit NOT NULL,  MDCurrencyID uniqueidentifier NOT NULL,  IsActive bit NOT NULL,  VATNumber varchar(30) NOT NULL,  XMLConfig text NULL,  InsertName varchar(20) NOT NULL,  InsertDate datetime NOT NULL,  UpdateName varchar(20) NOT NULL,  UpdateDate datetime NOT NULL,  IsTenant bit NOT NULL )  ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
IF EXISTS(SELECT * FROM dbo.Company)
begin
	EXEC('INSERT INTO dbo.Tmp_Company (CompanyID, ParentCompanyID, CompanyNo, CompanyName, BillingMDTermOfPaymentID, ShippingMDTermOfPaymentID, UseBillingAccountNo, UseShippingAccountNo, BillingAccountNo, ShippingAccountNo, NoteInternal, NoteExternal, IsCustomer, IsDistributor, IsSalesLead, IsDistributorLead, IsOwnCompany, IsShipper, MDCurrencyID, IsActive, VATNumber, XMLConfig, InsertName, InsertDate, UpdateName, UpdateDate, IsTenant) SELECT CompanyID, ParentCompanyID, CompanyNo, CompanyName, BillingMDTermOfPaymentID, ShippingMDTermOfPaymentID, UseBillingAccountNo, UseShippingAccountNo, BillingAccountNo, ShippingAccountNo, NoteInternal, NoteExternal, IsCustomer, IsDistributor, IsSalesLead, IsDistributorLead, IsOwnCompany, IsShipper, MDCurrencyID, IsActive, VATNumber, XMLConfig, InsertName, InsertDate, UpdateName, UpdateDate, IsTenant FROM dbo.Company WITH (HOLDLOCK TABLOCKX)');
end
DROP TABLE dbo.Company;
GO
EXECUTE sp_rename N'dbo.Tmp_Company', N'Company', 'OBJECT';
GO
ALTER TABLE dbo.Company ADD CONSTRAINT PK_Company PRIMARY KEY CLUSTERED ( CompanyID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ;
CREATE NONCLUSTERED INDEX NCI_FK_Company_BillingMDTermOfPaymentID ON dbo.Company ( BillingMDTermOfPaymentID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_Company_MDCurrencyID ON dbo.Company ( MDCurrencyID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_Company_ParentCompanyID ON dbo.Company ( ParentCompanyID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_Company_ShippingMDTermOfPaymentID ON dbo.Company ( ShippingMDTermOfPaymentID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE UNIQUE NONCLUSTERED INDEX UIX_Company ON dbo.Company ( CompanyNo ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
ALTER TABLE dbo.Company ADD CONSTRAINT FK_Company_BillingMDTermOfPaymentID FOREIGN KEY ( BillingMDTermOfPaymentID ) REFERENCES dbo.MDTermOfPayment ( MDTermOfPaymentID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Company ADD CONSTRAINT FK_Company_MDCurrencyID FOREIGN KEY ( MDCurrencyID ) REFERENCES dbo.MDCurrency ( MDCurrencyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Company ADD CONSTRAINT FK_Company_ParentCompanyID FOREIGN KEY ( ParentCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Company ADD CONSTRAINT FK_Company_ShippingMDTermOfPaymentID FOREIGN KEY ( ShippingMDTermOfPaymentID ) REFERENCES dbo.MDTermOfPayment ( MDTermOfPaymentID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Company ADD CONSTRAINT FK_Company_VBUser FOREIGN KEY ( VBUserID ) REFERENCES dbo.VBUser ( VBUserID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.FacilityBooking ADD CONSTRAINT FK_FacilityBooking_CompanyID FOREIGN KEY ( CPartnerCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Facility ADD CONSTRAINT FK_Facility_CompanyID FOREIGN KEY ( CompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Rating ADD CONSTRAINT FK_Rating_Company FOREIGN KEY ( CompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.CompanyPerson ADD CONSTRAINT FK_CompanyPerson_CompanyID FOREIGN KEY ( CompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  CASCADE ;
ALTER TABLE dbo.VisitorVoucher ADD CONSTRAINT FK_VisitorVoucher_VisitorCompanyID FOREIGN KEY ( VisitorCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Visitor ADD CONSTRAINT FK_Visitor_VisitedCompanyID FOREIGN KEY ( VisitedCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  CASCADE ;
ALTER TABLE dbo.Visitor ADD CONSTRAINT FK_Visitor_VisitorCompanyID FOREIGN KEY ( VisitorCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.CompanyMaterial ADD CONSTRAINT FK_CompanyMaterial_CompanyID FOREIGN KEY ( CompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.TourplanPos DROP CONSTRAINT FK_TourplanPos_CompanyAddressID;
ALTER TABLE dbo.OutOffer DROP CONSTRAINT FK_OutOffer_BillingCompanyAddressID;
ALTER TABLE dbo.OutOffer DROP CONSTRAINT FK_OutOffer_DeliveryCompanyAddressID;
ALTER TABLE dbo.CompanyAddressDepartment DROP CONSTRAINT FK_CompanyAddressDepartment_CompanyAddressID;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_CompanyAddressID;
ALTER TABLE dbo.CompanyAddressUnloadingpoint DROP CONSTRAINT FK_CompanyAddressUnloadingpoint_CompanyAddressID;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_CompanyAddressID1;
ALTER TABLE dbo.DeliveryNote DROP CONSTRAINT FK_DeliveryNote_Delivery2CompanyAddressID;
ALTER TABLE dbo.DeliveryNote DROP CONSTRAINT FK_DeliveryNote_DeliveryCompanyAddressID;
ALTER TABLE dbo.InOrder DROP CONSTRAINT FK_InOrder_BillingCompanyAddressID;
ALTER TABLE dbo.DeliveryNote DROP CONSTRAINT FK_DeliveryNote_ShipperCompanyAddressID;
ALTER TABLE dbo.InOrder DROP CONSTRAINT FK_InOrder_DeliveryCompanyAddressID;
ALTER TABLE dbo.InRequest DROP CONSTRAINT FK_InRequest_BillingCompanyAddressID;
ALTER TABLE dbo.InRequest DROP CONSTRAINT FK_InRequest_DeliveryCompanyAddressID;
ALTER TABLE dbo.OutOrder DROP CONSTRAINT FK_OutOrder_BillingCompanyAddressID;
ALTER TABLE dbo.OutOrder DROP CONSTRAINT FK_OutOrder_DeliveryCompanyAddressID;
CREATE TABLE dbo.Tmp_CompanyAddress ( CompanyAddressID uniqueidentifier NOT NULL,  CompanyID uniqueidentifier NOT NULL,  VBUserID uniqueidentifier NULL,  IsHouseCompanyAddress bit NOT NULL,  IsBillingCompanyAddress bit NOT NULL,  IsDeliveryCompanyAddress bit NOT NULL,  IsFactory bit NOT NULL,  InvoiceIssuerNo nvarchar(50) NULL,  Name1 varchar(40) NOT NULL,  Name2 varchar(40) NOT NULL,  Name3 varchar(40) NOT NULL,  Street varchar(40) NOT NULL,  City varchar(40) NOT NULL,  Postcode varchar(10) NOT NULL,  PostOfficeBox varchar(10) NOT NULL,  Phone varchar(20) NOT NULL,  Fax varchar(20) NOT NULL,  Mobile varchar(20) NOT NULL,  EMail varchar(50) NOT NULL,  MDCountryID uniqueidentifier NULL,  MDCountryLandID uniqueidentifier NULL,  MDDelivTypeID uniqueidentifier NOT NULL,  RouteNo int NULL,  GEO_x int NULL,  GEO_y int NULL,  XMLConfig text NULL,  UpdateDate datetime NOT NULL,  UpdateName varchar(20) NOT NULL,  InsertDate datetime NOT NULL,  InsertName varchar(20) NOT NULL )  ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
IF EXISTS(SELECT * FROM dbo.CompanyAddress)
begin
	EXEC('INSERT INTO dbo.Tmp_CompanyAddress (CompanyAddressID, CompanyID, IsHouseCompanyAddress, IsBillingCompanyAddress, IsDeliveryCompanyAddress, IsFactory, Name1, Name2, Name3, Street, City, Postcode, PostOfficeBox, Phone, Fax, Mobile, EMail, MDCountryID, MDCountryLandID, MDDelivTypeID, RouteNo, GEO_x, GEO_y, XMLConfig, UpdateDate, UpdateName, InsertDate, InsertName) SELECT CompanyAddressID, CompanyID, IsHouseCompanyAddress, IsBillingCompanyAddress, IsDeliveryCompanyAddress, IsFactory, Name1, Name2, Name3, Street, City, Postcode, PostOfficeBox, Phone, Fax, Mobile, EMail, MDCountryID, MDCountryLandID, MDDelivTypeID, RouteNo, GEO_x, GEO_y, XMLConfig, UpdateDate, UpdateName, InsertDate, InsertName FROM dbo.CompanyAddress WITH (HOLDLOCK TABLOCKX)');
end
DROP TABLE dbo.CompanyAddress;
GO
EXECUTE sp_rename N'dbo.Tmp_CompanyAddress', N'CompanyAddress', 'OBJECT';
GO
ALTER TABLE dbo.CompanyAddress ADD CONSTRAINT PK_CompanyAddress PRIMARY KEY CLUSTERED ( CompanyAddressID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ;
CREATE NONCLUSTERED INDEX NCI_FK_CompanyAddress_CompanyID ON dbo.CompanyAddress ( CompanyID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_CompanyAddress_MDCountryID ON dbo.CompanyAddress ( MDCountryID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_CompanyAddress_MDCountryLandID ON dbo.CompanyAddress ( MDCountryLandID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_CompanyAddress_MDDelivTypeID ON dbo.CompanyAddress ( MDDelivTypeID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
ALTER TABLE dbo.CompanyAddress ADD CONSTRAINT FK_CompanyAddress_CompanyID FOREIGN KEY ( CompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.CompanyAddress ADD CONSTRAINT FK_CompanyAddress_MDCountryID FOREIGN KEY ( MDCountryID ) REFERENCES dbo.MDCountry ( MDCountryID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.CompanyAddress ADD CONSTRAINT FK_CompanyAddress_MDCountryLandID FOREIGN KEY ( MDCountryLandID ) REFERENCES dbo.MDCountryLand ( MDCountryLandID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.CompanyAddress ADD CONSTRAINT FK_CompanyAddress_MDDelivTypeID FOREIGN KEY ( MDDelivTypeID ) REFERENCES dbo.MDDelivType ( MDDelivTypeID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.CompanyAddress ADD CONSTRAINT FK_CompanyAddress_VBUser FOREIGN KEY ( VBUserID ) REFERENCES dbo.VBUser ( VBUserID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InRequest ADD CONSTRAINT FK_InRequest_BillingCompanyAddressID FOREIGN KEY ( BillingCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InRequest ADD CONSTRAINT FK_InRequest_CompanyID FOREIGN KEY ( DistributorCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InRequest ADD CONSTRAINT FK_InRequest_DeliveryCompanyAddressID FOREIGN KEY ( DeliveryCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InOrder ADD CONSTRAINT FK_InOrder_BillingCompanyAddressID FOREIGN KEY ( BillingCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InOrder ADD CONSTRAINT FK_InOrder_CompanyID FOREIGN KEY ( DistributorCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InOrder ADD CONSTRAINT FK_InOrder_CPartnerCompanyID FOREIGN KEY ( CPartnerCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InOrder ADD CONSTRAINT FK_InOrder_DeliveryCompanyAddressID FOREIGN KEY ( DeliveryCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.DeliveryNote ADD CONSTRAINT FK_DeliveryNote_Delivery2CompanyAddressID FOREIGN KEY ( Delivery2CompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.DeliveryNote ADD CONSTRAINT FK_DeliveryNote_DeliveryCompanyAddressID FOREIGN KEY ( DeliveryCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.DeliveryNote ADD CONSTRAINT FK_DeliveryNote_ShipperCompanyAddressID FOREIGN KEY ( ShipperCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.CompanyAddressUnloadingpoint ADD CONSTRAINT FK_CompanyAddressUnloadingpoint_CompanyAddressID FOREIGN KEY ( CompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  CASCADE ;
ALTER TABLE dbo.CompanyAddressDepartment ADD CONSTRAINT FK_CompanyAddressDepartment_CompanyAddressID FOREIGN KEY ( CompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  CASCADE ;
ALTER TABLE dbo.ProdOrder ADD CONSTRAINT FK_ProdOrder_CPartnerCompanyID FOREIGN KEY ( CPartnerCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.TourplanPos ADD CONSTRAINT FK_TourplanPos_CompanyAddressID FOREIGN KEY ( CompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.TourplanPos ADD CONSTRAINT FK_TourplanPos_CompanyID FOREIGN KEY ( CompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOfferConfig DROP CONSTRAINT FK_OutOfferConfig_OutOfferID;
ALTER TABLE dbo.OutOrder DROP CONSTRAINT FK_OutOrder_BasedOnOutOfferID;
ALTER TABLE dbo.OutOfferPos DROP CONSTRAINT FK_OutOfferPos_OutOfferID;
CREATE TABLE dbo.Tmp_OutOffer ( OutOfferID uniqueidentifier NOT NULL,  OutOfferNo varchar(20) NOT NULL,  OutOfferVersion int NOT NULL,  OutOfferDate datetime NULL,  MDOutOrderTypeID uniqueidentifier NOT NULL,  MDOutOfferStateID uniqueidentifier NOT NULL,  CustomerCompanyID uniqueidentifier NOT NULL,  CustRequestNo varchar(20) NULL,  DeliveryCompanyAddressID uniqueidentifier NULL,  BillingCompanyAddressID uniqueidentifier NOT NULL,  IssuerCompanyAddressID uniqueidentifier NULL,  IssuerCompanyPersonID uniqueidentifier NULL,  TargetDeliveryDate datetime NOT NULL,  TargetDeliveryMaxDate datetime NULL,  MDTimeRangeID uniqueidentifier NULL,  MDDelivTypeID uniqueidentifier NOT NULL,  PriceNet money NOT NULL,  PriceGross money NOT NULL,  SalesTax real NOT NULL,  MDTermOfPaymentID uniqueidentifier NULL,  Comment varchar(MAX) NULL,  XMLDesignStart text NULL,  XMLDesignEnd text NULL,  XMLConfig text NULL,  InsertName varchar(20) NOT NULL,  InsertDate datetime NOT NULL,  UpdateName varchar(20) NOT NULL,  UpdateDate datetime NOT NULL )  ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
IF EXISTS(SELECT * FROM dbo.OutOffer)
begin
	EXEC('INSERT INTO dbo.Tmp_OutOffer (OutOfferID, OutOfferNo, OutOfferVersion, OutOfferDate, MDOutOrderTypeID, MDOutOfferStateID, CustomerCompanyID, CustRequestNo, DeliveryCompanyAddressID, BillingCompanyAddressID, TargetDeliveryDate, TargetDeliveryMaxDate, MDTimeRangeID, MDDelivTypeID, PriceNet, PriceGross, SalesTax, MDTermOfPaymentID, Comment, XMLDesignStart, XMLDesignEnd, XMLConfig, InsertName, InsertDate, UpdateName, UpdateDate) SELECT OutOfferID, OutOfferNo, OutOfferVersion, OutOfferDate, MDOutOrderTypeID, MDOutOfferStateID, CustomerCompanyID, CustRequestNo, DeliveryCompanyAddressID, BillingCompanyAddressID, TargetDeliveryDate, TargetDeliveryMaxDate, MDTimeRangeID, MDDelivTypeID, PriceNet, PriceGross, SalesTax, MDTermOfPaymentID, Comment, XMLDesignStart, XMLDesignEnd, XMLConfig, InsertName, InsertDate, UpdateName, UpdateDate FROM dbo.OutOffer WITH (HOLDLOCK TABLOCKX)');
end
GO
DROP TABLE dbo.OutOffer;
EXECUTE sp_rename N'dbo.Tmp_OutOffer', N'OutOffer', 'OBJECT';
GO
ALTER TABLE dbo.OutOffer ADD CONSTRAINT PK_OutOffer PRIMARY KEY CLUSTERED ( OutOfferID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ;
ALTER TABLE dbo.OutOffer ADD CONSTRAINT FK_OutOffer_BillingCompanyAddressID FOREIGN KEY ( BillingCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOffer ADD CONSTRAINT FK_OutOffer_CompanyID FOREIGN KEY ( CustomerCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOffer ADD CONSTRAINT FK_OutOffer_DeliveryCompanyAddressID FOREIGN KEY ( DeliveryCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOffer ADD CONSTRAINT FK_OutOffer_MDDelivTypeID FOREIGN KEY ( MDDelivTypeID ) REFERENCES dbo.MDDelivType ( MDDelivTypeID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOffer ADD CONSTRAINT FK_OutOffer_MDOutOfferStateID FOREIGN KEY ( MDOutOfferStateID ) REFERENCES dbo.MDOutOfferState ( MDOutOfferStateID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOffer ADD CONSTRAINT FK_OutOffer_MDOutOrderTypeID FOREIGN KEY ( MDOutOrderTypeID ) REFERENCES dbo.MDOutOrderType ( MDOutOrderTypeID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOffer ADD CONSTRAINT FK_OutOffer_MDTermOfPaymentID FOREIGN KEY ( MDTermOfPaymentID ) REFERENCES dbo.MDTermOfPayment ( MDTermOfPaymentID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOffer ADD CONSTRAINT FK_OutOffer_MDTimeRangeID FOREIGN KEY ( MDTimeRangeID ) REFERENCES dbo.MDTimeRange ( MDTimeRangeID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOffer ADD CONSTRAINT FK_OutOffer_CompanyAddress FOREIGN KEY ( IssuerCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOffer ADD CONSTRAINT FK_OutOffer_CompanyPerson FOREIGN KEY ( IssuerCompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOfferPos ADD CONSTRAINT FK_OutOfferPos_OutOfferID FOREIGN KEY ( OutOfferID ) REFERENCES dbo.OutOffer ( OutOfferID ) ON UPDATE  NO ACTION ON DELETE  CASCADE ;
ALTER TABLE dbo.Invoice DROP CONSTRAINT FK_Invoice_OutOrderID;
ALTER TABLE dbo.OutOrderPos DROP CONSTRAINT FK_OutOrderPos_OutOrderID;
ALTER TABLE dbo.OutOrderConfig DROP CONSTRAINT FK_OutOrderConfig_OutOrderID;
ALTER TABLE dbo.TandTv2StepItem DROP CONSTRAINT FK_TandTv2StepItem_OutOrderID;
CREATE TABLE dbo.Tmp_OutOrder ( OutOrderID uniqueidentifier NOT NULL,  OutOrderNo varchar(20) NOT NULL,  OutOrderDate datetime NOT NULL,  BasedOnOutOfferID uniqueidentifier NULL,  MDOutOrderTypeID uniqueidentifier NOT NULL,  MDOutOrderStateID uniqueidentifier NOT NULL,  MDTermOfPaymentID uniqueidentifier NULL,  CPartnerCompanyID uniqueidentifier NULL,  CustomerCompanyID uniqueidentifier NOT NULL,  IssuerCompanyAddressID uniqueidentifier NULL,  IssuerCompanyPersonID uniqueidentifier NULL,  CustOrderNo varchar(20) NOT NULL,  DeliveryCompanyAddressID uniqueidentifier NULL,  BillingCompanyAddressID uniqueidentifier NULL,  TargetDeliveryDate datetime NOT NULL,  TargetDeliveryMaxDate datetime NULL,  MDTimeRangeID uniqueidentifier NULL,  MDDelivTypeID uniqueidentifier NOT NULL,  PriceNet money NOT NULL,  PriceGross money NOT NULL,  SalesTax real NOT NULL,  Comment varchar(MAX) NULL,  XMLDesignStart text NULL,  XMLDesignEnd text NULL,  XMLConfig text NULL,  InsertName varchar(20) NOT NULL,  InsertDate datetime NOT NULL,  UpdateName varchar(20) NOT NULL,  UpdateDate datetime NOT NULL )  ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
IF EXISTS(SELECT * FROM dbo.OutOrder)
begin
	EXEC('INSERT INTO dbo.Tmp_OutOrder (OutOrderID, OutOrderNo, OutOrderDate, BasedOnOutOfferID, MDOutOrderTypeID, MDOutOrderStateID, MDTermOfPaymentID, CPartnerCompanyID, CustomerCompanyID, CustOrderNo, DeliveryCompanyAddressID, BillingCompanyAddressID, TargetDeliveryDate, TargetDeliveryMaxDate, MDTimeRangeID, MDDelivTypeID, PriceNet, PriceGross, SalesTax, Comment, XMLDesignStart, XMLDesignEnd, XMLConfig, InsertName, InsertDate, UpdateName, UpdateDate) SELECT OutOrderID, OutOrderNo, OutOrderDate, BasedOnOutOfferID, MDOutOrderTypeID, MDOutOrderStateID, MDTermOfPaymentID, CPartnerCompanyID, CustomerCompanyID, CustOrderNo, DeliveryCompanyAddressID, BillingCompanyAddressID, TargetDeliveryDate, TargetDeliveryMaxDate, MDTimeRangeID, MDDelivTypeID, PriceNet, PriceGross, SalesTax, Comment, XMLDesignStart, XMLDesignEnd, XMLConfig, InsertName, InsertDate, UpdateName, UpdateDate FROM dbo.OutOrder WITH (HOLDLOCK TABLOCKX)');
end
DROP TABLE dbo.OutOrder;
GO
EXECUTE sp_rename N'dbo.Tmp_OutOrder', N'OutOrder', 'OBJECT';
GO
ALTER TABLE dbo.OutOrder ADD CONSTRAINT PK_OutOrder PRIMARY KEY CLUSTERED ( OutOrderID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ;
CREATE NONCLUSTERED INDEX NCI_FK_OutOrder_BasedOnOutOfferingID ON dbo.OutOrder ( BasedOnOutOfferID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_OutOrder_BillingCompanyAddressID ON dbo.OutOrder ( BillingCompanyAddressID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_OutOrder_CPartnerCompanyID ON dbo.OutOrder ( CPartnerCompanyID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_OutOrder_CustomerCompanyID ON dbo.OutOrder ( CustomerCompanyID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_OutOrder_DeliveryCompanyAddressID ON dbo.OutOrder ( DeliveryCompanyAddressID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_OutOrder_MDDelivTypeID ON dbo.OutOrder ( MDDelivTypeID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_OutOrder_MDOutOrderStateID ON dbo.OutOrder ( MDOutOrderStateID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_OutOrder_MDOutOrderTypeID ON dbo.OutOrder ( MDOutOrderTypeID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_OutOrder_MDTermOfPaymentID ON dbo.OutOrder ( MDTermOfPaymentID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE NONCLUSTERED INDEX NCI_FK_OutOrder_MDTimeRangeID ON dbo.OutOrder ( MDTimeRangeID ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
CREATE UNIQUE NONCLUSTERED INDEX UIX_OutOrder ON dbo.OutOrder ( OutOrderNo ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
ALTER TABLE dbo.OutOrder ADD CONSTRAINT FK_OutOrder_BasedOnOutOfferID FOREIGN KEY ( BasedOnOutOfferID ) REFERENCES dbo.OutOffer ( OutOfferID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOrder ADD CONSTRAINT FK_OutOrder_BillingCompanyAddressID FOREIGN KEY ( BillingCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOrder ADD CONSTRAINT FK_OutOrder_CompanyID FOREIGN KEY ( CustomerCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOrder ADD CONSTRAINT FK_OutOrder_CPartnerCompanyID FOREIGN KEY ( CPartnerCompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOrder ADD CONSTRAINT FK_OutOrder_DeliveryCompanyAddressID FOREIGN KEY ( DeliveryCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOrder ADD CONSTRAINT FK_OutOrder_MDDelivTypeID FOREIGN KEY ( MDDelivTypeID ) REFERENCES dbo.MDDelivType ( MDDelivTypeID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOrder ADD CONSTRAINT FK_OutOrder_MDOutOrderStateID FOREIGN KEY ( MDOutOrderStateID ) REFERENCES dbo.MDOutOrderState ( MDOutOrderStateID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOrder ADD CONSTRAINT FK_OutOrder_MDOutOrderTypeID FOREIGN KEY ( MDOutOrderTypeID ) REFERENCES dbo.MDOutOrderType ( MDOutOrderTypeID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOrder ADD CONSTRAINT FK_OutOrder_MDTermOfPaymentID FOREIGN KEY ( MDTermOfPaymentID ) REFERENCES dbo.MDTermOfPayment ( MDTermOfPaymentID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOrder ADD CONSTRAINT FK_OutOrder_MDTimeRangeID FOREIGN KEY ( MDTimeRangeID ) REFERENCES dbo.MDTimeRange ( MDTimeRangeID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOrder ADD CONSTRAINT FK_OutOrder_CompanyAddress FOREIGN KEY ( IssuerCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOrder ADD CONSTRAINT FK_OutOrder_CompanyPerson FOREIGN KEY ( IssuerCompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.TandTv2StepItem ADD CONSTRAINT FK_TandTv2StepItem_OutOrderID FOREIGN KEY ( OutOrderID ) REFERENCES dbo.OutOrder ( OutOrderID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOrderConfig ADD CONSTRAINT FK_OutOrderConfig_OutOrderID FOREIGN KEY ( OutOrderID ) REFERENCES dbo.OutOrder ( OutOrderID ) ON UPDATE  NO ACTION ON DELETE  CASCADE ;
ALTER TABLE dbo.OutOrderPos ADD CONSTRAINT FK_OutOrderPos_OutOrderID FOREIGN KEY ( OutOrderID ) REFERENCES dbo.OutOrder ( OutOrderID ) ON UPDATE  NO ACTION ON DELETE  CASCADE ;
ALTER TABLE dbo.InvoicePos DROP CONSTRAINT FK_InvoicePos_InvoiceID;
CREATE TABLE dbo.Tmp_Invoice ( InvoiceID uniqueidentifier NOT NULL,  MDInvoiceTypeID uniqueidentifier NOT NULL,  MDInvoiceStateID uniqueidentifier NOT NULL,  InvoiceNo varchar(50) NOT NULL,  InvoiceDate datetime NOT NULL,  CustomerCompanyID uniqueidentifier NOT NULL,  CustRequestNo varchar(50) NULL,  DeliveryCompanyAddressID uniqueidentifier NULL,  BillingCompanyAddressID uniqueidentifier NULL,  IssuerCompanyAddressID uniqueidentifier NULL,  IssuerCompanyPersonID uniqueidentifier NULL,  OutOrderID uniqueidentifier NULL,  PriceNet money NOT NULL,  PriceGross money NOT NULL,  SalesTax real NOT NULL,  Comment varchar(MAX) NULL,  XMLDesignStart text NULL,  XMLDesignEnd text NULL,  XMLConfig text NULL,  InsertName varchar(20) NOT NULL,  InsertDate datetime NOT NULL,  UpdateName varchar(20) NOT NULL,  UpdateDate datetime NOT NULL )  ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
IF EXISTS(SELECT * FROM dbo.Invoice)
begin
	EXEC('INSERT INTO dbo.Tmp_Invoice (InvoiceID, MDInvoiceTypeID, MDInvoiceStateID, InvoiceNo, InvoiceDate, CustomerCompanyID, CustRequestNo, DeliveryCompanyAddressID, BillingCompanyAddressID, OutOrderID, PriceNet, PriceGross, SalesTax, Comment, XMLDesignStart, XMLDesignEnd, XMLConfig, InsertName, InsertDate, UpdateName, UpdateDate) SELECT InvoiceID, MDInvoiceTypeID, MDInvoiceStateID, InvoiceNo, InvoiceDate, CustomerCompanyID, CustRequestNo, DeliveryCompanyAddressID, BillingCompanyAddressID, OutOrderID, PriceNet, PriceGross, SalesTax, Comment, XMLDesignStart, XMLDesignEnd, XMLConfig, InsertName, InsertDate, UpdateName, UpdateDate FROM dbo.Invoice WITH (HOLDLOCK TABLOCKX)');
end
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
ALTER TABLE dbo.Invoice ADD CONSTRAINT FK_Invoice_CompanyPerson FOREIGN KEY ( IssuerCompanyPersonID ) REFERENCES dbo.CompanyPerson ( CompanyPersonID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.Invoice ADD CONSTRAINT FK_Invoice_CompanyAddress FOREIGN KEY ( IssuerCompanyAddressID ) REFERENCES dbo.CompanyAddress ( CompanyAddressID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.InvoicePos ADD CONSTRAINT FK_InvoicePos_InvoiceID FOREIGN KEY ( InvoiceID ) REFERENCES dbo.Invoice ( InvoiceID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION ;
ALTER TABLE dbo.OutOfferConfig ADD CONSTRAINT FK_OutOfferConfig_OutOfferID FOREIGN KEY ( OutOfferID ) REFERENCES dbo.OutOffer ( OutOfferID ) ON UPDATE  NO ACTION ON DELETE  CASCADE ;
ALTER TABLE dbo.Tourplan ADD CONSTRAINT FK_Tourplan_CompanyID FOREIGN KEY ( CompanyID ) REFERENCES dbo.Company ( CompanyID ) ON UPDATE  NO ACTION ON DELETE  NO ACTION 