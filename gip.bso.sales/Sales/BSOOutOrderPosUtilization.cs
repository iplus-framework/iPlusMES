using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.mes.datamodel; using gip.core.datamodel;
//using gip.core.manager;
using gip.core.autocomponent;
using gip.mes.autocomponent;

namespace gip.bso.sales
{
    /// <summary>
    /// Neue Masken:
    /// 1. Aufwanderfassung
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Outorderposutilization'}de{'Aufwandserfassung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOrderPosUtilization.ClassName)]
    public class BSOOutOrderPosUtilization : ACBSOvbNav 
    {
        #region c´tors

        public BSOOutOrderPosUtilization(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
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
        public override IAccessNav AccessNav { get {return AccessPrimary;} }

        ACAccessNav<OutOrderPosUtilization> _AccessPrimary;
        [ACPropertyAccessPrimary(690, "OutOrderPosUtilization")]
        public ACAccessNav<OutOrderPosUtilization> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<OutOrderPosUtilization>("OutOrderPosUtilization", this);
                }
                return _AccessPrimary;
            }
        }

        [ACPropertyCurrent(600, "OutOrderPosUtilization")]
        public OutOrderPosUtilization CurrentOutOrderPosUtilization
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentOutOrderPosUtilization");
            }
        }

        [ACPropertyList(601, "OutOrderPosUtilization")]
        public IEnumerable<OutOrderPosUtilization> OutOrderPosUtilizationList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, "OutOrderPosUtilization")]
        public OutOrderPosUtilization SelectedOutOrderPosUtilization
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedOutOrderPosUtilization");
            }
        }
        #endregion

        #region BSO->ACMethod
        [ACMethodInteraction("OutOrderPosUtilization", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedOutOrderPosUtilization", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<OutOrderPosUtilization>(requery, () => SelectedOutOrderPosUtilization, () => CurrentOutOrderPosUtilization, c => CurrentOutOrderPosUtilization = c,
                        DatabaseApp.OutOrderPosUtilization
                        .Where(c => c.OutOrderPosUtilizationID == SelectedOutOrderPosUtilization.OutOrderPosUtilizationID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedOutOrderPosUtilization != null;
        }

        [ACMethodInteraction("OutOrderPosUtilization", "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedOutOrderPosUtilization", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(OutOrderPosUtilization), OutOrderPosUtilization.NoColumnName, OutOrderPosUtilization.FormatNewNo, this);
            CurrentOutOrderPosUtilization = OutOrderPosUtilization.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.OutOrderPosUtilization.AddObject(CurrentOutOrderPosUtilization);
            ACState = Const.SMNew;
            PostExecute("New");
           
        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction("OutOrderPosUtilization", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentOutOrderPosUtilization", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentOutOrderPosUtilization.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentOutOrderPosUtilization);
            SelectedOutOrderPosUtilization = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        public bool IsEnabledDelete()
        {
            return CurrentOutOrderPosUtilization != null;
        }

        [ACMethodCommand("OutOrderPosUtilization", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("OutOrderPosUtilizationList");
        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
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
