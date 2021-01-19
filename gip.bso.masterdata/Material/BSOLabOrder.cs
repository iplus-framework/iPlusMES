using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Reflection;

namespace gip.bso.masterdata
{
    /// <summary>
    /// The bussiness object for a laboratory orders.
    /// </summary>
    ///<summary xml:lang="de">
    /// Das Geschäftsobjekt für einen Laborauftrag.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Lab Order'}de{'Laborauftrag'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + LabOrder.ClassName)]
    [ACQueryInfo(Const.PackName_VarioMaterial, Const.QueryPrefix + LabOrder.ClassName, "en{'Lab Order'}de{'Laborauftrag'}", typeof(LabOrder), LabOrder.ClassName, "LabOrderTypeIndex", "LabOrderNo")]
    public class BSOLabOrder : BSOLabOrderBase
    {
        #region c'tors
        public BSOLabOrder(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            //OnPropertyChanged("LabOrderPosAVGList");
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_LastLabOrder != null)
                _LastLabOrder.PropertyChanged -= CurrentLabOrder_PropertyChanged;
            _LastLabOrder = null;
            this._DialogSelectedTemplate = null;
            this._LabOrderMaterialState = null;
            this._LabOrderTemplate = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }
        #endregion

        #region Filters

        private bool _IsConnectedWithDeliveryNote;
        [ACPropertyInfo(750, "FilterConnectedWithDN", "en{'Has Delivery Note'}de{'Verbunden mit Eingangslieferschein'}")]
        public bool IsConnectedWithDeliveryNote
        {
            get
            {
                return _IsConnectedWithDeliveryNote;
            }
            set
            {
                if (_IsConnectedWithDeliveryNote != value)
                {
                    _IsConnectedWithDeliveryNote = value;
                    OnIsConnectedWithDeliveryNoteChanged(value);
                    OnPropertyChanged("IsConnectedWithDeliveryNote");
                }
            }
        }

        public virtual void OnIsConnectedWithDeliveryNoteChanged(bool value)
        {
            // do nothing
        }


        #region Filters -> FilterMaterialGroup

        private MDMaterialGroup _SelectedFilterMaterialGroup;
        /// <summary>
        /// Selected property for MDMaterialGroup
        /// </summary>
        /// <value>The selected FilterMaterialGroup</value>
        [ACPropertySelected(751, "FilterMaterialGroup", "en{'Material Group'}de{'Materialgruppe'}")]
        public MDMaterialGroup SelectedFilterMaterialGroup
        {
            get
            {
                return _SelectedFilterMaterialGroup;
            }
            set
            {
                _SelectedFilterMaterialGroup = value;
                OnPropertyChanged("SelectedFilterMaterialGroup");
            }
        }

        private IEnumerable<MDMaterialGroup> _FilterMaterialGroupList;
        /// <summary>
        /// List property for MDMaterialGroup
        /// </summary>
        /// <value>The FilterMaterialGroup list</value>
        [ACPropertyList(752, "FilterMaterialGroup")]
        public IEnumerable<MDMaterialGroup> FilterMaterialGroupList
        {
            get
            {
                if (_FilterMaterialGroupList == null)
                {
                    _FilterMaterialGroupList = DatabaseApp.MDMaterialGroup.OrderBy(x => x.MDKey);
                }
                return _FilterMaterialGroupList;
            }
        }

        #endregion

        #region Filter -> FilterDistributorCompany (Delivery company)

        #region FilterShipperCompany
        private Company _SelectedFilterDistributorCompany;
        /// <summary>
        /// Selected property for Company
        /// </summary>
        /// <value>The selected FilterDistributorCompany</value>
        [ACPropertySelected(753, "FilterDistributorCompany", "en{'Distributor'}de{'Lieferant'}")]
        public Company SelectedFilterDistributorCompany
        {
            get
            {
                return _SelectedFilterDistributorCompany;
            }
            set
            {
                if (_SelectedFilterDistributorCompany != value)
                {
                    _SelectedFilterDistributorCompany = value;
                    OnPropertyChanged("SelectedFilterDistributorCompany");
                }
            }
        }

        private List<Company> _FilterDistributorCompanyList;
        /// <summary>
        /// List property for Company
        /// </summary>
        /// <value>The FilterDistributorCompany list</value>
        [ACPropertyList(754, "FilterDistributorCompany")]
        public List<Company> FilterDistributorCompanyList
        {
            get
            {
                if (_FilterDistributorCompanyList == null)
                    LoadFilterDistributorCompanyList();
                return _FilterDistributorCompanyList;
            }
        }

        private void LoadFilterDistributorCompanyList()
        {
            // @aagincic NOTE: IsActive is not edited in system so there is not this condition applyed jet.
            _FilterDistributorCompanyList =
                DatabaseApp.Company.Where(x => x.IsDistributor /* && x.IsActive*/).OrderBy(x => x.CompanyName).ToList();
        }
        #endregion


        #endregion

        #endregion

        #region BSO -> ACProperties
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<LabOrder> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        /// <summary xml:lang="de">
        /// Ruft den primären Zugriff ab.
        /// </summary>
        /// <value xml:lang="de">Der Zugang primär.</value>
        [ACPropertyAccessPrimary(9999, "LabOrder")]
        public override ACAccessNav<LabOrder> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + LabOrder.ClassName, ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {

                        ACSortItem sortItem = navACQueryDefinition.ACSortColumns.Where(c => c.ACIdentifier == "LabOrderNo").FirstOrDefault();
                        if (sortItem != null && sortItem.IsConfiguration)
                            sortItem.SortDirection = Global.SortDirections.descending;
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = 50;
                    }

                    short loType = (short)GlobalApp.LabOrderType.Order;
                    bool rebuildACQueryDef = false;
                    if (!navACQueryDefinition.ACFilterColumns.Any())
                    {
                        rebuildACQueryDef = true;
                    }
                    else
                    {
                        int countFoundCorrect = 0;
                        foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            if (filterItem.PropertyName == "LabOrderTypeIndex")
                            {
                                if (filterItem.SearchWord == loType.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal)
                                    countFoundCorrect++;
                            }
                            else if (filterItem.PropertyName == "LabOrderNo")
                            {
                                countFoundCorrect++;
                            }
                        }
                        if (countFoundCorrect < 2)
                            rebuildACQueryDef = true;
                    }
                    if (rebuildACQueryDef)
                    {
                        navACQueryDefinition.ClearFilter(true);
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "LabOrderTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, loType.ToString(), true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "LabOrderNo", Global.LogicalOperators.equal, Global.Operators.and, null, true));
                        navACQueryDefinition.SaveConfig(true);
                    }



                    _AccessPrimary = navACQueryDefinition.NewAccessNav<LabOrder>("LabOrder", this);
                    _AccessPrimary.NavSearchExecuting += LabOrder_AccessPrimary_NavSearchExecuting;
                }

                ACFilterItem filterItemMaterialGroup = _AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(x => x.PropertyName == @"Material\MDMaterialGroup\MDKey").FirstOrDefault();
                if (SelectedFilterMaterialGroup != null)
                {
                    if (filterItemMaterialGroup == null)
                    {
                        filterItemMaterialGroup = new ACFilterItem(Global.FilterTypes.filter, @"Material\MDMaterialGroup\MDKey", Global.LogicalOperators.equal, Global.Operators.and, "", false);
                        _AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(filterItemMaterialGroup);
                    }
                    filterItemMaterialGroup.SearchWord = SelectedFilterMaterialGroup.MDKey;
                }
                else if (filterItemMaterialGroup != null)
                {
                    _AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemMaterialGroup);
                }
                return _AccessPrimary;
            }
        }

        public virtual IQueryable<LabOrder> LabOrder_AccessPrimary_NavSearchExecuting(IQueryable<LabOrder> result)
        {
            if (IsConnectedWithDeliveryNote)
            {
                result = result.Where(x => x.InOrderPosID != null && x.InOrderPos.DeliveryNotePos_InOrderPos.Any());
            }
            return result;
        }

        LabOrder _LastLabOrder = null;


        /// <summary>
        /// Gets or sets the current laboratory order.
        /// </summary>
        /// <summary xml:lang="de">
        /// Liest oder setzt den aktuellen Laborauftrag.
        /// </summary>
        [ACPropertyCurrent(701, "LabOrder")]
        public override LabOrder CurrentLabOrder
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (base.CurrentLabOrder != value)
                {
                    if (_LastLabOrder != null)
                        _LastLabOrder.PropertyChanged -= CurrentLabOrder_PropertyChanged;
                    SetCurrentSelected(value);
                    _LastLabOrder = value;
                    if (_LastLabOrder != null)
                    {
                        _LastLabOrder.PropertyChanged += CurrentLabOrder_PropertyChanged;
                    }
                }
            }
        }

        protected virtual void CurrentLabOrder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MaterialID")
                OnPropertyChanged("DialogTemplateList");

            if (e.PropertyName == "InOrderPosID")
            {
                if (CurrentLabOrder != null && CurrentLabOrder.InOrderPos != null)
                    CurrentLabOrder.Material = CurrentLabOrder.InOrderPos.Material;
                OnPropertyChanged("CurrentLabOrder");
            }

            if (e.PropertyName == "OutOrderPosID")
            {
                if (CurrentLabOrder != null && CurrentLabOrder.OutOrderPos != null)
                    CurrentLabOrder.Material = CurrentLabOrder.OutOrderPos.Material;
                OnPropertyChanged("CurrentLabOrder");
            }

            if (e.PropertyName == "ProdOrderPartslistPosID")
            {
                if (CurrentLabOrder != null && CurrentLabOrder.ProdOrderPartslistPos != null)
                {
                    // Last mixure represent a finall product - used templates for a this finall product
                    CurrentLabOrder.Material = CurrentLabOrder.ProdOrderPartslistPos.BookingMaterial;
                }
                OnPropertyChanged("CurrentLabOrder");
            }

            if (e.PropertyName == "FacilityLotID")
            {
                if (CurrentLabOrder != null && CurrentLabOrder.FacilityLot != null)
                    CurrentLabOrder.Material = CurrentLabOrder.FacilityLot.Material;
                OnPropertyChanged("CurrentLabOrder");
            }
        }

        protected override void CurrentLabOrderPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.CurrentLabOrderPos_PropertyChanged(sender, e);
        }

        /// <summary>
        /// Gets the list of laboratory templates in laboratory order dialog.
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft die Liste der Laborvorlagen im Laborauftragsdialog ab.
        /// </summary>
        [ACPropertyInfo(710, "Dialog", "en{'Template'}de{'Vorlage'}", "DialogTemplateList", false)]
        public IQueryable<LabOrder> DialogTemplateList
        {
            get
            {
                if (CurrentLabOrder != null)
                    return LabOrderManager.ReturnLabOrderTemplateList(DatabaseApp).Where(c => c.MaterialID == CurrentLabOrder.MaterialID);
                return null;
            }
        }

        LabOrder _DialogSelectedTemplate;
        /// <summary>
        /// Gets or sets the selected laboratory template in laboratory order dialog.
        /// </summary>
        /// <summary xml:lang="de">
        /// Liest oder setzt die ausgewählte Laborvorlage im Laborauftragsdialog.
        /// </summary>
        [ACPropertyInfo(711, "Dialog", "en{'Template'}de{'Vorlage'}", "DialogSelectedTemplate", false)]
        public LabOrder DialogSelectedTemplate
        {
            get
            {
                return _DialogSelectedTemplate;
            }
            set
            {
                if (_DialogSelectedTemplate != value)
                    _DialogSelectedTemplate = value;
            }
        }

        /// <summary>
        /// Gets the list of material state in laboratory order.
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft die Liste der Materialzustände im Laborauftrag ab.
        /// </summary>
        [ACPropertyInfo(712, "Dialog", "en{'From Position Type'}de{'Aus Positionsart'}", "LabOrderMaterialStateList", false)] //"en{'Material state'}de{'Material state'}"
        public ACValueItemList LabOrderMaterialStateList
        {
            get
            {
                return GlobalApp.LabOrderMaterialStateList;
            }
        }

        ACValueItem _LabOrderMaterialState = GlobalApp.LabOrderMaterialStateList.First();
        /// <summary>
        /// Gets or sets the material state in laboratory order.
        /// </summary>
        /// <summary xml:lang="de">
        /// Liest oder setzt den Materialzustand im Laborauftrag.
        /// </summary>
        [ACPropertyInfo(713, "Dialog", "en{'From Position Type'}de{'Aus Positionsart'}", "LabOrderMaterialState", false)] //"en{'Material state'}de{'Material state'}"ter
        public ACValueItem LabOrderMaterialState
        {
            get
            {
                return _LabOrderMaterialState;
            }
            set
            {
                if (_LabOrderMaterialState != value)
                {
                    _LabOrderMaterialState = value;
                    CurrentLabOrder.Material = null;
                    _DialogSelectedTemplate = null;
                    OnPropertyChanged("CurrentLabOrder");
                    OnPropertyChanged("DialogSelectedTemplate");
                    OnPropertyChanged("DialogTemplateList");
                }
            }
        }

        LabOrder _LabOrderTemplate;
        /// <summary>
        /// Gets the numeration of the current laboratory template for the laboratory order.
        /// </summary>
        /// <summary xml:lang="de">
        /// Liefert die Nummerierung der aktuellen Laborvorlage für den Laborauftrag.
        /// </summary>
        [ACPropertyInfo(714, "LabOrder", "en{'Template No.'}de{'Vorlage Nr.'}", "LabOrderTemplateNo", false)]
        public string LabOrderTemplateNo
        {
            get
            {
                if (_LabOrderTemplate != null)
                    return _LabOrderTemplate.LabOrderNo.ToString();
                return "";
            }
        }

        /// <summary>
        /// Gets the name of laboratory template for the current laboratory order.
        /// </summary>
        /// <summary xml:lang="de">
        /// Liefert den Namen der Laborvorlage für den aktuellen Laborauftrag.
        /// </summary>
        [ACPropertyInfo(715, "LabOrder", "en{'Template Name'}de{'Vorlagebezeichnung'}", "LabOrderTemplateName", false)]
        public string LabOrderTemplateName
        {
            get
            {
                if (_LabOrderTemplate != null)
                {
                    if (!string.IsNullOrEmpty(_LabOrderTemplate.TemplateName))
                    {
                        return _LabOrderTemplate.TemplateName;
                    }
                    else
                        return "";
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets the value is laboratory order have a parent.
        /// </summary>
        /// <summary lang="de">
        /// Liefert den Wert ist Laborauftrag haben übergeordnet.
        /// </summary>
        [ACPropertyInfo(716, "LabOrder", "en{'IsLabOrderParent'}de{'IsLabOrderParent'}", "LabOrderTemplateName", false)]
        public bool IsLabOrderParent
        {
            get
            {
                return ParentACComponent is Businessobjects;
            }
        }

        /// <summary>
        /// Gets the list of inorder positions which is available for the laboratory order.
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft die Liste der Auftragspositionen ab, die für den Laborauftrag verfügbar ist.
        /// </summary>
        [ACPropertyList(717, "LabOrderInOrderPos", "en{'InOrderPos'}de{'InOrderPos'}")]
        public List<InOrderPos> LabOrderInOrderPosList
        {
            get
            {
                return DatabaseApp.InOrderPos.Where(c => c.DeliveryNotePos_InOrderPos.Any()).ToList();
            }
        }

        /// <summary>
        /// Gets the list of outorder positions which is available for the laboratory order.
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft die Liste der Outorder-Positionen ab, die für den Laborauftrag verfügbar ist.
        /// </summary>
        [ACPropertyList(718, "LabOrderOutOrderPos", "en{'OutOrderPos'}de{'OutOrderPos'}")]
        public List<OutOrderPos> LabOrderOutOrderPosList
        {
            get
            {
                return DatabaseApp.OutOrderPos.Where(c => c.DeliveryNotePos_OutOrderPos.Any()).ToList();
            }
        }
        #endregion

        #region BSO -> ACMethods

        bool _IsMaterialStateEnabled = true;
        bool _IsLabOrderPosViewDialog = false;
        bool _IsNewLabOrderDialogOpen = false;
        public VBDialogResult DialogResult;

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
            if (_IsNewLabOrderDialogOpen && !string.IsNullOrEmpty(vbControl.VBContent))
            {
                if (vbControl.VBContent.StartsWith("CurrentLabOrder\\InOrderPos"))
                    if (!_IsMaterialStateEnabled && CurrentLabOrder.InOrderPos != null)
                        result = Global.ControlModes.Disabled;
                    else if (_LabOrderMaterialState != null && _LabOrderMaterialState.Value.ToString() == InOrderPos.ClassName)
                        result = Global.ControlModes.Enabled;
                    else
                    {
                        CurrentLabOrder.InOrderPos = null;
                        result = Global.ControlModes.Hidden;
                    }

                else if (vbControl.VBContent.StartsWith("CurrentLabOrder\\OutOrderPos"))
                    if (!_IsMaterialStateEnabled && CurrentLabOrder.OutOrderPos != null)
                        result = Global.ControlModes.Disabled;
                    else if (_LabOrderMaterialState != null && _LabOrderMaterialState.Value.ToString() == OutOrderPos.ClassName)
                        result = Global.ControlModes.Enabled;
                    else
                    {
                        CurrentLabOrder.OutOrderPos = null;
                        result = Global.ControlModes.Hidden;
                    }

                else if (vbControl.VBContent.StartsWith("CurrentLabOrder\\ProdOrderPartslistPos"))
                    if (!_IsMaterialStateEnabled && CurrentLabOrder.ProdOrderPartslistPos != null)
                        result = Global.ControlModes.Disabled;
                    else if (_LabOrderMaterialState != null && _LabOrderMaterialState.Value.ToString() == "PartslistPos")
                        result = Global.ControlModes.Enabled;
                    else
                    {
                        CurrentLabOrder.ProdOrderPartslistPos = null;
                        result = Global.ControlModes.Hidden;
                    }

                else if (vbControl.VBContent.StartsWith("CurrentLabOrder\\FacilityLot"))
                    if (!_IsMaterialStateEnabled && CurrentLabOrder.FacilityLot != null)
                        result = Global.ControlModes.Disabled;
                    else if (_LabOrderMaterialState != null && _LabOrderMaterialState.Value.ToString() == "LotCharge")
                        result = Global.ControlModes.Enabled;
                    else
                    {
                        CurrentLabOrder.FacilityLot = null;
                        result = Global.ControlModes.Hidden;
                    }
            }
            else if (!string.IsNullOrEmpty(vbControl.VBContent) && CurrentLabOrder != null && !_IsNewLabOrderDialogOpen)
            {
                if (vbControl.VBContent.StartsWith("CurrentLabOrder\\InOrderPos"))
                    if (CurrentLabOrder.InOrderPos != null)
                        result = Global.ControlModes.Disabled;
                    else
                        result = Global.ControlModes.Hidden;

                else if (vbControl.VBContent.StartsWith("CurrentLabOrder\\OutOrderPos"))
                    if (CurrentLabOrder.OutOrderPos != null)
                        result = Global.ControlModes.Disabled;
                    else
                        result = Global.ControlModes.Hidden;

                else if (vbControl.VBContent.StartsWith("CurrentLabOrder\\ProdOrderPartslistPos"))
                    if (CurrentLabOrder.ProdOrderPartslistPos != null)
                        result = Global.ControlModes.Disabled;
                    else
                        result = Global.ControlModes.Hidden;

                else if (vbControl.VBContent.StartsWith("CurrentLabOrder\\FacilityLot"))
                    if (CurrentLabOrder.FacilityLot != null)
                        result = Global.ControlModes.Disabled;
                    else
                        result = Global.ControlModes.Hidden;
            }
            else if (vbControl.VBContent != null && CurrentLabOrder == null && !_IsNewLabOrderDialogOpen)
            {
                if (vbControl.VBContent.StartsWith("CurrentLabOrder\\InOrderPos") || vbControl.VBContent.StartsWith("CurrentLabOrder\\OutOrderPos") ||
                    vbControl.VBContent.StartsWith("CurrentLabOrder\\ProdOrderPartslistPos") || vbControl.VBContent.StartsWith("CurrentLabOrder\\FacilityLot"))
                    result = Global.ControlModes.Hidden;
            }

            if (!string.IsNullOrEmpty(vbControl.VBContent) && vbControl.VBContent.StartsWith("DialogSelectedTemplate"))
            {
                result = Global.ControlModes.EnabledRequired;
            }

            if (!_IsMaterialStateEnabled && !string.IsNullOrEmpty(vbControl.VBContent) && vbControl.VBContent.StartsWith("LabOrderMaterialState"))
                result = Global.ControlModes.Disabled;

            if (isCurrentPosInChange && vbControl.VBContent == @"CurrentLabOrderPos\ActualValue")
            {
                // @aagincic: i don't know exist bether VB way to doing this (focusing ActualValue by selection new pos param)
                MethodInfo mth = vbControl.GetType().GetMethod("Focus");
                if (mth != null)
                    mth.Invoke(vbControl, null);
                isCurrentPosInChange = false;
            }

            return result;
        }

        private void SetPosToNull()
        {
            CurrentLabOrder.InOrderPos = null;
            CurrentLabOrder.OutOrderPos = null;
            CurrentLabOrder.ProdOrderPartslistPos = null;
            CurrentLabOrder.FacilityLot = null;
        }

        public override void SetCurrentSelected(LabOrder value)
        {
            if (AccessPrimary == null)
                return;
            if (value != CurrentLabOrder)
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentLabOrder");
                OnPropertyChanged("LabOrderPosList");
                _LabOrderPosItemAVGList = null;
                OnPropertyChanged("LabOrderPosItemAVGList");
                ChangeLabOrderTemplateNoName();
            }
            if (value != SelectedLabOrder)
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedLabOrder");
                OnPropertyChanged("LabOrderPosList");
                ChangeLabOrderTemplateNoName();
            }
        }

        /// <summary>
        /// Crates a new laboratory order in dialog.
        /// </summary>
        /// <param name="inOrderPos">The in order position.</param>
        /// <param name="outOrderPos">The out order position.</param>
        /// <param name="prodOrderPartslistPos">The production order partslist position</param>
        /// <param name="facilityLot">The facility lot.</param>
        /// <returns>The result in <see cref="VBDialogResult"/> object</returns>
        /// <summary xml:lang="de">
        /// Crates a new laboratory order in dialog.
        /// </summary>
        /// <param xml:lang="de" name="inOrderPos">The in order position.</param>
        /// <param xml:lang="de" name="outOrderPos">The out order position.</param>
        /// <param xml:lang="de" name="prodOrderPartslistPos">The production order partslist position</param>
        /// <param xml:lang="de" name="facilityLot">The facility lot.</param>
        /// <returns xml:lang="de">The result in <see cref="VBDialogResult"/> object</returns>
        [ACMethodInfo("Dialog", "en{'New Lab Order'}de{'Neuer Laborauftrag'}", 701)]
        public VBDialogResult NewLabOrderDialog(DeliveryNotePos inOrderPos, DeliveryNotePos outOrderPos, ProdOrderPartslistPos prodOrderPartslistPos, FacilityLot facilityLot)
        {
            if (DialogResult == null)
                DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            if (inOrderPos == null && outOrderPos == null && prodOrderPartslistPos == null && facilityLot == null)
            {
                _IsMaterialStateEnabled = true;
            }
            else
                FilterDialog(inOrderPos != null ? inOrderPos.InOrderPos : null, outOrderPos != null ? outOrderPos.OutOrderPos : null, prodOrderPartslistPos, facilityLot, null, null);

            base.New();
            CurrentLabOrder.MDLabOrderState = DatabaseApp.MDLabOrderState.FirstOrDefault(c => c.IsDefault);
            if (inOrderPos != null)
            {
                _IsMaterialStateEnabled = false;
                _LabOrderMaterialState = LabOrderMaterialStateList.FirstOrDefault(c => c.Value.ToString() == InOrderPos.ClassName);
                CurrentLabOrder.InOrderPosID = inOrderPos.InOrderPosID;
            }

            if (outOrderPos != null)
            {
                _IsMaterialStateEnabled = false;
                _LabOrderMaterialState = LabOrderMaterialStateList.FirstOrDefault(c => c.Value.ToString() == OutOrderPos.ClassName);
                CurrentLabOrder.OutOrderPosID = outOrderPos.OutOrderPosID;
            }

            if (prodOrderPartslistPos != null)
            {
                _IsMaterialStateEnabled = false;
                _LabOrderMaterialState = LabOrderMaterialStateList.FirstOrDefault(c => c.Value.ToString() == PartslistPos.ClassName);
                CurrentLabOrder.ProdOrderPartslistPos = prodOrderPartslistPos;
            }

            if (facilityLot != null)
            {
                _IsMaterialStateEnabled = false;
                _LabOrderMaterialState = LabOrderMaterialStateList.FirstOrDefault(c => c.Value.ToString() == "LotCharge");
                CurrentLabOrder.FacilityLot = facilityLot;
            }

            _IsNewLabOrderDialogOpen = true;
            //do not change DialogTemplateList.Count() == 1 to DialogTemplateList.Any() if exist more than one template user must select template manually
            if (DialogTemplateList != null && DialogTemplateList.Count() == 1)
            {
                _DialogSelectedTemplate = DialogTemplateList.FirstOrDefault();
                DialogCreatePos();
            }
            else
            {
                ShowDialog(this, "LabOrderDialog");
                if (DialogResult.SelectedCommand != eMsgButton.OK)
                {
                    base.UndoSave();
                    _IsNewLabOrderDialogOpen = false;
                    Search();
                }
            }
            return DialogResult;
        }

        /// <summary>
        /// Creates a positions in the laboratory order.
        /// </summary>
        /// <summary xml:lang="de">
        /// Legt eine Planstelle im Laborauftrag an.
        /// </summary>
        [ACMethodInteraction("Dialog", "en{'Create'}de{'Generieren'}", 702, true, "DialogCreatePos", Global.ACKinds.MSMethodPrePost)]
        public void DialogCreatePos()
        {
            if (CurrentLabOrder.Material == null)
            {
                Messages.Warning(this, "Warning50002");
                return;
            }
            if (_DialogSelectedTemplate == null && !_IsMaterialStateEnabled)
            {
                Messages.Warning(this, "Warning50003");
                return;
            }
            DialogResult.SelectedCommand = eMsgButton.OK;
            LabOrderManager.CopyLabOrderTemplatePos(DatabaseApp, CurrentLabOrder, _DialogSelectedTemplate);
            OnLaborderCopied(DatabaseApp, CurrentLabOrder, _DialogSelectedTemplate);
            _IsNewLabOrderDialogOpen = false;
            CloseTopDialog();
            _IsLabOrderPosViewDialog = false;
            if (!IsLabOrderParent)
            {
                if (CurrentLabOrder.InOrderPos != null)
                    ShowLabOrderViewDialog(CurrentLabOrder.InOrderPos, null, null, null, null, CurrentLabOrder.EntityState != System.Data.EntityState.Added, null);
                else if (CurrentLabOrder.OutOrderPos != null)
                    ShowLabOrderViewDialog(null, CurrentLabOrder.OutOrderPos, null, null, null, CurrentLabOrder.EntityState != System.Data.EntityState.Added, null);
                else if (CurrentLabOrder.ProdOrderPartslistPos != null)
                    ShowLabOrderViewDialog(null, null, CurrentLabOrder.ProdOrderPartslistPos, null, null, CurrentLabOrder.EntityState != System.Data.EntityState.Added, null);
                else if (CurrentLabOrder.FacilityLot != null)
                    ShowLabOrderViewDialog(null, null, null, CurrentLabOrder.FacilityLot, null, CurrentLabOrder.EntityState != System.Data.EntityState.Added, null);
                else
                    ShowLabOrderViewDialog(null, null, null, null, null, CurrentLabOrder.EntityState != System.Data.EntityState.Added, null);
            }
            else
            {
                Save();
                base.OnPropertyChanged("LabOrderPosList");
                ChangeLabOrderTemplateNoName();
            }
        }

        protected virtual void OnLaborderCopied(DatabaseApp dbApp, LabOrder current, LabOrder template)
        {
        }

        /// <summary>
        /// Canceles the creation of a positions.
        /// </summary>
        /// <summary xml:lang="de">
        /// Bricht das Anlegen einer Planstelle ab.
        /// </summary>
        [ACMethodInteraction("Dialog", "en{'Cancel'}de{'Abbrechen'}", 703, true, "DialogCancelPos", Global.ACKinds.MSMethodPrePost)]
        public void DialogCancelPos()
        {
            CloseTopDialog();
        }

        /// <summary>
        /// Shows the laboratory order in a dialog.
        /// </summary>
        /// <param name="inOrderPos">The in order position.</param>
        /// <param name="outOrderPos">The out order position.</param>
        /// <param name="prodOrderPartslistPos">The production order partslist position.</param>
        /// <param name="facilityLot">The facility lot.</param>
        /// <param name="labOrder">The laboratory order.</param>
        /// <param name="filter">The filter, enables or disables filter.</param>
        /// <param name="orderInfo"> The PAOrderInfo.</param>
        /// <summary xml:lang="de">
        /// Zeigt den Laborauftrag in einem Dialog an.
        /// </summary>
        /// <param xml:lang="de" name="inOrderPos">Die in der Reihenfolge.</param>
        /// <param xml:lang="de" name="outOrderPos">Die Out-Order-Position.</param>
        /// <param xml:lang="de" name="prodOrderPartslistPos">Die Position der Fertigungsauftragsstückliste.</param>
        /// <param xml:lang="de" name="facilityLot">Das Grundstück der Einrichtung.</param>
        /// <param xml:lang="de" name="labOrder">Der Laborauftrag.</param>
        /// <param xml:lang="de" name="filter">Der Filter, aktiviert oder deaktiviert den Filter.</param>
        /// <param xml:lang="de" name="orderInfo">Die PAOrderInfo.</param>
        [ACMethodInteraction("Dialog", "en{'View Lab Order'}de{'Laborauftrag anzeigen'}", 704, true, "ShowLabOrderViewDialog", Global.ACKinds.MSMethodPrePost)]
        public void ShowLabOrderViewDialog(
            InOrderPos inOrderPos,
            OutOrderPos outOrderPos,
            ProdOrderPartslistPos prodOrderPartslistPos,
            FacilityLot facilityLot,
            LabOrder labOrder,
            bool filter,
            PAOrderInfo orderInfo)
        {
            if (filter)
                FilterDialog(inOrderPos, outOrderPos, prodOrderPartslistPos, facilityLot, labOrder, orderInfo);

            if (inOrderPos == null && outOrderPos == null && prodOrderPartslistPos == null && facilityLot == null)
            {
                ACComponent childBSO = ACUrlCommand("?LabOrderViewDialog") as ACComponent;
                if (childBSO == null)
                    childBSO = StartComponent("LabOrderViewDialog", null, new object[] { }) as ACComponent;
            }
            ShowDialog(this, "LabOrderViewDialog");
            Save();
            CloseTopDialog();
            this.ParentACComponent.StopComponent(this);
        }

        protected void FilterDialog(
            InOrderPos inOrderPos,
            OutOrderPos outOrderPos,
            ProdOrderPartslistPos prodOrderPartslistPos,
            FacilityLot facilityLot,
            LabOrder labOrder,
            PAOrderInfo orderInfo)
        {
            if (AccessPrimary == null)
                return;
            if (orderInfo != null && inOrderPos == null && outOrderPos == null && prodOrderPartslistPos == null && facilityLot == null && labOrder == null)
            {
                foreach (var entry in orderInfo.Entities)
                {
                    if (entry.EntityName == ProdOrderPartslistPosRelation.ClassName)
                    {
                        ProdOrderPartslistPosRelation relation = this.DatabaseApp.ProdOrderPartslistPosRelation
                            .Include(c => c.TargetProdOrderPartslistPos)
                            .Include(c => c.TargetProdOrderPartslistPos.ProdOrderPartslist)
                            .Include(c => c.TargetProdOrderPartslistPos.ProdOrderPartslist.ProdOrder)
                            .Where(c => c.ProdOrderPartslistPosRelationID == entry.EntityID)
                            .FirstOrDefault();
                        if (relation != null)
                        {
                            prodOrderPartslistPos = relation.TargetProdOrderPartslistPos;
                            break;
                        }
                    }
                    else if (entry.EntityName == ProdOrderBatch.ClassName)
                    {
                        ProdOrderBatch batch = this.DatabaseApp.ProdOrderBatch
                            .Include(c => c.ProdOrderPartslistPos_ProdOrderBatch)
                            .Include(c => c.ProdOrderPartslist)
                            .Include(c => c.ProdOrderPartslist.ProdOrder)
                            .Where(c => c.ProdOrderBatchID == entry.EntityID).FirstOrDefault();
                        if (batch != null)
                        {
                            prodOrderPartslistPos = batch.ProdOrderPartslistPos_ProdOrderBatch.ToArray().Where(c => c.IsFinalMixureBatch).FirstOrDefault();
                            if (prodOrderPartslistPos != null)
                                break;
                        }
                    }
                    else if (entry.EntityName == DeliveryNotePos.ClassName)
                    {
                        DeliveryNotePos dnPos = this.DatabaseApp.DeliveryNotePos
                            .Include(c => c.DeliveryNote)
                            .Where(c => c.DeliveryNotePosID == entry.EntityID)
                            .FirstOrDefault();
                        if (dnPos != null)
                        {
                            inOrderPos = dnPos.InOrderPos;
                            outOrderPos = dnPos.OutOrderPos;
                        }
                    }
                }
            }

            if (inOrderPos != null)
            {
                ACFilterItem filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "OutOrderPos\\OutOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "ProdOrderPartslistPos\\ProdOrderPartslistPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "LabOrderID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);

                _IsLabOrderPosViewDialog = true;
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "InOrderPos\\InOrderPosID").FirstOrDefault();
                if (filterItem == null)
                {
                    filterItem = new ACFilterItem(Global.FilterTypes.filter, "InOrderPos\\InOrderPosID", Global.LogicalOperators.equal, Global.Operators.and, inOrderPos.InOrderPosID.ToString(), false);
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);

                }
                else
                    filterItem.SearchWord = inOrderPos.InOrderPosID.ToString();
                this.Search();
            }
            else if (outOrderPos != null)
            {
                ACFilterItem filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "InOrderPos\\InOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "ProdOrderPartslistPos\\ProdOrderPartslistPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "LabOrderID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);

                _IsLabOrderPosViewDialog = true;
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "OutOrderPos\\OutOrderPosID").FirstOrDefault();
                if (filterItem == null)
                {
                    filterItem = new ACFilterItem(Global.FilterTypes.filter, "OutOrderPos\\OutOrderPosID", Global.LogicalOperators.equal, Global.Operators.and, outOrderPos.OutOrderPosID.ToString(), false);
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
                }
                else
                    filterItem.SearchWord = outOrderPos.OutOrderPosID.ToString();
                this.Search();
            }
            else if (prodOrderPartslistPos != null)
            {
                ACFilterItem filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "InOrderPos\\InOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "OutOrderPos\\OutOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "LabOrderID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);

                _IsLabOrderPosViewDialog = true;
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "ProdOrderPartslistPos\\ProdOrderPartslistPosID").FirstOrDefault();
                if (filterItem == null)
                {
                    filterItem = new ACFilterItem(Global.FilterTypes.filter, "ProdOrderPartslistPos\\ProdOrderPartslistPosID", Global.LogicalOperators.equal, Global.Operators.and, prodOrderPartslistPos.ProdOrderPartslistPosID.ToString(), false);
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
                }
                else
                    filterItem.SearchWord = prodOrderPartslistPos.ProdOrderPartslistPosID.ToString();
                this.Search();
            }
            else if (labOrder != null)
            {
                ACFilterItem filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "InOrderPos\\InOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "OutOrderPos\\OutOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "ProdOrderPartslistPos\\ProdOrderPartslistPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);

                _IsLabOrderPosViewDialog = true;
                ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "LabOrderID").FirstOrDefault();
                if (filterItem == null)
                {
                    filterItem = new ACFilterItem(Global.FilterTypes.filter, "LabOrderID", Global.LogicalOperators.equal, Global.Operators.and, labOrder.LabOrderID.ToString(), false);
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
                }
                else
                    filterItem.SearchWord = labOrder.LabOrderID.ToString();
                this.Search();
            }
            else
            {
                ACFilterItem filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "InOrderPos\\InOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "OutOrderPos\\OutOrderPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "ProdOrderPartslistPos\\ProdOrderPartslistPosID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
                filterItemExits = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "LabOrderID").FirstOrDefault();
                if (filterItemExits != null)
                    AccessPrimary.NavACQueryDefinition.ACFilterColumns.Remove(filterItemExits);
            }
        }

        /// <summary>
        /// Closes the laboratory order dialog.
        /// </summary>
        /// <summary xml:lang="de">
        /// Schließt den Dialog Laborauftrag.
        /// </summary>
        [ACMethodInteraction("Dialog", "en{'Close'}de{'Schließen'}", 705, true, "CloseLabOrderViewDialog", Global.ACKinds.MSMethodPrePost)]
        public void CloseLabOrderViewDialog()
        {
            Save();
            CloseTopDialog();
            base.OnPropertyChanged("LabOrderPosList");
            ChangeLabOrderTemplateNoName();
        }

        public override bool IsEnabledNewLabOrderPos()
        {
            //if (_IsLabOrderPosViewDialog)
            //    return false;
            return base.IsEnabledNewLabOrderPos();
        }

        public override bool IsEnabledDeleteLabOrderPos()
        {
            if (_IsLabOrderPosViewDialog)
                return false;
            return base.IsEnabledDeleteLabOrderPos();
        }

        public void ChangeLabOrderTemplateNoName()
        {
            if (CurrentLabOrder != null)
                _LabOrderTemplate = DatabaseApp.LabOrder.FirstOrDefault(c => c.LabOrderID == CurrentLabOrder.BasedOnTemplateID && c.LabOrderTypeIndex == (short)GlobalApp.LabOrderType.Template);
            else
                _LabOrderTemplate = null;
            OnPropertyChanged("LabOrderTemplateNo");
            OnPropertyChanged("LabOrderTemplateName");
        }

        public override void New()
        {
            NewLabOrderDialog(null, null, null, null);
        }

        /// <summary>
        /// Searches a laboratory orders.
        /// </summary>
        /// <summary xml:lang="de">
        /// Durchsucht einen Laborauftrag.
        /// </summary>
        [ACMethodCommand("LabOrder", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public override void Search()
        {
            base.Search();
            _LabOrderPosAVGList = null;
            OnPropertyChanged("LabOrderPosAVGList");
        }
        #endregion

        #region LabOrderPosItemAVG

        private LabOrderPos _SelectedLabOrderPosItemAVG;
        /// <summary>
        /// Cumulative AVG LabOrderPos for entire LabOrder list
        /// </summary>
        /// <value>The selected PropertyName</value>
        /// <summary xml:lang="de">
        /// Kumulative AVG LabOrderPos für die gesamte LabOrder-Liste
        /// </summary>
        /// <value xml:lang="de">Der ausgewählte PropertyName</value>
        [ACPropertySelected(720, "LabOrderPosItemAVG", "en{'TODO: PropertyName'}de{'TODO: PropertyName'}")]
        public LabOrderPos SelectedLabOrderPosItemAVG
        {
            get
            {
                return _SelectedLabOrderPosItemAVG;
            }
            set
            {
                if (_SelectedLabOrderPosItemAVG != value)
                {
                    _SelectedLabOrderPosItemAVG = value;
                    OnPropertyChanged("SelectedLabOrderPosItemAVG");
                }
            }
        }

        private IEnumerable<LabOrderPos> _LabOrderPosItemAVGList;
        /// <summary>
        /// List property for LabOrderPos
        /// </summary>
        /// <value>The PropertyName list</value>
        /// <summary xml:lang="de">
        /// List Eigenschaft für LabOrderPos
        /// </summary>
        /// <value xml:lang="de">Die PropertyName-Liste</value>
        [ACPropertyList(721, "LabOrderPosItemAVG")]
        public IEnumerable<LabOrderPos> LabOrderPosItemAVGList
        {
            get
            {
                if (_LabOrderPosItemAVGList == null)
                {
                    LoadLabOrderPosItemAVG();
                }
                return _LabOrderPosItemAVGList;
            }
        }

        private void LoadLabOrderPosItemAVG()
        {

            if (CurrentLabOrder != null && CurrentLabOrder.LabOrderPos_LabOrder.Any())
            {
                _LabOrderPosItemAVGList =
                    CurrentLabOrder
                    .LabOrderPos_LabOrder
                    .GroupBy(x => new { MDKey = x.MDLabTag.MDKey, MDNameTrans = x.MDLabTag.MDNameTrans })
                    .OrderBy(x => x.Key.MDKey)
                    .Select(x => new LabOrderPos
                    {
                        LabOrderPosID = Guid.NewGuid(),
                        MDLabTag = new MDLabTag() { MDKey = x.Key.MDKey, MDNameTrans = x.Key.MDNameTrans },
                        ReferenceValue = x.Average(p => p.ReferenceValue ?? 0),
                        ActualValue = x.Average(p => p.ActualValue ?? 0),
                        ValueMax = x.Average(p => p.ValueMax ?? 0),
                        ValueMaxMax = x.Average(p => p.ValueMaxMax ?? 0),
                        ValueMin = x.Average(p => p.ValueMin ?? 0),
                        ValueMinMin = x.Average(p => p.ValueMinMin ?? 0)
                    });
                SelectedLabOrderPosItemAVG = _LabOrderPosItemAVGList.FirstOrDefault();
            }
            else
            {
                SelectedLabOrderPosItemAVG = null;
            }
        }

        #endregion

        #region LabOrderPosAVG

        private LabOrderPos _SelectedLabOrderPosAVG;
        /// <summary>
        /// Cumulative AVG LabOrderPos for entire LabOrder list
        /// </summary>
        /// <value>The selected PropertyName</value>
        /// <summary xml:lang="de">
        /// Kumulative AVG LabOrderPos für die gesamte LabOrder-Liste
        /// </summary>
        /// <value xml:lang="de">Der ausgewählte PropertyNam</value>
        [ACPropertySelected(722, "LabOrderPosAVG", "en{'TODO: PropertyName'}de{'TODO: PropertyName'}")]
        public LabOrderPos SelectedLabOrderPosAVG
        {
            get
            {
                return _SelectedLabOrderPosAVG;
            }
            set
            {
                if (_SelectedLabOrderPosAVG != value)
                {
                    _SelectedLabOrderPosAVG = value;
                    OnPropertyChanged("SelectedLabOrderPosAVG");
                }
            }
        }

        private IEnumerable<LabOrderPos> _LabOrderPosAVGList;
        /// <summary>
        /// List property for LabOrderPos
        /// </summary>
        /// <value>The PropertyName list</value>
        /// <summary xml:lang="de">
        /// List Eigenschaft für LabOrderPos
        /// </summary>
        /// <value xml:lang="de">Die PropertyName-Liste</value>
        [ACPropertyList(723, "LabOrderPosAVG")]
        public IEnumerable<LabOrderPos> LabOrderPosAVGList
        {
            get
            {
                if (_LabOrderPosAVGList == null)
                {
                    LoadLabOrderPosAVGList();
                }
                return _LabOrderPosAVGList;
            }
        }

        private void LoadLabOrderPosAVGList()
        {

            if (LabOrderList != null && LabOrderList.Any())
            {
                _LabOrderPosAVGList =
                    LabOrderList
                    .SelectMany(x => x.LabOrderPos_LabOrder)
                    .GroupBy(x => new { MDKey = x.MDLabTag.MDKey, MDNameTrans = x.MDLabTag.MDNameTrans })
                    .OrderBy(x => x.Key.MDKey)
                    .Select(x => new LabOrderPos
                    {
                        LabOrderPosID = Guid.NewGuid(),
                        MDLabTag = new MDLabTag() { MDKey = x.Key.MDKey, MDNameTrans = x.Key.MDNameTrans },
                        ReferenceValue = x.Average(p => p.ReferenceValue ?? 0),
                        ActualValue = x.Average(p => p.ActualValue ?? 0),
                        ValueMax = x.Average(p => p.ValueMax ?? 0),
                        ValueMaxMax = x.Average(p => p.ValueMaxMax ?? 0),
                        ValueMin = x.Average(p => p.ValueMin ?? 0),
                        ValueMinMin = x.Average(p => p.ValueMinMin ?? 0)
                    });
                SelectedLabOrderPosAVG = _LabOrderPosAVGList.FirstOrDefault();
            }
            else
            {
                SelectedLabOrderPosAVG = null;
            }
        }

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "NewLabOrderDialog":
                    result = NewLabOrderDialog((DeliveryNotePos)acParameter[0], (DeliveryNotePos)acParameter[1], (ProdOrderPartslistPos)acParameter[2], (FacilityLot)acParameter[3]);
                    return true;
                case "DialogCreatePos":
                    DialogCreatePos();
                    return true;
                case "DialogCancelPos":
                    DialogCancelPos();
                    return true;
                case "ShowLabOrderViewDialog":
                    ShowLabOrderViewDialog(acParameter[0] as InOrderPos, acParameter[1] as OutOrderPos, acParameter[2] as ProdOrderPartslistPos, acParameter[3] as FacilityLot, acParameter[4] as LabOrder, (Boolean)acParameter[5], acParameter[6] as PAOrderInfo);
                    return true;
                case "CloseLabOrderViewDialog":
                    CloseLabOrderViewDialog();
                    return true;
                case "IsEnabledNewLabOrderPos":
                    result = IsEnabledNewLabOrderPos();
                    return true;
                case "IsEnabledDeleteLabOrderPos":
                    result = IsEnabledDeleteLabOrderPos();
                    return true;
                case "Search":
                    Search();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
