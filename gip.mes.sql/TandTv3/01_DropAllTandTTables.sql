-- v2 Step Item table
drop table TandT_TempPos
drop table TandT_StepLot
drop table TandT_StepItemRelation
drop table TandT_StepItem
drop table TandT_Step

-- job main tables (filtering)
drop table TandT_JobMaterial;
drop table TandT_Job;

-- v2 lookup tables
drop table TandT_Operation;
drop table TandT_RelationType;
drop table TandT_ItemType;

-- lookup tables (common)
drop table TandT_MaterialPosType;
drop table TandT_TrackingStyle;


IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'udp_TandT_JobDelete')
	BEGIN
		DROP  procedure  dbo.[udp_TandT_JobDelete]
	END
GO