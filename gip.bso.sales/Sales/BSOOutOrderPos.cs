using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using gip.core.manager;
using gip.mes.datamodel; using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;

namespace gip.bso.sales
{
    /// <summary>
    /// Version 3
    /// Neue Masken:
    /// 1. Auftragspositionsverwaltung
    /// 
    /// TODO: Betroffene Tabellen: OutOrder, OutOrderPos, OutOrderPosDeclaration
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Jobentry'}de{'Aufgabenerfassung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOrderPos.ClassName)]
    public class BSOOutOrderPos : ACBSOvbNav 
    {
        #region c´tors

        public BSOOutOrderPos(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit(deleteACClassTask);
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
        ACAccessNav<OutOrderPos> _AccessPrimary;
        [ACPropertyAccessPrimary(690, OutOrderPos.ClassName)]
        public ACAccessNav<OutOrderPos> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<OutOrderPos>(OutOrderPos.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        [ACPropertyCurrent(600, OutOrderPos.ClassName)]
        public OutOrderPos CurrentOutOrderPos
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentOutOrderPos");
                OnPropertyChanged("CurrentOutOrder");
                OnPropertyChanged("OutOrderPosDeclarationList");
                OnPropertyChanged("CurrentBillingCompanyAddress");
                OnPropertyChanged("CurrentDeliveryCompanyAddress");
            }
        }

        [ACPropertyList(601, OutOrderPos.ClassName)]
        public IEnumerable<OutOrderPos> OutOrderPosList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, OutOrderPos.ClassName)]
        public OutOrderPos SelectedOutOrderPos
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedOutOrderPos");
            }
        }

        [ACPropertyCurrent(603, OutOrder.ClassName)]
        public OutOrder CurrentOutOrder
        {
            get
            {
                if (CurrentOutOrderPos == null)
                    return null;
                return CurrentOutOrderPos.OutOrder;
            }
        }


        /// <summary>
        /// Liste aller Unternehmen, die Lieferanten sind
        /// </summary>
        [ACPropertyList(604, Company.ClassName)]
        public IEnumerable<Company> CompanyList
        {
            get
            {
                return DatabaseApp.Company.Where(c => c.IsCustomer).OrderBy(c => c.CompanyName).AsEnumerable();
            }
        }

        [ACPropertyList(605, "BillingCompanyAddress")]
        public IEnumerable<CompanyAddress> BillingCompanyAddressList
        {
            get
            {
                if (CurrentOutOrder == null || CurrentOutOrder.CustomerCompany == null)
                    return null;
                if (!CurrentOutOrder.CustomerCompany.CompanyAddress_Company.IsLoaded)
                    CurrentOutOrder.CustomerCompany.CompanyAddress_Company.Load();
                return CurrentOutOrder.CustomerCompany.CompanyAddress_Company.Where(c => c.IsBillingCompanyAddress).OrderBy(c => c.Name1).AsEnumerable();
            }
        }

        [ACPropertyList(606, "DeliveryCompanyAddress")]
        public IEnumerable<CompanyAddress> DeliveryCompanyAddressList
        {
            get
            {
                if (CurrentOutOrder == null || CurrentOutOrder.CustomerCompany == null)
                    return null;
                if (!CurrentOutOrder.CustomerCompany.CompanyAddress_Company.IsLoaded)
                    CurrentOutOrder.CustomerCompany.CompanyAddress_Company.Load();
                return CurrentOutOrder.CustomerCompany.CompanyAddress_Company.Where(c => c.IsDeliveryCompanyAddress).OrderBy(c => c.Name1).AsEnumerable();
            }
        }

        [ACPropertyCurrent(607, "BillingCompanyAddress")]
        public CompanyAddress CurrentBillingCompanyAddress
        {
            get
            {
                if ( CurrentOutOrder == null )
                    return null;
                return CurrentOutOrder.BillingCompanyAddress;
            }
        }

        [ACPropertyCurrent(608, "DeliveryCompanyAddress")]
        public CompanyAddress CurrentDeliveryCompanyAddress
        {
            get
            {
                if (CurrentOutOrder == null)
                    return null;
                return CurrentOutOrder.DeliveryCompanyAddress;
            }
        }

        #endregion

        #region BSO->ACMethod
        [ACMethodCommand(OutOrderPos.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(OutOrderPos.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction(OutOrderPos.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedOutOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<OutOrderPos>(requery, () => SelectedOutOrderPos, () => CurrentOutOrderPos, c => CurrentOutOrderPos = c,
                        DatabaseApp.OutOrderPos
                        .Where(c => c.OutOrderPosID == SelectedOutOrderPos.OutOrderPosID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedOutOrderPos != null;
        }

        [ACMethodInteraction(OutOrderPos.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedOutOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentOutOrderPos = OutOrderPos.NewACObject(DatabaseApp, null);
            ACState = Const.SMNew;
            PostExecute("New");
           
        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction(OutOrderPos.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentOutOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentOutOrderPos.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentOutOrderPos);
            SelectedOutOrderPos = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");

        }

        public bool IsEnabledDelete()
        {
            return CurrentOutOrderPos != null;
        }

        [ACMethodCommand(OutOrderPos.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("OutOrderPosList");
        }
        #endregion

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
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}
