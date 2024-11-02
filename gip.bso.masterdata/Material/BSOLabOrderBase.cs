// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;
using gip.mes.facility;
using static gip.core.datamodel.Global;
using Microsoft.EntityFrameworkCore;

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
        public BSOLabOrderBase(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
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
            bool skipSearchOnStart = ParameterValueT<bool>(Const.SkipSearchOnStart);
            if (!skipSearchOnStart)
            {
                Search();
            }
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


        #region BSO -> ACProperty -> AccessPrimary
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
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        if(navACQueryDefinition.TakeCount == 0)
                        {
                            // dp something
                        }
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(NavigationqueryDefaultSort);
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);
                        _AccessPrimary = navACQueryDefinition.NewAccessNav<LabOrder>(LabOrder.ClassName, this);
                        _AccessPrimary.NavSearchExecuting += LabOrder_AccessPrimary_NavSearchExecuting;
                    }
                }

                return _AccessPrimary;
            }
        }

        public virtual IQueryable<LabOrder> LabOrder_AccessPrimary_NavSearchExecuting(IQueryable<LabOrder> result)
        {
            if (IsEnabledApplyValueFilter())
            {
                if (ValueFilterFieldType == LOPosValueFieldEnum.MinMin)
                    result = result.Where(c => c.LabOrderPos_LabOrder.Where(l => l.MDLabTagID == this.SelectedLabTag.MDLabTagID
                                                                    && l.ValueMinMin >= FilterValueFrom.Value
                                                                    && l.ValueMinMin <= FilterValueTo.Value).Any());
                else if (ValueFilterFieldType == LOPosValueFieldEnum.Min)
                    result = result.Where(c => c.LabOrderPos_LabOrder.Where(l => l.MDLabTagID == this.SelectedLabTag.MDLabTagID
                                                                    && l.ValueMin >= FilterValueFrom.Value
                                                                    && l.ValueMin <= FilterValueTo.Value).Any());
                else if (ValueFilterFieldType == LOPosValueFieldEnum.Max)
                    result = result.Where(c => c.LabOrderPos_LabOrder.Where(l => l.MDLabTagID == this.SelectedLabTag.MDLabTagID
                                                                    && l.ValueMax >= FilterValueFrom.Value
                                                                    && l.ValueMax <= FilterValueTo.Value).Any());
                else //if (ValueFilterFieldType == LOPosValueFieldEnum.MaxMax)
                    result = result.Where(c => c.LabOrderPos_LabOrder.Where(l => l.MDLabTagID == this.SelectedLabTag.MDLabTagID
                                                                    && l.ValueMaxMax >= FilterValueFrom.Value
                                                                    && l.ValueMaxMax <= FilterValueTo.Value).Any());
            }
            return result;
        }


        public virtual List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem partslistNo = new ACSortItem(nameof(LabOrder.LabOrderNo), SortDirections.descending, true);
                acSortItems.Add(partslistNo);

                return acSortItems;
            }
        }

        public virtual List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                ACFilterItem phLabOrderTypeIndex = new ACFilterItem(Global.FilterTypes.filter, nameof(LabOrder.LabOrderTypeIndex), Global.LogicalOperators.equal, Global.Operators.and, ((short)FilterLabOrderType).ToString(), true);
                aCFilterItems.Add(phLabOrderTypeIndex);

                ACFilterItem phLabOrderNo = new ACFilterItem(FilterTypes.filter, nameof(LabOrder.LabOrderNo), LogicalOperators.contains, Operators.and, null, true, true);
                aCFilterItems.Add(phLabOrderNo);

                return aCFilterItems;
            }
        }

        public virtual GlobalApp.LabOrderType FilterLabOrderType
        {
            get
            {
                return GlobalApp.LabOrderType.Order;
            }
        }

        #endregion

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
                return AccessPrimary.Selected;
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
                return AccessPrimary.Current;
            }
            set
            {
                SetCurrentSelected(value);
            }
        }


        public virtual bool SetCurrentSelected(LabOrder value)
        {
            bool isChanged = false;
            if (AccessPrimary == null)
                return false;
            if (value != CurrentLabOrder)
            {
                AccessPrimary.Current = value;
                OnPropertyChanged(nameof(CurrentLabOrder));
                isChanged = true;
            }
            if (value != SelectedLabOrder)
            {
                AccessPrimary.Selected = value;
                OnPropertyChanged(nameof(SelectedLabOrder));
                isChanged = true;
            }

            if (isChanged)
                LoadLabOrderPosList(value);

            return isChanged;
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
                if (_CurrentLabOrderPos != null)
                    CurrentLabOrderPos.PropertyChanged += CurrentLabOrderPos_PropertyChanged;
            }
        }

        protected virtual void CurrentLabOrderPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MDLabTagID" || e.PropertyName == "MDLabTag")
                OnPropertyChanged(nameof(LabOrderPosList));
        }

        private List<LabOrderPos> _LabOrderPosList;
        /// <summary>
        /// Gets the lab order pos list.
        /// </summary>
        /// <value>The lab order pos list.</value>
        [ACPropertyList(605, "LabOrderPos")]
        public List<LabOrderPos> LabOrderPosList
        {
            get
            {
                if (_LabOrderPosList == null)
                    _LabOrderPosList = new List<LabOrderPos>();
                return _LabOrderPosList;
            }
        }

        public void LoadLabOrderPosList(LabOrder labOrder)
        {
            _LabOrderPosList = null;
            if (labOrder != null)
            {
                if(labOrder.EntityState != EntityState.Added)
                {
                	labOrder.LabOrderPos_LabOrder.AutoRefresh(labOrder.LabOrderPos_LabOrderReference, labOrder);				}
                _LabOrderPosList = labOrder.LabOrderPos_LabOrder.OrderBy(c => c.Sequence).ToList();
            }
            if (_LabOrderPosList == null)
                SelectedLabOrderPos = null;
            else
                SelectedLabOrderPos = _LabOrderPosList.FirstOrDefault();
            OnPropertyChanged(nameof(LabOrderPosList));
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
                OnPropertyChanged(nameof(CurrentLabOrderPos));
            }
            if (SelectedLabOrderPos != value)
            {
                _SelectedLabOrderPos = value;
                OnPropertyChanged(nameof(SelectedLabOrderPos));
            }
        }
        #endregion

        #region Find and Replace
        private double? _FilterValueFrom;
        [ACPropertyInfo(610, "", "en{'Limit value from'}de{'Grenzwert von'}")]
        public double? FilterValueFrom
        {
            get
            {
                return _FilterValueFrom;
            }
            set
            {
                _FilterValueFrom = value;
                OnPropertyChanged(nameof(FilterValueFrom));
            }
        }

        private double? _FilterValueTo;
        [ACPropertyInfo(611, "", "en{'Limit value to'}de{'Grenzwert bis'}")]
        public double? FilterValueTo
        {
            get
            {
                return _FilterValueTo;
            }
            set
            {
                _FilterValueTo = value;
                OnPropertyChanged(nameof(FilterValueTo));
            }
        }

        private ACValueListLOPosValueFieldEnum _ValueFilterFieldList = new ACValueListLOPosValueFieldEnum();
        [ACPropertyList(620, "ValueFilterField", "en{'Search in Field'}de{'Suche in Feld'}")]
        public List<ACValueItem> ValueFilterFieldList
        {
            get
            {
                return _ValueFilterFieldList;
            }
        }

        ACValueItem _SelectedValueFilterField;
        [ACPropertySelected(621, "ValueFilterField", "en{'Search in Field'}de{'Suche in Feld'}")]
        public ACValueItem SelectedValueFilterField
        {
            get
            {
                return _SelectedValueFilterField;
            }
            set
            {
                _SelectedValueFilterField = value;
                OnPropertyChanged(nameof(SelectedValueFilterField));
            }
        }

        public LOPosValueFieldEnum ValueFilterFieldType
        {
            get
            {
                if (SelectedValueFilterField == null)
                    return LOPosValueFieldEnum.MinMin;
                return (LOPosValueFieldEnum)SelectedValueFilterField.Value;
            }
        }

        MDLabTag _SelectedLabTag;
        [ACPropertySelected(624, "MDLabTag", "en{'Laboratory Tag'}de{'Laborkennzeichen'}")]
        public MDLabTag SelectedLabTag
        {
            get
            {
                return _SelectedLabTag;
            }
            set
            {
                _SelectedLabTag = value;
                OnPropertyChanged(nameof(SelectedLabTag));
            }
        }

        [ACPropertyList(625, "MDLabTag", "en{'Laboratory Tag'}de{'Laborkennzeichen'}")]
        public IEnumerable<MDLabTag> LabTagList
        {
            get
            {
                return DatabaseApp.MDLabTag.ToArray();
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
            if (OnSave())
                Search();
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
            DatabaseApp.LabOrder.Add(newLabOrder);
            ACState = Const.SMNew;
            AccessPrimary.NavList.Add(newLabOrder);
            CurrentLabOrder = newLabOrder;
            OnPropertyChanged(nameof(LabOrderList));
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
            if (AccessPrimary == null) 
				return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(LabOrderList));
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
            CurrentLabOrderPos = CurrentLabOrder.LabOrderPos_LabOrder.Where(c => c.LabOrderPosID == SelectedLabOrderPos.LabOrderPosID).FirstOrDefault();
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
            LabOrderPosList.Add(CurrentLabOrderPos);
            OnPropertyChanged(nameof(LabOrderPosList));
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
            LabOrderPosList.Remove(CurrentLabOrderPos);
            Msg msg = CurrentLabOrderPos.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            OnPropertyChanged(nameof(LabOrderPosList));
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

        #region Filter
        [ACMethodCommand("ValueFilterField", "en{'Search over filtered value range'}de{'Suche per gefiltertem Grenzwert'}", 500, false)]
        public void ApplyValueFilter()
        {
            if (!IsEnabledApplyValueFilter())
                return;
            Search();
        }

        public virtual bool IsEnabledApplyValueFilter()
        {
            return FilterValueFrom.HasValue
                && FilterValueTo.HasValue
                && SelectedValueFilterField != null
                && SelectedLabTag != null;
        }

        [ACMethodCommand("ValueFilterField", "en{'Clear search filter'}de{'Lösche Suchfilter'}", 501, false)]
        public void ClearValueFilter()
        {
            FilterValueFrom = null;
            FilterValueTo = null;
            //SelectedValueFilterField = null;
            SelectedLabTag = null;
            Search();
        }

        public virtual bool IsEnabledClearValueFilter()
        {
            return true;
        }

        #endregion

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
                case nameof(LoadLabOrderPos):
                    LoadLabOrderPos();
                    return true;
                case nameof(IsEnabledLoadLabOrderPos):
                    result = IsEnabledLoadLabOrderPos();
                    return true;
                case nameof(NewLabOrderPos):
                    NewLabOrderPos();
                    return true;
                case nameof(IsEnabledNewLabOrderPos):
                    result = IsEnabledNewLabOrderPos();
                    return true;
                case nameof(DeleteLabOrderPos):
                    DeleteLabOrderPos();
                    return true;
                case nameof(IsEnabledDeleteLabOrderPos):
                    result = IsEnabledDeleteLabOrderPos();
                    return true;
                case nameof(ApplyValueFilter):
                    ApplyValueFilter();
                    return true;
                case nameof(IsEnabledApplyValueFilter):
                    result = IsEnabledApplyValueFilter();
                    return true;
                case nameof(ClearValueFilter):
                    ClearValueFilter();
                    return true;
                case nameof(IsEnabledClearValueFilter):
                    result = IsEnabledClearValueFilter();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}