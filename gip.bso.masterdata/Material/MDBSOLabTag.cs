// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 10.01.2018
// ***********************************************************************
// <copyright file="MDBSOLabTag.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.mes.autocomponent;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Allgemeine Stammdatenmaske für MDLabTag
    /// Bei den einfachen MD-Tabellen wird bewußt auf die Managerklassen verzichtet.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Laboratory Tag'}de{'Laborkennzeichen'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDLabTag.ClassName)]
    public class MDBSOLabTag : ACBSOvbNav
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOLabTag"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOLabTag(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<MDLabTag> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "MDLabTag")]
        public ACAccessNav<MDLabTag> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDLabTag>("MDLabTag", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected lab tag.
        /// </summary>
        /// <value>The selected lab tag.</value>
        [ACPropertySelected(9999, "MDLabTag")]
        public MDLabTag SelectedLabTag
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedLabTag");
            }
        }

        /// <summary>
        /// Gets or sets the current lab tag.
        /// </summary>
        /// <value>The current lab tag.</value>
        [ACPropertyCurrent(9999, "MDLabTag")]
        public MDLabTag CurrentLabTag
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentLabTag");
            }
        }

        /// <summary>
        /// Gets the lab tag list.
        /// </summary>
        /// <value>The lab tag list.</value>
        [ACPropertyList(9999, "MDLabTag")]
        public IEnumerable<MDLabTag> LabTagList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("MDLabTag", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            if (OnSave())
                Search();
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
        [ACMethodCommand("MDLabTag", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction("MDLabTag", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedLabTag", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDLabTag>(requery, () => SelectedLabTag, () => CurrentLabTag, c => CurrentLabTag = c,
                        DatabaseApp.MDLabTag.Where(c => c.MDLabTagID == SelectedLabTag.MDLabTagID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedLabTag != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("MDLabTag", Const.New, (short)MISort.New, true, "SelectedLabTag", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentLabTag = MDLabTag.NewACObject(DatabaseApp, null);
            DatabaseApp.MDLabTag.Add(CurrentLabTag);
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
        [ACMethodInteraction("MDLabTag", Const.Delete, (short)MISort.Delete, true, "CurrentLabTag", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentLabTag.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentLabTag);
            SelectedLabTag = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(LabTagList));
        }

        /// <summary>
        /// Determines whether [is enabled delete lab tag].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete lab tag]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteLabTag()
        {
            return CurrentLabTag != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("MDLabTag", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(LabTagList));
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
                case nameof(IsEnabledDeleteLabTag):
                    result = IsEnabledDeleteLabTag();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}