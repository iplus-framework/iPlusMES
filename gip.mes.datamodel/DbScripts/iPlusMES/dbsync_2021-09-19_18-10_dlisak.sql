ALTER TABLE dbo.OutOffer ADD MDCurrencyID uniqueidentifier NULL
GO
ALTER TABLE dbo.OutOffer ADD CONSTRAINT
	FK_OutOffer_MDCurrencyID FOREIGN KEY
	(
	MDCurrencyID
	) REFERENCES dbo.MDCurrency
	(
	MDCurrencyID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO

UPDATE oo 
SET oo.MDCurrencyID = cty.MDCurrencyID
FROM OutOffer oo
inner join CompanyAddress ca on ca.CompanyAddressID = oo.BillingCompanyAddressID
inner join MDCountry cty on cty.MDCountryID = ca.MDCountryID;
GO

ALTER TABLE dbo.OutOrder ADD MDCurrencyID uniqueidentifier NULL
GO
ALTER TABLE dbo.OutOrder ADD CONSTRAINT
	FK_OutOrder_MDCurrencyID FOREIGN KEY
	(
	MDCurrencyID
	) REFERENCES dbo.MDCurrency
	(
	MDCurrencyID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO

UPDATE oo 
SET oo.MDCurrencyID = cty.MDCurrencyID
FROM OutOrder oo
inner join CompanyAddress ca on ca.CompanyAddressID = oo.BillingCompanyAddressID
inner join MDCountry cty on cty.MDCountryID = ca.MDCountryID;
GO

ALTER TABLE dbo.Invoice ADD MDCurrencyID uniqueidentifier NULL
GO
ALTER TABLE dbo.Invoice ADD MDCurrencyExchangeID uniqueidentifier NULL
GO
ALTER TABLE dbo.Invoice ADD CONSTRAINT
	FK_Invoice_MDCurrencyID FOREIGN KEY
	(
	MDCurrencyID
	) REFERENCES dbo.MDCurrency
	(
	MDCurrencyID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO
ALTER TABLE dbo.Invoice ADD CONSTRAINT
	FK_Invoice_MDCurrencyExchangeID FOREIGN KEY
	(
	MDCurrencyExchangeID
	) REFERENCES dbo.MDCurrencyExchange
	(
	MDCurrencyExchangeID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO

UPDATE oo 
SET oo.MDCurrencyID = cty.MDCurrencyID
FROM Invoice oo
inner join CompanyAddress ca on ca.CompanyAddressID = oo.IssuerCompanyAddressID
inner join MDCountry cty on cty.MDCountryID = ca.MDCountryID;
GO


--ALTER TABLE dbo.OutOffer ALTER COLUMN MDCurrencyID uniqueidentifier NOT NULL
-- GO
--ALTER TABLE dbo.OutOffer ALTER COLUMN MDCurrencyID uniqueidentifier NOT NULL
-- GO
ALTER TABLE dbo.Invoice ALTER COLUMN MDCurrencyID uniqueidentifier NOT NULL
GO

ALTER TABLE [dbo].[MDCurrencyExchange] ALTER COLUMN ExchangeRate float NOT NULL
GO
ALTER TABLE [dbo].[MDCurrencyExchange] ADD ExchangeNo [varchar](20) NULL



