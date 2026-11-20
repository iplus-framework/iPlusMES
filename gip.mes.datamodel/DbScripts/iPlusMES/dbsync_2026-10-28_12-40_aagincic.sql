IF NOT EXISTS (
	SELECT 1
	FROM sys.indexes
	WHERE [name] = 'UX_FacilityInventory_FacilityCharge'
		AND [object_id] = OBJECT_ID(N'[dbo].[FacilityInventoryPos]')
)
BEGIN
	IF NOT EXISTS (
		SELECT 1
		FROM dbo.FacilityInventoryPos
		GROUP BY FacilityInventoryID, FacilityChargeID
		HAVING COUNT(*) > 1
	)
	BEGIN
		CREATE UNIQUE NONCLUSTERED INDEX [UX_FacilityInventory_FacilityCharge]
		ON [dbo].[FacilityInventoryPos]
		(
			[FacilityInventoryID] ASC,
			[FacilityChargeID] ASC
		)
		WITH
		(
			PAD_INDEX = OFF,
			STATISTICS_NORECOMPUTE = OFF,
			SORT_IN_TEMPDB = OFF,
			IGNORE_DUP_KEY = OFF,
			DROP_EXISTING = OFF,
			ONLINE = OFF,
			ALLOW_ROW_LOCKS = ON,
			ALLOW_PAGE_LOCKS = ON
		)
		ON [PRIMARY]
	END
	ELSE
	BEGIN
		RAISERROR ('UX_FacilityInventory_FacilityCharge not created: dbo.FacilityInventoryPos contains duplicate (FacilityInventoryID, FacilityChargeID) pairs. Resolve duplicates manually, then re-run the update.', 0, 1) WITH NOWAIT;
	END
END
GO