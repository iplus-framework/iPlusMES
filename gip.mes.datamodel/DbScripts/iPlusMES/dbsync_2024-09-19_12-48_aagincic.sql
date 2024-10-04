update ACClass 
	set 
		ACIdentifier = 'BSOFacilityReservationOverview', 
		AssemblyQualifiedName = 'gip.bso.masterdata.BSOFacilityReservationOverview, gip.bso.masterdata, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' 
	where 
		ACIdentifier = 'BSOFacilityReservationOverview' 
		and AssemblyQualifiedName <> 'gip.bso.masterdata.BSOFacilityReservationOverview, gip.bso.masterdata, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null';


update ACClass 
	set 
		ACIdentifier = 'BSOFacilityReservation', 
		AssemblyQualifiedName = 'gip.bso.masterdata.BSOFacilityReservation, gip.bso.masterdata, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' 
	where 
		ACIdentifier = 'BSOFacilityReservation' 
		and AssemblyQualifiedName <> 'gip.bso.masterdata.BSOFacilityReservation, gip.bso.masterdata, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null';
