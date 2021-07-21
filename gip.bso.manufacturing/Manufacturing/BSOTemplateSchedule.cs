using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Batch scheduler'}de{'Batch Zeitplaner'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + PlanningMR.ClassName)]
    public class BSOTemplateSchedule : ACBSOvbNav
    {
        #region c´tors

        public BSOTemplateSchedule(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            
        }


        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion
        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<PlanningMR> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, PlanningMR.ClassName)]
        public ACAccessNav<PlanningMR> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<PlanningMR>(PlanningMR.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected balancing mode.
        /// </summary>
        /// <value>The selected balancing mode.</value>
        [ACPropertySelected(9999, PlanningMR.ClassName)]
        public PlanningMR SelectedPlanningMR
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedPlanningMR");
            }
        }

        /// <summary>
        /// Gets or sets the current balancing mode.
        /// </summary>
        /// <value>The current balancing mode.</value>
        [ACPropertyCurrent(9999, PlanningMR.ClassName)]
        public PlanningMR CurrentPlanningMR
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Current = value;
                OnPropertyChanged("CurrentPlanningMR");
            }
        }

        /// <summary>
        /// Gets the balancing mode list.
        /// </summary>
        /// <value>The balancing mode list.</value>
        [ACPropertyList(9999, PlanningMR.ClassName)]
        public IEnumerable<PlanningMR> PlanningMRList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(PlanningMR.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(PlanningMR.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(PlanningMR.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedPlanningMR", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<PlanningMR>(requery, () => SelectedPlanningMR, () => CurrentPlanningMR, c => CurrentPlanningMR = c,
                        DatabaseApp
                        .PlanningMR
                        .Where(c => c.PlanningMRID == SelectedPlanningMR.PlanningMRID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedPlanningMR != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(PlanningMR.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedPlanningMR", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(PlanningMR), PlanningMR.NoColumnName, PlanningMR.FormatNewNo, this);
            CurrentPlanningMR = PlanningMR.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.PlanningMR.AddObject(CurrentPlanningMR);
            ACState = Const.SMNew;

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
        [ACMethodInteraction(PlanningMR.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentPlanningMR", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (AccessPrimary == null)
                return;
            Msg msg = CurrentPlanningMR.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }


            AccessPrimary.NavList.Remove(CurrentPlanningMR);
            SelectedPlanningMR = AccessPrimary.NavList.FirstOrDefault();
            Load();
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentPlanningMR != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(PlanningMR.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("PlanningMRList");
        }
        #endregion


    }
}
