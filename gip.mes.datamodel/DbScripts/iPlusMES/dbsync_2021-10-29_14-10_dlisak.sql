ALTER TABLE dbo.InOrder ADD IssuerCompanyPersonID uniqueidentifier NULL
GO
ALTER TABLE dbo.InOrder ADD MDCurrencyID uniqueidentifier NULL
GO
ALTER TABLE dbo.InOrder ADD CONSTRAINT
	FK_InOrder_IssuerCompanyPersonID FOREIGN KEY
	(
	  IssuerCompanyPersonID
	) 
	REFERENCES dbo.CompanyPerson
	(
	  CompanyPersonID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO
ALTER TABLE dbo.InOrder ADD CONSTRAINT
	FK_InOrder_MDCurrencyID FOREIGN KEY
	(
	MDCurrencyID
	) REFERENCES dbo.MDCurrency
	(
	MDCurrencyID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
