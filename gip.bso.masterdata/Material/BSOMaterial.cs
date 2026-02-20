// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
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
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using static gip.core.datamodel.Global;
using Microsoft.EntityFrameworkCore;
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
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOMaterial&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
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

        /// <summary>
        /// Deinitializes the current instance and releases associated resources.
        /// </summary>
        /// <remarks>This method performs cleanup operations by releasing references to internal resources
        /// and detaching  service instances. It also invokes the base class's deinitialization logic. If any associated
        /// components require their own deinitialization, this method ensures they are properly deinitialized  before
        /// being set to null.</remarks>
        /// <param name="deleteACClassTask">A boolean value indicating whether to delete the associated AC class task.  If <see langword="true"/>, the
        /// AC class task will be deleted; otherwise, it will not.</param>
        /// <returns><see langword="true"/> if the deinitialization was successful; otherwise, <see langword="false"/>.</returns>
        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
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
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessMaterialUnit != null)
            {
                await _AccessMaterialUnit.ACDeInit(false);
                _AccessMaterialUnit = null;
            }
            if (_AccessMaterialCalculation != null)
            {
                await _AccessMaterialCalculation.ACDeInit(false);
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
        /// <summary>
        /// Gets the configuration manager.
        /// </summary>
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
        /// <summary>
        /// Gets the facility OEE (Overall Equipment Effectiveness) manager.
        /// </summary>
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
        /// <summary>
        /// Gets the child item representing the facility explorer associated with this instance.
        /// The child item is lazily initialized upon first access. This property ensures that
        /// the associated facility explorer is properly linked to the parent object.
        /// </summary>
        [ACPropertyInfo(600, Description =
                        @"Gets the child item representing the facility explorer associated with this instance.
                         The child item is lazily initialized upon first access. This property ensures that
                         the associated facility explorer is properly linked to the parent object.")]
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
        /// <summary>
        /// Gets the child item representing the associated BSOMedia object.
        /// This property initializes the child item on first access and associates it with the
        /// current object.
        /// </summary>
        [ACPropertyInfo(9999, Description =
                        @"Gets the child item representing the facility explorer associated with this instance.
                          The child item is lazily initialized upon first access. This property ensures that
                          the associated facility explorer is properly linked to the parent object.")]
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
        /// Gets or sets the current material associated with the object. Is used to display and edit the currently selected record.
        /// Changing the value of this property triggers updates to related properties and state,
        /// including resetting certain dependent values and notifying listeners of property changes. If the provided
        /// material is not null, additional operations such as loading media or updating visited
        /// materials may be performed.</summary>
        [ACPropertyCurrent(9999, Material.ClassName, Description =
                           @"Gets or sets the current material associated with the object. Is used to display and edit the currently selected record.
                             Changing the value of this property triggers updates to related properties and state,
                             including resetting certain dependent values and notifying listeners of property changes. If the provided
                             material is not null, additional operations such as loading media or updating visited
                             materials may be performed.")]
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
                            if (value != null && value.EntityState != EntityState.Added)
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
                        Messages.ExceptionAsync(this, e.Message, true);
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

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <remarks>The cloned object is a deep copy of the current instance, with the exception of the 
        /// <c>_CloneFrom</c> field, which is set to reference the original instance.</remarks>
        /// <returns>A new <see cref="BSOMaterial"/> object that is a copy of the current instance.</returns>
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
        /// Gets access to the MaterialUnit associated with the current material.
        /// </summary>
        /// <remarks>This property dynamically initializes the access object for <see
        /// cref="MaterialUnit"/> if it has not already been created. The initialization depends on the current
        /// <c>ACType</c> and related query definitions.</remarks>
        [ACPropertyAccess(9999, "MaterialUnit", Description = "Gets access to the MaterialUnit associated with the current material.")]
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
        [ACPropertyCurrent(9999, nameof(MaterialUnit), Description = "Gets or sets the current material unit.")]
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
        /// Gets the list of material units associated with the current material.
        /// </summary>
        /// <remarks>The material units are ordered by their sort index, if available. If no sort index is
        /// defined, the default order is used.</remarks>
        [ACPropertyList(9999, nameof(MaterialUnit), Description = "Gets the list of material units associated with the current material.")]
        public IEnumerable<MaterialUnit> MaterialUnitList
        {
            get
            {
                if (CurrentMaterial == null)
                {
                    CurrentMaterialUnit = null;
                    return null;
                }
                if (CurrentMaterial.MaterialUnit_Material == null || CurrentMaterial.MaterialUnit_Material.Count <= 0)
                    return null;
                return CurrentMaterial.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0).ToList();
            }
        }

        MaterialUnit _CurrentNewMaterialUnit;
        /// <summary>
        /// Gets or sets the current new  material unit for material.
        /// </summary>
        [ACPropertyCurrent(9999, nameof(NewMaterialUnit), Description = "Gets or sets the current new  material unit for material.")]
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

        /// <summary>
        /// Gets a collection of units that can be converted from the base unit of the current material.
        /// The returned collection includes units that are not already defined as alternative
        /// units for the current material.  If the database state has not changed, the query ensures that the latest
        /// data is retrieved by overwriting any cached changes.</summary>
        [ACPropertyList(9999, nameof(ConvertableUnits), Description =
                        @"Gets a collection of units that can be converted from the base unit of the current material.
                          The returned collection includes units that are not already defined as alternative
                          units for the current material.  If the database state has not changed, the query ensures that the latest
                          data is retrieved by overwriting any cached changes.")]
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
                //No Replacement for MergeOption and ObjectQuery
                //if (!DatabaseApp.IsChanged)
                //    (query as ObjectQuery).MergeOption = MergeOption.OverwriteChanges;
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
        /// Gets the access object for performing operations on MaterialCalculation entities.
        /// This property allows interaction with MaterialCalculation entities
        /// through the associated access object.
        /// </summary>
        [ACPropertyAccess(9999, "MaterialCalculation", Description =
                          @"Gets the access object for performing operations on MaterialCalculation entities.
                            This property allows interaction with MaterialCalculation entities
                            through the associated access object.")]
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
        /// This property represents the currently selected material calculation that is being viewed or edited.
        /// It provides access to calculation data including costs, production quantities, and validation dates for the current material.
        /// </summary>
        [ACPropertyCurrent(9999, nameof(MaterialCalculation), Description =
                           @"Gets or sets the current material calculation.
                             This property represents the currently selected material calculation that is being viewed or edited.
                             It provides access to calculation data including costs, production quantities, and validation dates for the current material.")]
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
        [ACPropertySelected(9999, nameof(MaterialCalculation), Description = "Gets or sets the selected material calculation.")]
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
        /// Gets the list of material calculations associated with the current material.
        /// Returns null if no material is currently selected. If a material is selected,
        /// returns all material calculations ordered by their validation date in descending order.
        /// </summary>
        [ACPropertyList(9999, nameof(MaterialCalculation), Description =
                       @"Gets the list of material calculations associated with the current material.
                         Returns null if no material is currently selected. If a material is selected,
                         returns all material calculations ordered by their validation date in descending order.")]
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
        /// Gets or sets the current material configuration.
        /// This property represents the currently selected material configuration that is being viewed or edited.
        /// It provides access to configuration settings specific to the current material.
        /// </summary>
        [ACPropertyCurrent(9999, "ACConfig", Description =
                           @"Gets or sets the current material configuration.
                             This property represents the currently selected material configuration that is being viewed or edited.
                             It provides access to configuration settings specific to the current material.")]
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
        /// Gets the list of material configurations associated with the current material.
        /// This property provides access to all configuration entries for the current material,
        /// enabling viewing and management of material-specific configuration settings.
        /// Returns null if no material is currently selected.
        /// </summary>
        [ACPropertyList(9999, "ACConfig", Description =
                        @"Gets the list of material configurations associated with the current material.
                          This property provides access to all configuration entries for the current material,
                          enabling viewing and management of material-specific configuration settings.
                          Returns null if no material is currently selected.")]
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
        /// Gets or sets the selected material configuration.
        /// This property represents the material configuration that is currently selected in the user interface.
        /// It is used for highlighting and identifying which configuration entry the user has chosen from the list.
        /// </summary>
        [ACPropertySelected(9999, "ACConfig", Description =
                            @"Gets or sets the selected material configuration.
                              This property represents the material configuration that is currently selected in the user interface.
                              It is used for highlighting and identifying which configuration entry the user has chosen from the list.")]
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
        /// <summary>
        /// Gets or sets the ACUrl of the remote material forwarder service.
        /// This property configures the URL for a remote service that forwards material changes
        /// to other systems or services when materials are saved. The forwarder is used to
        /// synchronize material data across distributed systems or notify external services
        /// of material updates.
        /// </summary>
        [ACPropertyConfig("en{'ACUrl of RemoteMaterialForwarder'}de{'ACUrl of RemoteMaterialForwarder'}", Description =
                          @"Gets or sets the ACUrl of the remote material forwarder service.
                            This property configures the URL for a remote service that forwards material changes
                            to other systems or services when materials are saved. The forwarder is used to
                            synchronize material data across distributed systems or notify external services
                            of material updates.")]
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
        /// This property represents the facility charge that is currently selected in the user interface.
        /// </summary>
        [ACPropertySelected(9999, FacilityCharge.ClassName, Description =
                            @"Gets or sets the selected facility charge.
                              This property represents the facility charge that is currently selected in the user interface.")]
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
        /// This property represents the currently selected facility charge that is being viewed or edited.
        /// </summary>
        [ACPropertyCurrent(9999, FacilityCharge.ClassName, Description =
                           @"Gets or sets the current facility charge.
                             This property represents the currently selected facility charge that is being viewed or edited.")]
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
        /// Gets the list of facility charges associated with the current material.
        /// Returns null if no material is currently selected.
        /// </summary>
        [ACPropertyList(9999, FacilityCharge.ClassName, Description =
                        @"Gets the list of facility charges associated with the current material.
                          Returns null if no material is currently selected.")]
        public IEnumerable<FacilityCharge> FacilityChargeList
        {
            get
            {
                if (CurrentMaterial == null)
                    return null;
                return CurrentMaterial.FacilityCharge_Material.ToList();
            }
        }
        #endregion

        #region Conversion-Test
        private double _ConvertTestInput;
        /// <summary>
        /// Gets or sets the input value for unit conversion testing.
        /// This property is used to specify the source quantity that will be converted 
        /// between different units of measurement for the current material.
        /// </summary>
        [ACPropertyInfo(9999, "Conversiontest", "en{'Input'}de{'Eingabe'}", Description =
                        @"Gets or sets the input value for unit conversion testing.
                          This property is used to specify the source quantity that will be converted 
                          between different units of measurement for the current material.")]
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
        /// <summary>
        /// Gets or sets the output value for unit conversion testing.
        /// This property represents the result of the conversion calculation when 
        /// testing unit conversions for the current material. The value is computed
        /// based on the input quantity and the selected target unit of measurement.
        /// </summary>
        [ACPropertyInfo(9999, "Conversiontest", "en{'Output'}de{'Ausgabe'}", Description =
                        @"Gets or sets the output value for unit conversion testing.
                          This property represents the result of the conversion calculation when 
                          testing unit conversions for the current material. The value is computed
                          based on the input quantity and the selected target unit of measurement.")]
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
        /// <summary>
        /// Gets or sets the selected unit for conversion testing.
        /// This property represents the target unit of measurement that will be used
        /// in unit conversion tests for the current material. When set, it enables
        /// conversion calculations between the material's base unit and the selected unit.
        /// </summary>
        [ACPropertySelected(9999, "Conversiontest", "en{'To Unit'}de{'Nach Einheit'}", Description =
                            @"Gets or sets the selected unit for conversion testing.
                              This property represents the target unit of measurement that will be used
                              in unit conversion tests for the current material. When set, it enables
                              conversion calculations between the material's base unit and the selected unit.")]
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
        /// <summary>
        /// Gets or sets the current unit for conversion testing.
        /// This property represents the unit of measurement that is currently being used
        /// for unit conversion tests and calculations for the current material.
        /// </summary>
        [ACPropertyCurrent(9999, "Conversiontest", "en{'To Unit'}de{'Nach Einheit'}", Description =
                          @"Gets or sets the current unit for conversion testing.
                            This property represents the unit of measurement that is currently being used
                            for unit conversion tests and calculations for the current material.")]
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

        /// <summary>
        /// Gets the list of units available for conversion testing with the current material.
        /// Returns null if no material is currently selected. If a material is selected,
        /// returns all units of measurement that can be used for unit conversion tests,
        /// including the material's base unit and any alternative units defined for the material.
        /// </summary>
        [ACPropertyList(9999, "Conversiontest", Description =
                        @"Gets the list of units available for conversion testing with the current material.
                          Returns null if no material is currently selected. If a material is selected,
                          returns all units of measurement that can be used for unit conversion tests,
                          including the material's base unit and any alternative units defined for the material.")]
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
        /// Gets or sets the selected facility material.
        /// This property represents the facility material that is currently selected in the user interface,
        /// typically used for highlighting and identifying which facility material entry the user has chosen from the list.
        /// </summary>
        [ACPropertySelected(9999, "FacilityMaterial", "en{'TODO: FacilityMaterial'}de{'TODO: FacilityMaterial'}", Description =
                            @"Gets or sets the selected facility material.
                              This property represents the facility material that is currently selected in the user interface,
                              typically used for highlighting and identifying which facility material entry the user has chosen from the list.")]
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
        /// Gets or sets the list of facility materials associated with the selected material.
        /// This property provides access to all facility-material relationships for the current material,
        /// allowing management of which facilities can store or handle the selected material.
        /// The list is automatically loaded when accessed and can be refreshed by setting a new value.
        /// </summary>
        [ACPropertyList(9999, "FacilityMaterial", Description =
                        @"Gets or sets the list of facility materials associated with the selected material.
                          This property provides access to all facility-material relationships for the current material,
                          allowing management of which facilities can store or handle the selected material.
                          The list is automatically loaded when accessed and can be refreshed by setting a new value.")]
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
                        if (CurrentMaterial != null && CurrentMaterial.FacilityCharge_Material != null && CurrentMaterial.FacilityCharge_Material.Count > 0)
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
        /// Saves the current material and all related changes to the database.
        /// This method calls the base OnSave() method to persist all modifications made to the CurrentMaterial
        /// and its associated entities such as material units, calculations, configurations, and facility materials.
        /// The method should be called after making changes to material properties to ensure data consistency.
        /// </summary>
        [ACMethodCommand(Material.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Saves the current material and all related changes to the database.
                           This method calls the base OnSave() method to persist all modifications made to the CurrentMaterial
                           and its associated entities such as material units, calculations, configurations, and facility materials.
                           The method should be called after making changes to material properties to ensure data consistency.")]
        public async Task Save()
        {
            await OnSave(); 
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
        /// Reverts all unsaved changes made to the current material and related entities back to their original database state.
        /// This method calls the base OnUndoSave() method to discard any modifications and then reloads the current material
        /// with fresh data from the database to ensure consistency. All pending changes to material properties, units,
        /// calculations, configurations, and other related entities will be lost.
        /// </summary>
        [ACMethodCommand(Material.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Reverts all unsaved changes made to the current material and related entities back to their original database state.
                           This method calls the base OnUndoSave() method to discard any modifications and then reloads the current material
                           with fresh data from the database to ensure consistency. All pending changes to material properties, units,
                           calculations, configurations, and other related entities will be lost.")]
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
        /// Loads the current material entity from the database with fresh data and related entities.
        /// This method refreshes the CurrentMaterial with the latest data from the database, including
        /// all related entities such as base unit, material groups, units, calculations, and translations.
        /// If requery is true, it also refreshes the material unit collection and triggers property
        /// change notifications to update the UI.
        /// </summary>
        /// <param name="requery">If true, forces a refresh of related collections and triggers UI updates</param>
        [ACMethodInteraction(Material.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, nameof(SelectedMaterial), Global.ACKinds.MSMethodPrePost, Description =
                             @"Loads the current material entity from the database with fresh data and related entities.
                               This method refreshes the CurrentMaterial with the latest data from the database, including
                               all related entities such as base unit, material groups, units, calculations, and translations.
                               If requery is true, it also refreshes the material unit collection and triggers property
                               change notifications to update the UI.")]
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
                	CurrentMaterial.MaterialUnit_Material.AutoRefresh(CurrentMaterial.MaterialUnit_MaterialReference, CurrentMaterial);
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
        /// Creates a new material record and sets it as the current material.
        /// This method instantiates a new Material entity object and adds it to the database context,
        /// inserts it at the beginning of the navigation list, and sets it as both the current and selected material.
        /// The ACState is set to indicate a new record state for proper UI handling.
        /// </summary>
        [ACMethodInteraction(Material.ClassName, Const.New, (short)MISort.New, true, nameof(SelectedMaterial), Global.ACKinds.MSMethodPrePost, Description =
                             @"Creates a new material record and sets it as the current material.
                               This method instantiates a new Material entity object and adds it to the database context,
                               inserts it at the beginning of the navigation list, and sets it as both the current and selected material.
                               The ACState is set to indicate a new record state for proper UI handling.")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentMaterial = Material.NewACObject(DatabaseApp, null);
            DatabaseApp.Material.Add(CurrentMaterial);
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
        /// Deletes the current material record from the database.
        /// This method performs the following operations:
        /// 1. Calls PreExecute to validate the delete operation can proceed
        /// 2. If BSOMedia_Child exists, deletes any associated media objects for the material
        /// 3. Attempts to delete the material entity using DeleteACObject with validation enabled
        /// 4. If deletion fails, displays the error message and returns
        /// 5. If successful, removes the material from the navigation list
        /// 6. Sets the SelectedMaterial to the first available material in the list
        /// 7. Triggers property change notification to update the UI
        /// Note: Always call Save() after Delete() to execute the delete operation in the database.
        /// </summary>
        [ACMethodInteraction(Material.ClassName, Const.Delete, (short)MISort.Delete, true, nameof(CurrentMaterial), Global.ACKinds.MSMethodPrePost, Description =
                             @"Deletes the current material record from the database.
                               This method performs the following operations:
                               1. Calls PreExecute to validate the delete operation can proceed
                               2. If BSOMedia_Child exists, deletes any associated media objects for the material
                               3. Attempts to delete the material entity using DeleteACObject with validation enabled
                               4. If deletion fails, displays the error message and returns
                               5. If successful, removes the material from the navigation list
                               6. Sets the SelectedMaterial to the first available material in the list
                               7. Triggers property change notification to update the UI
                               Note: Always call Save() after Delete() to execute the delete operation in the database.")]
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
                Messages.MsgAsync(msg);
                return;
            }
            if (AccessPrimary == null) return;
            AccessPrimary.NavList.Remove(CurrentMaterial);

            SelectedMaterial = AccessPrimary.NavList.FirstOrDefault();
            Load();
            OnPropertyChanged(nameof(MaterialList));
            PostExecute("Delete");
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
        /// Searches for materials in the database based on the current filter criteria.
        /// This method executes a navigation search using the primary access object and applies
        /// the configured search parameters from the query definition. After the search completes,
        /// it triggers a property change notification to update the MaterialList in the user interface.
        /// </summary>
        [ACMethodCommand(Material.ClassName, "en{'Search material'}de{'Materialsuche'}", (short)MISort.Search, Description =
                         @"Searches for materials in the database based on the current filter criteria.
                           This method executes a navigation search using the primary access object and applies
                           the configured search parameters from the query definition. After the search completes,
                           it triggers a property change notification to update the MaterialList in the user interface.")]
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
                _VisitedMaterials.RemoveAll(c => c.EntityState == EntityState.Unchanged);
            }
            if (
                CurrentMaterial != null
                && BSOMedia_Child != null
                && BSOMedia_Child.Value != null
                && CurrentMaterial.EntityState == EntityState.Added
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
        /// Creates a new material unit for the current material and displays a dialog for configuration.
        /// This method instantiates a new MaterialUnit entity object, assigns it to CurrentNewMaterialUnit,
        /// and opens the "MaterialUnitNew" dialog for the user to configure the unit conversion parameters.
        /// After successful configuration, the new material unit will be added to the current material's
        /// unit collection, enabling alternative units of measurement for the material.
        /// </summary>
        [ACMethodInteraction("MaterialUnit", "en{'New Material Unit'}de{'Neue Materialeinheit'}", (short)MISort.New, true, nameof(CurrentMaterialUnit), Global.ACKinds.MSMethodPrePost, Description =
                             @"Creates a new material unit for the current material and displays a dialog for configuration.
                               This method instantiates a new MaterialUnit entity object, assigns it to CurrentNewMaterialUnit,
                               and opens the ""MaterialUnitNew"" dialog for the user to configure the unit conversion parameters.
                               After successful configuration, the new material unit will be added to the current material's
                               unit collection, enabling alternative units of measurement for the material.")]
        public async Task NewMaterialUnit()
        {
            if (!PreExecute(nameof(NewMaterialUnit))) 
                return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            CurrentNewMaterialUnit = MaterialUnit.NewACObject(DatabaseApp, CurrentMaterial);
            await ShowDialogAsync(this, "MaterialUnitNew");
            OnPropertyChanged(nameof(MaterialUnitList));

            PostExecute(nameof(NewMaterialUnit));
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
        /// Deletes the selected material unit from the current material.
        /// This method removes the material unit from the current material's unit collection
        /// and deletes it from the database. If the deletion fails due to validation errors
        /// or database constraints, an error message is displayed to the user.
        /// After successful deletion, the material unit list is refreshed to reflect the changes.
        /// </summary>
        [ACMethodInteraction("MaterialUnit", "en{'Delete Material Unit'}de{'Materialeinheit löschen'}", (short)MISort.Delete, true, nameof(CurrentMaterialUnit), Global.ACKinds.MSMethodPrePost, Description =
                             @"Deletes the selected material unit from the current material.
                               This method removes the material unit from the current material's unit collection
                               and deletes it from the database. If the deletion fails due to validation errors
                               or database constraints, an error message is displayed to the user.
                               After successful deletion, the material unit list is refreshed to reflect the changes.")]
        public void DeleteMaterialUnit()
        {
            if (CurrentMaterialUnit == null)
                return;
            if (!PreExecute(nameof(DeleteMaterialUnit))) 
                return;
            // Einfügen einer Eigenschaft 
            CurrentMaterial.MaterialUnit_Material.Remove(CurrentMaterialUnit);
            Msg msg = CurrentMaterialUnit.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            OnPropertyChanged(nameof(MaterialUnitList));

            PostExecute(nameof(DeleteMaterialUnit));
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
        /// Confirms the creation of a new material unit and adds it to the current material.
        /// This method closes the material unit creation dialog, assigns the new material unit as the current one,
        /// adds it to the current material's unit collection, and ensures proper unit assignment.
        /// It also triggers property change notifications to update the UI for material unit list and convertable units.
        /// </summary>
        [ACMethodCommand("NewMaterialUnit", Const.Ok, (short)MISort.Okay, Description =
                         @"Deletes the selected material unit from the current material.
                           This method removes the material unit from the current material's unit collection
                           and deletes it from the database. If the deletion fails due to validation errors
                           or database constraints, an error message is displayed to the user.
                           After successful deletion, the material unit list is refreshed to reflect the changes.")]
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
        /// Cancels the creation of a new material unit and closes the dialog.
        /// This method closes the material unit creation dialog, deletes the temporary material unit object
        /// from the database context if it exists, and resets the CurrentNewMaterialUnit property to null.
        /// If deletion fails, an error message is displayed to the user.
        /// </summary>
        [ACMethodCommand("NewMaterialUnit", Const.Cancel, (short)MISort.Cancel, Description =
                         @"Cancels the creation of a new material unit and closes the dialog.
                           This method closes the material unit creation dialog, deletes the temporary material unit object
                           from the database context if it exists, and resets the CurrentNewMaterialUnit property to null.
                           If deletion fails, an error message is displayed to the user.")]
        public void NewMaterialUnitCancel()
        {
            CloseTopDialog();
            if (CurrentNewMaterialUnit != null)
            {
                Msg msg = CurrentNewMaterialUnit.DeleteACObject(DatabaseApp, true);
                if (msg != null)
                {
                    Messages.MsgAsync(msg);
                    return;
                }

            }
            CurrentNewMaterialUnit = null;
        }

        #endregion

        #region BSO->ACMethod->MaterialCalculation

        /// <summary>
        /// Loads the currently selected material calculation and sets it as the current material calculation.
        /// This method retrieves the material calculation that matches the selected material calculation ID
        /// from the current material's collection and assigns it to the CurrentMaterialCalculation property.
        /// The method includes validation checks to ensure the operation can proceed safely.
        /// </summary>
        [ACMethodInteraction(nameof(MaterialCalculation), "en{'Load Position'}de{'Position laden'}", (short)MISort.Load, false, nameof(SelectedMaterialCalculation), Global.ACKinds.MSMethodPrePost, Description =
                             @"Loads the currently selected material calculation and sets it as the current material calculation.
                               This method retrieves the material calculation that matches the selected material calculation ID
                               from the current material's collection and assigns it to the CurrentMaterialCalculation property.
                               The method includes validation checks to ensure the operation can proceed safely.")]
        public void LoadMaterialCalculation()
        {
            if (!IsEnabledLoadMaterialCalculation())
                return;
            if (!PreExecute(nameof(LoadMaterialCalculation))) 
                return;
            // Laden des aktuell selektierten MaterialCalculation 
            CurrentMaterialCalculation = CurrentMaterial.MaterialCalculation_Material.Where(c => c.MaterialCalculationID == SelectedMaterialCalculation.MaterialCalculationID).FirstOrDefault();
            PostExecute(nameof(LoadMaterialCalculation));
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
        /// Creates a new material calculation for the current material and assigns it to CurrentMaterialCalculation.
        /// This method generates a unique secondary key using the NoManager, creates a new MaterialCalculation entity object,
        /// and adds it to the current material's calculation collection. The method includes proper pre/post execution validation
        /// and triggers a property change notification to update the MaterialCalculationList in the user interface.
        /// </summary>
        [ACMethodInteraction("MaterialCalculation", "en{'New MaterialCalculation'}de{'Neue Materialeinheit'}", (short)MISort.New, true, nameof(SelectedMaterialCalculation), Global.ACKinds.MSMethodPrePost, Description =
                             @"Creates a new material calculation for the current material and assigns it to CurrentMaterialCalculation.
                               This method generates a unique secondary key using the NoManager, creates a new MaterialCalculation entity object,
                               and adds it to the current material's calculation collection. The method includes proper pre/post execution validation
                               and triggers a property change notification to update the MaterialCalculationList in the user interface.")]
        public void NewMaterialCalculation()
        {
            if (!PreExecute(nameof(NewMaterialCalculation))) 
                return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(MaterialCalculation), MaterialCalculation.NoColumnName, MaterialCalculation.FormatNewNo, this);
            CurrentMaterialCalculation = MaterialCalculation.NewACObject(DatabaseApp, CurrentMaterial, secondaryKey);
            OnPropertyChanged(nameof(MaterialCalculationList));

            PostExecute(nameof(NewMaterialCalculation));
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
        /// Deletes the current material calculation from the database and its associated material.
        /// This method removes the material calculation from the current material's calculation collection
        /// and deletes it from the database context. If the deletion fails due to validation errors
        /// or database constraints, an error message is displayed to the user.
        /// The method includes proper pre/post execution validation and error handling.
        /// </summary>
        [ACMethodInteraction(nameof(MaterialCalculation), "en{'Delete MaterialCalculation'}de{'Materialeinheit löschen'}", (short)MISort.Delete, true, nameof(CurrentMaterialCalculation), Global.ACKinds.MSMethodPrePost, Description =
                             @"Deletes the current material calculation from the database and its associated material.
                               This method removes the material calculation from the current material's calculation collection
                               and deletes it from the database context. If the deletion fails due to validation errors
                               or database constraints, an error message is displayed to the user.
                               The method includes proper pre/post execution validation and error handling.")]
        public void DeleteMaterialCalculation()
        {
            if (CurrentMaterialCalculation == null)
                return;
            if (!PreExecute(nameof(DeleteMaterialCalculation))) 
                return;
            // Einfügen einer Eigenschaft 
            Msg msg = CurrentMaterialCalculation.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            PostExecute(nameof(DeleteMaterialCalculation));
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
        /// Loads the currently selected ACConfig (material configuration) and sets it as the current ACConfig.
        /// This method retrieves the material configuration that is currently selected in the user interface
        /// and assigns it to the CurrentACConfig property for viewing or editing. The method includes
        /// proper validation and pre/post execution handling to ensure the operation can proceed safely.
        /// </summary>
        [ACMethodInteraction("ACConfig", "en{'Load Attribute'}de{'Attribut laden'}", (short)MISort.Load, false, nameof(SelectedACConfig), Global.ACKinds.MSMethodPrePost, Description =
                             @"Loads the currently selected ACConfig (material configuration) and sets it as the current ACConfig.
                               This method retrieves the material configuration that is currently selected in the user interface
                               and assigns it to the CurrentACConfig property for viewing or editing. The method includes
                               proper validation and pre/post execution handling to ensure the operation can proceed safely.")]
        public void LoadACConfig()
        {
            if (!IsEnabledLoadACConfig())
                return;
            if (!PreExecute(nameof(LoadACConfig))) 
                return;
            // Laden des aktuell selektierten ACConfig 
            CurrentACConfig = SelectedACConfig;
            PostExecute(nameof(LoadACConfig));
        }

        /// <summary>
        /// Determines whether the loading of the AC configuration is enabled.
        /// </summary>
        /// <returns><see langword="true"/> if an AC configuration is selected; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledLoadACConfig()
        {
            return SelectedACConfig != null;
        }

        /// <summary>
        /// Creates a new material configuration (ACConfig) for the current material and adds it to the material's configuration collection.
        /// This method instantiates a new MaterialConfig entity object using the current material's NewACConfig() method,
        /// adds it to the CurrentMaterial's configuration collection, and triggers property change notifications
        /// to update the ACConfigList and CurrentACConfig properties in the user interface.
        /// The method includes proper pre/post execution validation to ensure the operation can proceed safely.
        /// </summary>
        [ACMethodInteraction("ACConfig", "en{'New Attribute'}de{'Neues Attribut'}", (short)MISort.New, true, nameof(SelectedACConfig), Global.ACKinds.MSMethodPrePost, Description =
                             @"Creates a new material configuration (ACConfig) for the current material and adds it to the material's configuration collection.
                               This method instantiates a new MaterialConfig entity object using the current material's NewACConfig() method,
                               adds it to the CurrentMaterial's configuration collection, and triggers property change notifications
                               to update the ACConfigList and CurrentACConfig properties in the user interface.
                               The method includes proper pre/post execution validation to ensure the operation can proceed safely.")]
        public void NewACConfig()
        {
            if (!PreExecute(nameof(NewACConfig))) 
                return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            //TODO: VBConfig stimmt was von Logik nicht, da MaterialConfig zurückkommt und neu erzeugte Objekt sowieso in anderem Context gar nicht existiert:
            CurrentACConfig = CurrentMaterial.NewACConfig() as MaterialConfig;
            OnPropertyChanged(nameof(ACConfigList));
            OnPropertyChanged(nameof(CurrentACConfig));
            PostExecute(nameof(NewACConfig));
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
        /// Deletes the current material configuration (ACConfig) from the database and its associated material.
        /// This method removes the material configuration from the current material's configuration collection
        /// and deletes it from the database context. If the deletion fails due to validation errors
        /// or database constraints, an error message is displayed to the user.
        /// The method includes proper pre/post execution validation and error handling.
        /// </summary>
        [ACMethodInteraction("ACConfig", "en{'Delete Attribute'}de{'Attribut löschen'}", (short)MISort.Delete, true, nameof(CurrentACConfig), Global.ACKinds.MSMethodPrePost, Description =
                             @"Deletes the current material configuration (ACConfig) from the database and its associated material.
                               This method removes the material configuration from the current material's configuration collection
                               and deletes it from the database context. If the deletion fails due to validation errors
                               or database constraints, an error message is displayed to the user.
                               The method includes proper pre/post execution validation and error handling.")]
        public void DeleteACConfig()
        {
            try
            {
                if (!PreExecute("DeleteACConfig")) return;
                Msg msg = CurrentACConfig.DeleteACObject(DatabaseApp, true);
                if (msg != null)
                {
                    Messages.MsgAsync(msg);
                    return;
                }

            }
            catch (Exception e)
            {
                Messages.ErrorAsync(this, "Message00001", false, e.Message);
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

        /// <summary>
        /// Converts the input test value from the selected unit to the base unit of measure (UOM) of the current material.
        /// This method performs unit conversion testing by taking the value in ConvertTestInput and the unit specified
        /// in SelectedUnitConvertTest, then converting it to the material's base unit and storing the result in ConvertTestOutput.
        /// If conversion fails due to incompatible units or other errors, the output is set to 0 and the error is logged.
        /// </summary>
        [ACMethodCommand("Conversiontest1", "en{'Convert to Base UOM'}de{'Umrechnen nach Basiseinheit'}", (short)MISort.Okay, Description =
                         @"Converts the input test value from the selected unit to the base unit of measure (UOM) of the current material.
                           This method performs unit conversion testing by taking the value in ConvertTestInput and the unit specified
                           in SelectedUnitConvertTest, then converting it to the material's base unit and storing the result in ConvertTestOutput.
                           If conversion fails due to incompatible units or other errors, the output is set to 0 and the error is logged.")]
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

        /// <summary>
        /// Determines whether the conversion from test to base is enabled.
        /// </summary>
        /// <remarks>The conversion is enabled if <c>SelectedUnitConvertTest</c> is not <see
        /// langword="null"/>.</remarks>
        /// <returns><see langword="true"/> if the conversion is enabled; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledConvertTestToBase()
        {
            if (SelectedUnitConvertTest != null)
                return true;
            return false;
        }

        /// <summary>
        /// Converts the input test value from the material's base unit to the selected target unit.
        /// This method performs unit conversion testing by taking the value in ConvertTestInput (assumed to be in the material's base unit)
        /// and converting it to the unit specified in SelectedUnitConvertTest, storing the result in ConvertTestOutput.
        /// If conversion fails due to incompatible units or other errors, the output is set to 0 and the error is logged.
        /// </summary>
        [ACMethodCommand("Conversiontest2", "en{'Convert from Base UOM'}de{'Umrechnen von Basiseinheit'}", (short)MISort.Okay, Description =
                         @"Converts the input test value from the material's base unit to the selected target unit.
                           This method performs unit conversion testing by taking the value in ConvertTestInput (assumed to be in the material's base unit)
                           and converting it to the unit specified in SelectedUnitConvertTest, storing the result in ConvertTestOutput.
                           If conversion fails due to incompatible units or other errors, the output is set to 0 and the error is logged.")]
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

        /// <summary>
        /// Determines whether the conversion test from the base unit is enabled.
        /// </summary>
        /// <returns><see langword="true"/> if a conversion test is selected and the base material unit is defined;  otherwise,
        /// <see langword="false"/>.</returns>
        public bool IsEnabledConvertTestFromBase()
        {
            if ((SelectedUnitConvertTest != null) && (CurrentMaterial.BaseMDUnit != null))
                return true;
            return false;
        }

        /// <summary>
        /// Converts the input test value from the current material unit to the selected target unit.
        /// This method performs unit conversion testing by taking the value in ConvertTestInput and the unit specified
        /// in CurrentMaterialUnit.ToMDUnit, then converting it to the unit specified in SelectedUnitConvertTest and storing the result in ConvertTestOutput.
        /// If conversion fails due to incompatible units or other errors, the output is set to 0 and the error is logged.
        /// </summary>
        [ACMethodCommand("Conversiontest3", "en{'Convert from Material Unit'}de{'Umrechnen von alt. Einheit'}", (short)MISort.Okay, Description =
                         @"Converts the input test value from the current material unit to the selected target unit.
                           This method performs unit conversion testing by taking the value in ConvertTestInput and the unit specified
                           in CurrentMaterialUnit.ToMDUnit, then converting it to the unit specified in SelectedUnitConvertTest and storing the result in ConvertTestOutput.
                           If conversion fails due to incompatible units or other errors, the output is set to 0 and the error is logged.")]
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

        /// <summary>
        /// Determines whether the unit conversion test is enabled based on the current selection and material unit.
        /// </summary>
        /// <returns><see langword="true"/> if both <see cref="SelectedUnitConvertTest"/> and <see cref="CurrentMaterialUnit"/>
        /// are not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledConvertTest()
        {
            if ((SelectedUnitConvertTest != null) && (this.CurrentMaterialUnit != null))
                return true;
            return false;
        }

        #endregion

        #region FacilityMaterial

        /// <summary>
        /// Adds a new facility material relationship for the currently selected material.
        /// This method creates a new FacilityMaterial entity object, associates it with the selected material,
        /// adds it to both the material's facility collection and the local FacilityMaterialList,
        /// and sets it as the currently selected facility material for further editing.
        /// </summary>
        [ACMethodInfo("", "en{'Add'}de{'Neu'}", 700, Description =
                      @"Adds a new facility material relationship for the currently selected material.
                        This method creates a new FacilityMaterial entity object, associates it with the selected material,
                        adds it to both the material's facility collection and the local FacilityMaterialList,
                        and sets it as the currently selected facility material for further editing.")]
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

        /// <summary>
        /// Determines whether the "Add Facility" operation is enabled.
        /// </summary>
        /// <returns><see langword="true"/> if a material is selected; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledAddFacility()
        {
            return SelectedMaterial != null;
        }

        /// <summary>
        /// Deletes the selected facility material relationship from the current material.
        /// This method removes the facility material from both the material's facility collection 
        /// and the local FacilityMaterialList, then deletes the entity from the database context.
        /// After successful deletion, the FacilityMaterialList property is refreshed to update the UI.
        /// </summary>
        [ACMethodInfo("", "en{'Delete'}de{'Löschen'}", 701, Description =
                      @"Deletes the selected facility material relationship from the current material.
                        This method removes the facility material from both the material's facility collection 
                        and the local FacilityMaterialList, then deletes the entity from the database context.
                        After successful deletion, the FacilityMaterialList property is refreshed to update the UI.")]
        public void DeleteFacility()
        {
            if (!IsEnabledDeleteFacility())
                return;

            SelectedMaterial.FacilityMaterial_Material.Remove(SelectedFacilityMaterial);
            FacilityMaterialList.Remove(SelectedFacilityMaterial);

            SelectedFacilityMaterial.DeleteACObject(DatabaseApp, false);

            OnPropertyChanged(nameof(FacilityMaterialList));
        }

        /// <summary>
        /// Determines whether the delete operation for the selected facility is enabled.
        /// </summary>
        /// <returns><see langword="true"/> if a facility material is selected; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledDeleteFacility()
        {
            return SelectedFacilityMaterial != null;
        }

        /// <summary>
        /// Displays a dialog for selecting a facility to associate with the currently selected facility material.
        /// This method opens a facility explorer dialog, allows the user to select a facility, and if confirmed,
        /// assigns the selected facility to the SelectedFacilityMaterial.Facility property.
        /// The method includes validation to ensure it can proceed safely and updates the UI after assignment.
        /// </summary>
        [ACMethodInfo("", "en{'Choose facility'}de{'Lager auswählen'}", 702, Description =
                      @"Displays a dialog for selecting a facility to associate with the currently selected facility material.
                        This method opens a facility explorer dialog, allows the user to select a facility, and if confirmed,
                        assigns the selected facility to the SelectedFacilityMaterial.Facility property.
                        The method includes validation to ensure it can proceed safely and updates the UI after assignment.")]
        public async Task ShowFacility()
        {
            if (!IsEnabledShowFacility())
                return;

            VBDialogResult dlgResult = await BSOFacilityExplorer_Child.Value.ShowDialog(SelectedFacilityMaterial?.Facility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                SelectedFacilityMaterial.Facility = facility;
                OnPropertyChanged(nameof(SelectedFacilityMaterial));
            }
        }

        /// <summary>
        /// Determines whether the facility display feature is enabled.
        /// </summary>
        /// <returns><see langword="true"/> if a facility material is selected; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledShowFacility()
        {
            return SelectedFacilityMaterial != null;
        }

        /// <summary>
        /// Generates test OEE (Overall Equipment Effectiveness) data for the selected facility material.
        /// This method creates sample OEE data entries that can be used for testing and simulation purposes.
        /// The test data includes availability, performance, and quality metrics for the facility-material combination.
        /// The operation is performed in a separate database context to ensure data integrity.
        /// If an error occurs during generation, the error message is displayed to the user.
        /// </summary>
        [ACMethodInfo("", "en{'Generate OEE test data'}de{'OEE Testdaten generieren'}", 703, Description =
                      @"Generates test OEE (Overall Equipment Effectiveness) data for the selected facility material.
                        This method creates sample OEE data entries that can be used for testing and simulation purposes.
                        The test data includes availability, performance, and quality metrics for the facility-material combination.
                        The operation is performed in a separate database context to ensure data integrity.
                        If an error occurs during generation, the error message is displayed to the user.")]
        public void GenerateTestOEEData()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                FacilityMaterial facilityMaterial = SelectedFacilityMaterial.FromAppContext<FacilityMaterial>(dbApp);
                Msg msg = FacilityOEEManager.GenerateTestOEEData(dbApp, facilityMaterial);
                if (msg != null)
                    Messages.MsgAsync(msg);
            }
        }

        /// <summary>
        /// Determines whether generating test OEE (Overall Equipment Effectiveness) data is enabled.
        /// </summary>
        /// <remarks>This method checks the availability of the required components for generating test
        /// OEE data. Ensure that <see cref="SelectedFacilityMaterial"/> and <see cref="FacilityOEEManager"/> are
        /// properly initialized before calling this method.</remarks>
        /// <returns><see langword="true"/> if both <see cref="SelectedFacilityMaterial"/> and <see cref="FacilityOEEManager"/>
        /// are not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledGenerateTestOEEData()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }

        /// <summary>
        /// Deletes test OEE (Overall Equipment Effectiveness) data for the selected facility material.
        /// This method removes all test data entries that were previously generated for testing and simulation purposes.
        /// The operation is performed in a separate database context to ensure data integrity.
        /// This method can be used to clean up test data after testing is complete.
        /// </summary>
        [ACMethodInfo("", "en{'Delete OEE test data'}de{'OEE Testdaten löschen'}", 704, Description =
                      @"Deletes test OEE (Overall Equipment Effectiveness) data for the selected facility material.
                        This method removes all test data entries that were previously generated for testing and simulation purposes.
                        The operation is performed in a separate database context to ensure data integrity.
                        This method can be used to clean up test data after testing is complete.")]
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

        /// <summary>
        /// Determines whether the deletion of test OEE data is enabled based on the current state.
        /// </summary>
        /// <remarks>This method checks the prerequisites for enabling the deletion of test OEE data.
        /// Ensure that both <see cref="SelectedFacilityMaterial"/> and <see cref="FacilityOEEManager"/> are properly
        /// initialized before invoking this method.</remarks>
        /// <returns><see langword="true"/> if both <see cref="SelectedFacilityMaterial"/> and <see cref="FacilityOEEManager"/>
        /// are not null; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledDeleteTestOEEData()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }

        /// <summary>
        /// Recalculates the average throughput for the selected facility material.
        /// This method updates the throughput average based on historical OEE data
        /// for the facility-material combination. The calculation uses the latest
        /// entries to determine an accurate average throughput value.
        /// </summary>
        [ACMethodInfo("", "en{'Recalc average throughput'}de{'Aktualisiere Mittelwert Durchsatz'}", 705, Description =
                      @"Recalculates the average throughput for the selected facility material.
                        This method updates the throughput average based on historical OEE data
                        for the facility-material combination. The calculation uses the latest
                        entries to determine an accurate average throughput value.")]
        public void RecalcThroughputAverage()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;

            Msg msg = FacilityOEEManager.RecalcThroughputAverage(this.DatabaseApp, SelectedFacilityMaterial, false);
            if (msg != null)
                Messages.MsgAsync(msg);
        }

        /// <summary>
        /// Determines whether recalculation of the throughput average is enabled.
        /// </summary>
        /// <remarks>This method checks the availability of the required components to enable throughput
        /// average recalculation. Ensure that both <see cref="SelectedFacilityMaterial"/> and <see
        /// cref="FacilityOEEManager"/> are properly initialized  before invoking this method.</remarks>
        /// <returns><see langword="true"/> if both <see cref="SelectedFacilityMaterial"/> and <see cref="FacilityOEEManager"/>
        /// are not null; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledRecalcThroughputAverage()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }

        /// <summary>
        /// Recalculates throughput and OEE (Overall Equipment Effectiveness) values for the selected facility material.
        /// This method corrects and updates throughput measurements and OEE calculations based on historical data
        /// for the facility-material combination. The operation is performed in a separate database context to ensure
        /// data integrity and processes all relevant OEE entries to provide accurate performance metrics.
        /// If an error occurs during the recalculation process, the error message is displayed to the user.
        /// </summary>
        [ACMethodInfo("", "en{'Correct throuhputs and OEE'}de{'Korrigiere Durchsätze und OEE'}", 706, Description =
                      @"Recalculates throughput and OEE (Overall Equipment Effectiveness) values for the selected facility material.
                        This method corrects and updates throughput measurements and OEE calculations based on historical data
                        for the facility-material combination. The operation is performed in a separate database context to ensure
                        data integrity and processes all relevant OEE entries to provide accurate performance metrics.
                        If an error occurs during the recalculation process, the error message is displayed to the user.")]
        public void RecalcThroughputAndOEE()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                FacilityMaterial facilityMaterial = SelectedFacilityMaterial.FromAppContext<FacilityMaterial>(dbApp);
                Msg msg = FacilityOEEManager.RecalcThroughputAndOEE(dbApp, facilityMaterial, null, null);
                if (msg != null)
                    Messages.MsgAsync(msg);
            }
        }

        /// <summary>
        /// Determines whether recalculation of throughput and OEE is enabled.
        /// </summary>
        /// <returns><see langword="true"/> if both the selected facility material and the facility OEE manager are set;
        /// otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledRecalcThroughputAndOEE()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }
        #endregion

        #region Show Dialog

        /// <summary>
        /// Displays a dialog for a material based on order information.
        /// This method extracts material information from the provided PAOrderInfo object,
        /// locates the corresponding material in the database, and shows a dialog
        /// displaying the material details using the material number.
        /// </summary>
        /// <param name="paOrderInfo">The order information containing entity details including material ID</param>
        [ACMethodInfo("Dialog", "en{'Dialog lot overview'}de{'Dialog Losübersicht'}", (short)MISort.QueryPrintDlg + 1, Description =
                      @"Displays a dialog for a material based on order information.
                        This method extracts material information from the provided PAOrderInfo object,
                        locates the corresponding material in the database, and shows a dialog
                        displaying the material details using the material number.")]
        public virtual async Task ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;

            PAOrderInfoEntry entityInfo = paOrderInfo.Entities.Where(c => c.EntityName == nameof(Material)).FirstOrDefault();
            if (entityInfo == null)
                return;

            Material material = this.DatabaseApp.Material.Where(c => c.MaterialID == entityInfo.EntityID).FirstOrDefault();
            if (material == null)
                return;

            await ShowDialogMaterial(material.MaterialNo);
        }

        /// <summary>
        /// Displays a material dialog for the specified material number.
        /// This method searches for the material by material number, opens the "OrderInfoDialog" to display material details,
        /// and then stops the current component. The method configures the search filter to locate the material
        /// and shows it in a dialog interface for viewing material information.
        /// </summary>
        /// <param name="materialNo">The material number to search for and display in the dialog</param>
        [ACMethodInfo("Dialog", "en{'Dialog Material'}de{'Dialog Material'}", (short)MISort.QueryPrintDlg, Description =
                      @"Displays a material dialog for the specified material number.
                        This method searches for the material by material number, opens the ""OrderInfoDialog"" to display material details,
                        and then stops the current component. The method configures the search filter to locate the material
                        and shows it in a dialog interface for viewing material information.")]
        public async Task ShowDialogMaterial(string materialNo)
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
            await ShowDialogAsync(this, "OrderInfoDialog");
            await this.ParentACComponent.StopComponent(this);
        }
        #endregion

        #region Navigation
        
        /// <summary>
        /// Navigates to the material overview dialog to display stock information and history for the selected material.
        /// This method uses the PAShowDlgManagerBase service to open a dialog that shows comprehensive information
        /// about the material including current stock levels, stock movements, and historical data.
        /// The dialog is configured with DialogSelectInfo = 1 to display the material overview mode.
        /// </summary>
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

        /// <summary>
        /// Determines whether navigation to the material overview is enabled.
        /// </summary>
        /// <returns><see langword="true"/> if a material is selected; otherwise, <see langword="false"/>.</returns>
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
        /// <summary>
        /// Gets the collection of available translation languages from the database.
        /// This property provides access to all VBLanguage entities, ordered by their language code,
        /// for use in translation functionality. The languages are cached after the first access
        /// to improve performance on subsequent calls.
        /// </summary>
        [ACPropertyInfo(9999, "TranslationLang", Description =
                        @"Gets the collection of available translation languages from the database.
                          This property provides access to all VBLanguage entities, ordered by their language code,
                          for use in translation functionality. The languages are cached after the first access
                          to improve performance on subsequent calls.")]
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
        /// <summary>
        /// Gets or sets the selected label translation for the current material.
        /// This property represents the translation entry that is currently selected in the user interface
        /// for viewing or editing. The selected translation corresponds to a specific language translation
        /// of the material's label text. When set, it triggers property change notifications to update the UI.
        /// </summary>
        [ACPropertySelected(9999, "Translation", Description =
                           @"Gets or sets the selected label translation for the current material.
                             This property represents the translation entry that is currently selected in the user interface
                             for viewing or editing. The selected translation corresponds to a specific language translation
                             of the material's label text. When set, it triggers property change notifications to update the UI.")]
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

        /// <summary>
        /// Gets the list of label translations associated with the current material.
        /// This property provides access to all translation entries for the material's label,
        /// enabling viewing and management of multilingual text for the material.
        /// Returns an empty list if no material is currently selected or if the material has no label.
        /// </summary>
        [ACPropertyList(9999, "Translation", Description =
                        @"Gets the list of label translations associated with the current material.
                          This property provides access to all translation entries for the material's label,
                          enabling viewing and management of multilingual text for the material.
                          Returns an empty list if no material is currently selected or if the material has no label.")]
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

        /// <summary>
        /// Creates a new translation entry for the current material's label.
        /// This method ensures that the material has a label object, creates a new LabelTranslation entity,
        /// adds it to the database context and the material's label translation collection,
        /// and sets it as the currently selected translation for editing.
        /// </summary>
        [ACMethodInteraction(Material.ClassName, Const.New, (short)MISort.New, true, "SelectedTranslation", Global.ACKinds.MSMethodPrePost, Description =
                             @"Creates a new translation entry for the current material's label.
                               This method ensures that the material has a label object, creates a new LabelTranslation entity,
                               adds it to the database context and the material's label translation collection,
                               and sets it as the currently selected translation for editing.")]
        public void TranslationNew()
        {
            if (CurrentMaterial.Label == null)
            {
                CurrentMaterial.Label = Label.NewACObject(DatabaseApp, CurrentMaterial);
                DatabaseApp.Label.Add(CurrentMaterial.Label);
            }
            LabelTranslation translation = LabelTranslation.NewACObject(DatabaseApp, CurrentMaterial.Label);
            DatabaseApp.LabelTranslation.Add(translation);
            CurrentMaterial.Label.LabelTranslation_Label.Add(translation);
            SelectedTranslation = translation;
            OnPropertyChanged(nameof(TranslationList));
        }

        /// <summary>
        /// Deletes the selected label translation from the current material's label.
        /// This method removes the translation entry from the material's label translation collection
        /// and deletes it from the database context. If the deletion fails due to validation errors
        /// or database constraints, an error message is displayed to the user. After successful deletion,
        /// the selected translation is set to the first available translation in the list.
        /// </summary>
        [ACMethodInteraction(Material.ClassName, Const.Delete, (short)MISort.Delete, true, "SelectedTranslation", Global.ACKinds.MSMethodPrePost, Description =
                             @"Deletes the selected label translation from the current material's label.
                               This method removes the translation entry from the material's label translation collection
                               and deletes it from the database context. If the deletion fails due to validation errors
                               or database constraints, an error message is displayed to the user. After successful deletion,
                               the selected translation is set to the first available translation in the list.")]
        public void TranslationDelete()
        {
            Msg msg = SelectedTranslation.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            else
            {
                SelectedTranslation = TranslationList.FirstOrDefault();
            }
            OnPropertyChanged(nameof(TranslationList));
        }

        #region Translation -> Methods -> IsEnabled

        /// <summary>
        /// Determines whether the translation feature is enabled for the current material.
        /// </summary>
        /// <returns><see langword="true"/> if the current material is not <see langword="null"/>; otherwise, <see
        /// langword="false"/>.</returns>
        public bool IsEnabledTranslationNew()
        {
            return CurrentMaterial != null;
        }

        /// <summary>
        /// Determines whether the delete operation for the selected translation is enabled.
        /// </summary>
        /// <returns><see langword="true"/> if a translation is selected and the delete operation can be performed; otherwise,
        /// <see langword="false"/>.</returns>
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

        /// <summary>
        /// Gets the access object for navigating and querying PartslistPos entities that are associated with the current material.
        /// This property provides access to parts list positions that reference the current material as an intermediate product,
        /// enabling filtering and searching of bill of material positions across different workflows and production orders.
        /// The access object is lazily initialized with default sorting and filtering configurations.
        /// </summary>
        [ACPropertyAccess(100, "AssociatedPartslistPos", Description =
                          @"Gets the access object for navigating and querying PartslistPos entities that are associated with the current material.
                            This property provides access to parts list positions that reference the current material as an intermediate product,
                            enabling filtering and searching of bill of material positions across different workflows and production orders.
                            The access object is lazily initialized with default sorting and filtering configurations.")]
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

        /// <summary>
        /// Gets the default filter configuration for accessing associated parts list positions.
        /// </summary>
        /// <remarks>This property provides a predefined set of filters that can be used to query
        /// associated parts list positions. The filters include logical groupings and conditions to ensure relevant
        /// data is retrieved. Callers can use this property to apply consistent filtering logic across
        /// operations.</remarks>
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
        /// Gets or sets the selected PartslistPos (Bill of Material position) that is associated with the current material.
        /// This property represents the PartslistPos that is currently selected in the user interface for viewing.
        /// </summary>
        [ACPropertySelected(101, "AssociatedPartslistPos", Description =
                            @"Gets or sets the selected PartslistPos (Bill of Material position) that is associated with the current material.
                              This property represents the PartslistPos that is currently selected in the user interface for viewing.")]
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
        /// Gets the list of parts list positions (bill of material positions) that are associated with the current material.
        /// This property provides access to all PartslistPos entities that reference the current material as an intermediate product,
        /// enabling viewing and management of where the material is used in different bill of materials and production workflows.
        /// Returns null if the AccessAssociatedPartslistPos is not initialized.
        /// </summary>
        [ACPropertyList(102, "AssociatedPartslistPos", Description =
                        @"Gets the list of parts list positions (bill of material positions) that are associated with the current material.
                          This property provides access to all PartslistPos entities that reference the current material as an intermediate product,
                          enabling viewing and management of where the material is used in different bill of materials and production workflows.
                          Returns null if the AccessAssociatedPartslistPos is not initialized.")]
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
        /// Gets or sets the minimum quantity filter for associated parts list positions.
        /// This property allows filtering parts list positions based on their target quantity,
        /// showing only positions where the target quantity is greater than or equal to this value.
        /// When set, it updates the quantity search filter and triggers property change notification.
        /// </summary>
        [ACPropertyInfo(999, nameof(FilterAssociatedPosTargetQFrom), "en{'Quantity from'}de{'Menge von'}", Description =
                       @"Gets or sets the minimum quantity filter for associated parts list positions.
                         This property allows filtering parts list positions based on their target quantity,
                         showing only positions where the target quantity is greater than or equal to this value.
                         When set, it updates the quantity search filter and triggers property change notification.")]
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
        /// Gets or sets the maximum target quantity filter for associated parts list positions.
        /// This property allows filtering parts list positions based on their target quantity,
        /// showing only positions where the target quantity is less than or equal to this value.
        /// When set, it updates the quantity search filter and triggers property change notification.
        /// </summary>
        [ACPropertyInfo(999, nameof(FilterAssociatedPosTargetQTo), "en{'Quantity to'}de{'Menge bis'}", Description =
                        @"Gets or sets the maximum target quantity filter for associated parts list positions.
                          This property allows filtering parts list positions based on their target quantity,
                          showing only positions where the target quantity is less than or equal to this value.
                          When set, it updates the quantity search filter and triggers property change notification.")]
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
        /// Gets or sets the intermediate material number filter for associated parts list positions.
        /// This property allows filtering parts list positions based on their intermediate material number,
        /// enabling users to search for specific intermediate products used in bill of materials.
        /// When set, it triggers property change notification to update the UI.
        /// </summary>
        [ACPropertyInfo(9999, nameof(FilterAssociatedPosIntermMatNo), "en{'Intermediate product No'}de{'Zwischenprodukt Nr.'}", Description =
                        @"Gets or sets the intermediate material number filter for associated parts list positions.
                          This property allows filtering parts list positions based on their intermediate material number,
                          enabling users to search for specific intermediate products used in bill of materials.
                          When set, it triggers property change notification to update the UI.")]
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
        /// Gets or sets a value indicating whether the associated parts list is enabled for filtering.
        /// This property provides access to the filter configuration for enabling/disabling parts lists
        /// in the associated parts list position search. When set to true, only enabled parts lists are included
        /// in the search results. When set to false, only disabled parts lists are shown. When set to null,
        /// both enabled and disabled parts lists are included in the results.
        /// </summary>
        [ACPropertyInfo(999, nameof(FilterIsPartslistEnabled), "en{'Enabled'}de{'Freigegeben'}", Description =
                        @"Gets or sets a value indicating whether the associated parts list is enabled for filtering.
                          This property provides access to the filter configuration for enabling/disabling parts lists
                          in the associated parts list position search. When set to true, only enabled parts lists are included
                          in the search results. When set to false, only disabled parts lists are shown. When set to null,
                          both enabled and disabled parts lists are included in the results.")]
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

        /// <summary>
        /// Updates the state of the associated parts list and facility material list based on the selected material.
        /// </summary>
        /// <remarks>This method clears the navigation list, updates the search filter for associated
        /// positions based on the  material's number, and reloads the facility material list. If no material is
        /// selected, the search filter is cleared.</remarks>
        /// <param name="material">The selected <see cref="Material"/>. Can be null to clear the selection.</param>
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
        /// Searches for associated parts list positions based on the current filter criteria.
        /// This method executes a navigation search using the AccessAssociatedPartslistPos object
        /// and triggers a property change notification to update the AssociatedPartslistPosList in the user interface.
        /// The search applies filters for material numbers, quantities, and parts list enabled status.
        /// </summary>
        [ACMethodInfo(nameof(SearchAssociatedPos), "en{'Search'}de{'Suchen'}", 9999, false, false, true, Description =
                      @"Searches for associated parts list positions based on the current filter criteria.
                        This method executes a navigation search using the AccessAssociatedPartslistPos object
                        and triggers a property change notification to update the AssociatedPartslistPosList in the user interface.
                        The search applies filters for material numbers, quantities, and parts list enabled status.")]
        public void SearchAssociatedPos()
        {
            if (!IsEnabledSearchAssociatedPos())
                return;
            if (_AccessAssociatedPartslistPos != null)
                _AccessAssociatedPartslistPos.NavSearch();

            OnPropertyChanged(nameof(AssociatedPartslistPosList));
        }

        /// <summary>
        /// Determines whether the search for associated positions is enabled.
        /// </summary>
        /// <remarks>The search is considered enabled if the internal associated parts list is not
        /// null.</remarks>
        /// <returns><see langword="true"/> if the search for associated positions is enabled; otherwise, <see
        /// langword="false"/>.</returns>
        public bool IsEnabledSearchAssociatedPos()
        {
            return _AccessAssociatedPartslistPos != null;
        }

        /// <summary>
        /// Opens a query configuration dialog that allows users to modify filter and sort criteria for associated parts list positions.
        /// This method displays the ACQueryDialog interface which enables users to interactively configure search parameters,
        /// add or modify filter conditions, and adjust sorting options for the associated parts list position query.
        /// If the user confirms the dialog (OK button), the search is automatically executed with the new criteria.
        /// </summary>
        /// <returns>True if the dialog was confirmed and query configuration was applied; otherwise, false.</returns>
        [ACMethodInfo(nameof(OpenQueryDialog), "en{'Query dialog'}de{'Abfragedialog'}", 503, false)]
        public async Task<bool> OpenQueryDialog()
        {
            if (!IsEnabledSearchAssociatedPos())
                return false;

            bool result = false;
            if (_AccessAssociatedPartslistPos != null)
                result = await _AccessAssociatedPartslistPos.ShowACQueryDialog();
            if (result)
                SearchAssociatedPos();
            return result;
        }

        #endregion



        #endregion

        #region Workflow

        #region Properties

        private core.datamodel.ACClass _SelectedPWMethodNode;

        /// <summary>
        /// Gets or sets the selected Process Workflow (PW) method node.
        /// This property represents the workflow method node type that is currently selected in the user interface
        /// for configuring material-specific workflow rules. When a PW method node is selected, it filters
        /// the available workflows and assigned configurations to show only those related to the selected workflow type.
        /// Setting this property triggers updates to the AvailablePWMethodNodes and AssignedPWMethodNodes lists.
        /// </summary>
        [ACPropertySelected(800, "PWMethodNode", Description =
                            @"Gets or sets the selected Process Workflow (PW) method node.
                              This property represents the workflow method node type that is currently selected in the user interface
                              for configuring material-specific workflow rules. When a PW method node is selected, it filters
                              the available workflows and assigned configurations to show only those related to the selected workflow type.
                              Setting this property triggers updates to the AvailablePWMethodNodes and AssignedPWMethodNodes lists.")]
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
        /// <summary>
        /// Gets the list of process workflow (PW) method node classes available in the system.
        /// This property provides access to all non-abstract workflow method types that can be
        /// configured for materials. The list is lazily initialized and cached after first access
        /// to improve performance.
        /// </summary>
        [ACPropertyList(800, "PWMethodNode", Description =
                        @"Gets the list of process workflow (PW) method node classes available in the system.
                          This property provides access to all non-abstract workflow method types that can be
                          configured for materials. The list is lazily initialized and cached after first access
                          to improve performance.")]
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

        /// <summary>
        /// Gets or sets the selected assigned Process Workflow (PW) method node configuration.
        /// This property represents the material configuration that is currently selected in the user interface
        /// for workflow method node assignments. It corresponds to a MaterialConfig entry that defines
        /// which workflow method nodes (PWMethodNode) are assigned to the current material.
        /// </summary>
        [ACPropertySelected(800, "AssignedPWMethodNode", Description =
                            @"Gets or sets the selected assigned Process Workflow (PW) method node configuration.
                              This property represents the material configuration that is currently selected in the user interface
                              for workflow method node assignments. It corresponds to a MaterialConfig entry that defines
                              which workflow method nodes (PWMethodNode) are assigned to the current material.")]
        public gip.mes.datamodel.MaterialConfig SelectedAssignedPWMethodNode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the list of assigned Process Workflow (PW) method node configurations for the current material.
        /// This property returns all MaterialConfig entries that define workflow method node assignments
        /// for the selected PW method node type. The configurations specify which specific workflow
        /// instances and machines are assigned to the current material for the selected workflow type.
        /// Returns null if no PW method node is selected.
        /// </summary>
        [ACPropertyList(800, "AssignedPWMethodNode", Description =
                        @"Gets the list of assigned Process Workflow (PW) method node configurations for the current material.
                          This property returns all MaterialConfig entries that define workflow method node assignments
                          for the selected PW method node type. The configurations specify which specific workflow
                          instances and machines are assigned to the current material for the selected workflow type.
                          Returns null if no PW method node is selected.")]
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

        /// <summary>
        /// Gets or sets the selected available Process Workflow (PW) method node for configuration.
        /// This property represents the workflow method node that is currently selected from the list
        /// of available workflow nodes that can be assigned to the current material. When a workflow
        /// is selected, it enables the user to choose specific machines and create workflow assignment
        /// configurations for the material.
        /// </summary>
        [ACPropertySelected(800, "WF", "en{'Single dosing workflow'}de{'Einzeldosierung Workflow'}", Description =
                            @"Gets or sets the selected available Process Workflow (PW) method node for configuration.
                              This property represents the workflow method node that is currently selected from the list
                              of available workflow nodes that can be assigned to the current material. When a workflow
                              is selected, it enables the user to choose specific machines and create workflow assignment
                              configurations for the material.")]
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

        /// <summary>
        /// Gets the list of available Process Workflow (PW) method nodes that can be assigned to the current material.
        /// This property returns workflow method nodes that match the selected PW method node type and are available
        /// for assignment to the material. It filters out workflows that are already assigned to the material through
        /// MaterialConfig entries. Returns null if no PW method node type is selected.
        /// </summary>
        [ACPropertyList(800, "WF", Description =
                        @"Gets the list of available Process Workflow (PW) method nodes that can be assigned to the current material.
                          This property returns workflow method nodes that match the selected PW method node type and are available
                          for assignment to the material. It filters out workflows that are already assigned to the material through
                          MaterialConfig entries. Returns null if no PW method node type is selected.")]
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

        /// <summary>
        /// Gets or sets the selected available machine for single dosing operations.
        /// This property represents the machine that is currently selected from the list of available machines
        /// that can be used for the selected dosing workflow. When a workflow is selected through SelectedAvailablePWMethodNode,
        /// this property allows the user to choose a specific machine from the compatible machines for that workflow type.
        /// The selected machine will be used when creating workflow assignment configurations for the material.
        /// </summary>
        [ACPropertyInfo(810, "AvailableMachine", "en{'Single dosing machine'}de{'Einzel-Dosiermaschine'}", Description =
                        @"Gets the list of available Process Workflow (PW) method nodes that can be assigned to the current material.
                          This property returns workflow method nodes that match the selected PW method node type and are available
                          for assignment to the material. It filters out workflows that are already assigned to the material through
                          MaterialConfig entries. Returns null if no PW method node type is selected.")]
        public core.datamodel.ACClass SelectedAvailableMachine
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the list of available machines that can be used for the selected workflow.
        /// This property returns machines that are compatible with the currently selected Process Workflow (PW) method node
        /// for single dosing operations. The machines are derived from the PW groups associated with the selected workflow
        /// and represent the actual equipment instances that can execute the workflow for the current material.
        /// Returns null if no workflow is selected through SelectedAvailablePWMethodNode.
        /// </summary>
        [ACPropertyList(810, "AvailableMachine", Description =
                        @"Gets the list of available machines that can be used for the selected workflow.
                          This property returns machines that are compatible with the currently selected Process Workflow (PW) method node
                          for single dosing operations. The machines are derived from the PW groups associated with the selected workflow
                          and represent the actual equipment instances that can execute the workflow for the current material.
                          Returns null if no workflow is selected through SelectedAvailablePWMethodNode.")]
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

        /// <summary>
        /// Adds a new process workflow method node configuration to the current material.
        /// This method creates a MaterialConfig entry that associates a specific workflow and machine
        /// with the current material for process automation. The configuration defines which
        /// workflow method nodes (PWMethodNode) and machines are available for processing this material.
        /// After creation, the configuration is added to the material's configuration collection
        /// and the UI is updated to reflect the changes.
        /// </summary>
        [ACMethodInfo("", "en{'Add rule'}de{'Regel hinzufügen'}", 800, Description =
                     @"Adds a new process workflow method node configuration to the current material.
                       This method creates a MaterialConfig entry that associates a specific workflow and machine
                       with the current material for process automation. The configuration defines which
                       workflow method nodes (PWMethodNode) and machines are available for processing this material.
                       After creation, the configuration is added to the material's configuration collection
                       and the UI is updated to reflect the changes.")]
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

        /// <summary>
        /// Determines whether the "Add PW Method Node" configuration is enabled.
        /// </summary>
        /// <remarks>This method checks the state of the required properties to determine if the "Add PW
        /// Method Node"  configuration can be enabled. Ensure that both <see cref="SelectedAvailablePWMethodNode"/> and
        /// <see cref="SelectedAvailableMachine"/> are properly set before invoking this method.</remarks>
        /// <returns><see langword="true"/> if both <see cref="SelectedAvailablePWMethodNode"/> and  <see
        /// cref="SelectedAvailableMachine"/> are not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
        public bool IsEnabledAddPWMethodNodeConfig()
        {
            return SelectedAvailablePWMethodNode != null && SelectedAvailableMachine != null;
        }

        /// <summary>
        /// Deletes the selected Process Workflow (PW) method node configuration from the current material.
        /// This method removes the assignment of a specific workflow method node from the material by
        /// removing the corresponding MaterialConfig entry from the material's configuration collection
        /// and deleting it from the database context. After successful deletion, the UI is updated
        /// to reflect the changes in both the assigned and available PW method node lists.
        /// </summary>
        [ACMethodInfo("", "en{'Delete rule'}de{'Regel löschen'}", 800, Description =
                      @"Deletes the selected Process Workflow (PW) method node configuration from the current material.
                        This method removes the assignment of a specific workflow method node from the material by
                        removing the corresponding MaterialConfig entry from the material's configuration collection
                        and deleting it from the database context. After successful deletion, the UI is updated
                        to reflect the changes in both the assigned and available PW method node lists.")]
        public void DeletePWMethodNodeConfig()
        {
            SelectedMaterial.MaterialConfig_Material.Remove(SelectedAssignedPWMethodNode);
            DatabaseApp.Remove(SelectedAssignedPWMethodNode);
            OnPropertyChanged(nameof(AssignedPWMethodNodes));
            OnPropertyChanged(nameof(AvailablePWMethodNodes));
        }

        /// <summary>
        /// Determines whether the "Delete" operation is enabled for the currently selected  assigned password method
        /// node.
        /// </summary>
        /// <returns><see langword="true"/> if a password method node is currently selected; otherwise,  <see langword="false"/>.</returns>
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
                    _ = Save();
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
                    _ = NewMaterialUnit();
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
                    _ = ShowFacility();
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
                    _ = ShowMaterialOptions();
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
                    _ = ShowDialogOrderInfo((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(ShowDialogMaterial):
                    _ = ShowDialogMaterial((String)acParameter[0]);
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