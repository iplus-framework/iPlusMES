update [dbo].[Material] set [MRPProcedureIndex] = 0
GO
alter table [dbo].[Material]  alter column [MRPProcedureIndex] smallint not null
GO
ALTER TABLE [dbo].[PlanningMR] ADD [PlanningMRPhaseIndex] smallint NULL
GO
update [dbo].[PlanningMR] set [PlanningMRPhaseIndex] = 0;
GO
ALTER TABLE [dbo].[PlanningMR] alter column [PlanningMRPhaseIndex] smallint not null
GO
update ACClass 
	set 
		AssemblyQualifiedName = 'gip.mes.datamodel.MRPProcedure, gip.mes.datamodel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' 
	where 
		ACIdentifier = 'MRPProcedure' 
		and assemblyQualifiedName like 'gip.mes.datamodel.GlobalApp+MRPProcedure%'