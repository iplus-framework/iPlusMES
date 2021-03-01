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
    /// TODO: Betroffene Tabellen: OutOffering, OutOfferingPos
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Offering'}de{'Angebot'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOffering.ClassName)]
    public class BSOOutOffering : ACBSOvbNav
    {
        #region c´tors

        public BSOOutOffering(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            this._AccessOutOfferingPos = null;
            this._CurrentOutOfferingPos = null;
            this._SelectedOutOfferingPos = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessOutOfferingPos != null)
            {
                _AccessOutOfferingPos.ACDeInit(false);
                _AccessOutOfferingPos = null;
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
        ACAccessNav<OutOffering> _AccessPrimary;
        [ACPropertyAccessPrimary(690, OutOffering.ClassName)]
        public ACAccessNav<OutOffering> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<OutOffering>(OutOffering.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        [ACPropertyCurrent(600, OutOffering.ClassName)]
        public OutOffering CurrentOutOffering
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (CurrentOutOffering != null)
                {
                    CurrentOutOffering.PropertyChanged -= CurrentOutOffering_PropertyChanged;
                }
                if (AccessPrimary == null) return; AccessPrimary.Current = value;

                if (CurrentOutOffering != null)
                {
                    CurrentOutOffering.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentOutOffering_PropertyChanged);
                }
                CurrentOutOfferingPos = null;

                CurrentOutOfferingPos = OutOfferingPosList.FirstOrDefault();
                OnPropertyChanged("CurrentOutOffering");
                OnPropertyChanged("OutOfferingPosList");
                OnPropertyChanged("CompanyList");
                OnPropertyChanged("BillingCompanyAddressList");
                OnPropertyChanged("DeliveryCompanyAddressList");
                OnPropertyChanged("CurrentBillingCompanyAddress");
                OnPropertyChanged("CurrentDeliveryCompanyAddress");
            }
        }

        void CurrentOutOffering_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CustomerCompanyID":
                    if (CurrentOutOffering.CustomerCompany != null)
                    {
                        CurrentOutOffering.BillingCompanyAddress = CurrentOutOffering.CustomerCompany.BillingCompanyAddress;
                        CurrentOutOffering.DeliveryCompanyAddress = CurrentOutOffering.CustomerCompany.DeliveryCompanyAddress;
                    }
                    else
                    {
                        CurrentOutOffering.BillingCompanyAddress = null;
                        CurrentOutOffering.DeliveryCompanyAddress = null;
                    }
                    OnPropertyChanged("BillingCompanyAddressList");
                    OnPropertyChanged("DeliveryCompanyAddressList");
                    OnPropertyChanged("CurrentBillingCompanyAddress");
                    OnPropertyChanged("CurrentDeliveryCompanyAddress");

                    break;
            }
        }

        [ACPropertyList(601, OutOffering.ClassName)]
        public IEnumerable<OutOffering> OutOfferingList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, OutOffering.ClassName)]
        public OutOffering SelectedOutOffering
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedOutOffering");
            }
        }

        ACAccess<OutOfferingPos> _AccessOutOfferingPos;
        [ACPropertyAccess(603, "OutOfferingPos")]
        public ACAccess<OutOfferingPos> AccessOutOfferingPos
        {
            get
            {
                if (_AccessOutOfferingPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + OutOfferingPos.ClassName) as ACQueryDefinition;
                    _AccessOutOfferingPos = acQueryDefinition.NewAccess<OutOfferingPos>("OutOfferingPos", this);
                }
                return _AccessOutOfferingPos;
            }
        }

        OutOfferingPos _CurrentOutOfferingPos;
        [ACPropertyCurrent(604, "OutOfferingPos")]
        public OutOfferingPos CurrentOutOfferingPos
        {
            get
            {
                return _CurrentOutOfferingPos;
            }
            set
            {
                if (_CurrentOutOfferingPos != null)
                    _CurrentOutOfferingPos.PropertyChanged -= CurrentOutOfferingPos_PropertyChanged;
                _CurrentOutOfferingPos = value;
                if (_CurrentOutOfferingPos != null)
                    _CurrentOutOfferingPos.PropertyChanged += CurrentOutOfferingPos_PropertyChanged;
                OnPropertyChanged("MDUnitList");
                OnPropertyChanged("CurrentOutOfferingPos");
                OnPropertyChanged("CurrentMDUnit");
            }
        }

        void CurrentOutOfferingPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MaterialID":
                    {
                        OnPropertyChanged("MDUnitList");
                        if (CurrentOutOfferingPos.Material != null && CurrentOutOfferingPos.Material.BaseMDUnit != null)
                            CurrentMDUnit = CurrentOutOfferingPos.Material.BaseMDUnit;
                        else
                            CurrentMDUnit = null;
                        OnPropertyChanged("CurrentOutOfferingPos");
                    }
                    break;
                case "TargetQuantityUOM":
                case "MDUnitID":
                    {
                        CurrentOutOfferingPos.TargetQuantity = CurrentOutOfferingPos.Material.ConvertToBaseQuantity(CurrentOutOfferingPos.TargetQuantityUOM, CurrentOutOfferingPos.MDUnit);
                        CurrentOutOfferingPos.TargetWeight = CurrentOutOfferingPos.Material.ConvertToBaseWeight(CurrentOutOfferingPos.TargetQuantityUOM, CurrentOutOfferingPos.MDUnit);
                    }
                    break;
            }
        }

        [ACPropertyList(605, "OutOfferingPos")]
        public IEnumerable<OutOfferingPos> OutOfferingPosList
        {
            get
            {
                if (CurrentOutOffering == null)
                    return null;
                return from c in CurrentOutOffering.OutOfferingPos_OutOffering
                       select c;

            }
        }

        OutOfferingPos _SelectedOutOfferingPos;
        [ACPropertySelected(606, "OutOfferingPos")]
        public OutOfferingPos SelectedOutOfferingPos
        {
            get
            {
                return _SelectedOutOfferingPos;
            }
            set
            {
                _SelectedOutOfferingPos = value;
                OnPropertyChanged("SelectedOutOfferingPos");
            }
        }

        [ACPropertyList(607, MDUnit.ClassName)]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                if (CurrentOutOfferingPos == null || CurrentOutOfferingPos.Material == null)
                    return null;
                return CurrentOutOfferingPos.Material.MDUnitList;
            }
        }

        [ACPropertyCurrent(608, MDUnit.ClassName)]
        public MDUnit CurrentMDUnit
        {
            get
            {
                if (CurrentOutOfferingPos == null)
                    return null;
                return CurrentOutOfferingPos.MDUnit;
            }
            set
            {
                if (CurrentOutOfferingPos != null && value != null)
                {
                    CurrentOutOfferingPos.MDUnit = value;
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
                if (CurrentOutOffering == null || CurrentOutOffering.CustomerCompany == null)
                    return null;
                if (!CurrentOutOffering.CustomerCompany.CompanyAddress_Company.IsLoaded)
                    CurrentOutOffering.CustomerCompany.CompanyAddress_Company.Load();
                return from c in CurrentOutOffering.CustomerCompany.CompanyAddress_Company
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
                if (CurrentOutOffering == null || CurrentOutOffering.CustomerCompany == null)
                    return null;
                if (!CurrentOutOffering.CustomerCompany.CompanyAddress_Company.IsLoaded)
                    CurrentOutOffering.CustomerCompany.CompanyAddress_Company.Load();
                return from c in CurrentOutOffering.CustomerCompany.CompanyAddress_Company
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
                if (CurrentOutOffering == null)
                    return null;
                return CurrentOutOffering.BillingCompanyAddress;
            }
        }

        [ACPropertyCurrent(613, "DeliveryCompanyAddress")]
        public CompanyAddress CurrentDeliveryCompanyAddress
        {
            get
            {
                if (CurrentOutOffering == null)
                    return null;
                return CurrentOutOffering.DeliveryCompanyAddress;
            }
        }

        [ACPropertyInfo(9999)]
        public string XMLDesign
        {
            get 
            {
                if (CurrentOutOfferingPos != null)
                    return CurrentOutOfferingPos.XMLConfig;
                return "";
            }
            set
            {
                if (CurrentOutOfferingPos != null)
                    CurrentOutOfferingPos.XMLConfig = value;
                OnPropertyChanged("XMLDesign");
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


        #endregion

        #region BSO->ACMethod
        [ACMethodCommand(OutOffering.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(OutOffering.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction(OutOffering.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedOutOffering", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<OutOffering>(requery, () => SelectedOutOffering, () => CurrentOutOffering, c => CurrentOutOffering = c,
                        DatabaseApp.OutOffering
                        .Include(c => c.OutOfferingPos_OutOffering)
                        .Where(c => c.OutOfferingID == SelectedOutOffering.OutOfferingID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedOutOffering != null;
        }

        [ACMethodInteraction(OutOffering.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedOutOffering", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(OutOffering), OutOffering.NoColumnName, OutOffering.FormatNewNo, this);
            CurrentOutOffering = OutOffering.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.OutOffering.AddObject(CurrentOutOffering);

            CurrentOutOfferingPos = OutOfferingPos.NewACObject(DatabaseApp, CurrentOutOffering);
            CurrentOutOfferingPos.OutOffering = CurrentOutOffering;
            CurrentOutOffering.OutOfferingPos_OutOffering.Add(CurrentOutOfferingPos);
            ACState = Const.SMNew;
            PostExecute("New");

        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction(OutOffering.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentOutOffering", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentOutOffering.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentOutOffering);
            SelectedOutOffering = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        public bool IsEnabledDelete()
        {
            return true;
        }

        [ACMethodCommand(OutOffering.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("OutOfferingList");
        }

        [ACMethodInteraction("OutOfferingPos", "en{'Load Position'}de{'Position laden'}", (short)MISort.Load, false, "SelectedOutOfferingPos", Global.ACKinds.MSMethodPrePost)]
        public void LoadOutOfferingPos()
        {
            if (!IsEnabledLoadOutOfferingPos())
                return;
            if (!PreExecute("LoadOutOfferingPos")) return;
            // Laden des aktuell selektierten OutOfferingPos 
            CurrentOutOfferingPos = (from c in CurrentOutOffering.OutOfferingPos_OutOffering
                                     where c.OutOfferingPosID == SelectedOutOfferingPos.OutOfferingPosID
                                     select c).First();
            PostExecute("LoadOutOfferingPos");
        }

        public bool IsEnabledLoadOutOfferingPos()
        {
            return SelectedOutOfferingPos != null && CurrentOutOffering != null;
        }

        [ACMethodInteraction("OutOfferingPos", "en{'New Position'}de{'Neue Position'}", (short)MISort.New, true, "SelectedOutOfferingPos", Global.ACKinds.MSMethodPrePost)]
        public void NewOutOfferingPos()
        {
            if (!PreExecute("NewOutOfferingPos")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            CurrentOutOfferingPos = OutOfferingPos.NewACObject(DatabaseApp, CurrentOutOffering);
            CurrentOutOfferingPos.OutOffering = CurrentOutOffering;
            CurrentOutOffering.OutOfferingPos_OutOffering.Add(CurrentOutOfferingPos);
            OnPropertyChanged("OutOfferingPosList");
            PostExecute("NewOutOfferingPos");
        }

        public bool IsEnabledNewOutOfferingPos()
        {
            return CurrentOutOffering != null;
        }

        [ACMethodInteraction("OutOfferingPos", "en{'Delete Position'}de{'Position löschen'}", (short)MISort.Delete, true, "CurrentOutOfferingPos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteOutOfferingPos()
        {
            if (!PreExecute("DeleteOutOfferingPos")) return;
            Msg msg = CurrentOutOfferingPos.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            PostExecute("DeleteOutOfferingPos");
        }

        public bool IsEnabledDeleteOutOfferingPos()
        {
            return CurrentOutOffering != null && CurrentOutOfferingPos != null;
        }
        #endregion

        public override void OnPrintingPhase(object reportEngine, ACPrintingPhase printingPhase)
        {
            base.OnPrintingPhase(reportEngine, printingPhase);

            ReportDocument reportDocument = reportEngine as ReportDocument;
            if(reportDocument != null && printingPhase == ACPrintingPhase.Started)
            {
                string xamlData = reportDocument.XamlData;
                string dynamicComment = ExtractDynamicContent(CurrentOutOffering.Comment);


                xamlData = xamlData.Replace("<Paragraph>DynamicComment</Paragraph>", dynamicComment);

                reportDocument.XamlData = xamlData;
            }
        }

        private string ExtractDynamicContent(string content)
        {
            string result = "";

            int startIndex = content.IndexOf('<');
            int endIndex = content.IndexOf('>');



            if (startIndex >= 0 && endIndex > startIndex)
            {
                result = content.Remove(startIndex, endIndex - startIndex+1);
                startIndex = result.IndexOf('<');
                endIndex = result.IndexOf('>');

                result = result.Remove(startIndex, endIndex - startIndex + 1);

                int lastStartIndex = result.LastIndexOf('<');
                int lastEndIndex = result.LastIndexOf('>');
                result = result.Remove(lastStartIndex, lastEndIndex - lastStartIndex+1);
                
            }

            return result;
        }


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
                case "LoadOutOfferingPos":
                    LoadOutOfferingPos();
                    return true;
                case "IsEnabledLoadOutOfferingPos":
                    result = IsEnabledLoadOutOfferingPos();
                    return true;
                case "NewOutOfferingPos":
                    NewOutOfferingPos();
                    return true;
                case "IsEnabledNewOutOfferingPos":
                    result = IsEnabledNewOutOfferingPos();
                    return true;
                case "DeleteOutOfferingPos":
                    DeleteOutOfferingPos();
                    return true;
                case "IsEnabledDeleteOutOfferingPos":
                    result = IsEnabledDeleteOutOfferingPos();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
