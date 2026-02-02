// ***********************************************************************
// Assembly         : gip.bso.manufacturing
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-27-2012
// ***********************************************************************
// <copyright file="BSODemandOrder.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.datamodel; 
using gip.core.datamodel;
//using gip.core.manager;
using gip.core.autocomponent;
using gip.mes.autocomponent;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Class BSODemandOrder
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Demandorder'}de{'Produktionsvorschlag'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + DemandOrder.ClassName)]
    public class BSODemandOrder : ACBSOvbNav 
    {
        #region private members 
        //VBBSODesignManagerWFWorkOrderMethodCreator _WFWorkOrderMethodCreator;
        #endregion

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSODemandOrder"/> class.
        /// </summary>
        /// <param name="typeACClass">The type AC class.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSODemandOrder(gip.core.datamodel.ACClass typeACClass, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(typeACClass, content, parentACObject, parameter)
        {
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
            //OnValueChanged += new ValueChangedEventHandler(BSODemandOrder_OnValueChanged);

            Search();
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._AccessDemandOrderPos = null;
            this._CurrentDemandOrderPos = null;
            this._DemandOrderPosList = null;
            this._SelectedDemandOrderPos = null;
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessDemandOrderPos != null)
            {
                await _AccessDemandOrderPos.ACDeInit(false);
                _AccessDemandOrderPos = null;
            }
            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        /// <summary>
        /// Handles the PropertyChanged event of the CurrentDemandOrderPos control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void CurrentDemandOrderPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MaterialID":
                    if (CurrentDemandOrderPos.Material != null)
                    {
                        CurrentDemandOrderPos.Partslist = CurrentDemandOrderPos.Material.GetDefaultPartslist();
                        if (CurrentDemandOrderPos.Partslist != null)
                            CurrentDemandOrderPos.ProgramACClassMethod = CurrentDemandOrderPos.Partslist.GetDefaultProgramACClassMethod();
                        OnPropertyChanged("MDUnitList");
                        if (CurrentDemandOrderPos.Material != null && CurrentDemandOrderPos.Material.BaseMDUnit != null)
                            CurrentMDUnit = CurrentDemandOrderPos.Material.BaseMDUnit;
                        else
                            CurrentMDUnit = null;
                        OnPropertyChanged("ProgramACClassMethodList");
                        OnPropertyChanged("CurrentDemandOrderPos");
                    }
                    break;
                case "TargetQuantityUOM":
                case "MDUnitID":
                    {
                        // TODO Damir: Umrechnungsfunktionen checken
                        //CurrentDemandOrderPos.TargetQuantity = CurrentDemandOrderPos.Material.ConvertToBaseQuantity(CurrentDemandOrderPos.TargetQuantityUOM, CurrentDemandOrderPos.MDUnit);
                        //CurrentDemandOrderPos.TargetWeight = CurrentDemandOrderPos.Material.ConvertToBaseWeight(CurrentDemandOrderPos.TargetQuantityUOM, CurrentDemandOrderPos.MDUnit);
                    }
                    break;
                case "PartslistID":
                    OnPropertyChanged("ProgramACClassMethodList");
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the current MU quantity unit.
        /// </summary>
        /// <value>The current MU quantity unit.</value>
        [ACPropertyCurrent(9999, MDUnit.ClassName)]
        public MDUnit CurrentMDUnit
        {
            get
            {
                if (CurrentDemandOrderPos == null)
                    return null;
                // TODO Damir: Umrechnungsfunktionen checken
                //return CurrentDemandOrderPos.MDUnit;
                return null;
            }
            set
            {
                if (CurrentDemandOrderPos != null && value != null)
                {
                    // TODO Damir: Umrechnungsfunktionen checken
                    //CurrentDemandOrderPos.MDUnit = value;
                    OnPropertyChanged("CurrentMDUnit");
                }
            }
        }

        /// <summary>
        /// Gets the MU quantity unit list.
        /// </summary>
        /// <value>The MU quantity unit list.</value>
        [ACPropertyList(9999, MDUnit.ClassName)]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                if (CurrentDemandOrderPos == null || CurrentDemandOrderPos.Material == null)
                    return null;
                return CurrentDemandOrderPos.Material.MDUnitList;
            }
        }

        #endregion

        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<DemandOrder> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, "DemandOrder")]
        public ACAccessNav<DemandOrder> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<DemandOrder>("DemandOrder", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the current demand order.
        /// </summary>
        /// <value>The current demand order.</value>
        [ACPropertyCurrent(600, "DemandOrder")]
        public DemandOrder CurrentDemandOrder
        {
            get
            {
                return AccessPrimary.Current;
            }
            set
            {
                AccessPrimary.Current = value;
                OnPropertyChanged("CurrentDemandOrder");
                OnPropertyChanged("DemandOrderPosList");
            }
        }

        /// <summary>
        /// Gets the demand order list.
        /// </summary>
        /// <value>The demand order list.</value>
        [ACPropertyList(601, "DemandOrder")]
        public IEnumerable<DemandOrder> DemandOrderList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected demand order.
        /// </summary>
        /// <value>The selected demand order.</value>
        [ACPropertySelected(602, "DemandOrder")]
        public DemandOrder SelectedDemandOrder
        {
            get
            {
                return AccessPrimary.Selected;
            }
            set
            {
                AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedDemandOrder");
            }
        }

        /// <summary>
        /// The _ access demand order pos
        /// </summary>
        ACAccess<DemandOrderPos> _AccessDemandOrderPos;
        /// <summary>
        /// Gets the access demand order pos.
        /// </summary>
        /// <value>The access demand order pos.</value>
        [ACPropertyAccess(691, "DemandOrderPos")]
        public ACAccess<DemandOrderPos> AccessDemandOrderPos
        {
            get
            {
                if (_AccessDemandOrderPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + DemandOrderPos.ClassName) as ACQueryDefinition;
                    _AccessDemandOrderPos = acQueryDefinition.NewAccess<DemandOrderPos>("DemandOrderPos", this);
                }
                return _AccessDemandOrderPos;
            }
        }

        /// <summary>
        /// The _ current demand order pos
        /// </summary>
        DemandOrderPos _CurrentDemandOrderPos;
        /// <summary>
        /// Gets or sets the current demand order pos.
        /// </summary>
        /// <value>The current demand order pos.</value>
        [ACPropertyCurrent(603, "DemandOrderPos")]
        public DemandOrderPos CurrentDemandOrderPos
        {
            get
            {
                return _CurrentDemandOrderPos;
            }
            set
            {
                if (_CurrentDemandOrderPos != value)
                {
                    if (CurrentDemandOrderPos != null)
                    {
                        CurrentDemandOrderPos.PropertyChanged -= CurrentDemandOrderPos_PropertyChanged;
                    }
                    _CurrentDemandOrderPos = value;
                    OnPropertyChanged("CurrentDemandOrderPos");
                    OnPropertyChanged("ProgramACClassMethodList");
                    OnPropertyChanged("PartslistList");
                    if (CurrentDemandOrderPos != null)
                    {
                        CurrentDemandOrderPos.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentDemandOrderPos_PropertyChanged);
                    }
                }
                OnPropertyChanged("MDUnitList");
                OnPropertyChanged("CurrentDemandOrderPos");
                OnPropertyChanged("CurrentMDUnit");
            }
        }


        /// <summary>
        /// The _ demand order pos list
        /// </summary>
        List<DemandOrderPos> _DemandOrderPosList;
        /// <summary>
        /// Gets the demand order pos list.
        /// </summary>
        /// <value>The demand order pos list.</value>
        [ACPropertyList(604, "DemandOrderPos")]
        public IEnumerable<DemandOrderPos> DemandOrderPosList
        {
            get
            {
                if (CurrentDemandOrder == null)
                    return null;
                _DemandOrderPosList = CurrentDemandOrder.DemandOrderPos_DemandOrder.OrderBy(c => c.Sequence).ToList();
                return _DemandOrderPosList;
            }
        }

        /// <summary>
        /// The _ selected demand order pos
        /// </summary>
        DemandOrderPos _SelectedDemandOrderPos;
        /// <summary>
        /// Gets or sets the selected demand order pos.
        /// </summary>
        /// <value>The selected demand order pos.</value>
        [ACPropertySelected(605, "DemandOrderPos")]
        public DemandOrderPos SelectedDemandOrderPos
        {
            get
            {
                return _SelectedDemandOrderPos;
            }
            set
            {
                _SelectedDemandOrderPos = value;
                OnPropertyChanged("SelectedDemandOrderPos");
            }
        }

        /// <summary>
        /// Gets the partslist list.
        /// </summary>
        /// <value>The partslist list.</value>
        [ACPropertyList(606, Partslist.ClassName)]
        public IEnumerable<Partslist> PartslistList
        {
            get
            {
                if (CurrentDemandOrderPos == null || CurrentDemandOrderPos.Material == null)
                    return null;
                return CurrentDemandOrderPos.Material.Partslist_Material.OrderBy(c => c.PartslistName);
            }
        }

        /// <summary>
        /// Gets the program AC class method list.
        /// </summary>
        /// <value>The program AC class method list.</value>
        [ACPropertyList(607, "WorkACClassMethod")]
        public IEnumerable<gip.core.datamodel.ACClassMethod> ProgramACClassMethodList
        {
            get
            {
                if (CurrentDemandOrderPos == null || CurrentDemandOrderPos.Material == null || CurrentDemandOrderPos.Partslist == null)
                    return null;
                return null;
                //return CurrentDemandOrderPos.Partslist.PartslistProgram_Partslist.Select(c => c.ProgramACClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>()).OrderBy(c => c.ACCaptionTranslation);
            }
        }
        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("DemandOrder", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand("DemandOrder", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction("DemandOrder", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedDemandOrder", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<DemandOrder>(requery, () => SelectedDemandOrder, () => CurrentDemandOrder, c => CurrentDemandOrder = c,
                        DatabaseApp.DemandOrder
                        .Include(c => c.DemandOrderPos_DemandOrder)
                        .Where(c => c.DemandOrderID == SelectedDemandOrder.DemandOrderID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedDemandOrder != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("DemandOrder", Const.New, (short)MISort.New, true, "SelectedDemandOrder", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(DemandOrder), DemandOrder.NoColumnName, DemandOrder.FormatNewNo, this);
            CurrentDemandOrder = DemandOrder.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.DemandOrder.Add(CurrentDemandOrder);

            CurrentDemandOrderPos = DemandOrderPos.NewACObject(DatabaseApp, CurrentDemandOrder);
            CurrentDemandOrderPos.DemandOrder = CurrentDemandOrder;
            CurrentDemandOrder.DemandOrderPos_DemandOrder.Add(CurrentDemandOrderPos);
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
        [ACMethodInteraction("DemandOrder", Const.Delete, (short)MISort.Delete, true, "CurrentDemandOrder", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentDemandOrder.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            AccessPrimary.NavList.Remove(CurrentDemandOrder);
            SelectedDemandOrder = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentDemandOrder != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("DemandOrder", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("DemandOrderList");
        }

        /// <summary>
        /// Loads the demand order pos.
        /// </summary>
        [ACMethodInteraction("DemandOrderPos", "en{'Load Position'}de{'Position laden'}", (short)MISort.Load, false, "SelectedDemandOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void LoadDemandOrderPos()
        {
            if (!IsEnabledLoadDemandOrderPos())
                return;
            if (!PreExecute("LoadDemandOrderPos")) return;
            // Laden des aktuell selektierten DemandOrderPos 
            CurrentDemandOrderPos = CurrentDemandOrder.DemandOrderPos_DemandOrder.Where(c => c.DemandOrderPosID == SelectedDemandOrderPos.DemandOrderPosID).FirstOrDefault();
            PostExecute("LoadDemandOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled load demand order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load demand order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadDemandOrderPos()
        {
            return SelectedDemandOrderPos != null && CurrentDemandOrder != null;
        }

        /// <summary>
        /// News the demand order pos.
        /// </summary>
        [ACMethodInteraction("DemandOrderPos", "en{'New Position'}de{'Neue Position'}", (short)MISort.New, true, "SelectedDemandOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void NewDemandOrderPos()
        {
            if (!PreExecute("NewDemandOrderPos")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            CurrentDemandOrderPos = DemandOrderPos.NewACObject(DatabaseApp, CurrentDemandOrder);
            CurrentDemandOrderPos.DemandOrder = CurrentDemandOrder;
            CurrentDemandOrder.DemandOrderPos_DemandOrder.Add(CurrentDemandOrderPos);
            OnPropertyChanged("DemandOrderPosList");
            PostExecute("NewDemandOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled new demand order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new demand order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewDemandOrderPos()
        {
            return CurrentDemandOrder != null;
        }

        /// <summary>
        /// Deletes the demand order pos.
        /// </summary>
        [ACMethodInteraction("DemandOrderPos", "en{'Delete Position'}de{'Position löschen'}", (short)MISort.Delete, true, "CurrentDemandOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteDemandOrderPos()
        {
            if (!PreExecute("DeleteDemandOrderPos")) return;
            Msg msg = CurrentDemandOrderPos.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            PostExecute("DeleteDemandOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled delete demand order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete demand order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteDemandOrderPos()
        {
            return CurrentDemandOrder != null && CurrentDemandOrderPos != null;
        }
        #endregion

        /// <summary>
        /// Creates the work order from demand order pos.
        /// </summary>
        [ACMethodInteraction("DemandOrderPos", "en{'Generate Processorder for Position'}de{'Prozessauftrag für Position anlegen'}", 600, true, "CurrentDemandOrderPos")]
        public void CreateWorkOrderFromDemandOrderPos()
        {
            //if (_WFWorkOrderMethodCreator.CreateWorkOrderFromDemandOrderPos(CurrentDemandOrderPos) != null)
            //{
            //    Save();

            //    OnPropertyChanged("CurrentDemandOrder");
            //    OnPropertyChanged("DemandOrderPosList");
            //}
        }

        /// <summary>
        /// Determines whether [is enabled create work order from demand order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled create work order from demand order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledCreateWorkOrderFromDemandOrderPos()
        {
            return  CurrentDemandOrder != null && CurrentDemandOrderPos != null;
        }

        /// <summary>
        /// Creates the work order from demand order.
        /// </summary>
        [ACMethodInteraction("DemandOrder", "en{'Generate alle Processorders'}de{'Alle Prozessaufträge anlegen'}", 601, true, "CurrentDemandOrder")]
        public void CreateWorkOrderFromDemandOrder()
        {
            //if (_WFWorkOrderMethodCreator.CreateWorkOrderFromDemandOrder(CurrentDemandOrder))
            //{
            //    Save();

            //    OnPropertyChanged("CurrentDemandOrder");
            //    OnPropertyChanged("DemandOrderPosList");
            //}
        }

        /// <summary>
        /// Determines whether [is enabled create work order from demand order].
        /// </summary>
        /// <returns><c>true</c> if [is enabled create work order from demand order]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledCreateWorkOrderFromDemandOrder()
        {
            return CurrentDemandOrder != null;
        }

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"Save":
                    Save();
                    return true;
                case"IsEnabledSave":
                    result = IsEnabledSave();
                    return true;
                case"UndoSave":
                    UndoSave();
                    return true;
                case"IsEnabledUndoSave":
                    result = IsEnabledUndoSave();
                    return true;
                case"Load":
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case"IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
                case"New":
                    New();
                    return true;
                case"IsEnabledNew":
                    result = IsEnabledNew();
                    return true;
                case"Delete":
                    Delete();
                    return true;
                case"IsEnabledDelete":
                    result = IsEnabledDelete();
                    return true;
                case"Search":
                    Search();
                    return true;
                case"LoadDemandOrderPos":
                    LoadDemandOrderPos();
                    return true;
                case"IsEnabledLoadDemandOrderPos":
                    result = IsEnabledLoadDemandOrderPos();
                    return true;
                case"NewDemandOrderPos":
                    NewDemandOrderPos();
                    return true;
                case"IsEnabledNewDemandOrderPos":
                    result = IsEnabledNewDemandOrderPos();
                    return true;
                case"DeleteDemandOrderPos":
                    DeleteDemandOrderPos();
                    return true;
                case"IsEnabledDeleteDemandOrderPos":
                    result = IsEnabledDeleteDemandOrderPos();
                    return true;
                case"CreateWorkOrderFromDemandOrderPos":
                    CreateWorkOrderFromDemandOrderPos();
                    return true;
                case"IsEnabledCreateWorkOrderFromDemandOrderPos":
                    result = IsEnabledCreateWorkOrderFromDemandOrderPos();
                    return true;
                case"CreateWorkOrderFromDemandOrder":
                    CreateWorkOrderFromDemandOrder();
                    return true;
                case"IsEnabledCreateWorkOrderFromDemandOrder":
                    result = IsEnabledCreateWorkOrderFromDemandOrder();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}
