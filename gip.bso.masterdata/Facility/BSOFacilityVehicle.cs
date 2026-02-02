// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 22.01.2018
// ***********************************************************************
// <copyright file="BSOFacilityVehicle.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.mes.autocomponent;
using Microsoft.EntityFrameworkCore;
using gip.mes.facility;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Folgende alte Masken sind in diesem BSO enthalten:
    /// 1. Lagerorte
    /// Neue Masken:
    /// 1. Lagerorte
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Vehicle'}de{'Fahrzeug'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Facility.ClassName)]
    [ACQueryInfo(Const.PackName_VarioPurchase, Const.QueryPrefix + "Vehicle", "en{'Vehicle'}de{'Fahrzeug'}", typeof(Facility), Facility.ClassName, "FacilityNo,FacilityName", "FacilityNo")]
    public class BSOFacilityVehicle : ACBSOvbNav
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityVehicle"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityVehicle(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
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

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            Search();
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            this._CurrentVehicleContainer = null;
            this._SelectedVehicleContainer = null;
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region BSO->ACProperty

        #region Manager
        protected ACRef<ACComponent> _ACFacilityManager = null;
        public FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }
        #endregion

        #region Vehicle
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<Facility> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "Vehicle")]
        public ACAccessNav<Facility> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "Vehicle", ACType.ACIdentifier);
                     bool rebuildACQueryDef = false;
                    short fcTypeLocation = (short)FacilityTypesEnum.Vehicle;
                    if (navACQueryDefinition.ACFilterColumns.Count <= 0)
                    {
                        rebuildACQueryDef = true;
                    }
                    else
                    {
                        int countFoundCorrect = 0;
                        foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            if (filterItem.PropertyName == "MDFacilityType\\MDFacilityTypeIndex")
                            {
                                if (filterItem.SearchWord == fcTypeLocation.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal)
                                    countFoundCorrect++;
                            }
                            //else if (filterItem.PropertyName == "Facility1_ParentFacility")
                            //{
                            //    if (String.IsNullOrEmpty(filterItem.SearchWord) && filterItem.LogicalOperator == Global.LogicalOperators.isNull)
                            //        countFoundCorrect++;
                            //}
                        }
                        if (countFoundCorrect < 2)
                            rebuildACQueryDef = true;
                    }
                    if (rebuildACQueryDef)
                    {
                        navACQueryDefinition.ClearFilter(true);
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "FacilityNo", Global.LogicalOperators.contains, Global.Operators.or, "", true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "FacilityName", Global.LogicalOperators.contains, Global.Operators.or, "", true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, fcTypeLocation.ToString(), true));
                        //navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "Facility1_ParentFacility", Global.LogicalOperators.isNull, Global.Operators.and, "", true));
                        navACQueryDefinition.SaveConfig(true);
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Facility>("Vehicle", this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;

                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected facility location.
        /// </summary>
        /// <value>The selected facility location.</value>
        [ACPropertySelected(9999, "Vehicle")]
        public Facility SelectedVehicle
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedVehicle");
            }
        }

        /// <summary>
        /// Gets or sets the current facility location.
        /// </summary>
        /// <value>The current facility location.</value>
        [ACPropertyCurrent(9999, "Vehicle")]
        public Facility CurrentVehicle
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                if (CurrentVehicle != null)
                    CurrentVehicle.PropertyChanged -= CurrentVehicle_PropertyChanged;

                AccessPrimary.Current = value;

                if (CurrentVehicle != null)
                    CurrentVehicle.PropertyChanged += new PropertyChangedEventHandler(CurrentVehicle_PropertyChanged);

                OnPropertyChanged("CurrentVehicle");
                OnPropertyChanged("VehicleContainerList");
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the CurrentVehicle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void CurrentVehicle_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CompanyID":
                    OnPropertyChanged("CurrentVehicle");
                    break;
            }
        }

        /// <summary>
        /// Gets the facility location list.
        /// </summary>
        /// <value>The facility location list.</value>
        [ACPropertyList(9999, "Vehicle")]
        public IEnumerable<Facility> VehicleList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region Vehicle-Container
        /// <summary>
        /// The _ selected facility
        /// </summary>
        Facility _SelectedVehicleContainer;
        /// <summary>
        /// Gets or sets the selected facility.
        /// </summary>
        /// <value>The selected facility.</value>
        [ACPropertySelected(9999, "VehicleContainer")]
        public Facility SelectedVehicleContainer
        {
            get
            {
                return _SelectedVehicleContainer;
            }
            set
            {
                _SelectedVehicleContainer = value;
                OnPropertyChanged("SelectedVehicleContainer");
                if (_SelectedVehicleContainer == null)
                {
                    CurrentVehicleContainer = null;
                }
            }
        }

        /// <summary>
        /// The _ current facility
        /// </summary>
        Facility _CurrentVehicleContainer;
        /// <summary>
        /// Gets or sets the current facility.
        /// </summary>
        /// <value>The current facility.</value>
        [ACPropertyCurrent(9999, "VehicleContainer")]
        public Facility CurrentVehicleContainer
        {
            get
            {
                return _CurrentVehicleContainer;
            }
            set
            {
                if (_CurrentVehicleContainer != null)
                {
                    _CurrentVehicleContainer.PropertyChanged -= _CurrentVehicleContainer_PropertyChanged;
                }

                _CurrentVehicleContainer = value;
                if (_CurrentVehicleContainer != null)
                {
                    _CurrentVehicleContainer.PropertyChanged += new PropertyChangedEventHandler(_CurrentVehicleContainer_PropertyChanged); ;
                }
                OnPropertyChanged("CurrentVehicleContainer");
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the _CurrentVehicleContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        void _CurrentVehicleContainer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets the facility list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(9999, "VehicleContainer")]
        public IEnumerable<Facility> VehicleContainerList
        {
            get
            {
                if (CurrentVehicle == null)
                    return null;
                return CurrentVehicle.Facility_ParentFacility;
            }
        }
        #endregion

        #endregion

        #region BSO->ACMethod

        #region Vehicle
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("Vehicle", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand("Vehicle", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
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
        [ACMethodInteraction("Vehicle", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedVehicle", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<Facility>(requery, () => SelectedVehicle, () => CurrentVehicle, c => CurrentVehicle = c,
                        DatabaseApp.Facility
                        .Where(c => c.FacilityID == SelectedVehicle.FacilityID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedVehicle != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("Vehicle", Const.New, (short)MISort.New, true, "SelectedVehicle", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New"))
                return;
            MDFacilityType facilityType = DatabaseApp.MDFacilityType.Where(c => c.MDFacilityTypeIndex == (short)FacilityTypesEnum.Vehicle).FirstOrDefault();
            if (facilityType == null)
                return;

            Facility rootStore = this.ACFacilityManager.GetRootStoreForVehicles(DatabaseApp);
            Facility vehicle = Facility.NewACObject(DatabaseApp, rootStore);
            vehicle.MDFacilityType = facilityType;
            vehicle.InwardEnabled = true;
            vehicle.OutwardEnabled = true;
            DatabaseApp.Facility.Add(vehicle);
            _AccessPrimary.NavList.Add(vehicle);
            OnPropertyChanged("VehicleList");
            CurrentVehicle = vehicle;
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
        [ACMethodInteraction("Vehicle", Const.Delete, (short)MISort.Delete, true, "CurrentVehicle", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (AccessPrimary == null)
                return;
            Msg msg = CurrentVehicle.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            AccessPrimary.NavList.Remove(CurrentVehicle);
            SelectedVehicle = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentVehicle != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("Vehicle", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("VehicleList");
        }

        private IQueryable<Facility> _AccessPrimary_NavSearchExecuting(IQueryable<Facility> result)
        {
            IQueryable<Facility> query = result as IQueryable<Facility>;
            if (query != null)
            {
                query.Include(c => c.Company);
            }
            return result;
        }

        #endregion

        #region Storage-Bin
        /// <summary>
        /// Loads the facility.
        /// </summary>
        [ACMethodInteraction("VehicleContainer", "en{'Load Chamber'}de{'Kammer laden'}", (short)MISort.Load, false, "SelectedVehicleContainer", Global.ACKinds.MSMethodPrePost)]
        public void LoadVehicleContainer()
        {
            if (SelectedVehicleContainer != null)
            {
                if (!PreExecute("LoadVehicleContainer")) return;
                CurrentVehicleContainer = SelectedVehicleContainer;
                PostExecute("LoadVehicleContainer");

            }
        }

        /// <summary>
        /// Determines whether [is enabled load facility].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load facility]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadVehicleContainer()
        {
            return SelectedVehicle != null && SelectedVehicleContainer != null;
        }

        /// <summary>
        /// News the facility.
        /// </summary>
        [ACMethodInteraction("VehicleContainer", "en{'New Chamber'}de{'Neue Kammer'}", 9999, true, "SelectedVehicleContainer", Global.ACKinds.MSMethodPrePost)]
        public void NewVehicleContainer()
        {
            if (!PreExecute("NewVehicleContainer")) return;
            CurrentVehicleContainer = Facility.NewACObject(DatabaseApp, CurrentVehicle);
            CurrentVehicleContainer.MDFacilityType = DatabaseApp.MDFacilityType.Where(c => c.MDFacilityTypeIndex == (short)FacilityTypesEnum.VehicleContainer).FirstOrDefault();
            CurrentVehicleContainer.MDFacilityVehicleType = CurrentVehicle.MDFacilityVehicleType;
            CurrentVehicleContainer.InwardEnabled = true;
            CurrentVehicleContainer.OutwardEnabled = true;
            OnPropertyChanged("VehicleContainerList");
            PostExecute("NewVehicleContainer");

        }

        /// <summary>
        /// Determines whether [is enabled new facility].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new facility]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewVehicleContainer()
        {
            return CurrentVehicle != null;
        }

        /// <summary>
        /// Deletes the facility.
        /// </summary>
        [ACMethodInteraction("VehicleContainer", "en{'Delete Chamber'}de{'Kammer löschen'}", 9999, true, "CurrentVehicleContainer", Global.ACKinds.MSMethodPrePost)]
        public void DeleteVehicleContainer()
        {
            if (!PreExecute("DeleteVehicleContainer")) return;
            Msg msg = CurrentVehicleContainer.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            PostExecute("DeleteVehicleContainer");
        }

        /// <summary>
        /// Determines whether [is enabled delete facility].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete facility]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteVehicleContainer()
        {
            if (CurrentVehicle == null || CurrentVehicleContainer == null)
                return false;
            //if (CurrentVehicleContainer.FacilityACClass != null)
            //{
            //    if (CurrentVehicleContainer.FacilityACClass.FromIPlusContext<gip.core.datamodel.ACClass>().ACKind != Global.ACKinds.TACFacility)
            //        return false;
            //}
            return true;
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
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
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
                case nameof(LoadVehicleContainer):
                    LoadVehicleContainer();
                    return true;
                case nameof(IsEnabledLoadVehicleContainer):
                    result = IsEnabledLoadVehicleContainer();
                    return true;
                case nameof(NewVehicleContainer):
                    NewVehicleContainer();
                    return true;
                case nameof(IsEnabledNewVehicleContainer):
                    result = IsEnabledNewVehicleContainer();
                    return true;
                case nameof(DeleteVehicleContainer):
                    DeleteVehicleContainer();
                    return true;
                case nameof(IsEnabledDeleteVehicleContainer):
                    result = IsEnabledDeleteVehicleContainer();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}