using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.reporthandler.Flowdoc;
using System.Windows.Media;
using System.Windows.Documents;

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

                CurrentOutOfferPos = OutOfferPosList?.FirstOrDefault();
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
                        if (CurrentOutOfferPos != null && CurrentOutOfferPos.Material != null)
                        {
                            CurrentOutOfferPos.TargetQuantity = CurrentOutOfferPos.Material.ConvertToBaseQuantity(CurrentOutOfferPos.TargetQuantityUOM, CurrentOutOfferPos.MDUnit);
                            //CurrentOutOfferPos.TargetWeight = CurrentOutOfferPos.Material.ConvertToBaseWeight(CurrentOutOfferPos.TargetQuantityUOM, CurrentOutOfferPos.MDUnit);
                        }
                    }
                    break;
                case "PriceNet":
                    {
                        if (CurrentOutOffer != null)
                        {
                            CurrentOutOffer.OnEntityPropertyChanged("PosPriceNetTotal");
                        }
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

        #region Properties => Report

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
        public List<OutOfferPos> OutOfferPosDataList
        {
            get;
            set;
        }

        #endregion

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

        [ACMethodInteraction(OutOffer.ClassName, "en{'New version'}de{'Neue Version'}", (short)MISort.New+1, true, "", Global.ACKinds.MSMethodPrePost)]
        public void NewVersion()
        {
            OutOffer newOfferVersion = OutOffer.NewACObject(DatabaseApp, null, CurrentOutOffer.OutOfferNo);
            newOfferVersion.OutOfferDate = DateTime.Now;
            newOfferVersion.OutOfferVersion = DatabaseApp.OutOffer.Where(c => c.OutOfferNo == CurrentOutOffer.OutOfferNo).Max(v => v.OutOfferVersion) + 1;
            newOfferVersion.BillingCompanyAddress = CurrentOutOffer.BillingCompanyAddress;
            newOfferVersion.Comment = CurrentOutOffer.Comment;
            newOfferVersion.CustomerCompany = CurrentOutOffer.CustomerCompany;
            newOfferVersion.CustRequestNo = CurrentOutOffer.CustRequestNo;
            newOfferVersion.DeliveryCompanyAddress = CurrentOutOffer.DeliveryCompanyAddress;
            newOfferVersion.MDDelivType = CurrentOutOffer.MDDelivType;
            //newVersion.MDOutOfferState = CurrentOutOffer.MDOutOfferState;
            newOfferVersion.MDOutOrderType = CurrentOutOffer.MDOutOrderType;
            newOfferVersion.MDTermOfPayment = CurrentOutOffer.MDTermOfPayment;
            newOfferVersion.MDTimeRange = CurrentOutOffer.MDTimeRange;
            newOfferVersion.PriceGross = CurrentOutOffer.PriceGross;
            newOfferVersion.PriceNet = CurrentOutOffer.PriceNet;
            newOfferVersion.TargetDeliveryDate = CurrentOutOffer.TargetDeliveryDate;
            newOfferVersion.TargetDeliveryMaxDate = CurrentOutOffer.TargetDeliveryMaxDate;
            newOfferVersion.XMLConfig = CurrentOutOffer.XMLConfig;
            newOfferVersion.XMLDesign = CurrentOutOffer.XMLDesign;

            DatabaseApp.OutOffer.AddObject(newOfferVersion);


            foreach (OutOfferPos pos in CurrentOutOffer.OutOfferPos_OutOffer.Where(c => !c.GroupOutOfferPosID.HasValue))
            {
                NewVersionPos(newOfferVersion, pos, null);
            }

            DatabaseApp.ACSaveChanges();
            Search();
            SelectedOutOffer = newOfferVersion;
        }

        public bool IsEnabledNewVersion()
        {
            return CurrentOutOffer != null;
        }

        private void NewVersionPos(OutOffer outOffer, OutOfferPos pos, OutOfferPos groupPos)
        {
            OutOfferPos newPos = OutOfferPos.NewACObject(DatabaseApp, null, groupPos);
            newPos.OutOfferPos1_GroupOutOfferPos = groupPos;
            newPos.Comment = pos.Comment;
            newPos.Comment2 = pos.Comment2;
            newPos.GroupSum = pos.GroupSum;
            newPos.Material = pos.Material;
            newPos.MDCountrySalesTax = pos.MDCountrySalesTax;
            newPos.MDTimeRange = pos.MDTimeRange;
            newPos.MDUnit = pos.MDUnit;
            newPos.OutOffer = outOffer;
            newPos.PriceGross = pos.PriceGross;
            newPos.PriceNet = pos.PriceNet;
            newPos.Sequence = pos.Sequence;
            newPos.TargetDeliveryDate = pos.TargetDeliveryDate;
            newPos.TargetDeliveryMaxDate = pos.TargetDeliveryMaxDate;
            newPos.TargetDeliveryPriority = pos.TargetDeliveryPriority;
            newPos.TargetQuantity = pos.TargetQuantity;
            newPos.TargetQuantityUOM = pos.TargetQuantityUOM;
            newPos.TargetWeight = pos.TargetWeight;
            newPos.XMLConfig = pos.XMLConfig;
            newPos.XMLDesign = pos.XMLDesign;

            outOffer.OutOfferPos_OutOffer.Add(newPos);
            DatabaseApp.OutOfferPos.AddObject(newPos);

            foreach(OutOfferPos subPos in pos.OutOfferPos_GroupOutOfferPos)
            {
                NewVersionPos(outOffer, subPos, newPos);
            }
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
            OutOfferPos groupPos = CurrentOutOfferPos?.OutOfferPos1_GroupOutOfferPos;
            CurrentOutOfferPos = OutOfferPos.NewACObject(DatabaseApp, CurrentOutOffer, groupPos);
            CurrentOutOfferPos.OutOffer = CurrentOutOffer;
            CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos = groupPos;
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
            OnPropertyChanged("OutOfferPosList");
            if (CurrentOutOffer != null)
            {
                CurrentOutOffer.OnEntityPropertyChanged("PosPriceNetTotal");
            }
        }

        public bool IsEnabledDeleteOutOfferPos()
        {
            return CurrentOutOffer != null && CurrentOutOfferPos != null;
        }

        [ACMethodInteraction("OutOfferPos", "en{'Position up'}de{'Position oben'}", 10, true, "CurrentOutOfferPos", Global.ACKinds.MSMethodPrePost)]
        public void OutOrderPosUp()
        {
            int sequencePre = 0;
            if(CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos != null)
            {
                var posPre = CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos.OutOfferPos_GroupOutOfferPos
                                                 .Where(c => c.Sequence < CurrentOutOfferPos.Sequence).OrderByDescending(x => x.Sequence)
                                                 .FirstOrDefault();

                if (posPre == null)
                    return;

                sequencePre = posPre.Sequence;

                posPre.Sequence = CurrentOutOfferPos.Sequence;
                CurrentOutOfferPos.Sequence = sequencePre;
            }
            else
            {
                var posPre = OutOfferPosList.Where(c => c.OutOfferPos1_GroupOutOfferPos == null && c.Sequence < CurrentOutOfferPos.Sequence)
                                            .OrderByDescending(x => x.Sequence)
                                            .FirstOrDefault();

                if (posPre == null)
                    return;

                sequencePre = posPre.Sequence;
                posPre.Sequence = CurrentOutOfferPos.Sequence;
                CurrentOutOfferPos.Sequence = sequencePre;
            }
            OnPropertyChanged("OutOfferPosList");
        }

        public bool IsEnabledOutOrderPosUp()
        {
            return CurrentOutOfferPos != null && CurrentOutOfferPos.Sequence > 1;
        }

        [ACMethodInteraction("OutOfferPos", "en{'Position down'}de{'Position unten'}", 11, true, "CurrentOutOfferPos", Global.ACKinds.MSMethodPrePost)]
        public void OutOrderPosDown()
        {
            int sequencePre = 0;
            if (CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos != null)
            {
                var posPre = CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos.OutOfferPos_GroupOutOfferPos
                                                 .Where(c => c.Sequence > CurrentOutOfferPos.Sequence)
                                                 .OrderBy(x => x.Sequence)
                                                 .FirstOrDefault();

                if (posPre == null)
                    return;

                sequencePre = posPre.Sequence;

                posPre.Sequence = CurrentOutOfferPos.Sequence;
                CurrentOutOfferPos.Sequence = sequencePre;
            }
            else
            {
                var posPre = OutOfferPosList.Where(c => c.OutOfferPos1_GroupOutOfferPos == null && c.Sequence > CurrentOutOfferPos.Sequence)
                                            .OrderBy(x => x.Sequence)
                                            .FirstOrDefault();

                if (posPre == null)
                    return;

                sequencePre = posPre.Sequence;
                posPre.Sequence = CurrentOutOfferPos.Sequence;
                CurrentOutOfferPos.Sequence = sequencePre;
            }
            OnPropertyChanged("OutOfferPosList");
        }

        public bool IsEnabledOutOrderPosDown()
        {
            return CurrentOutOfferPos != null
                && CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos == null
                   ? CurrentOutOfferPos.Sequence < OutOfferPosList.Where(x => x.OutOfferPos1_GroupOutOfferPos == null).Max(c => c.Sequence)
                   : CurrentOutOfferPos.Sequence < CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos.OutOfferPos_GroupOutOfferPos.Max(x => x.Sequence);
        }

        #region Methods => Report

        private void BuildOutOfferPosData()
        {
            List<OutOfferPos> posData = new List<OutOfferPos>();

            foreach(var outOfferPos in CurrentOutOffer.OutOfferPos_OutOffer.Where(c => c.GroupOutOfferPosID == null && c.PriceNet >= 0).OrderBy(p => p.Position))
            {
                posData.Add(outOfferPos);
                BuildOutOfferPosDataRecursive(posData, outOfferPos.Items);
                if(outOfferPos.GroupSum)
                {
                    OutOfferPos sumPos = new OutOfferPos();
                    sumPos.Total = outOfferPos.Items.Sum(c => c.TotalPrice).ToString("N");
                    sumPos.MaterialNo = "SubTotal " + outOfferPos.Material.MaterialNo;
                    sumPos.Sequence = outOfferPos.Sequence;
                    sumPos.GroupSum = outOfferPos.GroupSum;
                    posData.Add(sumPos);
                }
            }

            OutOfferPosDataList = posData;
        }

        private void BuildOutOfferPosDataRecursive(List<OutOfferPos> posDataList, IEnumerable<OutOfferPos> outOfferPosList)
        {
            foreach (var outOfferPos in outOfferPosList.Where(c => c.PriceNet >= 0).OrderBy(p => p.Position))
            {
                posDataList.Add(outOfferPos);
                BuildOutOfferPosDataRecursive(posDataList, outOfferPos.Items);
            }
        }

        public override void OnPrintingPhase(object reportEngine, ACPrintingPhase printingPhase)
        {
            if(printingPhase == ACPrintingPhase.Started)
            {
                ReportDocument doc = reportEngine as ReportDocument;
                if (doc != null && doc.ReportData != null && doc.ReportData.Any(c => c.ACClassDesign != null 
                                                                                 && (c.ACClassDesign.ACIdentifier == "OfferDe") || c.ACClassDesign.ACIdentifier == "OfferEn"))
                {
                    doc.SetFlowDocObjValue += Doc_SetFlowDocObjValue;

                    BuildOutOfferPosData();
                }
            }
            else
            {
                ReportDocument doc = reportEngine as ReportDocument;
                if (doc != null)
                {
                    doc.SetFlowDocObjValue -= Doc_SetFlowDocObjValue;
                }
            }

            base.OnPrintingPhase(reportEngine, printingPhase);
        }

        private void Doc_SetFlowDocObjValue(object sender, PaginatorOnSetValueEventArgs e)
        {
            OutOfferPos pos = e.ParentDataRow as OutOfferPos;
            if(pos != null && pos.GroupSum && pos.OutOfferPosID == new Guid())
            {
                var inlineCell = e.FlowDocObj as InlineTableCellValue;
                if (inlineCell != null)
                {
                    var tableCell = (inlineCell.Parent as Paragraph)?.Parent as TableCell;
                    if (tableCell != null)
                    {
                        if (inlineCell.VBContent == "Total")
                         {
                            tableCell.BorderBrush = Brushes.Black;
                            tableCell.BorderThickness = new System.Windows.Thickness(0, 1, 0, 1);
                        }
                        tableCell.FontWeight = System.Windows.FontWeights.Bold;
                    }
                }
            }
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
}
