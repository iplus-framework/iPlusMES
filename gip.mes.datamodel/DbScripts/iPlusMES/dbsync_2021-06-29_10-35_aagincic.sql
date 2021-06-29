IF EXISTS (SELECT * FROM sysobjects WHERE type = 'V' AND name = 'VBTranslationView')
	BEGIN
		DROP  View dbo.[VBTranslationView]
	END
GO
CREATE VIEW [dbo].[VBTranslationView]
	AS 
select
pr.ACProjectName as							ACProjectName,
'000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000

' as								
											TableName,
cl.ACClassID as								MandatoryID,
cl.ACIdentifier as							MandatoryACIdentifier,
cl.ACURLCached as							MandatoryACURLCached,
cl.ACClassID as								ID,
cl.ACIdentifier as							ACIdentifier,
cl.ACCaptionTranslation as					TranslationValue,
isnull(cl.UpdateName, '') as				UpdateName,
isnull(cl.UpdateDate, getdate()) as			UpdateDate
from ACClass cl
inner join ACProject pr on pr.ACProjectID = cl.ACProjectID

GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'udpTranslation')
	BEGIN
		DROP  procedure  dbo.[udpTranslation]
	END
GO
CREATE PROCEDURE [dbo].[udpTranslation]
	@acProjectID uniqueidentifier,
	@mandatoryID uniqueidentifier,
	@onlyClassTables bit,
	@onlyMDTables bit,
	@searchClassACIdentifier varchar(150),
	@searchACIdentifier varchar(150),
	@searchTranslation varchar(150),
	@notHaveInTranslation varchar(150)
AS
begin
	begin tran
-- begin code
-- declare internal variables
	IF OBJECT_ID('dbo.#translationViewResults') IS NULL
		begin
			CREATE TABLE #translationViewResults
			(
				JobID					uniqueidentifier,
				ACProjectName			varchar(200) COLLATE DATABASE_DEFAULT,
				TableName				varchar(150) COLLATE DATABASE_DEFAULT,
				MandatoryID				uniqueidentifier,
				MandatoryACIdentifier	varchar(200) COLLATE DATABASE_DEFAULT,
				MandatoryACURLCached    varchar(max),
				ID						uniqueidentifier,
				ACIdentifier			varchar(200) COLLATE DATABASE_DEFAULT,
				TranslationValue		varchar(max) COLLATE DATABASE_DEFAULT,
				UpdateName				varchar(20) COLLATE DATABASE_DEFAULT,
				UpdateDate				datetime
			);
		end
		declare @jobID uniqueidentifier;
		declare @emptyID uniqueidentifier;
		declare @cnt int;
		-- start process
		set @jobID = NEWID();
		set @emptyID = '00000000-0000-0000-0000-000000000000';

		-- # ACClass Tables
		if @onlyClassTables is null or @onlyClassTables = 1
		begin

				-- ACClass
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					Pr.ACProjectName as			ACProjectName,
					N'ACClass' as				TableName,
					cl.ACClassID as				MandatoryID,
					cl.ACIdentifier as			MandatoryACIdentifier,
					cl.ACURLCached	as			MandatoryACURLCached,
					cl.ACClassID as				ID,
					cl.ACIdentifier as			ACIdentifier,
					cl.ACCaptionTranslation as	TranslationValue,
					cl.UpdateName as			UpdateName,
					cl.UpdateDate as			UpdateDate
				from ACClass cl
				inner join ACProject pr on pr.ACProjectID = cl.ACProjectID
					where 
						(@acProjectID is null or cl.ACProjectID = @acProjectID) and
						(@mandatoryID is null or cl.ACClassID = @mandatoryID) and
						(@searchClassACIdentifier is null or lower(cl.ACIdentifier) like '%' + @searchClassACIdentifier + '%') and
						(@searchACIdentifier is null or lower(cl.ACIdentifier) like '%' + @searchACIdentifier + '%') and
						(@searchTranslation is null or LOWER(cl.ACCaptionTranslation) like '%' + @searchTranslation + '%') and 
						(@notHaveInTranslation is null or LOWER(cl.ACCaptionTranslation) not like '%' + @notHaveInTranslation + '%')
					order by cl.ACIdentifier

				--ACClassMessage
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					cl.ACIdentifier as			ACProjectName,
					'ACClassMessage' as			TableName,
					cl.ACClassID as				MandatoryID,
					cl.ACIdentifier as			MandatoryACIdentifier,
					cl.ACURLCached	as			MandatoryACURLCached,
					msg.ACClassMessageID as		ID,
					msg.ACIdentifier as			ACIdentifier,
					msg.ACCaptionTranslation as TranslationValue,
					msg.UpdateName	as			UpdateName,
					msg.UpdateDate	as			UpdateDate
				from ACClassMessage msg
				inner join ACClass cl on cl.ACClassID = msg.ACClassID
					where 
						(@acProjectID is null or cl.ACProjectID = @acProjectID) and
						(@mandatoryID is null or cl.ACClassID = @mandatoryID) and
						(@searchClassACIdentifier is null or lower(cl.ACIdentifier) like '%' + @searchClassACIdentifier + '%') and
						(@searchACIdentifier is null or lower(msg.ACIdentifier) like '%' + @searchACIdentifier + '%') and
						(@searchTranslation is null or LOWER(msg.ACCaptionTranslation) like '%' + @searchTranslation + '%') and 
						(@notHaveInTranslation is null or LOWER(msg.ACCaptionTranslation) not like '%' + @notHaveInTranslation + '%')
					order by msg.ACIdentifier

				--ACClassMethod
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					cl.ACIdentifier as			ACProjectName,
					N'ACClassMethod' as			TableName,
					cl.ACClassID as				MandatoryID,
					cl.ACIdentifier as			MandatoryACIdentifier,
					cl.ACURLCached	as			MandatoryACURLCached,
					mth.ACClassMethodID as		ID,
					mth.ACIdentifier as			ACIdentifier,
					mth.ACCaptionTranslation as TranslationValue,
					mth.UpdateName as			UpdateName,
					mth.UpdateDate as			UpdateDate
				from ACClassMethod mth
				inner join ACClass cl on cl.ACClassID = mth.ACClassID
					where 
						(@acProjectID is null or cl.ACProjectID = @acProjectID) and
						(@mandatoryID is null or cl.ACClassID = @mandatoryID) and
						(@searchClassACIdentifier is null or lower(cl.ACIdentifier) like '%' + @searchClassACIdentifier + '%') and
						(@searchACIdentifier is null or lower(mth.ACIdentifier) like '%' + @searchACIdentifier + '%') and
						(@searchTranslation is null or LOWER(mth.ACCaptionTranslation) like '%' + @searchTranslation + '%') and 
						(@notHaveInTranslation is null or LOWER(mth.ACCaptionTranslation) not like '%' + @notHaveInTranslation + '%')
					order by mth.ACIdentifier
			
				--ACClassProperty
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					cl.ACIdentifier as			ACProjectName,
					N'ACClassProperty' as		TableName,
					cl.ACClassID as				MandatoryID,
					cl.ACIdentifier as			MandatoryACIdentifier,
					cl.ACURLCached	as			MandatoryACURLCached,
					prop.ACClassPropertyID as	ID,
					prop.ACIdentifier as		ACIdentifier,
					prop.ACCaptionTranslation as TranslationValue,
					prop.UpdateName as			UpdateName,
					prop.UpdateDate as			UpdateDate
				from ACClassProperty prop
				inner join ACClass cl on cl.ACClassID = prop.ACClassID
					where 
						(@acProjectID is null or cl.ACProjectID = @acProjectID) and
						(@mandatoryID is null or cl.ACClassID = @mandatoryID) and
						(@searchClassACIdentifier is null or lower(cl.ACIdentifier) like '%' + @searchClassACIdentifier + '%') and
						(@searchACIdentifier is null or lower(prop.ACIdentifier) like '%' + @searchACIdentifier + '%') and
						(@searchTranslation is null or LOWER(prop.ACCaptionTranslation) like '%' + @searchTranslation + '%') and 
						(@notHaveInTranslation is null or LOWER(prop.ACCaptionTranslation) not like '%' + @notHaveInTranslation + '%')
					order by prop.ACIdentifier

				--ACClassText
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					cl.ACIdentifier as			ACProjectName,
					N'ACClassText' as			TableName,
					cl.ACClassID as				MandatoryID,
					cl.ACIdentifier as			MandatoryACIdentifier,
					cl.ACURLCached	as			MandatoryACURLCached,
					txt.ACClassTextID as		ID,
					txt.ACIdentifier as			ACIdentifier,
					txt.ACCaptionTranslation as TranslationValue,
					txt.UpdateName as			UpdateName,
					txt.UpdateDate as			UpdateDate
				from ACClassText txt
				inner join ACClass cl on cl.ACClassID = txt.ACClassID
					where 
						(@acProjectID is null or cl.ACProjectID = @acProjectID) and
						(@mandatoryID is null or cl.ACClassID = @mandatoryID) and
						(@searchClassACIdentifier is null or lower(cl.ACIdentifier) like '%' + @searchClassACIdentifier + '%') and
						(@searchACIdentifier is null or lower(txt.ACIdentifier) like '%' + @searchACIdentifier + '%') and
						(@searchTranslation is null or LOWER(txt.ACCaptionTranslation) like '%' + @searchTranslation + '%') and 
						(@notHaveInTranslation is null or LOWER(txt.ACCaptionTranslation) not like '%' + @notHaveInTranslation + '%')
					order by txt.ACIdentifier
			
				--ACClassDesign
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					cl.ACIdentifier as			ACProjectName,
					N'ACClassDesign' as			TableName,
					cl.ACClassID as				MandatoryID,
					cl.ACIdentifier as			MandatoryACIdentifier,
					cl.ACURLCached	as			MandatoryACURLCached,
					de.ACClassDesignID as		ID,
					de.ACIdentifier as			ACIdentifier,
					de.ACCaptionTranslation as	TranslationValue,
					de.UpdateName as			UpdateName,
					de.UpdateDate as			UpdateDate
				from ACClassDesign de
				inner join ACClass cl on cl.ACClassID = de.ACClassID
					where 
						(@acProjectID is null or cl.ACProjectID = @acProjectID) and
						(@mandatoryID is null or cl.ACClassID = @mandatoryID) and
						(@searchClassACIdentifier is null or lower(cl.ACIdentifier) like '%' + @searchClassACIdentifier + '%') and
						(@searchACIdentifier is null or lower(de.ACIdentifier) like '%' + @searchACIdentifier + '%') and
						(@searchTranslation is null or LOWER(de.ACCaptionTranslation) like '%' + @searchTranslation + '%') and 
						(@notHaveInTranslation is null or LOWER(de.ACCaptionTranslation) not like '%' + @notHaveInTranslation + '%')
					order by de.ACIdentifier
			end -- end cause only class tables
			
		-- # MDTables
		if  (@onlyMDTables is null or @onlyMDTables = 1) and @mandatoryID is null
		begin

			-- MDBalancingMode
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDBalancingMode')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDBalancingMode' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDBalancingModeID as		ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDBalancingMode md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and 
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			

			-- MDBookingNotAvailableMode
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDBookingNotAvailableMode')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as							JobID,
					'' as								ACProjectName,
					'MDBookingNotAvailableMode' as		TableName,
					@emptyID as							MandatoryID,
					'' as								MandatoryACIdentifier,
					null as								MandatoryACURLCached,
					md.MDBookingNotAvailableModeID as	ID,
					md.MDKey as							ACIdentifier,
					md.MDNameTrans as					TranslationValue,
					md.UpdateName as					UpdateName,
					md.UpdateDate as					UpdateDate
				from MDBookingNotAvailableMode md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
		

			-- MDCostCenter
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDCostCenter')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as				JobID,
					'' as					ACProjectName,
					'MDCostCenter' as		TableName,
					@emptyID as				MandatoryID,
					'' as					MandatoryACIdentifier,
					null as					MandatoryACURLCached,
					md.MDCostCenterID as	ID,
					md.MDKey as				ACIdentifier,
					md.MDNameTrans as		TranslationValue,
					md.UpdateName as		UpdateName,
					md.UpdateDate as		UpdateDate
				from MDCostCenter md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			
					
			-- MDCountry
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDCountry')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as			JobID,
					'' as				ACProjectName,
					'MDCountry' as		TableName,
					@emptyID as			MandatoryID,
					'' as				MandatoryACIdentifier,
					null as				MandatoryACURLCached,
					md.MDCountryID as	ID,
					md.MDKey as			ACIdentifier,
					md.MDNameTrans as	TranslationValue,
					md.UpdateName as	UpdateName,
					md.UpdateDate as	UpdateDate
				from MDCountry md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			

			-- MDCountryLand
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDCountryLand')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as				JobID,
					'' as					ACProjectName,
					'MDCountryLand' as		TableName,
					@emptyID as				MandatoryID,
					'' as					MandatoryACIdentifier,
					null as					MandatoryACURLCached,
					md.MDCountryLandID as	ID,
					md.MDKey as				ACIdentifier,
					md.MDNameTrans as		TranslationValue,
					md.UpdateName as		UpdateName,
					md.UpdateDate as		UpdateDate
				from MDCountryLand md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			
			-- MDCountrySalesTax
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDCountrySalesTax')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDCountrySalesTax' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDCountrySalesTaxID as	ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDCountrySalesTax md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
					
			-- MDCurrency
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDCurrency')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as				JobID,
					'' as					ACProjectName,
					'MDCurrency' as			TableName,
					@emptyID as				MandatoryID,
					'' as					MandatoryACIdentifier,
					null as					MandatoryACURLCached,
					md.MDCurrencyID as		ID,
					md.MDKey as				ACIdentifier,
					md.MDNameTrans as		TranslationValue,
					md.UpdateName as		UpdateName,
					md.UpdateDate as		UpdateDate
				from MDCurrency md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%')
					 and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDDelivNoteState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDDelivNoteState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDDelivNoteState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDDelivNoteStateID as	ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDDelivNoteState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDDelivPosLoadState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDDelivPosLoadState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as						JobID,
					'' as							ACProjectName,
					'MDDelivPosLoadState' as		TableName,
					@emptyID as						MandatoryID,
					'' as							MandatoryACIdentifier,
					null as							MandatoryACURLCached,
					md.MDDelivPosLoadStateID as		ID,
					md.MDKey as						ACIdentifier,
					md.MDNameTrans as				TranslationValue,
					md.UpdateName as				UpdateName,
					md.UpdateDate as				UpdateDate
				from MDDelivPosLoadState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDDelivPosState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDDelivPosState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDDelivPosState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDDelivPosStateID as		ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDDelivPosState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDDelivType
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDDelivType')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as				JobID,
					'' as					ACProjectName,
					'MDDelivType' as		TableName,
					@emptyID as				MandatoryID,
					'' as					MandatoryACIdentifier,
					null as					MandatoryACURLCached,
					md.MDDelivTypeID as		ID,
					md.MDKey as				ACIdentifier,
					md.MDNameTrans as		TranslationValue,
					md.UpdateName as		UpdateName,
					md.UpdateDate as		UpdateDate
				from MDDelivType md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDDemandOrderState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDDemandOrderState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as						JobID,
					'' as							ACProjectName,
					'MDDemandOrderState' as			TableName,
					@emptyID as						MandatoryID,
					'' as							MandatoryACIdentifier,
					null as							MandatoryACURLCached,
					md.MDDemandOrderStateID as		ID,
					md.MDKey as						ACIdentifier,
					md.MDNameTrans as				TranslationValue,
					md.UpdateName as				UpdateName,
					md.UpdateDate as				UpdateDate
				from MDDemandOrderState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDFacilityInventoryPosState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDFacilityInventoryPosState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as JobID,
					'' as									ACProjectName,
					'MDFacilityInventoryPosState' as		TableName,
					@emptyID as								MandatoryID,
					'' as									MandatoryACIdentifier,
					null as									MandatoryACURLCached,
					md.MDFacilityInventoryPosStateID as		ID,
					md.MDKey as								ACIdentifier,
					md.MDNameTrans as						TranslationValue,
					md.UpdateName as						UpdateName,
					md.UpdateDate as						UpdateDate
				from MDFacilityInventoryPosState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDFacilityInventoryState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDFacilityInventoryState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as							JobID,
					'' as								ACProjectName,
					'MDFacilityInventoryState' as		TableName,
					@emptyID as							MandatoryID,
					'' as								MandatoryACIdentifier,
					null as								MandatoryACURLCached,
					md.MDFacilityInventoryStateID as	ID,
					md.MDKey as							ACIdentifier,
					md.MDNameTrans as					TranslationValue,
					md.UpdateName as					UpdateName,
					md.UpdateDate as					UpdateDate
				from MDFacilityInventoryState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDFacilityManagementType
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDFacilityManagementType')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as							JobID,
					'' as								ACProjectName,
					'MDFacilityManagementType' as		TableName,
					@emptyID as							MandatoryID,
					'' as								MandatoryACIdentifier,
					null as								MandatoryACURLCached,
					md.MDFacilityManagementTypeID as	ID,
					md.MDKey as							ACIdentifier,
					md.MDNameTrans as					TranslationValue,
					md.UpdateName as					UpdateName,
					md.UpdateDate as					UpdateDate
				from MDFacilityManagementType md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDFacilityType
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDFacilityType')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as				JobID,
					'' as					ACProjectName,
					'MDFacilityType' as		TableName,
					@emptyID as				MandatoryID,
					'' as					MandatoryACIdentifier,
					null as					MandatoryACURLCached,
					md.MDFacilityTypeID as	ID,
					md.MDKey as				ACIdentifier,
					md.MDNameTrans as		TranslationValue,
					md.UpdateName as		UpdateName,
					md.UpdateDate as		UpdateDate
				from MDFacilityType md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDFacilityVehicleType
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDFacilityVehicleType')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as						JobID,
					'' as							ACProjectName,
					'MDFacilityVehicleType' as		TableName,
					@emptyID as						MandatoryID,
					'' as							MandatoryACIdentifier,
					null as							MandatoryACURLCached,
					md.MDFacilityVehicleTypeID as	ID,
					md.MDKey as						ACIdentifier,
					md.MDNameTrans as				TranslationValue,
					md.UpdateName as				UpdateName,
					md.UpdateDate as				UpdateDate
				from MDFacilityVehicleType md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			

			-- MDGMPAdditive
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDGMPAdditive')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
			select
				@jobID as					JobID,
				'' as						ACProjectName,
				'MDGMPAdditive' as			TableName,
				@emptyID as					MandatoryID,
				'' as						MandatoryACIdentifier,
				null as						MandatoryACURLCached,
				md.MDGMPAdditiveID as		ID,
				md.MDKey as					ACIdentifier,
				md.MDNameTrans as			TranslationValue,
				md.UpdateName as			UpdateName,
				md.UpdateDate as			UpdateDate
			from MDGMPAdditive md
				where 
				(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
				(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
				order by md.MDKey
			end

			-- MDGMPMaterialGroup
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDGMPMaterialGroup')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDGMPMaterialGroup' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDGMPMaterialGroupID as	ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDGMPMaterialGroup md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			

			-- MDInOrderPosState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDInOrderPosState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as						JobID,
					'' as							ACProjectName,
					'MDInOrderPosState' as			TableName,
					@emptyID as						MandatoryID,
					'' as							MandatoryACIdentifier,
					null as							MandatoryACURLCached,
					md.MDInOrderPosStateID as		ID,
					md.MDKey as						ACIdentifier,
					md.MDNameTrans as				TranslationValue,
					md.UpdateName as				UpdateName,
					md.UpdateDate as				UpdateDate
				from MDInOrderPosState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			

			-- MDInOrderState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDInOrderState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDInOrderState' as			TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDInOrderStateID as		ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDInOrderState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			

			-- MDInOrderType
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDInOrderType')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as				JobID,
					'' as					ACProjectName,
					'MDInOrderType' as		TableName,
					@emptyID as				MandatoryID,
					'' as					MandatoryACIdentifier,
					null as					MandatoryACURLCached,
					md.MDInOrderTypeID as	ID,
					md.MDKey as				ACIdentifier,
					md.MDNameTrans as		TranslationValue,
					md.UpdateName as		UpdateName,
					md.UpdateDate as		UpdateDate
				from MDInOrderType md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
				end
			

			-- MDInRequestState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDInRequestState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDInRequestState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDInRequestStateID as	ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDInRequestState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
					
			-- MDInventoryManagementType
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDInventoryManagementType')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as							JobID,
					'' as								ACProjectName,
					'MDInventoryManagementType' as		TableName,
					@emptyID as							MandatoryID,
					'' as								MandatoryACIdentifier,
					null as								MandatoryACURLCached,
					md.MDInventoryManagementTypeID as	ID,
					md.MDKey as							ACIdentifier,
					md.MDNameTrans as					TranslationValue,
					md.UpdateName as					UpdateName,
					md.UpdateDate as					UpdateDate
				from MDInventoryManagementType md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
					
			-- MDLabOrderPosState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDLabOrderPosState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDLabOrderPosState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDLabOrderPosStateID as	ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDLabOrderPosState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			
			-- MDLabOrderState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDLabOrderState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDLabOrderState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDLabOrderStateID as		ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDLabOrderState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			

			-- MDLabTag
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDLabTag')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as			JobID,
					'' as				ACProjectName,
					'MDLabTag' as		TableName,
					@emptyID as			MandatoryID,
					'' as				MandatoryACIdentifier,
					null as				MandatoryACURLCached,
					md.MDLabTagID as	ID,
					md.MDKey as			ACIdentifier,
					md.MDNameTrans as	TranslationValue,
					md.UpdateName as	UpdateName,
					md.UpdateDate as	UpdateDate
				from MDLabTag md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDMaintMode
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDMaintMode')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as				JobID,
					'' as					ACProjectName,
					'MDMaintMode' as		TableName,
					@emptyID as				MandatoryID,
					'' as					MandatoryACIdentifier,
					null as					MandatoryACURLCached,
					md.MDMaintModeID as		ID,
					md.MDKey as				ACIdentifier,
					md.MDNameTrans as		TranslationValue,
					md.UpdateName as		UpdateName,
					md.UpdateDate as		UpdateDate
				from MDMaintMode md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDMaintOrderPropertyState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDMaintOrderPropertyState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as							JobID,
					'' as								ACProjectName,
					'MDMaintOrderPropertyState' as		TableName,
					@emptyID as							MandatoryID,
					'' as								MandatoryACIdentifier,
					null as								MandatoryACURLCached,
					md.MDMaintOrderPropertyStateID as	ID,
					md.MDKey as							ACIdentifier,
					md.MDNameTrans as					TranslationValue,
					md.UpdateName as					UpdateName,
					md.UpdateDate as					UpdateDate
				from MDMaintOrderPropertyState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			

			-- MDMaintOrderState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDMaintOrderState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDMaintOrderState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDMaintOrderStateID as	ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDMaintOrderState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDMaterialGroup
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDMaterialGroup')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDMaterialGroup' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDMaterialGroupID as		ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDMaterialGroup md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
		

			-- MDMaterialType
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDMaterialType')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDMaterialType' as			TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDMaterialTypeID as		ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDMaterialType md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDMovementReason
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDMovementReason')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDMovementReason' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDMovementReasonID as	ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDMovementReason md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDOutOfferState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDOutOfferState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDOutOfferState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDOutOfferStateID as		ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDOutOfferState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDOutOrderPlanState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDOutOrderPlanState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as						JobID,
					'' as							ACProjectName,
					'MDOutOrderPlanState' as		TableName,
					@emptyID as       MandatoryID,
					'' as							MandatoryACIdentifier,
					null as							MandatoryACURLCached,
					md.MDOutOrderPlanStateID as		ID,
					md.MDKey as						ACIdentifier,
					md.MDNameTrans as				TranslationValue,
					md.UpdateName as				UpdateName,
					md.UpdateDate as				UpdateDate
				from MDOutOrderPlanState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDOutOrderPosState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDOutOrderPosState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDOutOrderPosState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDOutOrderPosStateID as	ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDOutOrderPosState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
					
			-- MDOutOrderState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDOutOrderState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDOutOrderState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDOutOrderStateID as		ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDOutOrderState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDOutOrderType
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDOutOrderType')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as				JobID,
					'' as					ACProjectName,
					'MDOutOrderType' as		TableName,
					@emptyID as				MandatoryID,
					'' as					MandatoryACIdentifier,
					null as					MandatoryACURLCached,
					md.MDOutOrderTypeID as	ID,
					md.MDKey as				ACIdentifier,
					md.MDNameTrans as		TranslationValue,
					md.UpdateName as		UpdateName,
					md.UpdateDate as		UpdateDate
				from MDOutOrderType md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDProcessErrorAction
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDProcessErrorAction')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as						JobID,
					'' as							ACProjectName,
					'MDProcessErrorAction' as		TableName,
					@emptyID as						MandatoryID,
					'' as							MandatoryACIdentifier,
					null as							MandatoryACURLCached,
					md.MDProcessErrorActionID as	ID,
					md.MDKey as						ACIdentifier,
					md.MDNameTrans as				TranslationValue,
					md.UpdateName as				UpdateName,
					md.UpdateDate as				UpdateDate
				from MDProcessErrorAction md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDProdOrderPartslistPosState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDProdOrderPartslistPosState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
			select
				@jobID as								JobID,
				'' as									ACProjectName,
				'MDProdOrderPartslistPosState' as		TableName,
				@emptyID as								MandatoryID,
				'' as									MandatoryACIdentifier,
				null as									MandatoryACURLCached,
				md.MDProdOrderPartslistPosStateID as	ID,
				md.MDKey as								ACIdentifier,
				md.MDNameTrans as						TranslationValue,
				md.UpdateName as						UpdateName,
				md.UpdateDate as						UpdateDate
			from MDProdOrderPartslistPosState md
				where 
				(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
				(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
				order by md.MDKey
			end

			-- MDProdOrderState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDProdOrderState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDProdOrderState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDProdOrderStateID as	ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDProdOrderState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			

			-- MDRatingComplaintType
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDRatingComplaintType')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as						JobID,
					'' as							ACProjectName,
					'MDRatingComplaintType' as		TableName,
					@emptyID as						MandatoryID,
					'' as							MandatoryACIdentifier,
					null as							MandatoryACURLCached,
					md.MDRatingComplaintTypeID as	ID,
					md.MDKey as						ACIdentifier,
					md.MDNameTrans as				TranslationValue,
					md.UpdateName as				UpdateName,
					md.UpdateDate as				UpdateDate
				from MDRatingComplaintType md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			

			-- MDReleaseState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDReleaseState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDReleaseState' as			TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDReleaseStateID as		ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDReleaseState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDReservationMode
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDReservationMode')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDReservationMode' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDReservationModeID as	ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDReservationMode md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDTermOfPayment
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDTermOfPayment')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDTermOfPayment' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDTermOfPaymentID as		ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDTermOfPayment md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDTimeRange
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDTimeRange')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as				JobID,
					'' as					ACProjectName,
					'MDTimeRange' as		TableName,
					@emptyID as				MandatoryID,
					'' as					MandatoryACIdentifier,
					null as					MandatoryACURLCached,
					md.MDTimeRangeID as		ID,
					md.MDKey as				ACIdentifier,
					md.MDNameTrans as		TranslationValue,
					md.UpdateName as		UpdateName,
					md.UpdateDate as		UpdateDate
				from MDTimeRange md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDToleranceState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDToleranceState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDToleranceState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDToleranceStateID as	ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDToleranceState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDTour
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDTour')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as			JobID,
					'' as				ACProjectName,
					'MDTour' as			TableName,
					@emptyID as			MandatoryID,
					'' as				MandatoryACIdentifier,
					null as				MandatoryACURLCached,
					md.MDTourID			ID,
					md.MDKey as			ACIdentifier,
					md.MDNameTrans as	TranslationValue,
					md.UpdateName as	UpdateName,
					md.UpdateDate as	UpdateDate
				from MDTour md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDTourplanPosState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDTourplanPosState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as						JobID,
					'' as							ACProjectName,
					'MDTourplanPosState' as			TableName,
					@emptyID as						MandatoryID,
					'' as							MandatoryACIdentifier,
					null as							MandatoryACURLCached,
					md.MDTourplanPosStateID as		ID,
					md.MDKey as						ACIdentifier,
					md.MDNameTrans as				TranslationValue,
					md.UpdateName as				UpdateName,
					md.UpdateDate as				UpdateDate
				from MDTourplanPosState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			

			-- MDTourplanState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDTourplanState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDTourplanState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDTourplanStateID as		ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDTourplanState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDTransportMode
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDTransportMode')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as				JobID,
					'' as					ACProjectName,
					'MDTransportMode' as	TableName,
					@emptyID as				MandatoryID,
					'' as					MandatoryACIdentifier,
					null as					MandatoryACURLCached,
					md.MDTransportModeID as	ID,
					md.MDKey as				ACIdentifier,
					md.MDNameTrans as		TranslationValue,
					md.UpdateName as		UpdateName,
					md.UpdateDate as		UpdateDate
				from MDTransportMode md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end

			-- MDVisitorCardState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDVisitorCardState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDVisitorCardState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDVisitorCardStateID as	ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDVisitorCardState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			

			-- MDVisitorVoucherState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDVisitorVoucherState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as						JobID,
					'' as							ACProjectName,
					'MDVisitorVoucherState' as		TableName,
					@emptyID as						MandatoryID,
					'' as							MandatoryACIdentifier,
					null as							MandatoryACURLCached,
					md.MDVisitorVoucherStateID as	ID,
					md.MDKey as						ACIdentifier,
					md.MDNameTrans as				TranslationValue,
					md.UpdateName as				UpdateName,
					md.UpdateDate as				UpdateDate
				from MDVisitorVoucherState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
			end
			

			-- MDZeroStockState
			if(@searchClassACIdentifier is null or @searchClassACIdentifier = 'MDZeroStockState')
			begin
				insert into #translationViewResults(JobID, ACProjectName, TableName, MandatoryID, MandatoryACIdentifier, MandatoryACURLCached, ID, ACIdentifier, TranslationValue, UpdateName, UpdateDate)
				select
					@jobID as					JobID,
					'' as						ACProjectName,
					'MDZeroStockState' as		TableName,
					@emptyID as					MandatoryID,
					'' as						MandatoryACIdentifier,
					null as						MandatoryACURLCached,
					md.MDZeroStockStateID as	ID,
					md.MDKey as					ACIdentifier,
					md.MDNameTrans as			TranslationValue,
					md.UpdateName as			UpdateName,
					md.UpdateDate as			UpdateDate
				from MDZeroStockState md
					where 
					(@searchACIdentifier is null or lower(md.MDKey) like '%' + @searchACIdentifier + '%') and
					(@searchTranslation is null or LOWER(md.MDNameTrans) like '%' + @searchTranslation + '%') and
					(@notHaveInTranslation is null or LOWER(md.MDNameTrans) not like '%' + @notHaveInTranslation + '%')
					order by md.MDKey
				end
			
		-- ## end MDTables
		end
			-- output resutl
			select 
				ACProjectName,
				TableName,
				MandatoryID,
				MandatoryACIdentifier,
				MandatoryACURLCached,
				ID, 
				ACIdentifier, 
				TranslationValue,
				isnull(UpdateName, '') as UpdateName,
				isnull(UpdateDate, getdate()) as UpdateDate
			from  #translationViewResults 
				where JobID = @jobID
			order by TableName, ACIdentifier;
			-- delete result of work
			delete from #translationViewResults where JobID = @jobID;
			
			set @cnt = (select count(*) from #translationViewResults);
			-- delete temp table if not more used
			if OBJECT_ID('dbo.#translationViewResults') IS not NULL and @cnt = 0
			begin
				drop table #translationViewResults;
			end

			-- end code
	commit tran;
end