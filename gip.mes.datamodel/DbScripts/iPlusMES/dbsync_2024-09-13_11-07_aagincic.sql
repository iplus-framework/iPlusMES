update ACClass 
	set 
		ACIdentifier = 'BSOFacilityReservationOverview', 
		AssemblyQualifiedName = 'gip.bso.masterdata.BSOFacilityReservationOverview, gip.core.reporthandler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' 
	where 
		ACIdentifier = 'BSOFacilityReservationOverview' 
		and AssemblyQualifiedName <> 'gip.bso.masterdata.BSOFacilityReservationOverview, gip.core.reporthandler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null';
