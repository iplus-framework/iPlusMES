ALTER TABLE [dbo].[MDFacilityInventoryPosState] ALTER COLUMN [InsertName] varchar(20) not null;
ALTER TABLE [dbo].[MDFacilityInventoryPosState] ALTER COLUMN [UpdateName] varchar(20) not null;
GO
declare @existPosted int;
set @existPosted = (select COUNT(*) from [MDFacilityInventoryPosState] where MDKey = 'Posted');
if @existPosted = 0
begin
	INSERT [dbo].[MDFacilityInventoryPosState] ([MDFacilityInventoryPosStateID], [MDFacilityInventoryPosStateIndex], [MDNameTrans], [SortIndex], [XMLConfig], [IsDefault], [InsertName], [InsertDate], [UpdateName], [UpdateDate], [MDKey]) VALUES (N'582b5ac5-ec81-4403-bb67-6c4bed531cf3', 5, N'en{''Posted''}de{''Gesendet''}', 0, NULL, 0, N'aagincic', CAST(N'2022-02-21T12:30:49.953' AS DateTime), N'aagincic', CAST(N'2022-02-21T12:30:54.210' AS DateTime), N'Posted')
	INSERT [dbo].[MDFacilityInventoryState] ([MDFacilityInventoryStateID], [MDFacilityInventoryStateIndex], [MDNameTrans], [SortIndex], [XMLConfig], [IsDefault], [InsertName], [InsertDate], [UpdateName], [UpdateDate], [MDKey]) VALUES (N'91150481-b0ea-461d-bbb0-8cbf15bd5dc2', 4, N'en{''Posted''}de{''Gesendet''}', 0, NULL, 0, N'aagincic', CAST(N'2022-02-21T12:28:49.420' AS DateTime), N'aagincic', CAST(N'2022-02-21T12:29:55.020' AS DateTime), N'Posted')
end