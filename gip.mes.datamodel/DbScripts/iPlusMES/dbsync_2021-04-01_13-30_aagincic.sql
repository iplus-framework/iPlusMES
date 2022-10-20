alter table dbo.MDCountrySalesTax alter column SalesTax money not null;
alter table dbo.MDCountrySalesTaxMaterial alter column SalesTax money not null;
alter table dbo.MDCountrySalesTaxMDMaterialGroup alter column SalesTax money not null;
alter table dbo.OutOfferPos alter column SalesTax money not null;
alter table dbo.OutOrderPos alter column SalesTax money not null;
alter table dbo.InvoicePos alter column SalesTax money not null;