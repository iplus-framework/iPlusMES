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
    /// 1. Angebotsübersicht
    /// 
    /// TODO: Betroffene Tabellen: OutOffering, OutOfferingPos
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Offer Overview'}de{'Angebotsübersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOffering.ClassName)]
    public class BSOOutOfferingOverview : ACBSOvbNav 
    {
        #region c´tors

        public BSOOutOfferingOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
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
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentOutOffering");
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

        [ACMethodCommand(OutOffering.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("OutOfferingList");
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
                case"Search":
                    Search();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}
