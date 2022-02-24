CREATE TABLE [dbo].[TandTv3MixPointPickingPos](
	[TandTv3MixPointPickingPosID] [uniqueidentifier] NOT NULL,
	[TandTv3MixPointID] [uniqueidentifier] NOT NULL,
	[PickingPosID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TandTv3MixPointPickingPos] PRIMARY KEY CLUSTERED 
(
	[TandTv3MixPointPickingPosID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TandTv3MixPointPickingPos]  WITH CHECK ADD  CONSTRAINT [FK_TandTv3MixPointPickingPos_PickingPosID] FOREIGN KEY([PickingPosID]) REFERENCES [dbo].[PickingPos] ([PickingPosID])
ALTER TABLE [dbo].[TandTv3MixPointPickingPos]  WITH CHECK ADD  CONSTRAINT [FK_TandTv3MixPointPickingPos_TandTv3MixPointID] FOREIGN KEY([TandTv3MixPointID]) REFERENCES [dbo].[TandTv3MixPoint] ([TandTv3MixPointID])
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'udpTandTv3FilterTrackingDelete')
	BEGIN
		DROP  procedure  dbo.[udpTandTv3FilterTrackingDelete]
	END
GO
CREATE PROCEDURE [dbo].[udpTandTv3FilterTrackingDelete]
				
	@filterTrackingID uniqueidentifier
	
AS
begin
	begin tran
	-- declaration
	declare @collectIDs table
	(
		ID uniqueidentifier,
		TypeName nvarchar(50)
	)
	declare @tableName_TandTv3_MixPoint nvarchar(50);
	declare @tableName_TandTv3_Step nvarchar(50);
	-- setup constants
	set @tableName_TandTv3_MixPoint = N'TandTv3MixPoint';
	set @tableName_TandTv3_Step = N'TandTv3Step';
	-- insert affected IDs from TandTv3_MixPoint and TandTv3_Step
	insert into @collectIDs(ID, TypeName) 
	select 
		st.TandTv3StepID as ID,
		@tableName_TandTv3_Step as TypeName
	from [TandTv3Step] st
	where
		@filterTrackingID is null or st.TandTv3FilterTrackingID = @filterTrackingID;
	insert into @collectIDs(ID, TypeName) 
	select 
		mixPoint.TandTv3MixPointID as ID,
		@tableName_TandTv3_MixPoint as TypeName
	from [TandTv3MixPoint] mixPoint
	where
		mixPoint.TandTv3StepID in
		(select ID from @collectIDs where TypeName = @tableName_TandTv3_Step);
	-- deleting MixPoint Relative
	delete from TandTv3MixPointDeliveryNotePos where TandTv3MixPointID in (select ID from @collectIDs where TypeName = @tableName_TandTv3_MixPoint);
	delete from TandTv3MixPointFacility where TandTv3MixPointID in (select ID from @collectIDs where TypeName = @tableName_TandTv3_MixPoint);
	delete from TandTv3MixPointFacilityBookingCharge where TandTv3MixPointID in (select ID from @collectIDs where TypeName = @tableName_TandTv3_MixPoint);
	delete from TandTv3MixPointFacilityPreBooking where TandTv3MixPointID in (select ID from @collectIDs where TypeName = @tableName_TandTv3_MixPoint);
	delete from TandTv3MixPointFacilityLot where TandTv3MixPointID in (select ID from @collectIDs where TypeName = @tableName_TandTv3_MixPoint);
	delete from TandTv3MixPointInOrderPos where TandTv3MixPointID in (select ID from @collectIDs where TypeName = @tableName_TandTv3_MixPoint);
	delete from TandTv3MixPointOutOrderPos where TandTv3MixPointID in (select ID from @collectIDs where TypeName = @tableName_TandTv3_MixPoint);
	delete from TandTv3MixPointPickingPos where TandTv3MixPointID in (select ID from @collectIDs where TypeName = @tableName_TandTv3_MixPoint);
	delete from TandTv3MixPointProdOrderPartslistPos where TandTv3MixPointID in (select ID from @collectIDs where TypeName = @tableName_TandTv3_MixPoint);
	delete from TandTv3MixPointProdOrderPartslistPosRelation where TandTv3MixPointID in (select ID from @collectIDs where TypeName = @tableName_TandTv3_MixPoint);
	delete from TandTv3MixPointRelation where SourceTandTv3MixPointID in (select ID from @collectIDs where TypeName = @tableName_TandTv3_MixPoint);
	-- deleting MixPoint
	delete from TandTv3MixPoint where TandTv3MixPointID in (select ID from @collectIDs where TypeName = @tableName_TandTv3_MixPoint);
	-- deleting steps
	delete from TandTv3Step where TandTv3StepID in (select ID from @collectIDs where TypeName = @tableName_TandTv3_Step);
	-- delete filter definition
	delete from TandTv3FilterTrackingMaterial where @filterTrackingID is null or TandTv3FilterTrackingID = @filterTrackingID
	delete from TandTv3FilterTracking where @filterTrackingID is null or TandTv3FilterTrackingID = @filterTrackingID
	commit tran
end