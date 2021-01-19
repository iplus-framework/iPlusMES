using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;
using gip.mes.facility;

namespace gip.bso.masterdata
{
    /// <summary>
    /// The bussines object base class for a laboratory orders and templates.
    /// </summary>
    /// <summary xml:lang="de">
    /// Die Objektbasisklasse für einen Laborauftrag und Vorlagen.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'BSOLabOrderBase'}de{'BSOLabOrderBase'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public abstract class BSOLabOrderBase : ACBSOvbNav
    {
        protected ACRef<ACLabOrderManager> _LabOrderManager = null;
        protected ACLabOrderManager LabOrderManager
        {
            get
            {
                if (_LabOrderManager == null)
                    return null;
                return _LabOrderManager.ValueT;
            }
        }
        
        #region c'tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOLabOrderBase"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOLabOrderBase(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            :base(acType, content, parentACObject, parameter, acIdentifier)
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

            _LabOrderManager = ACLabOrderManager.ACRefToServiceInstance(this);
            if (_LabOrderManager == null)
                throw new Exception("LabOrderManager not configured");

            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACLabOrderManager.DetachACRefFromServiceInstance(this, _LabOrderManager);
            _LabOrderManager = null;
            if (CurrentLabOrderPos != null)
                CurrentLabOrderPos.PropertyChanged -= CurrentLabOrderPos_PropertyChanged;
            this._AccessLabOrderPos = null;
            this._CurrentLabOrderPos = null;
            this._SelectedLabOrderPos = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessLabOrderPos != null)
            {
                _AccessLabOrderPos.ACDeInit(false);
                _AccessLabOrderPos = null;
            }
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region BSO -> ACProperty

        #region LabOrder
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        /// <summary xml:lang="de">
        /// 
        /// </summary>
        ACAccessNav<LabOrder> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "LabOrder")]
        public virtual ACAccessNav<LabOrder> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(this, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        ACSortItem sortItem = navACQueryDefinition.ACSortColumns.Where(c => c.ACIdentifier == "LabOrderNo").FirstOrDefault();
                        if (sortItem != null && sortItem.IsConfiguration)
                            sortItem.SortDirection = Global.SortDirections.descending;
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<LabOrder>("LabOrder", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected lab order.
        /// </summary>
        /// <value>The selected lab order.</value>
        [ACPropertySelected(601, "LabOrder")]
        public virtual LabOrder SelectedLabOrder
        {
            get
            {
                if (AccessPrimary == null)
                    return null; 
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                SetCurrentSelected(value);
            }
        }

        /// <summary>
        /// Gets or sets the current lab order.
        /// </summary>
        /// <value>The current lab order.</value>
        [ACPropertyCurrent(602, "LabOrder")]
        public virtual LabOrder CurrentLabOrder
        {
            get
            {
                if (AccessPrimary == null)
                    return null; 
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {               
                SetCurrentSelected(value);
            }
        }


        public virtual void SetCurrentSelected(LabOrder value)
        {
            if (AccessPrimary == null)
                return;
            if (value != CurrentLabOrder)
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentLabOrder");
                OnPropertyChanged("LabOrderPosList");
            }
            if(value != SelectedLabOrder)
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedLabOrder");
                OnPropertyChanged("LabOrderPosList");
            }
        }

        /// <summary>
        /// Gets the lab order list.
        /// </summary>
        /// <value>The lab order list.</value>
        [ACPropertyList(603, "LabOrder")]
        public IEnumerable<LabOrder> LabOrderList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region LabOrderPos
        /// <summary>
        /// The _ access lab order pos
        /// </summary>
        ACAccess<LabOrderPos> _AccessLabOrderPos;
        /// <summary>
        /// Gets the access lab order pos.
        /// </summary>
        /// <value>The access lab order pos.</value>
        [ACPropertyAccess(9999, "LabOrderPos")]
        public ACAccess<LabOrderPos> AccessLabOrderPos
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                if (_AccessLabOrderPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + LabOrderPos.ClassName) as ACQueryDefinition;
                    _AccessLabOrderPos = acQueryDefinition.NewAccess<LabOrderPos>(LabOrderPos.ClassName, this);
                }
                return _AccessLabOrderPos;
            }
        }

        /// <summary>
        /// The _ current lab order pos
        /// </summary>
        protected bool isCurrentPosInChange = false;
        LabOrderPos _CurrentLabOrderPos;
        /// <summary>
        /// Gets or sets the current lab order pos.
        /// </summary>
        /// <value>The current lab order pos.</value>
        [ACPropertyCurrent(604, "LabOrderPos")]
        public LabOrderPos CurrentLabOrderPos
        {
            get
            {
                return _CurrentLabOrderPos;
            }
            set
            {
                if (_CurrentLabOrderPos != value)
                {
                    SetCurrentSelectedLabOrderPos(value);
                    CurrentLabOrderPos.PropertyChanged -= CurrentLabOrderPos_PropertyChanged;
                }
                if(_CurrentLabOrderPos != null)
                    CurrentLabOrderPos.PropertyChanged += CurrentLabOrderPos_PropertyChanged;
            }
        }

        protected virtual void CurrentLabOrderPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "MDLabTagID")
                OnPropertyChanged("LabOrderPosList");
        }

        /// <summary>
        /// Gets the lab order pos list.
        /// </summary>
        /// <value>The lab order pos list.</value>
        [ACPropertyList(605, "LabOrderPos")]
        public IEnumerable<LabOrderPos> LabOrderPosList
        {
            get
            {
                if (CurrentLabOrder == null)
                    return null;
                CurrentLabOrder.LabOrderPos_LabOrder.AutoRefresh(this.DatabaseApp);
                return CurrentLabOrder.LabOrderPos_LabOrder.OrderBy(c => c.Sequence);
            }
        }

        /// <summary>
        /// The _ selected lab order pos
        /// </summary>
        LabOrderPos _SelectedLabOrderPos;
        /// <summary>
        /// Gets or sets the selected lab order pos.
        /// </summary>
        /// <value>The selected lab order pos.</value>
        [ACPropertySelected(606, "LabOrderPos")]
        public LabOrderPos SelectedLabOrderPos
        {
            get
            {
                return _SelectedLabOrderPos;
            }
            set
            {
                SetCurrentSelectedLabOrderPos(value);
            }
        }

        public void SetCurrentSelectedLabOrderPos(LabOrderPos value)
        {
            if (CurrentLabOrderPos != value)
            {
                isCurrentPosInChange = true;
                _CurrentLabOrderPos = value;
                OnPropertyChanged("CurrentLabOrderPos");
            }
            if (SelectedLabOrderPos != value)
            {
                _SelectedLabOrderPos = value;
                OnPropertyChanged("SelectedLabOrderPos");
            }
        }
        #endregion

        #endregion

        #region BSO -> ACMethod

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
                case "CurrentLabOrderPos\\Sequence":
                    {
                        return result;
                    }
                default:
                    return result;
            }
        }

        #region LabOrder
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("LabOrder", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public virtual void Save()
        {
            OnSave();
        }

        /// <summary>
        /// Determines whether [is enabled save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled save]; otherwise, <c>false</c>.</returns>
        public virtual bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand("LabOrder", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction("LabOrder", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedLabOrder", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<LabOrder>(requery, () => SelectedLabOrder, () => CurrentLabOrder, c => CurrentLabOrder = c,
                        DatabaseApp.LabOrder
                        .Include(c => c.LabOrderPos_LabOrder)
                        .Where(c => c.LabOrderID == SelectedLabOrder.LabOrderID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedLabOrder != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("LabOrder", "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedLabOrder", Global.ACKinds.MSMethodPrePost)]
        public virtual void New()
        {
            if (!PreExecute("New")) 
                return;
            if (AccessPrimary == null)
                return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(LabOrder), LabOrder.NoColumnName, LabOrder.FormatNewNo, this);
            var newLabOrder = LabOrder.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.LabOrder.AddObject(newLabOrder);
            ACState = Const.SMNew;
            AccessPrimary.NavList.Add(newLabOrder);
            CurrentLabOrder = newLabOrder;
            OnPropertyChanged("LabOrderList");
            PostExecute("New");
        }

        /// <summary>
        /// Determines whether [is enabled new].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new]; otherwise, <c>false</c>.</returns>
        public virtual bool IsEnabledNew()
        {
            return true;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction("LabOrder", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentLabOrder", Global.ACKinds.MSMethodPrePost)]
        public virtual void Delete()
        {
            if (!PreExecute("Delete")) 
                return;
            if (AccessPrimary == null)
                return;
            Msg msg = CurrentLabOrder.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentLabOrder);
            SelectedLabOrder = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }
        
        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public virtual bool IsEnabledDelete()
        {
            return CurrentLabOrder != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("LabOrder", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public virtual void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("LabOrderList");
        }
        #endregion

        #region LabOrderPos
        /// <summary>
        /// Loads the lab order pos.
        /// </summary>
        [ACMethodInteraction("LabOrderPos", "en{'Load Position'}de{'Position laden'}", (short)MISort.Load, false, "SelectedLabOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void LoadLabOrderPos()
        {
            if (!IsEnabledLoadLabOrderPos())
                return;
            if (!PreExecute("LoadLabOrderPos")) return;
            // Laden des aktuell selektierten LabOrderPos 
            CurrentLabOrderPos = (from c in CurrentLabOrder.LabOrderPos_LabOrder
                                  where c.LabOrderPosID == SelectedLabOrderPos.LabOrderPosID
                                  select c).First();
            PostExecute("LoadLabOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled load lab order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load lab order pos]; otherwise, <c>false</c>.</returns>
        public virtual bool IsEnabledLoadLabOrderPos()
        {
            return SelectedLabOrderPos != null && CurrentLabOrder != null;
        }

        /// <summary>
        /// News the lab order pos.
        /// </summary>
        [ACMethodInteraction("LabOrderPos", "en{'New Position'}de{'Neue Position'}", (short)MISort.New, true, "SelectedLabOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void NewLabOrderPos()
        {
            if (!PreExecute("NewLabOrderPos")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            CurrentLabOrderPos = LabOrderPos.NewACObject(DatabaseApp, CurrentLabOrder);
            CurrentLabOrderPos.LabOrder = CurrentLabOrder;
            CurrentLabOrder.LabOrderPos_LabOrder.Add(CurrentLabOrderPos);
            OnPropertyChanged("LabOrderPosList");
            PostExecute("NewLabOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled new lab order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new lab order pos]; otherwise, <c>false</c>.</returns>
        public virtual bool IsEnabledNewLabOrderPos()
        {
            if (CurrentLabOrder != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Deletes the lab order pos.
        /// </summary>
        [ACMethodInteraction("LabOrderPos", "en{'Delete Position'}de{'Position löschen'}", (short)MISort.Delete, true, "CurrentLabOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteLabOrderPos()
        {
            if (!PreExecute("DeleteLabOrderPos")) return;
            Msg msg = CurrentLabOrderPos.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            OnPropertyChanged("LabOrderPosList");
            PostExecute("DeleteLabOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled delete lab order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete lab order pos]; otherwise, <c>false</c>.</returns>
        public virtual bool IsEnabledDeleteLabOrderPos()
        {
            return CurrentLabOrder != null && CurrentLabOrderPos != null;
        }

        #endregion
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "Save":
                    Save();
                    return true;
                case "IsEnabledSave":
                    result = IsEnabledSave();
                    return true;
                case "UndoSave":
                    UndoSave();
                    return true;
                case "IsEnabledUndoSave":
                    result = IsEnabledUndoSave();
                    return true;
                case "Load":
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case "IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
                case "New":
                    New();
                    return true;
                case "IsEnabledNew":
                    result = IsEnabledNew();
                    return true;
                case "Delete":
                    Delete();
                    return true;
                case "IsEnabledDelete":
                    result = IsEnabledDelete();
                    return true;
                case "Search":
                    Search();
                    return true;
                case "LoadLabOrderPos":
                    LoadLabOrderPos();
                    return true;
                case "IsEnabledLoadLabOrderPos":
                    result = IsEnabledLoadLabOrderPos();
                    return true;
                case "NewLabOrderPos":
                    NewLabOrderPos();
                    return true;
                case "IsEnabledNewLabOrderPos":
                    result = IsEnabledNewLabOrderPos();
                    return true;
                case "DeleteLabOrderPos":
                    DeleteLabOrderPos();
                    return true;
                case "IsEnabledDeleteLabOrderPos":
                    result = IsEnabledDeleteLabOrderPos();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}
