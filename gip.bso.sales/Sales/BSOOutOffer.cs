using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.reporthandler.Flowdoc;

namespace gip.bso.sales
{
    /// <summary>
    /// Version 3
    /// 
    /// Neue Masken:
    /// 1. Angebotsverwaltung
    /// 
    /// TODO: Betroffene Tabellen: OutOffer, OutOfferPos
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Offering'}de{'Angebot'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOffer.ClassName)]
    public class BSOOutOffer : ACBSOvbNav
    {
        #region c´tors

        public BSOOutOffer(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            TempReportData = new ReportData();

            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._AccessOutOfferPos = null;
            this._CurrentOutOfferPos = null;
            this._SelectedOutOfferPos = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessOutOfferPos != null)
            {
                _AccessOutOfferPos.ACDeInit(false);
                _AccessOutOfferPos = null;
            }
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        ACAccessNav<OutOffer> _AccessPrimary;
        [ACPropertyAccessPrimary(690, OutOffer.ClassName)]
        public ACAccessNav<OutOffer> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<OutOffer>(OutOffer.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        [ACPropertyCurrent(600, OutOffer.ClassName)]
        public OutOffer CurrentOutOffer
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (CurrentOutOffer != null)
                {
                    CurrentOutOffer.PropertyChanged -= CurrentOutOffer_PropertyChanged;
                }
                if (AccessPrimary == null) return; AccessPrimary.Current = value;

                if (CurrentOutOffer != null)
                {
                    CurrentOutOffer.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentOutOffer_PropertyChanged);
                }
                CurrentOutOfferPos = null;

                CurrentOutOfferPos = OutOfferPosList.FirstOrDefault();
                OnPropertyChanged("CurrentOutOffer");
                OnPropertyChanged("OutOfferPosList");
                OnPropertyChanged("CompanyList");
                OnPropertyChanged("BillingCompanyAddressList");
                OnPropertyChanged("DeliveryCompanyAddressList");
                OnPropertyChanged("CurrentBillingCompanyAddress");
                OnPropertyChanged("CurrentDeliveryCompanyAddress");
            }
        }

        void CurrentOutOffer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CustomerCompanyID":
                    if (CurrentOutOffer.CustomerCompany != null)
                    {
                        CurrentOutOffer.BillingCompanyAddress = CurrentOutOffer.CustomerCompany.BillingCompanyAddress;
                        CurrentOutOffer.DeliveryCompanyAddress = CurrentOutOffer.CustomerCompany.DeliveryCompanyAddress;
                    }
                    else
                    {
                        CurrentOutOffer.BillingCompanyAddress = null;
                        CurrentOutOffer.DeliveryCompanyAddress = null;
                    }
                    OnPropertyChanged("BillingCompanyAddressList");
                    OnPropertyChanged("DeliveryCompanyAddressList");
                    OnPropertyChanged("CurrentBillingCompanyAddress");
                    OnPropertyChanged("CurrentDeliveryCompanyAddress");

                    break;
            }
        }

        [ACPropertyList(601, OutOffer.ClassName)]
        public IEnumerable<OutOffer> OutOfferList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, OutOffer.ClassName)]
        public OutOffer SelectedOutOffer
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedOutOffer");
            }
        }

        ACAccess<OutOfferPos> _AccessOutOfferPos;
        [ACPropertyAccess(603, "OutOfferPos")]
        public ACAccess<OutOfferPos> AccessOutOfferPos
        {
            get
            {
                if (_AccessOutOfferPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + OutOfferPos.ClassName) as ACQueryDefinition;
                    _AccessOutOfferPos = acQueryDefinition.NewAccess<OutOfferPos>("OutOfferPos", this);
                }
                return _AccessOutOfferPos;
            }
        }

        OutOfferPos _CurrentOutOfferPos;
        [ACPropertyCurrent(604, "OutOfferPos")]
        public OutOfferPos CurrentOutOfferPos
        {
            get
            {
                return _CurrentOutOfferPos;
            }
            set
            {
                if (_CurrentOutOfferPos != null)
                    _CurrentOutOfferPos.PropertyChanged -= CurrentOutOfferPos_PropertyChanged;
                _CurrentOutOfferPos = value;
                if (_CurrentOutOfferPos != null)
                    _CurrentOutOfferPos.PropertyChanged += CurrentOutOfferPos_PropertyChanged;
                OnPropertyChanged("MDUnitList");
                OnPropertyChanged("CurrentOutOfferPos");
                OnPropertyChanged("CurrentMDUnit");
            }
        }

        void CurrentOutOfferPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MaterialID":
                    {
                        OnPropertyChanged("MDUnitList");
                        if (CurrentOutOfferPos.Material != null && CurrentOutOfferPos.Material.BaseMDUnit != null)
                            CurrentMDUnit = CurrentOutOfferPos.Material.BaseMDUnit;
                        else
                            CurrentMDUnit = null;
                        OnPropertyChanged("CurrentOutOfferPos");
                    }
                    break;
                case "TargetQuantityUOM":
                case "MDUnitID":
                    {
                        CurrentOutOfferPos.TargetQuantity = CurrentOutOfferPos.Material.ConvertToBaseQuantity(CurrentOutOfferPos.TargetQuantityUOM, CurrentOutOfferPos.MDUnit);
                        CurrentOutOfferPos.TargetWeight = CurrentOutOfferPos.Material.ConvertToBaseWeight(CurrentOutOfferPos.TargetQuantityUOM, CurrentOutOfferPos.MDUnit);
                    }
                    break;
            }
        }

        [ACPropertyList(605, "OutOfferPos")]
        public IEnumerable<OutOfferPos> OutOfferPosList
        {
            get
            {
                return CurrentOutOffer?.OutOfferPos_OutOffer.OrderBy(c => c.Position);
            }
        }

        OutOfferPos _SelectedOutOfferPos;
        [ACPropertySelected(606, "OutOfferPos")]
        public OutOfferPos SelectedOutOfferPos
        {
            get
            {
                return _SelectedOutOfferPos;
            }
            set
            {
                _SelectedOutOfferPos = value;
                OnPropertyChanged("SelectedOutOfferPos");
            }
        }

        [ACPropertyList(607, MDUnit.ClassName)]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                if (CurrentOutOfferPos == null || CurrentOutOfferPos.Material == null)
                    return null;
                return CurrentOutOfferPos.Material.MDUnitList;
            }
        }

        [ACPropertyCurrent(608, MDUnit.ClassName)]
        public MDUnit CurrentMDUnit
        {
            get
            {
                if (CurrentOutOfferPos == null)
                    return null;
                return CurrentOutOfferPos.MDUnit;
            }
            set
            {
                if (CurrentOutOfferPos != null && value != null)
                {
                    CurrentOutOfferPos.MDUnit = value;
                    OnPropertyChanged("CurrentMDUnit");
                }
            }
        }

        /// <summary>
        /// Liste aller Unternehmen, die Lieferanten sind
        /// </summary>
        [ACPropertyList(609, Company.ClassName)]
        public IEnumerable<Company> CompanyList
        {
            get
            {
                return from c in DatabaseApp.Company where c.IsCustomer orderby c.CompanyName select c;
            }
        }

        [ACPropertyList(610, "BillingCompanyAddress")]
        public IEnumerable<CompanyAddress> BillingCompanyAddressList
        {
            get
            {
                if (CurrentOutOffer == null || CurrentOutOffer.CustomerCompany == null)
                    return null;
                if (!CurrentOutOffer.CustomerCompany.CompanyAddress_Company.IsLoaded)
                    CurrentOutOffer.CustomerCompany.CompanyAddress_Company.Load();
                return from c in CurrentOutOffer.CustomerCompany.CompanyAddress_Company
                       where c.IsHouseCompanyAddress
                       orderby c.Name1
                       select c;
            }
        }

        [ACPropertyList(611, "DeliveryCompanyAddress")]
        public IEnumerable<CompanyAddress> DeliveryCompanyAddressList
        {
            get
            {
                if (CurrentOutOffer == null || CurrentOutOffer.CustomerCompany == null)
                    return null;
                if (!CurrentOutOffer.CustomerCompany.CompanyAddress_Company.IsLoaded)
                    CurrentOutOffer.CustomerCompany.CompanyAddress_Company.Load();
                return from c in CurrentOutOffer.CustomerCompany.CompanyAddress_Company
                       where c.IsDeliveryCompanyAddress
                       orderby c.Name1
                       select c;
            }
        }

        [ACPropertyCurrent(612, "BillingCompanyAddress")]
        public CompanyAddress CurrentBillingCompanyAddress
        {
            get
            {
                if (CurrentOutOffer == null)
                    return null;
                return CurrentOutOffer.BillingCompanyAddress;
            }
        }

        [ACPropertyCurrent(613, "DeliveryCompanyAddress")]
        public CompanyAddress CurrentDeliveryCompanyAddress
        {
            get
            {
                if (CurrentOutOffer == null)
                    return null;
                return CurrentOutOffer.DeliveryCompanyAddress;
            }
        }

        private ReportData _TempReportData;
        [ACPropertyInfo(9999)]
        public ReportData TempReportData
        {
            get
            {
                return _TempReportData;
            }
            set
            {
                _TempReportData = value;
                OnPropertyChanged("TempReportData");
            }
        }

        [ACPropertyInfo(650)]
        public List<OutOfferPosData> OutOfferPosDataList
        {
            get;
            set;
        }

        #endregion

        #region BSO->ACMethod

        [ACMethodCommand(OutOffer.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(OutOffer.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction(OutOffer.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedOutOffer", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<OutOffer>(requery, () => SelectedOutOffer, () => CurrentOutOffer, c => CurrentOutOffer = c,
                        DatabaseApp.OutOffer
                        .Include(c => c.OutOfferPos_OutOffer)
                        .Where(c => c.OutOfferID == SelectedOutOffer.OutOfferID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedOutOffer != null;
        }

        [ACMethodInteraction(OutOffer.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedOutOffer", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(OutOffer), OutOffer.NoColumnName, OutOffer.FormatNewNo, this);
            CurrentOutOffer = OutOffer.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.OutOffer.AddObject(CurrentOutOffer);

            ACState = Const.SMNew;
            PostExecute("New");

        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction(OutOffer.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentOutOffer", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentOutOffer.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentOutOffer);
            SelectedOutOffer = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        public bool IsEnabledDelete()
        {
            return true;
        }

        [ACMethodCommand(OutOffer.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("OutOfferList");
        }

        [ACMethodInteraction("OutOfferPos", "en{'Load Position'}de{'Position laden'}", (short)MISort.Load, false, "SelectedOutOfferPos", Global.ACKinds.MSMethodPrePost)]
        public void LoadOutOfferPos()
        {
            if (!IsEnabledLoadOutOfferPos())
                return;
            if (!PreExecute("LoadOutOfferPos")) return;
            // Laden des aktuell selektierten OutOfferPos 
            CurrentOutOfferPos = (from c in CurrentOutOffer.OutOfferPos_OutOffer
                                     where c.OutOfferPosID == SelectedOutOfferPos.OutOfferPosID
                                     select c).First();
            PostExecute("LoadOutOfferPos");
        }

        public bool IsEnabledLoadOutOfferPos()
        {
            return SelectedOutOfferPos != null && CurrentOutOffer != null;
        }

        [ACMethodInteraction("OutOfferPos", "en{'New Position'}de{'Neue Position'}", (short)MISort.New, true, "SelectedOutOfferPos", Global.ACKinds.MSMethodPrePost)]
        public void NewOutOfferPos()
        {
            if (!PreExecute("NewOutOfferPos")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            CurrentOutOfferPos = OutOfferPos.NewACObject(DatabaseApp, CurrentOutOffer, null);
            CurrentOutOfferPos.OutOffer = CurrentOutOffer;
            CurrentOutOffer.OutOfferPos_OutOffer.Add(CurrentOutOfferPos);
            OnPropertyChanged("OutOfferPosList");
            PostExecute("NewOutOfferPos");
        }

        public bool IsEnabledNewOutOfferPos()
        {
            return CurrentOutOffer != null;
        }

        [ACMethodInteraction("OutOfferPos", "en{'New sub Position'}de{'Neue sub Position'}", (short)MISort.New, true, "SelectedOutOfferPos", Global.ACKinds.MSMethodPrePost)]
        public void NewSubOutOfferPos()
        {
            if (!PreExecute("NewOutOfferPos")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            OutOfferPos subOutOfferPos = OutOfferPos.NewACObject(DatabaseApp, CurrentOutOffer, CurrentOutOfferPos);
            subOutOfferPos.OutOfferPos1_GroupOutOfferPos = CurrentOutOfferPos;
            subOutOfferPos.OutOffer = CurrentOutOffer;
            CurrentOutOffer.OutOfferPos_OutOffer.Add(subOutOfferPos);
            OnPropertyChanged("OutOfferPosList");
            CurrentOutOfferPos = subOutOfferPos;
            PostExecute("NewOutOfferPos");
        }

        public bool IsEnabledSubOutOfferPos()
        {
            return SelectedOutOfferPos != null;
        }

        [ACMethodInteraction("OutOfferPos", "en{'Delete Position'}de{'Position löschen'}", (short)MISort.Delete, true, "CurrentOutOfferPos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteOutOfferPos()
        {
            if (!PreExecute("DeleteOutOfferPos")) return;
            Msg msg = CurrentOutOfferPos.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            PostExecute("DeleteOutOfferPos");
        }

        public bool IsEnabledDeleteOutOfferPos()
        {
            return CurrentOutOffer != null && CurrentOutOfferPos != null;
        }

        private void BuildOutOfferPosData()
        {
            List<OutOfferPosData> posData = new List<OutOfferPosData>();

            foreach(var outOfferPos in CurrentOutOffer.OutOfferPos_OutOffer.Where(c => c.GroupOutOfferPosID == null).OrderBy(p => p.Position))
            {
                posData.Add(new OutOfferPosData(outOfferPos));
                BuildOutOfferPosDataRecursive(posData, outOfferPos.Items);
            }

            OutOfferPosDataList = posData;
        }

        private void BuildOutOfferPosDataRecursive(List<OutOfferPosData> posDataList, IEnumerable<OutOfferPos> outOfferPosList)
        {
            foreach (var outOfferPos in outOfferPosList.OrderBy(p => p.Position))
            {
                posDataList.Add(new OutOfferPosData(outOfferPos));
                BuildOutOfferPosDataRecursive(posDataList, outOfferPos.Items);
            }
        }

        public override void OnPrintingPhase(object reportEngine, ACPrintingPhase printingPhase)
        {
            if(printingPhase == ACPrintingPhase.Started)
            {
                BuildOutOfferPosData();
            }
            base.OnPrintingPhase(reportEngine, printingPhase);
        }

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
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
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
                case "LoadOutOfferPos":
                    LoadOutOfferPos();
                    return true;
                case "IsEnabledLoadOutOfferPos":
                    result = IsEnabledLoadOutOfferPos();
                    return true;
                case "NewOutOfferPos":
                    NewOutOfferPos();
                    return true;
                case "IsEnabledNewOutOfferPos":
                    result = IsEnabledNewOutOfferPos();
                    return true;
                case "DeleteOutOfferPos":
                    DeleteOutOfferPos();
                    return true;
                case "IsEnabledDeleteOutOfferPos":
                    result = IsEnabledDeleteOutOfferPos();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }

    [ACClassInfo(Const.PackName_VarioSales, "en{'OutOfferPosData'}de{'OutOfferPosData'}", Global.ACKinds.TACSimpleClass)]
    public class OutOfferPosData
    {
        public OutOfferPosData(OutOfferPos pos)
        {
            OutOfferPos = pos;
        }

        [ACPropertyInfo(1)]
        public OutOfferPos OutOfferPos
        {
            get;
            set;
        }


        [ACPropertyInfo(4)]
        public double? Quantity
        {
            get => OutOfferPos?.TargetQuantity;
        }

        [ACPropertyInfo(5)]
        public string QuantityUnit
        {
            get
            {
                if(Quantity > 0)
                    return Quantity + " " + OutOfferPos?.MDUnit.Symbol;

                return "";
            }
        }

        [ACPropertyInfo(6)]
        public string Price
        {
            get
            {
                if(OutOfferPos != null && OutOfferPos.PriceNet > 0)
                    return OutOfferPos.PriceNet.ToString();

                return "";
            }
        }

        [ACPropertyInfo(7)]
        public string TotalPrice
        {
            get
            {
                if (OutOfferPos != null && OutOfferPos.TotalPrice > 0)
                    return OutOfferPos.TotalPrice.ToString();

                return "";
            }
        }
    }
}
