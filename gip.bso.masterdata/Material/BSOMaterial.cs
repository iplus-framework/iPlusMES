// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOMaterial.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Data.Objects;
using static gip.core.datamodel.Global;
using gip.mes.facility;
using gip.core.media;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Businessobject or App for managing the material master data.
    /// To search for records, enter the search string in the SearchWord property. 
    /// The database result is copied to the MaterialList property. 
    /// Then call the NavigateFirst() method to set CurrentMaterial with the first record in the list. 
    /// CurrentMaterial is used to display and edit the currently selected record. 
    /// Property changes should always be made to CurrentMaterial and when all field values ​​have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record. 
    /// The New() method creates a new record and assigns the new entity object to the CurrentMaterial property. 
    /// Fill in all required fields before saving. Use the Delete() method to delete the material provided there are no foreign key relationships from other tables. 
    /// Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load method updates the CurrentMaterial object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOMaterial&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Material'}de{'Material'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Material.ClassName, 
        Description = @"Businessobject or App for managing the material master data.
        To search for records, enter the search string in the SearchWord property. 
        The database result is copied to the MaterialList property. 
        Then call the NavigateFirst() method to set CurrentMaterial with the first record in the list. 
        CurrentMaterial is used to display and edit the currently selected record. 
        Property changes should always be made to CurrentMaterial and when all field values ​​have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record. 
        The New() method creates a new record and assigns the new entity object to the CurrentMaterial property. 
        Fill in all required fields before saving. Use the Delete() method to delete the material provided there are no foreign key relationships from other tables.
        Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load method updates the CurrentMaterial object with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOMaterial&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    [ACQueryInfo(Const.PackName_VarioMaterial, Const.QueryPrefix + "AssociatedPartslistPos", "en{'Storage Bin'}de{'Lagerplatz'}", typeof(PartslistPos), nameof(PartslistPos), "Sequence", "Sequence")]
    public partial class BSOMaterial : BSOMaterialExplorer
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOMaterial"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOMaterial(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _RemoteMaterialForwarder = new ACPropertyConfigValue<string>(this, nameof(RemoteMaterialForwarder), "");
        }

        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            AccessTargetMaterial.NavSearch();
            AccessReplacementMaterial.NavSearch();

            _VarioConfigManager = ConfigManagerIPlus.ACRefToServiceInstance(this);
            _FacilityOEEManager = ACFacilityOEEManager.ACRefToServiceInstance(this);
            Search();

            if (BSOMedia_Child != null && BSOMedia_Child.Value != null)
                BSOMedia_Child.Value.OnDefaultImageDelete += Value_OnDefaultImageDelete;

            FilterAssociatedPosMaterial.SearchWord = null;
            if (CurrentMaterial != null)
                FilterAssociatedPosMaterial.SearchWord = CurrentMaterial.MaterialNo;

            IntermediateProductsList = new System.Collections.ObjectModel.ObservableCollection<Material>(DatabaseApp.Material.Where(c => c.IsIntermediate));

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._AccessMaterialCalculation = null;
            _AccessTargetMaterial = null;
            this._AccessMaterialUnit = null;
            //this._ACConfigList = null;
            this._AmbientTemperature = null;
            this._AmbientTemperature3 = null;
            this._CloneFrom = null;
            this._ConvertTest2AmbVol = null;
            this._ConvertTest2Mass = null;
            this._ConvertTest2Ref15Vol = null;
            this._ConvertTestDensity = null;
            this._ConvertTestMass3 = null;
            this._ConvertTestRef15Vol3 = null;
            this._CurrentACConfig = null;
            this._CurrentFacilityCharge = null;
            this._CurrentMaterialCalculation = null;
            this._CurrentMaterialUnit = null;
            this._CurrentNewMaterialUnit = null;
            this._CurrentUnitConvertTest = null;
            this._SelectedACConfig = null;
            this._SelectedFacilityCharge = null;
            this._SelectedMaterialCalculation = null;
            this._SelectedTranslation = null;
            this._SelectedUnitConvertTest = null;
            if (_VarioConfigManager != null)
                ConfigManagerIPlus.DetachACRefFromServiceInstance(this, _VarioConfigManager);
            _VarioConfigManager = null;
            if (_FacilityOEEManager != null)
                ACFacilityOEEManager.DetachACRefFromServiceInstance(this, _FacilityOEEManager);
            _FacilityOEEManager = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessMaterialUnit != null)
            {
                _AccessMaterialUnit.ACDeInit(false);
                _AccessMaterialUnit = null;
            }
            if (_AccessMaterialCalculation != null)
            {
                _AccessMaterialCalculation.ACDeInit(false);
                _AccessMaterialCalculation = null;
            }
            if (BSOMedia_Child != null && BSOMedia_Child.Value != null)
                BSOMedia_Child.Value.OnDefaultImageDelete -= Value_OnDefaultImageDelete;
            return b;
        }

        private void Value_OnDefaultImageDelete(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(MaterialList));
        }

        #endregion

        #region Managers

        protected ACRef<ConfigManagerIPlus> _VarioConfigManager = null;
        public ConfigManagerIPlus VarioConfigManager
        {
            get
            {
                if (_VarioConfigManager == null)
                    return null;
                return _VarioConfigManager.ValueT;
            }
        }

        protected ACRef<ACFacilityOEEManager> _FacilityOEEManager = null;
        public ACFacilityOEEManager FacilityOEEManager
        {
            get
            {
                if (_FacilityOEEManager == null)
                    return null;
                return _FacilityOEEManager.ValueT;
            }
        }

        #endregion

        #region Child BSO

        ACChildItem<BSOFacilityExplorer> _BSOFacilityExplorer_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo(nameof(BSOFacilityExplorer_Child), typeof(BSOFacilityExplorer))]
        public ACChildItem<BSOFacilityExplorer> BSOFacilityExplorer_Child
        {
            get
            {
                if (_BSOFacilityExplorer_Child == null)
                    _BSOFacilityExplorer_Child = new ACChildItem<BSOFacilityExplorer>(this, nameof(BSOFacilityExplorer_Child));
                return _BSOFacilityExplorer_Child;
            }
        }

        ACChildItem<BSOMedia> _BSOMedia_Child;
        [ACPropertyInfo(9999)]
        [ACChildInfo(nameof(BSOMedia_Child), typeof(BSOMedia))]
        public ACChildItem<BSOMedia> BSOMedia_Child
        {
            get
            {
                if (_BSOMedia_Child == null)
                    _BSOMedia_Child = new ACChildItem<BSOMedia>(this, nameof(BSOMedia_Child));
                return _BSOMedia_Child;
            }
        }

        #endregion

        #region BSO->ACProperty

        #region BSO->ACProperty->Material

        /// <summary>
        /// Gets or sets the current material.
        /// </summary>
        /// <value>The current material.</value>
        [ACPropertyCurrent(9999, Material.ClassName)]
        public override Material CurrentMaterial
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                if (AccessPrimary.Current != value)
                {
                    AccessPrimary.Current = value;
                    CurrentMaterialUnit = null;
                    if (value != null)
                    {
                        if (Math.Abs(value.Density - 0) <= Double.Epsilon)
                            ConvertTestDensity = null;
                        else
                            ConvertTestDensity = null;
                        //ConvertTestDensity = value.Density; // Wegen ELG
                    }
                    if (_CloneFrom == null)
                    {
                        AmbientTemperature = null;
                        ConvertTestAmbVol = 0;
                        ConvertTestRef15Vol = 0;
                        ConvertTestMass = 0;
                        ConvertTestMassVac = 0;

                        ConvertTest2AmbVol = null;
                        ConvertTest2Ref15Vol = null;
                        ConvertTest2Mass = null;
                        ConvertTest2MassVac = 0;
                        AmbientTemperature2 = 0;
                        ConvertTest2Density = 0;
                        ConvertTest2DensityAmb = 0;

                        AmbientTemperature3 = null;
                        ConvertTestRef15Vol3 = null;
                        ConvertTestMass3 = null;
                        ConvertTest3Density = 0;
                        ConvertTest3AmbVol = 0;
                    }
                    else
                    {
                        var bsoMaterial = _CloneFrom as BSOMaterial;
                        AmbientTemperature = bsoMaterial.AmbientTemperature;
                        ConvertTestAmbVol = bsoMaterial.ConvertTestAmbVol;
                        ConvertTestRef15Vol = bsoMaterial.ConvertTestRef15Vol;
                        ConvertTestMass = bsoMaterial.ConvertTestMass;
                        ConvertTestMassVac = bsoMaterial.ConvertTestMassVac;
                        ConvertTestDensity = bsoMaterial.ConvertTestDensity;

                        ConvertTest2AmbVol = bsoMaterial.ConvertTest2AmbVol;
                        ConvertTest2Ref15Vol = bsoMaterial.ConvertTest2Ref15Vol;
                        ConvertTest2Mass = bsoMaterial.ConvertTest2Mass;
                        ConvertTest2MassVac = bsoMaterial.ConvertTest2MassVac;
                        AmbientTemperature2 = bsoMaterial.AmbientTemperature2;
                        ConvertTest2Density = bsoMaterial.ConvertTest2Density;
                        ConvertTest2DensityAmb = bsoMaterial.ConvertTest2DensityAmb;

                        AmbientTemperature3 = bsoMaterial.AmbientTemperature3;
                        ConvertTestRef15Vol3 = bsoMaterial.ConvertTestRef15Vol3;
                        ConvertTestMass3 = bsoMaterial.ConvertTestMass3;
                        ConvertTest3Density = bsoMaterial.ConvertTest3Density;
                        ConvertTest3AmbVol = bsoMaterial.ConvertTest3AmbVol;
                    }


                    try
                    {
                        if (
                            value != null
                            && BSOMedia_Child != null
                            && BSOMedia_Child.Value != null
                            )
                        {
                            if (value != null && value.EntityState != System.Data.EntityState.Added)
                            {
                                BSOMedia_Child.Value.LoadMedia(value);
                            }
                            else
                            {
                                BSOMedia_Child.Value.Clean();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Messages.Exception(this, e.Message, true);
                    }

                    OnPropertyChanged(nameof(CurrentMaterial));
                    OnPropertyChanged(nameof(ACConfigList));
                    OnPropertyChanged(nameof(FacilityChargeList));
                    OnPropertyChanged(nameof(MaterialUnitList));
                    OnPropertyChanged(nameof(ConvertableUnits));
                    OnPropertyChanged(nameof(UnitConvertTestList));
                    OnPropertyChanged(nameof(TranslationList));
                    OnPropertyChanged(nameof(AvailablePWMethodNodes));
                    OnPropertyChanged(nameof(AssignedPWMethodNodes));
                    if (value != null
                        && !String.IsNullOrEmpty(RemoteMaterialForwarder))
                    {
                        if (_VisitedMaterials == null)
                            _VisitedMaterials = new List<Material>();
                        if (!_VisitedMaterials.Contains(value))
                            _VisitedMaterials.Add(value);
                    }
                }
            }
        }

        public override object Clone()
        {
            BSOMaterial clone = base.Clone() as BSOMaterial;
            clone._CloneFrom = this;
            return clone;
        }

        #endregion

        #region BSO->ACProperty->MaterialUnit
        /// <summary>
        /// The _ access material unit
        /// </summary>
        ACAccess<MaterialUnit> _AccessMaterialUnit;
        /// <summary>
        /// Gets the access material unit.
        /// </summary>
        /// <value>The access material unit.</value>
        [ACPropertyAccess(9999, "MaterialUnit")]
        public ACAccess<MaterialUnit> AccessMaterialUnit
        {
            get
            {
                if (_AccessMaterialUnit == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + MaterialUnit.ClassName) as ACQueryDefinition;
                    _AccessMaterialUnit = acQueryDefinition.NewAccess<MaterialUnit>("MaterialUnit", this);
                }
                return _AccessMaterialUnit;
            }
        }

        /// <summary>
        /// The _ current material unit
        /// </summary>
        MaterialUnit _CurrentMaterialUnit;
        /// <summary>
        /// Gets or sets the current material unit.
        /// </summary>
        /// <value>The current material unit.</value>
        [ACPropertyCurrent(9999, nameof(MaterialUnit))]
        public MaterialUnit CurrentMaterialUnit
        {
            get
            {
                return _CurrentMaterialUnit;
            }
            set
            {
                if (_CurrentMaterialUnit != value)
                {
                    _CurrentMaterialUnit = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the material unit list.
        /// </summary>
        /// <value>The material unit list.</value>
        [ACPropertyList(9999, nameof(MaterialUnit))]
        public IEnumerable<MaterialUnit> MaterialUnitList
        {
            get
            {
                if (CurrentMaterial == null)
                {
                    CurrentMaterialUnit = null;
                    return null;
                }
                if (CurrentMaterial.MaterialUnit_Material.Count <= 0)
                    return null;
                return CurrentMaterial.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0).ToList();
            }
        }

        MaterialUnit _CurrentNewMaterialUnit;
        [ACPropertyCurrent(9999, nameof(NewMaterialUnit))]
        public MaterialUnit CurrentNewMaterialUnit
        {
            get
            {
                return _CurrentNewMaterialUnit;
            }
            set
            {
                _CurrentNewMaterialUnit = value;
                OnPropertyChanged();
            }
        }

        [ACPropertyList(9999, nameof(ConvertableUnits))]
        public IEnumerable<MDUnit> ConvertableUnits
        {
            get
            {
                if (CurrentMaterial == null)
                    return null;
                if (CurrentMaterial.BaseMDUnit == null)
                    return null;
                List<MDUnit> convertableUnits = new List<MDUnit>();
                // Erstelle Liste mit allen Dimensionslosen Einheiten, die nicht bereits als Alternativ-Einheit angelegt worden sind
                var query = DatabaseApp.MDUnit.Where(c => c.MDUnitID != CurrentMaterial.BaseMDUnit.MDUnitID);
                if (!DatabaseApp.IsChanged)
                    (query as ObjectQuery).MergeOption = MergeOption.OverwriteChanges;
                if (query.Any())
                {
                    foreach (MDUnit unit in query)
                    {
                        if (!CurrentMaterial.MaterialUnit_Material.Where(c => c.ToMDUnitID == unit.MDUnitID).Any())
                        {
                            convertableUnits.Add(unit);
                        }
                    }
                }
                foreach (MDUnit unit in CurrentMaterial.BaseMDUnit.ConvertableUnits)
                {
                    if (!convertableUnits.Contains(unit) && !CurrentMaterial.MaterialUnit_Material.Where(c => c.ToMDUnitID == unit.MDUnitID).Any())
                    {
                        convertableUnits.Add(unit);
                    }
                }
                return convertableUnits;
            }
        }
        #endregion

        #region BSO->ACProperty->MaterialCalculation
        /// <summary>
        /// The _ access material calculation
        /// </summary>
        ACAccess<MaterialCalculation> _AccessMaterialCalculation;
        /// <summary>
        /// Gets the access material calculation.
        /// </summary>
        /// <value>The access material calculation.</value>
        [ACPropertyAccess(9999, "MaterialCalculation")]
        public ACAccess<MaterialCalculation> AccessMaterialCalculation
        {
            get
            {
                if (_AccessMaterialCalculation == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + MaterialCalculation.ClassName) as ACQueryDefinition;
                    _AccessMaterialCalculation = acQueryDefinition.NewAccess<MaterialCalculation>("MaterialCalculation", this);
                }
                return _AccessMaterialCalculation;
            }
        }

        /// <summary>
        /// The _ current material calculation
        /// </summary>
        MaterialCalculation _CurrentMaterialCalculation;
        /// <summary>
        /// Gets or sets the current material calculation.
        /// </summary>
        /// <value>The current material calculation.</value>
        [ACPropertyCurrent(9999, nameof(MaterialCalculation))]
        public MaterialCalculation CurrentMaterialCalculation
        {
            get
            {
                return _CurrentMaterialCalculation;
            }
            set
            {
                if (_CurrentMaterialCalculation != value)
                {
                    _CurrentMaterialCalculation = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The _ selected material calculation
        /// </summary>
        MaterialCalculation _SelectedMaterialCalculation;
        /// <summary>
        /// Gets or sets the selected material calculation.
        /// </summary>
        /// <value>The selected material calculation.</value>
        [ACPropertySelected(9999, nameof(MaterialCalculation))]
        public MaterialCalculation SelectedMaterialCalculation
        {
            get
            {
                return _SelectedMaterialCalculation;
            }
            set
            {
                if (_SelectedMaterialCalculation != value)
                {
                    _SelectedMaterialCalculation = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the material calculation list.
        /// </summary>
        /// <value>The material calculation list.</value>
        [ACPropertyList(9999, nameof(MaterialCalculation))]
        public IEnumerable<MaterialCalculation> MaterialCalculationList
        {
            get
            {
                if (CurrentMaterial == null)
                {
                    CurrentMaterialCalculation = null;
                    return null;
                }
                return CurrentMaterial.MaterialCalculation_Material.OrderByDescending(c => c.ValidFromDate);
            }
        }
        #endregion

        #region BSO->ACProperty->ACConfig
        /// <summary>
        /// The _ current AC config
        /// </summary>
        MaterialConfig _CurrentACConfig;
        /// <summary>
        /// Gets or sets the current AC config.
        /// </summary>
        /// <value>The current AC config.</value>
        [ACPropertyCurrent(9999, "ACConfig")]
        public MaterialConfig CurrentACConfig
        {
            get
            {
                return _CurrentACConfig;
            }
            set
            {
                _CurrentACConfig = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The _ AC config list
        /// </summary>
        [ACPropertyList(9999, "ACConfig")]
        public IEnumerable<MaterialConfig> ACConfigList
        {
            get
            {
                if (CurrentMaterial == null) return null;
                return CurrentMaterial.MaterialConfig_Material;
            }
        }

        /// <summary>
        /// The _ selected AC config
        /// </summary>
        MaterialConfig _SelectedACConfig;
        /// <summary>
        /// Gets or sets the selected AC config.
        /// </summary>
        /// <value>The selected AC config.</value>
        [ACPropertySelected(9999, "ACConfig")]
        public MaterialConfig SelectedACConfig
        {
            get
            {
                return _SelectedACConfig;
            }
            set
            {
                _SelectedACConfig = value;
                OnPropertyChanged();
            }
        }

        protected ACPropertyConfigValue<string> _RemoteMaterialForwarder;
        [ACPropertyConfig("en{'ACUrl of RemoteMaterialForwarder'}de{'ACUrl of RemoteMaterialForwarder'}")]
        public string RemoteMaterialForwarder
        {
            get
            {
                return _RemoteMaterialForwarder.ValueT;
            }
            set
            {
                _RemoteMaterialForwarder.ValueT = value;
            }
        }

        #endregion

        #region BSO->ACProperty->FacilityCharge
        /// <summary>
        /// The _ selected facility charge
        /// </summary>
        FacilityCharge _SelectedFacilityCharge;
        /// <summary>
        /// Gets or sets the selected facility charge.
        /// </summary>
        /// <value>The selected facility charge.</value>
        [ACPropertySelected(9999, FacilityCharge.ClassName)]
        public FacilityCharge SelectedFacilityCharge
        {
            get
            {
                return _SelectedFacilityCharge;
            }
            set
            {
                _SelectedFacilityCharge = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The _ current facility charge
        /// </summary>
        FacilityCharge _CurrentFacilityCharge;
        /// <summary>
        /// Gets or sets the current facility charge.
        /// </summary>
        /// <value>The current facility charge.</value>
        [ACPropertyCurrent(9999, FacilityCharge.ClassName)]
        public FacilityCharge CurrentFacilityCharge
        {
            get
            {
                return _CurrentFacilityCharge;
            }
            set
            {
                _CurrentFacilityCharge = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the facility charge list.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(9999, FacilityCharge.ClassName)]
        public IEnumerable<FacilityCharge> FacilityChargeList
        {
            get
            {
                if (CurrentMaterial == null)
                    return null;
                return CurrentMaterial.FacilityCharge_Material;
            }
        }
        #endregion

        #region Conversion-Test
        private double _ConvertTestInput;
        [ACPropertyInfo(9999, "Conversiontest", "en{'Input'}de{'Eingabe'}")]
        public double ConvertTestInput
        {
            get
            {
                return _ConvertTestInput;
            }
            set
            {
                _ConvertTestInput = value;
                OnPropertyChanged();
            }
        }

        private double _ConvertTestOutput;
        [ACPropertyInfo(9999, "Conversiontest", "en{'Output'}de{'Ausgabe'}")]
        public double ConvertTestOutput
        {
            get
            {
                return _ConvertTestOutput;
            }
            set
            {
                _ConvertTestOutput = value;
                OnPropertyChanged();
            }
        }

        MDUnit _SelectedUnitConvertTest;
        [ACPropertySelected(9999, "Conversiontest", "en{'To Unit'}de{'Nach Einheit'}")]
        public MDUnit SelectedUnitConvertTest
        {
            get
            {
                return _SelectedUnitConvertTest;
            }
            set
            {
                _SelectedUnitConvertTest = value;
                OnPropertyChanged();
            }
        }

        MDUnit _CurrentUnitConvertTest;
        [ACPropertyCurrent(9999, "Conversiontest", "en{'To Unit'}de{'Nach Einheit'}")]
        public MDUnit CurrentUnitConvertTest
        {
            get
            {
                return _CurrentUnitConvertTest;
            }
            set
            {
                _CurrentUnitConvertTest = value;
                OnPropertyChanged();
            }
        }

        [ACPropertyList(9999, "Conversiontest")]
        public IEnumerable<MDUnit> UnitConvertTestList
        {
            get
            {
                if (CurrentMaterial == null)
                    return null;
                return CurrentMaterial.MDUnitList;
            }
        }
        #endregion

        #region FacilityMaterial
        private FacilityMaterial _SelectedFacilityMaterial;
        /// <summary>
        /// Selected property for FacilityMaterial
        /// </summary>
        /// <value>The selected FacilityMaterial</value>
        [ACPropertySelected(9999, "FacilityMaterial", "en{'TODO: FacilityMaterial'}de{'TODO: FacilityMaterial'}")]
        public FacilityMaterial SelectedFacilityMaterial
        {
            get
            {
                return _SelectedFacilityMaterial;
            }
            set
            {
                if (_SelectedFacilityMaterial != value)
                {
                    _SelectedFacilityMaterial = value;
                    OnPropertyChanged();
                }
            }
        }


        private List<FacilityMaterial> _FacilityMaterialList;
        /// <summary>
        /// List property for FacilityMaterial
        /// </summary>
        /// <value>The FacilityMaterial list</value>
        [ACPropertyList(9999, "FacilityMaterial")]
        public List<FacilityMaterial> FacilityMaterialList
        {
            get
            {
                if (_FacilityMaterialList == null)
                    _FacilityMaterialList = LoadFacilityMaterialList();
                return _FacilityMaterialList;
            }
            set
            {
                _FacilityMaterialList = value;
                OnPropertyChanged();
            }
        }

        private List<FacilityMaterial> LoadFacilityMaterialList()
        {
            if (SelectedMaterial == null)
                return new List<FacilityMaterial>();
            return
                SelectedMaterial
                .FacilityMaterial_Material
                .OrderBy(c => c.Facility.FacilityNo)
                .ToList();
        }
        #endregion

        #endregion

        #region BSO->ACMethod

        #region ControlMode
        /// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        /// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        /// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        /// <returns>ControlModesInfo</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            switch (vbControl.VBContent)
            {
                case "CurrentMaterial\\MDFacilityManagementType":
                case "CurrentMaterial\\ContractorStock":
                case "CurrentMaterial\\BaseMDUnitID":
                case "CurrentMaterial\\BaseMDUnit":
                    {
                        if (CurrentMaterial != null && CurrentMaterial.FacilityCharge_Material.Count > 0)
                            return Global.ControlModes.Disabled;
                        return result;
                    }
                case nameof(AmbientTemperature):
                    {
                        if (!AmbientTemperature.HasValue)
                            return Global.ControlModes.EnabledRequired;
                        return result;
                    }
                case nameof(ConvertTestDensity):
                    {
                        if (!ConvertTestDensity.HasValue)
                            return Global.ControlModes.EnabledRequired;
                        return result;
                    }
                case nameof(ConvertTest2AmbVol):
                    {
                        if (!ConvertTest2AmbVol.HasValue)
                            return Global.ControlModes.EnabledRequired;
                        return result;
                    }
                case nameof(ConvertTest2Ref15Vol):
                    {
                        if (!ConvertTest2Ref15Vol.HasValue)
                            return Global.ControlModes.EnabledRequired;
                        return result;
                    }
                case nameof(ConvertTest2Mass):
                    {
                        if (!ConvertTest2Mass.HasValue)
                            return Global.ControlModes.EnabledRequired;
                        return result;
                    }
                case nameof(AmbientTemperature3):
                    {
                        if (!AmbientTemperature3.HasValue)
                            return Global.ControlModes.EnabledRequired;
                        return result;
                    }
                case nameof(ConvertTestRef15Vol3):
                    {
                        if (!ConvertTestRef15Vol3.HasValue)
                            return Global.ControlModes.EnabledRequired;
                        return result;
                    }
                case nameof(ConvertTestMass3):
                    {
                        if (!ConvertTestMass3.HasValue)
                            return Global.ControlModes.EnabledRequired;
                        return result;
                    }

                default:
                    return result;

            }
        }
        #endregion

        #region BSO->ACMethod->Material
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Material.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        /// <summary>
        /// Determines whether [is enabled save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand(Material.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
            Load();
        }

        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        [ACMethodInteraction(Material.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, nameof(SelectedMaterial), Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<Material>(requery, () => SelectedMaterial, () => CurrentMaterial, c => CurrentMaterial = c,
                        DatabaseApp.Material
                         .Include(c => c.BaseMDUnit)
                        .Include(c => c.MDMaterialGroup)
                        .Include(c => c.MDMaterialType)
                        .Include(c => c.MDGMPMaterialGroup)
                        .Include(c => c.MDInventoryManagementType)
                        .Include(c => c.MDFacilityManagementType)
                        .Include(c => c.MaterialUnit_Material)
                        .Include(c => c.MaterialCalculation_Material)
                        //.Include(c => c.FacilityCharge_Material)
                        .Include(c => c.Label)
                        .Include(c => c.Label.LabelTranslation_Label)
                        .Where(c => c.MaterialID == SelectedMaterial.MaterialID));
            PostExecute("Load");
            if (requery)
            {
                if (CurrentMaterial != null)
                {
                    CurrentMaterial.MaterialUnit_Material.AutoRefresh();
                    //CurrentMaterial.MaterialUnit_Material.AutoLoad();
                    OnPropertyChanged(nameof(MaterialUnitList));
                }
            }

            if (SelectedMaterial != null)
                ChangedSelectedMaterial(SelectedMaterial);
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedMaterial != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(Material.ClassName, Const.New, (short)MISort.New, true, nameof(SelectedMaterial), Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentMaterial = Material.NewACObject(DatabaseApp, null);
            DatabaseApp.Material.AddObject(CurrentMaterial);
            AccessPrimary.NavList.Insert(0, CurrentMaterial);
            SelectedMaterial = CurrentMaterial;
            OnPropertyChanged(nameof(MaterialList));

            ACState = Const.SMNew;
            PostExecute("New");
        }

        /// <summary>
        /// Determines whether [is enabled new].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNew()
        {
            return true;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(Material.ClassName, Const.Delete, (short)MISort.Delete, true, nameof(CurrentMaterial), Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;

            if (BSOMedia_Child != null && BSOMedia_Child.Value != null)
            {
                BSOMedia_Child.Value.DeleteACObject(CurrentMaterial);
            }

            Msg msg = CurrentMaterial.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return;
            AccessPrimary.NavList.Remove(CurrentMaterial);

            SelectedMaterial = AccessPrimary.NavList.FirstOrDefault();
            OnPropertyChanged(nameof(MaterialList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentMaterial != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Material.ClassName, "en{'Search material'}de{'Materialsuche'}", (short)MISort.Search)]
        public override void Search()
        {
            AccessPrimary.NavSearch(DatabaseApp, DatabaseApp.RecommendedMergeOption);
            OnPropertyChanged(nameof(MaterialList));
        }

        private List<Material> _VisitedMaterials = null;
        protected override Msg OnPreSave()
        {
            if (_VisitedMaterials != null && _VisitedMaterials.Any())
            {
                _VisitedMaterials.RemoveAll(c => c.EntityState == System.Data.EntityState.Unchanged);
            }
            if (
                CurrentMaterial != null
                && BSOMedia_Child != null
                && BSOMedia_Child.Value != null
                && CurrentMaterial.EntityState == System.Data.EntityState.Added
                && !string.IsNullOrEmpty(CurrentMaterial.MaterialNo)
              )
            {
                BSOMedia_Child.Value.LoadMedia(CurrentMaterial);
            }
            return base.OnPreSave();
        }

        protected override void OnPostSave()
        {
            if (_VisitedMaterials != null && _VisitedMaterials.Any())
            {
                foreach (var changedMaterial in _VisitedMaterials)
                {
                    gip.mes.facility.RemoteMaterialForwarder.ForwardMaterial(this, RemoteMaterialForwarder, changedMaterial.MaterialID);
                }
            }
            if (CurrentMaterial != null
                && !String.IsNullOrEmpty(RemoteMaterialForwarder))
            {
                _VisitedMaterials = new List<Material>();
                _VisitedMaterials.Add(CurrentMaterial);
            }
            else
                _VisitedMaterials = null;
            base.OnPostSave();
        }
        #endregion

        #region BSO->ACMethod->MaterialUnit
        /// <summary>
        /// Beim einfügen wird ein neues Current... erzeugt und in die ...List eingefügt
        /// Dies erfordert, das auf der Benutzeroberfläche die entsprechenden Steuerelemente
        /// aktualisiert werden.
        /// Auch wenn im XAML die Liste (MaterialUnitList) nicht direkt eingetragen wird,
        /// weis jedoch das VBDataGrid (schreibgeschützt und editierbar), das es diese darstellt.
        /// Diese ist immer zuerst zu aktualisieren.
        /// Anschließend werden alle Steuerelemente aktualisiert, welche das Current... darstellen.
        /// </summary>
        [ACMethodInteraction("MaterialUnit", "en{'New Material Unit'}de{'Neue Materialeinheit'}", (short)MISort.New, true, nameof(CurrentMaterialUnit), Global.ACKinds.MSMethodPrePost)]
        public void NewMaterialUnit()
        {
            if (!PreExecute("NewMaterialUnit")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            CurrentNewMaterialUnit = MaterialUnit.NewACObject(DatabaseApp, CurrentMaterial);
            ShowDialog(this, "MaterialUnitNew");
            OnPropertyChanged(nameof(MaterialUnitList));

            PostExecute("NewMaterialUnit");
        }

        /// <summary>
        /// Determines whether [is enabled new material unit].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new material unit]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewMaterialUnit()
        {
            return CurrentMaterial != null;
        }

        /// <summary>
        /// Deletes the material unit.
        /// </summary>
        [ACMethodInteraction("MaterialUnit", "en{'Delete Material Unit'}de{'Materialeinheit löschen'}", (short)MISort.Delete, true, nameof(CurrentMaterialUnit), Global.ACKinds.MSMethodPrePost)]
        public void DeleteMaterialUnit()
        {
            if (CurrentMaterialUnit == null)
                return;
            if (!PreExecute("DeleteMaterialUnit")) return;
            // Einfügen einer Eigenschaft 
            CurrentMaterial.MaterialUnit_Material.Remove(CurrentMaterialUnit);
            Msg msg = CurrentMaterialUnit.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            OnPropertyChanged(nameof(MaterialUnitList));

            PostExecute("DeleteMaterialUnit");
        }


        /// <summary>
        /// Determines whether [is enabled delete material unit].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete material unit]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteMaterialUnit()
        {
            return CurrentMaterial != null && CurrentMaterialUnit != null;
        }

        /// <summary>
        /// News the unit conversion OK.
        /// </summary>
        [ACMethodCommand("NewMaterialUnit", Const.Ok, (short)MISort.Okay)]
        public void NewMaterialUnitOK()
        {
            CloseTopDialog();
            CurrentMaterialUnit = CurrentNewMaterialUnit;
            MDUnit toUnit = CurrentNewMaterialUnit.ToMDUnit;
            CurrentMaterial.MaterialUnit_Material.Add(CurrentMaterialUnit);
            OnPropertyChanged(nameof(MaterialUnitList));
            OnPropertyChanged(nameof(ConvertableUnits));
            CurrentMaterialUnit.ToMDUnit = toUnit;
        }

        /// <summary>
        /// Determines whether [is enabled new unit conversion OK].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new unit conversion OK]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewMaterialUnitOK()
        {
            if (CurrentNewMaterialUnit == null
                || CurrentNewMaterialUnit.ToMDUnit == null
                || (Math.Abs(CurrentNewMaterialUnit.Multiplier - 0) <= Double.Epsilon)
                || (Math.Abs(CurrentNewMaterialUnit.Divisor - 0) <= Double.Epsilon))
                return false;
            return true;
        }

        /// <summary>
        /// News the unit conversion cancel.
        /// </summary>
        [ACMethodCommand("NewMaterialUnit", Const.Cancel, (short)MISort.Cancel)]
        public void NewMaterialUnitCancel()
        {
            CloseTopDialog();
            if (CurrentNewMaterialUnit != null)
            {
                Msg msg = CurrentNewMaterialUnit.DeleteACObject(DatabaseApp, true);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }

            }
            CurrentNewMaterialUnit = null;
        }

        #endregion

        #region BSO->ACMethod->MaterialCalculation
        /// <summary>
        /// Loads the material calculation.
        /// </summary>
        [ACMethodInteraction(nameof(MaterialCalculation), "en{'Load Position'}de{'Position laden'}", (short)MISort.Load, false, nameof(SelectedMaterialCalculation), Global.ACKinds.MSMethodPrePost)]
        public void LoadMaterialCalculation()
        {
            if (!IsEnabledLoadMaterialCalculation())
                return;
            if (!PreExecute("LoadMaterialCalculation")) return;
            // Laden des aktuell selektierten MaterialCalculation 
            CurrentMaterialCalculation = CurrentMaterial.MaterialCalculation_Material.Where(c => c.MaterialCalculationID == SelectedMaterialCalculation.MaterialCalculationID).FirstOrDefault();
            PostExecute("LoadMaterialCalculation");
        }

        /// <summary>
        /// Determines whether [is enabled load material calculation].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load material calculation]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadMaterialCalculation()
        {
            return SelectedMaterialCalculation != null && CurrentMaterial != null;
        }

        /// <summary>
        /// News the material calculation.
        /// </summary>
        [ACMethodInteraction("MaterialCalculation", "en{'New MaterialCalculation'}de{'Neue Materialeinheit'}", (short)MISort.New, true, nameof(SelectedMaterialCalculation), Global.ACKinds.MSMethodPrePost)]
        public void NewMaterialCalculation()
        {
            if (!PreExecute("NewMaterialCalculation")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(MaterialCalculation), MaterialCalculation.NoColumnName, MaterialCalculation.FormatNewNo, this);
            CurrentMaterialCalculation = MaterialCalculation.NewACObject(DatabaseApp, CurrentMaterial, secondaryKey);
            OnPropertyChanged("MaterialCalculationList");

            PostExecute("NewMaterialCalculation");
        }

        /// <summary>
        /// Determines whether [is enabled new material calculation].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new material calculation]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewMaterialCalculation()
        {
            return CurrentMaterial != null;
        }

        /// <summary>
        /// Deletes the material calculation.
        /// </summary>
        [ACMethodInteraction(nameof(MaterialCalculation), "en{'Delete MaterialCalculation'}de{'Materialeinheit löschen'}", (short)MISort.Delete, true, nameof(CurrentMaterialCalculation), Global.ACKinds.MSMethodPrePost)]
        public void DeleteMaterialCalculation()
        {
            if (CurrentMaterialCalculation == null)
                return;
            if (!PreExecute("DeleteMaterialCalculation")) return;
            // Einfügen einer Eigenschaft 
            Msg msg = CurrentMaterialCalculation.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            PostExecute("DeleteMaterialCalculation");
        }

        /// <summary>
        /// Determines whether [is enabled delete material calculation].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete material calculation]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteMaterialCalculation()
        {
            return CurrentMaterial != null && CurrentMaterialCalculation != null;
        }
        #endregion

        /*
         * Onlinehelp: Arbeiten mit schreibgeschützen und editierbaren VBDataGrid 
         * 
         * Grundsätzlich sind alle Listen die in einem BSO für beide Darstellungsarten 
         * geeignet. Vorraussetzung ist jedoch, das neben der Bereitstellung der Datenmember
         * über "AddEntityData", "AddEntityPick" und "AddEntityList" auch drei Methoden mit einer
         * bestimmten Funktionalität implementiert und mittels "AddCommand" registriert
         * werden:
         * -Load...
         * -New...
         * -Delete...
         * 
         * Diese drei Methoden müssen bestimmten Konventionen entsprechen, damit die Kommunikation
         * zur Oberfläche für beide Gridvarianten funktioniert.
         * (Beschreibung siehe bei den Methoden dieser Klasse)
         * 
         * 1. Schreibgeschützes DataGrid (Explorer)
         * XAML: <vb:VBDataGrid VBContent="SelectedMaterialUnitPackagingID" DblClick="!LoadMaterialUnitPackaging"></vb:VBDataGrid>
         * Hierbei ist es wichtig, das die Selected...ID (Property, Nullable<Guid>) als VBContent beim VBDataGrid angegeben wird.
         * Dies hat zur Folge, das immer der Primärschlüssel der im Datagrid ausgewählten Zeile über dieses Property
         * dieser Wert abgefragt werden kann.
         * 
         * Erst durch den DblClick oder dem Aufrug der Load...-Methode, wird der ausgewählte Datensatz zum Current...
         * (CurrentMaterialUnitPackaging). Der Current... wird dann in Eingabesteuerelementen (VBTextBox, VBComboBox, etc.)
         * zum bearbeiten oder auch schreibgeschützt angezeigt.
         * 
         * 2. Editierbares DataGrid
         * XAML: <vb:VBDataGrid VBContent="CurrentMaterialUnitPackaging" Enabled="true" 
         *              NewData="!NewMaterialUnitPackaging" DeleteData="!DeleteACConfig"></vb:VBDataGrid>
         * Damit man ein Grid editieren kann, ist als VBContent der Current... (CurrentMaterialUnitPackaging)
         * anzugeben. So wird beim Navigieren im Datagrid, die aktuelle Zeile automatisch zur Current..., welche
         * sich dann bearbeiten läst. 
         * Um ein Editieren zu ermöglichen ist die Eigenschaft Enabled auf true zu setzen.
         * Desweiteren sind NewData und DeleteData die entsprechenden Methoden zuzuweisen.
         */

        #region BSO->ACMethod->ACConfig
        /// <summary>
        /// Die Load-Methode lädt den aktuell selektierten Datensatz (Selected...) auf den
        /// Datenhalter (Current...)
        /// </summary>
        [ACMethodInteraction("ACConfig", "en{'Load Attribute'}de{'Attribut laden'}", (short)MISort.Load, false, nameof(SelectedACConfig), Global.ACKinds.MSMethodPrePost)]
        public void LoadACConfig()
        {
            if (!IsEnabledLoadACConfig())
                return;
            if (!PreExecute("LoadACConfig")) return;
            // Laden des aktuell selektierten ACConfig 
            CurrentACConfig = SelectedACConfig;
            PostExecute("LoadACConfig");
        }

        /// <summary>
        /// Laden ist nur erlaubt, wenn ein Zeile ausgewählt ist
        /// </summary>
        /// <returns><c>true</c> if [is enabled load AC config]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadACConfig()
        {
            return SelectedACConfig != null;
        }

        /// <summary>
        /// Beim einfügen wird ein neues Current... erzeugt und in die ...List eingefügt
        /// Dies erfordert, das auf der Benutzeroberfläche die entsprechenden Steuerelemente
        /// aktualisiert werden.
        /// Auch wenn im XAML die Liste (ACConfigList) nicht direkt eingetragen wird,
        /// weis jedoch das VBDataGrid (schreibgeschützt und editierbar), das es diese darstellt.
        /// Diese ist immer zuerst zu aktualisieren.
        /// Anschließend werden alle Steuerelemente aktualisiert, welche das Current... darstellen.
        /// </summary>
        [ACMethodInteraction("ACConfig", "en{'New Attribute'}de{'Neues Attribut'}", (short)MISort.New, true, nameof(SelectedACConfig), Global.ACKinds.MSMethodPrePost)]
        public void NewACConfig()
        {
            if (!PreExecute("NewACConfig")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            //TODO: VBConfig stimmt was von Logik nicht, da MaterialConfig zurückkommt und neu erzeugte Objekt sowieso in anderem Context gar nicht existiert:
            CurrentACConfig = CurrentMaterial.NewACConfig() as MaterialConfig;
            OnPropertyChanged(nameof(ACConfigList));
            OnPropertyChanged(nameof(CurrentACConfig));
            PostExecute("NewACConfig");
        }

        /// <summary>
        /// Determines whether [is enabled new AC config].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new AC config]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewACConfig()
        {
            return true;
        }

        /// <summary>
        /// Deletes the AC config.
        /// </summary>
        [ACMethodInteraction("ACConfig", "en{'Delete Attribute'}de{'Attribut löschen'}", (short)MISort.Delete, true, nameof(CurrentACConfig), Global.ACKinds.MSMethodPrePost)]
        public void DeleteACConfig()
        {
            try
            {
                if (!PreExecute("DeleteACConfig")) return;
                Msg msg = CurrentACConfig.DeleteACObject(DatabaseApp, true);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }

            }
            catch (Exception e)
            {
                Messages.Error(this, "Message00001", false, e.Message);
            }
            PostExecute("DeleteACConfig");
            OnPropertyChanged(nameof(ACConfigList));
            OnPropertyChanged(nameof(CurrentACConfig));
        }

        /// <summary>
        /// Determines whether [is enabled delete AC config].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete AC config]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteACConfig()
        {
            return CurrentMaterial != null && CurrentACConfig != null && CurrentACConfig.ParentACObject != null;
        }
        #endregion

        #region 1.2 Conversion-Test

        [ACMethodCommand("Conversiontest1", "en{'Convert to Base UOM'}de{'Umrechnen nach Basiseinheit'}", (short)MISort.Okay)]
        public void ConvertTestToBase()
        {
            if (!IsEnabledConvertTestToBase())
                return;
            try
            {
                ConvertTestOutput = CurrentMaterial.ConvertToBaseQuantity(ConvertTestInput, SelectedUnitConvertTest);
            }
            catch (Exception ec)
            {
                ConvertTestOutput = 0;

                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("BSOMaterial", "ConvertTestToBase", msg);
            }
        }

        public bool IsEnabledConvertTestToBase()
        {
            if (SelectedUnitConvertTest != null)
                return true;
            return false;
        }

        [ACMethodCommand("Conversiontest2", "en{'Convert from Base UOM'}de{'Umrechnen von Basiseinheit'}", (short)MISort.Okay)]
        public void ConvertTestFromBase()
        {
            if (!IsEnabledConvertTestFromBase())
                return;
            try
            {
                ConvertTestOutput = CurrentMaterial.ConvertFromBaseQuantity(ConvertTestInput, SelectedUnitConvertTest);
                //ConvertTestOutput = CurrentMaterial.ConvertQuantity(ConvertTestInput, CurrentMaterial.BaseMDUnit, SelectedUnitConvertTest);
            }
            catch (Exception ec)
            {
                ConvertTestOutput = 0;

                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("BSOMaterial", "ConvertTestFromBase", msg);
            }
        }

        public bool IsEnabledConvertTestFromBase()
        {
            if ((SelectedUnitConvertTest != null) && (CurrentMaterial.BaseMDUnit != null))
                return true;
            return false;
        }


        [ACMethodCommand("Conversiontest3", "en{'Convert from Material Unit'}de{'Umrechnen von alt. Einheit'}", (short)MISort.Okay)]
        public void ConvertTest()
        {
            if (!IsEnabledConvertTest())
                return;
            try
            {
                ConvertTestOutput = CurrentMaterial.ConvertQuantity(ConvertTestInput, CurrentMaterialUnit.ToMDUnit, SelectedUnitConvertTest);
            }
            catch (Exception ec)
            {
                ConvertTestOutput = 0;

                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("BSOMaterial", "ConvertTest", msg);
            }
        }

        public bool IsEnabledConvertTest()
        {
            if ((SelectedUnitConvertTest != null) && (this.CurrentMaterialUnit != null))
                return true;
            return false;
        }

        #endregion

        #region FacilityMaterial

        /// <summary>
        /// Source Property: AddFacility
        /// </summary>
        [ACMethodInfo("", "en{'Add'}de{'Neu'}", 700)]
        public void AddFacility()
        {
            if (!IsEnabledAddFacility())
                return;

            FacilityMaterial facilityMaterial = FacilityMaterial.NewACObject(DatabaseApp, null);
            facilityMaterial.Material = SelectedMaterial;
            SelectedMaterial.FacilityMaterial_Material.Add(facilityMaterial);
            FacilityMaterialList.Add(facilityMaterial);
            OnPropertyChanged(nameof(FacilityMaterialList));

            SelectedFacilityMaterial = facilityMaterial;
        }

        public bool IsEnabledAddFacility()
        {
            return SelectedMaterial != null;
        }


        /// <summary>
        /// Source Property: DeleteFacility
        /// </summary>
        [ACMethodInfo("", "en{'Delete'}de{'Löschen'}", 701)]
        public void DeleteFacility()
        {
            if (!IsEnabledDeleteFacility())
                return;

            SelectedMaterial.FacilityMaterial_Material.Remove(SelectedFacilityMaterial);
            FacilityMaterialList.Remove(SelectedFacilityMaterial);

            SelectedFacilityMaterial.DeleteACObject(DatabaseApp, false);

            OnPropertyChanged(nameof(FacilityMaterialList));
        }

        public bool IsEnabledDeleteFacility()
        {
            return SelectedFacilityMaterial != null;
        }

        /// <summary>
        /// Source Property: ShowDlgInwardFacility
        /// </summary>
        [ACMethodInfo("", "en{'Choose facility'}de{'Lager auswählen'}", 702)]
        public void ShowFacility()
        {
            if (!IsEnabledShowFacility())
                return;

            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(SelectedFacilityMaterial?.Facility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                SelectedFacilityMaterial.Facility = facility;
                OnPropertyChanged(nameof(SelectedFacilityMaterial));
            }
        }

        public bool IsEnabledShowFacility()
        {
            return SelectedFacilityMaterial != null;
        }

        [ACMethodInfo("", "en{'Generate OEE test data'}de{'OEE Testdaten generieren'}", 703)]
        public void GenerateTestOEEData()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                FacilityMaterial facilityMaterial = SelectedFacilityMaterial.FromAppContext<FacilityMaterial>(dbApp);
                Msg msg = FacilityOEEManager.GenerateTestOEEData(dbApp, facilityMaterial);
                if (msg != null)
                    Messages.Msg(msg);
            }
        }

        public bool IsEnabledGenerateTestOEEData()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }

        [ACMethodInfo("", "en{'Delete OEE test data'}de{'OEE Testdaten löschen'}", 704)]
        public void DeleteTestOEEData()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                FacilityMaterial facilityMaterial = SelectedFacilityMaterial.FromAppContext<FacilityMaterial>(dbApp);
                FacilityOEEManager.DeleteTestOEEData(dbApp, facilityMaterial);
            }
        }

        public bool IsEnabledDeleteTestOEEData()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }


        [ACMethodInfo("", "en{'Recalc average throughput'}de{'Aktualisiere Mittelwert Durchsatz'}", 705)]
        public void RecalcThroughputAverage()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;

            Msg msg = FacilityOEEManager.RecalcThroughputAverage(this.DatabaseApp, SelectedFacilityMaterial, false);
            if (msg != null)
                Messages.Msg(msg);
        }

        public bool IsEnabledRecalcThroughputAverage()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }

        [ACMethodInfo("", "en{'Correct throuhputs and OEE'}de{'Korrigiere Durchsätze und OEE'}", 706)]
        public void RecalcThroughputAndOEE()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                FacilityMaterial facilityMaterial = SelectedFacilityMaterial.FromAppContext<FacilityMaterial>(dbApp);
                Msg msg = FacilityOEEManager.RecalcThroughputAndOEE(dbApp, facilityMaterial, null, null);
                if (msg != null)
                    Messages.Msg(msg);
            }
        }

        public bool IsEnabledRecalcThroughputAndOEE()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }
        #endregion

        #region Show Dialog
        [ACMethodInfo("Dialog", "en{'Dialog lot overview'}de{'Dialog Losübersicht'}", (short)MISort.QueryPrintDlg + 1)]
        public virtual void ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;

            PAOrderInfoEntry entityInfo = paOrderInfo.Entities.Where(c => c.EntityName == nameof(Material)).FirstOrDefault();
            if (entityInfo == null)
                return;

            Material material = this.DatabaseApp.Material.Where(c => c.MaterialID == entityInfo.EntityID).FirstOrDefault();
            if (material == null)
                return;

            ShowDialogMaterial(material.MaterialNo);
        }

        [ACMethodInfo("Dialog", "en{'Dialog Material'}de{'Dialog Material'}", (short)MISort.QueryPrintDlg)]
        public void ShowDialogMaterial(string materialNo)
        {
            if (AccessPrimary == null)
                return;
            //AccessPrimary.NavACQueryDefinition.SearchWord = facilityNo;
            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "MaterialNo").FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, nameof(Facility.FacilityNo), Global.LogicalOperators.contains, Global.Operators.and, materialNo, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            else
                filterItem.SearchWord = materialNo;

            this.Search();
            ShowDialog(this, "OrderInfoDialog");
            this.ParentACComponent.StopComponent(this);
        }
        #endregion

        #region Navigation
        [ACMethodInteraction("", "en{'Show Material Stock and History'}de{'Zeige Materialbestand und Historie'}", 783, true, nameof(SelectedMaterial))]
        public void NavigateToMaterialOverview()
        {
            if (!IsEnabledNavigateToMaterialOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(Material), SelectedMaterial.MaterialID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToMaterialOverview()
        {
            if (SelectedMaterial != null)
                return true;
            return false;
        }
        #endregion

        #endregion

        #region Translation

        private List<gip.core.datamodel.VBLanguage> _TranslationLanguages;
        [ACPropertyInfo(9999, "TranslationLang")]
        public IEnumerable<gip.core.datamodel.VBLanguage> TranslationLanguages
        {
            get
            {
                if (_TranslationLanguages == null)
                {
                    using (Database db = new core.datamodel.Database())
                    {
                        _TranslationLanguages = db.VBLanguage.OrderBy(c => c.VBLanguageCode).ToList();
                    }
                }
                return _TranslationLanguages;
            }
        }

        #region Translation -> Select, (Current,) List

        private LabelTranslation _SelectedTranslation;
        [ACPropertySelected(9999, "Translation")]
        public LabelTranslation SelectedTranslation
        {
            get
            {
                return _SelectedTranslation;
            }
            set
            {
                if (_SelectedTranslation != value)
                {
                    _SelectedTranslation = value;
                    OnPropertyChanged("SelectedTranslation");
                }
            }
        }

        [ACPropertyList(9999, "Translation")]
        public IEnumerable<LabelTranslation> TranslationList
        {
            get
            {
                if (CurrentMaterial == null || CurrentMaterial.Label == null)
                    return new List<LabelTranslation>();
                return CurrentMaterial.Label.LabelTranslation_Label;
            }
        }

        #endregion

        #region Translation -> Methods

        [ACMethodInteraction(Material.ClassName, Const.New, (short)MISort.New, true, "SelectedTranslation", Global.ACKinds.MSMethodPrePost)]
        public void TranslationNew()
        {
            if (CurrentMaterial.Label == null)
            {
                CurrentMaterial.Label = Label.NewACObject(DatabaseApp, CurrentMaterial);
                DatabaseApp.Label.AddObject(CurrentMaterial.Label);
            }
            LabelTranslation translation = LabelTranslation.NewACObject(DatabaseApp, CurrentMaterial.Label);
            DatabaseApp.LabelTranslation.AddObject(translation);
            CurrentMaterial.Label.LabelTranslation_Label.Add(translation);
            SelectedTranslation = translation;
            OnPropertyChanged("TranslationList");
        }

        [ACMethodInteraction(Material.ClassName, Const.Delete, (short)MISort.Delete, true, "SelectedTranslation", Global.ACKinds.MSMethodPrePost)]
        public void TranslationDelete()
        {
            Msg msg = SelectedTranslation.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            else
            {
                SelectedTranslation = TranslationList.FirstOrDefault();
            }
            OnPropertyChanged("TranslationList");
        }

        #region Translation -> Methods -> IsEnabled

        public bool IsEnabledTranslationNew()
        {
            return CurrentMaterial != null;
        }

        public bool IsEnabledTranslationDelete()
        {
            return SelectedTranslation != null;
        }

        #endregion

        #endregion


        #endregion

        #region AssociatedPartslistPos

        #region AssociatedPartslistPos -> ACAccessNav

        ACAccessNav<PartslistPos> _AccessAssociatedPartslistPos;
        [ACPropertyAccess(100, "AssociatedPartslistPos")]
        public ACAccessNav<PartslistPos> AccessAssociatedPartslistPos
        {
            get
            {
                if (_AccessAssociatedPartslistPos == null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(this, Const.QueryPrefix + "AssociatedPartslistPos", nameof(PartslistPos));
                    navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(AccessAssociatedPartslistPosDefaultSort);
                    navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(AccessAssociatedPartslistPosDefaultFilter);
                    _AccessAssociatedPartslistPos = navACQueryDefinition.NewAccessNav<PartslistPos>("AssociatedPartslistPos", this);
                    _AccessAssociatedPartslistPos.NavSearchExecuting += _AccessAssociatedPartslistPos_NavSearchExecuting;
                    _AccessAssociatedPartslistPos.AutoSaveOnNavigation = false;
                }
                return _AccessAssociatedPartslistPos;
            }
        }


        public List<ACFilterItem> AccessAssociatedPartslistPosDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                ACFilterItem isDeletedFilter = new ACFilterItem(Global.FilterTypes.filter, "Partslist\\DeleteDate", Global.LogicalOperators.isNull, Global.Operators.and, null, true);
                aCFilterItems.Add(isDeletedFilter);

                ACFilterItem phOpen = new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true);
                aCFilterItems.Add(phOpen);

                ACFilterItem partslistNoFilter = new ACFilterItem(FilterTypes.filter, "Partslist\\PartslistNo", LogicalOperators.contains, Operators.or, "", true, true);
                aCFilterItems.Add(partslistNoFilter);

                ACFilterItem filterPartslistName = new ACFilterItem(FilterTypes.filter, "Partslist\\PartslistName", LogicalOperators.contains, Operators.or, "", true, true);
                aCFilterItems.Add(filterPartslistName);

                ACFilterItem phClose = new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true);
                aCFilterItems.Add(phClose);

                ACFilterItem filterIsEnabled = new ACFilterItem(FilterTypes.filter, FilterMaterialNoAssociatedPos_PartslistEnabled, LogicalOperators.contains, Operators.and, "True", true);
                aCFilterItems.Add(filterIsEnabled);

                ACFilterItem filterPartslistMaterial = new ACFilterItem(FilterTypes.filter, "Partslist\\Material\\MaterialNo", LogicalOperators.contains, Operators.and, "", true);
                aCFilterItems.Add(filterPartslistMaterial);

                ACFilterItem filterPosMaterial = new ACFilterItem(FilterTypes.filter, FilterMaterialNoAssociatedPos_PropertyName, LogicalOperators.contains, Operators.and, "", true);
                aCFilterItems.Add(filterPosMaterial);

                return aCFilterItems;
            }
        }

        private List<ACSortItem> AccessAssociatedPartslistPosDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem partslistNo = new ACSortItem("Partslist\\PartslistNo", SortDirections.ascending, true);
                acSortItems.Add(partslistNo);

                ACSortItem version = new ACSortItem("Partslist\\PartslistVersion", SortDirections.ascending, true);
                acSortItems.Add(version);

                return acSortItems;
            }
        }

        private IQueryable<PartslistPos> _AccessAssociatedPartslistPos_NavSearchExecuting(IQueryable<PartslistPos> result)
        {
            if (result != null)
            {
                string matNo = SelectedIntermediateProduct?.MaterialNo;
                if (SelectedMaterialWF != null)
                {
                    result =
                        result.Where(c => (string.IsNullOrEmpty(matNo) || c.PartslistPosRelation_SourcePartslistPos
                                                                                                    .Any(x => x.TargetPartslistPos.Material.MaterialNo == matNo
                                                                                                           && x.TargetQuantityUOM > 0.00001))
                                        && (c.Partslist.MaterialWFID == SelectedMaterialWF.MaterialWFID));
                }
                else
                {
                    result =
                        result.Where(c => string.IsNullOrEmpty(matNo) || c.PartslistPosRelation_SourcePartslistPos
                                                                                                    .Any(x => x.TargetPartslistPos.Material.MaterialNo == matNo
                                                                                                           && x.TargetQuantityUOM > 0.00001));
                }
            }
            return result;
        }

        #endregion

        #region AssociatedPartslistPos -> SelectedAssociatedPartslistPos

        /// <summary>
        /// Gets or sets the selected AssociatedPartslistPos.
        /// </summary>
        /// <value>The selected AssociatedPartslistPos.</value>
        [ACPropertySelected(101, "AssociatedPartslistPos")]
        public PartslistPos SelectedAssociatedPartslistPos
        {
            get
            {
                if (AccessAssociatedPartslistPos == null)
                    return null;
                return AccessAssociatedPartslistPos.Selected;
            }
            set
            {
                if (AccessAssociatedPartslistPos == null)
                    return;
                AccessAssociatedPartslistPos.Selected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the AssociatedPartslistPos list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(102, "AssociatedPartslistPos")]
        public IEnumerable<PartslistPos> AssociatedPartslistPosList
        {
            get
            {
                if (AccessAssociatedPartslistPos == null)
                    return null;
                return AccessAssociatedPartslistPos.NavList;
            }
        }

        #endregion

        #region AssociatedPartslistPos -> Filter fields
        private string FilterMaterialNoAssociatedPos_PropertyName = @"Material\MaterialNo";
        private string FilterMaterialNoAssociatedPos_PartslistEnabled = "Partslist\\IsEnabled";
        private ACFilterItem FilterAssociatedPosMaterial
        {
            get
            {
                return AccessAssociatedPartslistPos.NavACQueryDefinition.ACFilterColumns.FirstOrDefault(c => c.PropertyName == FilterMaterialNoAssociatedPos_PropertyName);
            }
        }

        private double? _FilterAssociatedPosTargetQFrom;
        /// <summary>
        /// Doc  FilterAssociatedPosTargetQFrom
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(FilterAssociatedPosTargetQFrom), "en{'Quantity from'}de{'Menge von'}")]
        public double? FilterAssociatedPosTargetQFrom
        {
            get
            {
                return _FilterAssociatedPosTargetQFrom;
            }
            set
            {
                if (_FilterAssociatedPosTargetQFrom != value)
                {
                    _FilterAssociatedPosTargetQFrom = value;
                    SetQuantitySearchFilter();
                    OnPropertyChanged();
                }
            }
        }

        private double? _FilterAssociatedPosTargetQTo;
        /// <summary>
        /// Doc  FilterAssociatedPosTargetQTo
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(FilterAssociatedPosTargetQTo), "en{'Quantity to'}de{'Menge bis'}")]
        public double? FilterAssociatedPosTargetQTo
        {
            get
            {
                return _FilterAssociatedPosTargetQTo;
            }
            set
            {
                if (_FilterAssociatedPosTargetQTo != value)
                {
                    _FilterAssociatedPosTargetQTo = value;
                    SetQuantitySearchFilter();
                    OnPropertyChanged();
                }
            }
        }

        private string _FilterAssociatedPosIntermMatNo;
        /// <summary>
        /// Doc  FilterAssociatedPosIntermMatNo
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(FilterAssociatedPosIntermMatNo), "en{'Mix product N'}de{'Zwischenprodukt Nr.'}")]
        public string FilterAssociatedPosIntermMatNo
        {
            get
            {
                return _FilterAssociatedPosIntermMatNo;
            }
            set
            {
                if (_FilterAssociatedPosIntermMatNo != value)
                {
                    _FilterAssociatedPosIntermMatNo = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Doc  FilterAssociatedPosIntermMatNo
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, nameof(FilterIsPartslistEnabled), "en{'Enabled'}de{'Freigegeben'}")]
        public bool? FilterIsPartslistEnabled
        {
            get
            {
                ACFilterItem isEnabledFilter =
                    AccessAssociatedPartslistPos
                    .NavACQueryDefinition
                    .ACFilterColumns.FirstOrDefault(c => c.PropertyName == FilterMaterialNoAssociatedPos_PartslistEnabled);
                bool filerIsPartslistEnabled = false;
                if (string.IsNullOrEmpty(isEnabledFilter.SearchWord))
                    return null;
                if (!bool.TryParse(isEnabledFilter.SearchWord, out filerIsPartslistEnabled))
                    return null;
                return filerIsPartslistEnabled;
            }
            set
            {
                ACFilterItem isEnabledFilter =
                    AccessAssociatedPartslistPos
                    .NavACQueryDefinition
                    .ACFilterColumns.FirstOrDefault(c => c.PropertyName == FilterMaterialNoAssociatedPos_PartslistEnabled);
                if (value != null)
                    isEnabledFilter.SearchWord = value.ToString();
                else
                    isEnabledFilter.SearchWord = null;
            }
        }

        #endregion

        #region AssociatedPartslistPos -> Helper methods 

        public override void ChangedSelectedMaterial(Material material)
        {
            base.ChangedSelectedMaterial(material);
            AccessAssociatedPartslistPos.NavList.Clear();
            if (material != null)
                FilterAssociatedPosMaterial.SearchWord = material.MaterialNo;
            else
                FilterAssociatedPosMaterial.SearchWord = null;

            FacilityMaterialList = LoadFacilityMaterialList();
            SelectedFacilityMaterial = FacilityMaterialList.FirstOrDefault();
        }

        private List<ACFilterItem> GetQuantitySearchFilter()
        {
            List<ACFilterItem> list = new List<ACFilterItem>();
            list.Add(new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true));
            list.Add(new ACFilterItem(Global.FilterTypes.filter, nameof(PartslistPos.TargetQuantityUOM), Global.LogicalOperators.greaterThanOrEqual, Global.Operators.and, FilterAssociatedPosTargetQFrom?.ToString(), true, true));
            list.Add(new ACFilterItem(Global.FilterTypes.filter, nameof(PartslistPos.TargetQuantityUOM), Global.LogicalOperators.lessThanOrEqual, Global.Operators.and, FilterAssociatedPosTargetQTo?.ToString(), true, true));
            list.Add(new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true));
            return list;
        }

        private void SetQuantitySearchFilter()
        {
            List<ACFilterItem> oldQuantityFilters = AccessAssociatedPartslistPos.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName != FilterMaterialNoAssociatedPos_PropertyName).ToList();
            foreach (var item in oldQuantityFilters)
                AccessAssociatedPartslistPos.NavACQueryDefinition.ACFilterColumns.Remove(item);

            var newQuantityFilters = GetQuantitySearchFilter();
            foreach (var item in newQuantityFilters)
                AccessAssociatedPartslistPos.NavACQueryDefinition.ACFilterColumns.Add(item);
        }

        #endregion

        #region AssociatedPartslistPos -> Methods

        /// <summary>
        /// Method SearchAssociatedPos
        /// </summary>
        [ACMethodInfo(nameof(SearchAssociatedPos), "en{'Search'}de{'Suchen'}", 9999, false, false, true)]
        public void SearchAssociatedPos()
        {
            if (!IsEnabledSearchAssociatedPos())
                return;
            if (_AccessAssociatedPartslistPos != null)
                _AccessAssociatedPartslistPos.NavSearch();

            OnPropertyChanged(nameof(AssociatedPartslistPosList));
        }

        public bool IsEnabledSearchAssociatedPos()
        {
            return _AccessAssociatedPartslistPos != null;
        }

        [ACMethodInfo(nameof(OpenQueryDialog), "en{'Query dialog'}de{'Abfragedialog'}", 503, false)]
        public bool OpenQueryDialog()
        {
            if (!IsEnabledSearchAssociatedPos())
                return false;

            bool result = false;
            if (_AccessAssociatedPartslistPos != null)
                result = _AccessAssociatedPartslistPos.ShowACQueryDialog();
            if (result)
                SearchAssociatedPos();
            return result;
        }

        #endregion



        #endregion

        #region Workflow

        #region Properties



        private core.datamodel.ACClass _SelectedPWMethodNode;
        [ACPropertySelected(800, "PWMethodNode")]
        public core.datamodel.ACClass SelectedPWMethodNode
        {
            get => _SelectedPWMethodNode;
            set
            {
                _SelectedPWMethodNode = value;
                OnPropertyChanged(nameof(SelectedPWMethodNode));
                OnPropertyChanged(nameof(AvailablePWMethodNodes));
                OnPropertyChanged(nameof(AssignedPWMethodNodes));
            }
        }

        private List<core.datamodel.ACClass> _PWMethodNodeList;
        [ACPropertyList(800, "PWMethodNode")]
        public IEnumerable<core.datamodel.ACClass> PWMethodNodeList
        {
            get
            {
                if (_PWMethodNodeList == null)
                {
                    _PWMethodNodeList = DatabaseApp.ContextIPlus.ACClass.Where(c => c.ACKindIndex == (short)Global.ACKinds.TPWMethod && !c.IsAbstract).ToList();
                }

                return _PWMethodNodeList;
            }
        }


        [ACPropertySelected(800, "AssignedPWMethodNode")]
        public gip.mes.datamodel.MaterialConfig SelectedAssignedPWMethodNode
        {
            get;
            set;
        }

        [ACPropertyList(800, "AssignedPWMethodNode")]
        public List<gip.mes.datamodel.MaterialConfig> AssignedPWMethodNodes
        {
            get
            {
                if (SelectedPWMethodNode == null)
                    return null;

                return CurrentMaterial?.MaterialConfig_Material.Where(c => c.KeyACUrl == MaterialConfig.PWMethodNodeConfigKeyACUrl
                                                                        && c.ACClassWF != null
                                                                        && c.ACClassWF.ACClassMethod != null
                                                                        && c.ACClassWF.ACClassMethod.WorkflowTypeACClass.ACClassID == SelectedPWMethodNode.ACClassID)
                                                               .ToList();
            }
        }

        private core.datamodel.ACClassWF _SelectedAvailablePWMethodNode;
        [ACPropertySelected(800, "WF", "en{'Single dosing workflow'}de{'Einzeldosierung Workflow'}")]
        public core.datamodel.ACClassWF SelectedAvailablePWMethodNode
        {
            get => _SelectedAvailablePWMethodNode;
            set
            {
                _SelectedAvailablePWMethodNode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AvailableMachineList));
            }
        }

        [ACPropertyList(800, "WF")]
        public IEnumerable<core.datamodel.ACClassWF> AvailablePWMethodNodes
        {
            get
            {
                if (SelectedPWMethodNode == null)
                    return null;

                List<core.datamodel.ACClassWF> wfRootList =
                    this.Database.ContextIPlus.ACClassWF.Where(c => !c.ParentACClassWFID.HasValue && !c.ACClassMethod.IsSubMethod)
                                                                .AsEnumerable()
                                                                .Where(c => c.ACClassMethod.WorkflowTypeACClass != null
                                                                       && c.ACClassMethod.WorkflowTypeACClass.ACClassID == SelectedPWMethodNode.ACClassID)
                                                                .SelectMany(x => x.ACClassWF_ParentACClassWF)
                                                                .Where(c => c.PWACClass.ACIdentifier == "PWNodeProcessWorkflowVB") //TODO: get from db 
                                                                .OrderBy(c => c.ACCaption)
                                                                .ToList();
                var configList = AssignedPWMethodNodes;
                if (configList != null)
                    wfRootList.RemoveAll(c => configList.Where(d => d.VBiACClassWFID == c.ACClassWFID).Any());
                return wfRootList;
            }
        }

        [ACPropertyInfo(810, "AvailableMachine", "en{'Single dosing machine'}de{'Einzel-Dosiermaschine'}")]
        public core.datamodel.ACClass SelectedAvailableMachine
        {
            get;
            set;
        }

        [ACPropertyList(810, "AvailableMachine")]
        public IEnumerable<core.datamodel.ACClass> AvailableMachineList
        {
            get
            {
                if (SelectedAvailablePWMethodNode == null)
                    return null;

                var pwGroups = SelectedAvailablePWMethodNode.GetPWGroups();

                var possibleMachines = pwGroups.SelectMany(c => c.RefPAACClass.DerivedClassesInProjects);

                return possibleMachines;
            }
        }

        #endregion

        #region Methods

        [ACMethodInfo("", "en{'Add rule'}de{'Regel hinzufügen'}", 800)]
        public void AddPWMethodNodeConfig()
        {
            MaterialConfig singleDosConfig = MaterialConfig.NewACObject(DatabaseApp, CurrentMaterial);
            singleDosConfig.KeyACUrl = MaterialConfig.PWMethodNodeConfigKeyACUrl;
            singleDosConfig.VBiACClassWFID = SelectedAvailablePWMethodNode.ACClassWFID;
            singleDosConfig.VBiValueTypeACClassID = DatabaseApp.ContextIPlus.ACClass.FirstOrDefault(c => c.ACIdentifier == "string").ACClassID;
            if (SelectedAvailableMachine != null)
                singleDosConfig.VBiACClassID = SelectedAvailableMachine.ACClassID;

            CurrentMaterial.MaterialConfig_Material.Add(singleDosConfig);
            //DatabaseApp.MaterialConfig.AddObject(singleDosConfig);

            OnPropertyChanged(nameof(AssignedPWMethodNodes));
            OnPropertyChanged(nameof(AvailablePWMethodNodes));
            CloseTopDialog();

            SelectedAvailablePWMethodNode = null;
            SelectedAvailableMachine = null;
        }

        public bool IsEnabledAddPWMethodNodeConfig()
        {
            return SelectedAvailablePWMethodNode != null && SelectedAvailableMachine != null;
        }

        [ACMethodInfo("", "en{'Delete rule'}de{'Regel löschen'}", 800)]
        public void DeletePWMethodNodeConfig()
        {
            SelectedMaterial.MaterialConfig_Material.Remove(SelectedAssignedPWMethodNode);
            DatabaseApp.DeleteObject(SelectedAssignedPWMethodNode);
            OnPropertyChanged(nameof(AssignedPWMethodNodes));
            OnPropertyChanged(nameof(AvailablePWMethodNodes));
        }

        public bool IsEnabledDeletePWMethodNodeConfig()
        {
            return SelectedAssignedPWMethodNode != null;
        }

        #endregion

        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Save):
                    Save();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(Load):
                    Load(acParameter != null && acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(NewMaterialUnit):
                    NewMaterialUnit();
                    return true;
                case nameof(IsEnabledNewMaterialUnit):
                    result = IsEnabledNewMaterialUnit();
                    return true;
                case nameof(DeleteMaterialUnit):
                    DeleteMaterialUnit();
                    return true;
                case nameof(IsEnabledDeleteMaterialUnit):
                    result = IsEnabledDeleteMaterialUnit();
                    return true;
                case nameof(NewMaterialUnitOK):
                    NewMaterialUnitOK();
                    return true;
                case nameof(IsEnabledNewMaterialUnitOK):
                    result = IsEnabledNewMaterialUnitOK();
                    return true;
                case nameof(NewMaterialUnitCancel):
                    NewMaterialUnitCancel();
                    return true;
                case nameof(LoadMaterialCalculation):
                    LoadMaterialCalculation();
                    return true;
                case nameof(IsEnabledLoadMaterialCalculation):
                    result = IsEnabledLoadMaterialCalculation();
                    return true;
                case nameof(NewMaterialCalculation):
                    NewMaterialCalculation();
                    return true;
                case nameof(IsEnabledNewMaterialCalculation):
                    result = IsEnabledNewMaterialCalculation();
                    return true;
                case nameof(DeleteMaterialCalculation):
                    DeleteMaterialCalculation();
                    return true;
                case nameof(IsEnabledDeleteMaterialCalculation):
                    result = IsEnabledDeleteMaterialCalculation();
                    return true;
                case nameof(LoadACConfig):
                    LoadACConfig();
                    return true;
                case nameof(IsEnabledLoadACConfig):
                    result = IsEnabledLoadACConfig();
                    return true;
                case nameof(NewACConfig):
                    NewACConfig();
                    return true;
                case nameof(IsEnabledNewACConfig):
                    result = IsEnabledNewACConfig();
                    return true;
                case nameof(DeleteACConfig):
                    DeleteACConfig();
                    return true;
                case nameof(IsEnabledDeleteACConfig):
                    result = IsEnabledDeleteACConfig();
                    return true;
                case nameof(ConvertTestToBase):
                    ConvertTestToBase();
                    return true;
                case nameof(IsEnabledConvertTestToBase):
                    result = IsEnabledConvertTestToBase();
                    return true;
                case nameof(ConvertTestFromBase):
                    ConvertTestFromBase();
                    return true;
                case nameof(IsEnabledConvertTestFromBase):
                    result = IsEnabledConvertTestFromBase();
                    return true;
                case nameof(ConvertTest):
                    ConvertTest();
                    return true;
                case nameof(IsEnabledConvertTest):
                    result = IsEnabledConvertTest();
                    return true;
                case nameof(AddFacility):
                    AddFacility();
                    return true;
                case nameof(IsEnabledAddFacility):
                    result = IsEnabledAddFacility();
                    return true;
                case nameof(DeleteFacility):
                    DeleteFacility();
                    return true;
                case nameof(IsEnabledDeleteFacility):
                    result = IsEnabledDeleteFacility();
                    return true;
                case nameof(ShowFacility):
                    ShowFacility();
                    return true;
                case nameof(IsEnabledShowFacility):
                    result = IsEnabledShowFacility();
                    return true;
                case nameof(GenerateTestOEEData):
                    GenerateTestOEEData();
                    return true;
                case nameof(IsEnabledGenerateTestOEEData):
                    result = IsEnabledGenerateTestOEEData();
                    return true;
                case nameof(DeleteTestOEEData):
                    DeleteTestOEEData();
                    return true;
                case nameof(IsEnabledDeleteTestOEEData):
                    result = IsEnabledDeleteTestOEEData();
                    return true;
                case nameof(RecalcThroughputAverage):
                    RecalcThroughputAverage();
                    return true;
                case nameof(IsEnabledRecalcThroughputAverage):
                    result = IsEnabledRecalcThroughputAverage();
                    return true;
                case nameof(RecalcThroughputAndOEE):
                    RecalcThroughputAndOEE();
                    return true;
                case nameof(IsEnabledRecalcThroughputAndOEE):
                    result = IsEnabledRecalcThroughputAndOEE();
                    return true;
                case nameof(TranslationNew):
                    TranslationNew();
                    return true;
                case nameof(TranslationDelete):
                    TranslationDelete();
                    return true;
                case nameof(IsEnabledTranslationNew):
                    result = IsEnabledTranslationNew();
                    return true;
                case nameof(IsEnabledTranslationDelete):
                    result = IsEnabledTranslationDelete();
                    return true;
                case nameof(SearchAssociatedPos):
                    SearchAssociatedPos();
                    return true;
                case nameof(IsEnabledSearchAssociatedPos):
                    result = IsEnabledSearchAssociatedPos();
                    return true;
                case nameof(OpenQueryDialog):
                    result = OpenQueryDialog();
                    return true;
                case nameof(AddPWMethodNodeConfig):
                    AddPWMethodNodeConfig();
                    return true;
                case nameof(IsEnabledAddPWMethodNodeConfig):
                    result = IsEnabledAddPWMethodNodeConfig();
                    return true;
                case nameof(DeletePWMethodNodeConfig):
                    DeletePWMethodNodeConfig();
                    return true;
                case nameof(IsEnabledDeletePWMethodNodeConfig):
                    result = IsEnabledDeletePWMethodNodeConfig();
                    return true;
                case nameof(ValidateInput):
                    result = ValidateInput((System.String)acParameter[0], (System.Object)acParameter[1], (System.Globalization.CultureInfo)acParameter[2]);
                    return true;
                case nameof(ConvertAmbientVolToRefVol15):
                    ConvertAmbientVolToRefVol15();
                    return true;
                case nameof(IsEnabledConvertAmbientVolToRefVol15):
                    result = IsEnabledConvertAmbientVolToRefVol15();
                    return true;
                case nameof(ConvertRefVol15ToAmbientVol):
                    ConvertRefVol15ToAmbientVol();
                    return true;
                case nameof(IsEnabledConvertRefVol15ToAmbientVol):
                    result = IsEnabledConvertRefVol15ToAmbientVol();
                    return true;
                case nameof(ConvertAmbVolToMass):
                    ConvertAmbVolToMass();
                    return true;
                case nameof(IsEnabledConvertAmbVolToMass):
                    result = IsEnabledConvertAmbVolToMass();
                    return true;
                case nameof(ConvertMassToAmbVol):
                    ConvertMassToAmbVol();
                    return true;
                case nameof(IsEnabledConvertMassToAmbVol):
                    result = IsEnabledConvertMassToAmbVol();
                    return true;
                case nameof(CalcDensityAndTemp):
                    CalcDensityAndTemp();
                    return true;
                case nameof(IsEnabledCalcDensityAndTemp):
                    result = IsEnabledCalcDensityAndTemp();
                    return true;
                case nameof(CalcDensityAndVol):
                    CalcDensityAndVol();
                    return true;
                case nameof(IsEnabledCalcDensityAndVol):
                    result = IsEnabledCalcDensityAndVol();
                    return true;
                case nameof(ShowMaterialOptions):
                    ShowMaterialOptions();
                    return true;
                case nameof(IsEnabledShowMaterialOptions):
                    result = IsEnabledShowMaterialOptions();
                    return true;
                case nameof(MoveToAnotherIntermediate):
                    MoveToAnotherIntermediate();
                    return true;
                case nameof(IsEnabledMoveToAnotherIntermediate):
                    result = IsEnabledMoveToAnotherIntermediate();
                    return true;
                case nameof(MoveAndReplaceMaterial):
                    MoveAndReplaceMaterial();
                    return true;
                case nameof(IsEnabledMoveAndReplaceMaterial):
                    result = IsEnabledMoveAndReplaceMaterial();
                    return true;
                case nameof(ReplaceMaterial):
                    ReplaceMaterial();
                    return true;
                case nameof(IsEnabledReplaceMaterial):
                    result = IsEnabledReplaceMaterial();
                    return true;
                case nameof(ShowDialogOrderInfo):
                    ShowDialogOrderInfo((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(ShowDialogMaterial):
                    ShowDialogMaterial((String)acParameter[0]);
                    return true;
                case nameof(NavigateToMaterialOverview):
                    NavigateToMaterialOverview();
                    return true;
                case nameof(IsEnabledNavigateToMaterialOverview):
                    result = IsEnabledNavigateToMaterialOverview();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}