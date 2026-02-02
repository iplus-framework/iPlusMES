// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOVehicleWeighing.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Linq;
using System.Threading.Tasks;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.mes.autocomponent;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Version 3
    /// Folgende alte Masken sind in diesem BSO enthalten:
    /// 1. Fahrzeugwaage
    /// Neue Masken:
    /// 1. Fahrzeugwaage
    /// TODO: Betroffene Tabellen: InDeliveryNotePosInWeighing
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Receiptweighing'}de{'Eingangsverwiegung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + "Weighing")]
    public class BSOVehicleWeighing : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOVehicleWeighing"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOVehicleWeighing(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
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
            Search();
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }
        #endregion

        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<Weighing> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "Weighing")]
        public IAccessNav AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    // Instanz anhand der ACQRY-Klasse erzeugen
                    // Parameter: acComponentParent (optional), qryACClass, acKey
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);

                    _AccessPrimary = navACQueryDefinition.NewAccessNav("Weighing", this) as ACAccessNav<Weighing>;
                }
                return _AccessPrimary;
            }
        }

        //[ACPropertyCurrent(9999, "InDeliveryNotePosInWeighing")]
        //public InDeliveryNotePosInWeighing CurrentInDeliveryNotePosInWeighing
        //{
        //    get
        //    {
        //        if (AccessPrimary == null) return null; return AccessPrimary.CurrentNavObject as InDeliveryNotePosInWeighing;
        //    }
        //    set
        //    {
        //        AccessPrimary.CurrentNavObject = value;
        //        OnPropertyChanged("CurrentInDeliveryNotePosInWeighing");
        //    }
        //}

        //[ACPropertyList(9999, "InDeliveryNotePosInWeighing")]
        //public IEnumerable<InDeliveryNotePosInWeighing> InDeliveryNotePosInWeighingList
        //{
        //    get
        //    {
        //        return AccessPrimary.NavObjectList as IEnumerable<InDeliveryNotePosInWeighing>;
        //    }
        //}

        //[ACPropertySelected(9999, "InDeliveryNotePosInWeighing")]
        //public InDeliveryNotePosInWeighing SelectedInDeliveryNotePosInWeighing
        //{
        //    get
        //    {
        //        if (AccessPrimary == null) return null; return AccessPrimary.SelectedNavObject as InDeliveryNotePosInWeighing;
        //    }
        //    set
        //    {
        //        AccessPrimary.SelectedNavObject = value;
        //        OnPropertyChanged("SelectedInDeliveryNotePosInWeighing");
        //    }
        //}
        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("InDeliveryNotePosInWeighing", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand("InDeliveryNotePosInWeighing", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction("InDeliveryNotePosInWeighing", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedInDeliveryNotePosInWeighing", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return false; // SelectedInDeliveryNotePosInWeighing != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("InDeliveryNotePosInWeighing", Const.New, (short)MISort.New, true, "SelectedInDeliveryNotePosInWeighing", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            //CurrentInDeliveryNotePosInWeighing = InDeliveryNotePosInWeighing.NewACObject(Database, null);
            //Database.InDeliveryNotePosInWeighing.AddObject(CurrentInDeliveryNotePosInWeighing);
            ACState = Const.SMNew;
            PostExecute("New");

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
        [ACMethodInteraction("InDeliveryNotePosInWeighing", Const.Delete, (short)MISort.Delete, true, "CurrentInDeliveryNotePosInWeighing", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            //Msg msg = CurrentInDeliveryNotePosInWeighing.DeleteACObject(Database, true);
            //if (msg != null)
            //{
            //    Messages.MsgAsync(msg);
            //    return;
            //}

            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return true;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("InDeliveryNotePosInWeighing", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("InDeliveryNotePosInWeighingList");
        }

        /// <summary>
        /// Resets the wheiging machine.
        /// </summary>
        [ACMethodCommand("InDeliveryNotePosInWeighing", "en{'Reset Scale'}de{'Reset Waage'}", 9999, false, Global.ACKinds.MSMethodPrePost)]
        public void ResetWheigingMachine()
        {
            if (!PreExecute("ResetWheigingMachine")) return;
            // TODO:
            PostExecute("ResetWheigingMachine");
        }
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
                case nameof(ResetWheigingMachine):
                    ResetWheigingMachine();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}