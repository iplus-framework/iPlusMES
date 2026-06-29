update ACClass 
	set 
		AssemblyQualifiedName = 'gip.mes.facility.MachineItem, gip.mes.facility, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' 
	where 
		ACIdentifier = 'MachineItem' 
		and assemblyQualifiedName like 'gip.bso.masterdata.MachineItem%'

update ACClass 
	set 
		AssemblyQualifiedName = 'gip.mes.facility.RuleGroup, gip.mes.facility, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' 
	where 
		ACIdentifier = 'RuleGroup' 
		and assemblyQualifiedName like 'gip.bso.masterdata.RuleGroup%'

update ACClass 
	set 
		AssemblyQualifiedName = 'gip.mes.facility.RuleSelection, gip.mes.facility, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' 
	where 
		ACIdentifier = 'RuleSelection' 
		and assemblyQualifiedName like 'gip.bso.masterdata.RuleSelection%'