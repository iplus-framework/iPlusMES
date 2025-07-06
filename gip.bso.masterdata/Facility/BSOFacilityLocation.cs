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
// <copyright file="BSOFacilityLocation.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Folgende alte Masken sind in diesem BSO enthalten:
    /// 1. Lagerorte
    /// Neue Masken:
    /// 1. Lagerorte
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Storage Location'}de{'Lagerort'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Facility.ClassName)]
    [ACQueryInfo(Const.PackName_VarioPurchase, Const.QueryPrefix + "StorageLocation", "en{'Storage Location'}de{'Lagerort'}", typeof(Facility), Facility.ClassName, "FacilityNo,FacilityName", "FacilityNo")]
    //[ACQueryInfo(Const.PackName_VarioPurchase, Const.QueryPrefix + "StorageBin", "en{'Storage bin'}de{'Lagerplatz'}", typeof(Facility), Facility.ClassName, "FacilityNo,FacilityName", "FacilityNo")]
    public class BSOFacilityLocation : ACBSOvbNav
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityLocation"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityLocation(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._CurrentStorageBin = null;
            this._SelectedStorageBin = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        public override object Clone()
        {
            BSOFacilityLocation clone = base.Clone() as BSOFacilityLocation;
            clone.SelectedStorageLocation = this.SelectedStorageLocation;
            clone.CurrentStorageLocation = this.CurrentStorageLocation;
            clone.SelectedStorageBin = this.SelectedStorageBin;
            clone.CurrentStorageBin = this.CurrentStorageBin;
            return clone;
        }
        #endregion

        #region BSO->ACProperty

        #region Storage-Location
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<Facility> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "StorageLocation")]
        public ACAccessNav<Facility> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "StorageLocation", ACType.ACIdentifier);
                    bool rebuildACQueryDef = false;
                    short fcTypeLocation = (short)FacilityTypesEnum.StorageLocation;
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
                            else if (filterItem.PropertyName == "Facility1_ParentFacility")
                            {
                                if (String.IsNullOrEmpty(filterItem.SearchWord) && filterItem.LogicalOperator == Global.LogicalOperators.isNull)
                                    countFoundCorrect++;
                            }
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
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "Facility1_ParentFacility", Global.LogicalOperators.isNull, Global.Operators.and, "", true));
                        navACQueryDefinition.SaveConfig(true);
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Facility>("StorageLocation", this);

                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected facility location.
        /// </summary>
        /// <value>The selected facility location.</value>
        [ACPropertySelected(9999, "StorageLocation")]
        public Facility SelectedStorageLocation
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
                OnPropertyChanged("SelectedStorageLocation");
            }
        }

        /// <summary>
        /// Gets or sets the current facility location.
        /// </summary>
        /// <value>The current facility location.</value>
        [ACPropertyCurrent(9999, "StorageLocation")]
        public Facility CurrentStorageLocation
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

                if (CurrentStorageLocation != null)
                    CurrentStorageLocation.PropertyChanged -= CurrentStorageLocation_PropertyChanged;

                AccessPrimary.Current = value;

                if (CurrentStorageLocation != null)
                    CurrentStorageLocation.PropertyChanged += new PropertyChangedEventHandler(CurrentStorageLocation_PropertyChanged);

                OnPropertyChanged("CurrentStorageLocation");
                OnPropertyChanged("StorageBinList");
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the CurrentStorageLocation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        void CurrentStorageLocation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets the facility location list.
        /// </summary>
        /// <value>The facility location list.</value>
        [ACPropertyList(9999, "StorageLocation")]
        public IEnumerable<Facility> StorageLocationList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region Storage-Bin
        /// <summary>
        /// The _ selected facility
        /// </summary>
        Facility _SelectedStorageBin;
        /// <summary>
        /// Gets or sets the selected facility.
        /// </summary>
        /// <value>The selected facility.</value>
        [ACPropertySelected(9999, "StorageBin")]
        public Facility SelectedStorageBin
        {
            get
            {
                return _SelectedStorageBin;
            }
            set
            {
                _SelectedStorageBin = value;
                OnPropertyChanged("SelectedStorageBin");
            }
        }

        /// <summary>
        /// The _ current facility
        /// </summary>
        Facility _CurrentStorageBin;
        /// <summary>
        /// Gets or sets the current facility.
        /// </summary>
        /// <value>The current facility.</value>
        [ACPropertyCurrent(9999, "StorageBin")]
        public Facility CurrentStorageBin
        {
            get
            {
                return _CurrentStorageBin;
            }
            set
            {
                if (_CurrentStorageBin != null)
                {
                    _CurrentStorageBin.PropertyChanged -= _CurrentStorageBin_PropertyChanged;
                }

                _CurrentStorageBin = value;
                if (_CurrentStorageBin != null)
                {
                    _CurrentStorageBin.PropertyChanged += new PropertyChangedEventHandler(_CurrentStorageBin_PropertyChanged); ;
                }
                OnPropertyChanged("CurrentStorageBin");
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the _CurrentStorageBin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        void _CurrentStorageBin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets the facility list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(9999, "StorageBin")]
        public IEnumerable<Facility> StorageBinList
        {
            get
            {
                if (CurrentStorageLocation == null)
                    return null;
                return CurrentStorageLocation.Facility_ParentFacility.OrderBy(c => c.FacilityNo).ToList();
            }
        }
        #endregion

        #endregion

        #region BSO->ACMethod

        #region Storage-Location
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("StorageLocation", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand("StorageLocation", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction("StorageLocation", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedStorageLocation", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<Facility>(requery, () => SelectedStorageLocation, () => CurrentStorageLocation, c => CurrentStorageLocation = c,
                        DatabaseApp.Facility
                        .Where(c => c.FacilityID == SelectedStorageLocation.FacilityID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedStorageLocation != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("StorageLocation", Const.New, (short)MISort.New, true, "SelectedStorageLocation", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentStorageLocation = Facility.NewACObject(DatabaseApp, null);
            CurrentStorageLocation.MDFacilityType = DatabaseApp.MDFacilityType.Where(c => c.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageLocation).FirstOrDefault();
            if (CurrentStorageLocation.MDFacilityType != null)
                DatabaseApp.Facility.Add(CurrentStorageLocation);
            _AccessPrimary.NavList.Add(CurrentStorageLocation);
            OnPropertyChanged("StorageLocationList");
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
        [ACMethodInteraction("StorageLocation", Const.Delete, (short)MISort.Delete, true, "CurrentStorageLocation", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (AccessPrimary == null)
                return;
            Msg msg = CurrentStorageLocation.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            AccessPrimary.NavList.Remove(CurrentStorageLocation);
            SelectedStorageLocation = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentStorageLocation != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("StorageLocation", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("StorageLocationList");
        }
        #endregion

        #region Storage-Bin
        /// <summary>
        /// Loads the facility.
        /// </summary>
        [ACMethodInteraction("StorageBin", "en{'Load Storage Bin'}de{'Lagerplatz laden'}", (short)MISort.Load, false, "SelectedStorageBin", Global.ACKinds.MSMethodPrePost)]
        public void LoadStorageBin()
        {
            if (SelectedStorageBin != null)
            {
                if (!PreExecute("LoadStorageBin")) return;
                CurrentStorageBin = SelectedStorageBin;
                PostExecute("LoadStorageBin");

            }
        }

        /// <summary>
        /// Determines whether [is enabled load facility].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load facility]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadStorageBin()
        {
            return SelectedStorageLocation != null && SelectedStorageBin != null;
        }

        /// <summary>
        /// News the facility.
        /// </summary>
        [ACMethodInteraction("StorageBin", "en{'New Storage Bin'}de{'Neuer Lagerplatz'}", 9999, true, "SelectedStorageBin", Global.ACKinds.MSMethodPrePost)]
        public void NewStorageBin()
        {
            if (!PreExecute("NewStorageBin")) return;
            CurrentStorageBin = Facility.NewACObject(DatabaseApp, CurrentStorageLocation);
            CurrentStorageBin.MDFacilityType = DatabaseApp.MDFacilityType.Where(c => c.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBin).FirstOrDefault();
            OnPropertyChanged("StorageBinList");
            PostExecute("NewStorageBin");

        }

        /// <summary>
        /// Determines whether [is enabled new facility].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new facility]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewStorageBin()
        {
            return CurrentStorageLocation != null;
        }

        /// <summary>
        /// Deletes the facility.
        /// </summary>
        [ACMethodInteraction("StorageBin", "en{'Delete Storage Bin'}de{'Lagerplatz löschen'}", 9999, true, "CurrentStorageBin", Global.ACKinds.MSMethodPrePost)]
        public void DeleteStorageBin()
        {
            if (!PreExecute("DeleteStorageBin")) return;
            Msg msg = CurrentStorageBin.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Global.MsgResult result = Messages.Msg(msg, Global.MsgResult.No, eMsgButton.YesNo);
                if (result == Global.MsgResult.Yes)
                {
                    msg = CurrentStorageBin.DeleteACObject(DatabaseApp, false);
                    if (msg != null)
                    {
                        Messages.Msg(msg);
                    }
                }
            }
            PostExecute("DeleteStorageBin");
        }

        /// <summary>
        /// Determines whether [is enabled delete facility].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete facility]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteStorageBin()
        {
            if (CurrentStorageLocation == null || CurrentStorageBin == null)
                return false;
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
                case nameof(LoadStorageBin):
                    LoadStorageBin();
                    return true;
                case nameof(IsEnabledLoadStorageBin):
                    result = IsEnabledLoadStorageBin();
                    return true;
                case nameof(NewStorageBin):
                    NewStorageBin();
                    return true;
                case nameof(IsEnabledNewStorageBin):
                    result = IsEnabledNewStorageBin();
                    return true;
                case nameof(DeleteStorageBin):
                    DeleteStorageBin();
                    return true;
                case nameof(IsEnabledDeleteStorageBin):
                    result = IsEnabledDeleteStorageBin();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}