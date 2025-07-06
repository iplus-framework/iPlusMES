// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 01-11-2018
// ***********************************************************************
// <copyright file="MDBSOGMPMaterialGroup.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Version 3
    /// Folgende alte Masken sind in diesem BSO enthalten:
    /// 1. GMP-HACCP Inhaltsstoffe je Komponente
    /// 2. GMP-HACCP Inhaltsstoffe aller Komponenten
    /// Neue Masken:
    /// 1. Inhaltsstoffe je Komponente
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'GMP-Material Group'}de{'GMP-Materialgruppe'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDGMPMaterialGroup.ClassName)]
    public class MDBSOGMPMaterialGroup : ACBSOvbNav
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOGMPMaterialGroup"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOGMPMaterialGroup(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            // Workaround
            var xx = DatabaseApp.MDGMPAdditive.ToList();
            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._AccessMDGMPMaterialGroupPos = null;
            this._CurrentGMPMaterialGroupPos = null;
            this._SelectedGMPMaterialGroupPos = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessMDGMPMaterialGroupPos != null)
            {
                _AccessMDGMPMaterialGroupPos.ACDeInit(false);
                _AccessMDGMPMaterialGroupPos = null;
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
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<MDGMPMaterialGroup> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDGMPMaterialGroup.ClassName)]
        public ACAccessNav<MDGMPMaterialGroup> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDGMPMaterialGroup>(MDGMPMaterialGroup.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the current GMP material group.
        /// </summary>
        /// <value>The current GMP material group.</value>
        [ACPropertyCurrent(9999, MDGMPMaterialGroup.ClassName)]
        public MDGMPMaterialGroup CurrentGMPMaterialGroup
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentGMPMaterialGroup");
            }
        }

        /// <summary>
        /// Gets or sets the selected GMP material group.
        /// </summary>
        /// <value>The selected GMP material group.</value>
        [ACPropertySelected(9999, MDGMPMaterialGroup.ClassName)]
        public MDGMPMaterialGroup SelectedGMPMaterialGroup
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedGMPMaterialGroup");
            }
        }

        /// <summary>
        /// Gets the GMP material group list.
        /// </summary>
        /// <value>The GMP material group list.</value>
        [ACPropertyList(9999, MDGMPMaterialGroup.ClassName)]
        public IEnumerable<MDGMPMaterialGroup> GMPMaterialGroupList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// The _ access MDGMP material group pos
        /// </summary>
        ACAccess<MDGMPMaterialGroupPos> _AccessMDGMPMaterialGroupPos;
        /// <summary>
        /// Gets the access GMP material group pos.
        /// </summary>
        /// <value>The access GMP material group pos.</value>
        [ACPropertyAccessPrimary(9999, "MDGMPMaterialGroupPos")]
        public ACAccess<MDGMPMaterialGroupPos> AccessGMPMaterialGroupPos
        {
            get
            {
                if (_AccessMDGMPMaterialGroupPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + "MDGMPMaterialGroupPos") as ACQueryDefinition;
                    _AccessMDGMPMaterialGroupPos = acQueryDefinition.NewAccess<MDGMPMaterialGroupPos>("MDGMPMaterialGroupPos", this);
                }
                return _AccessMDGMPMaterialGroupPos;
            }
        }

        /// <summary>
        /// The _ selected GMP material group pos
        /// </summary>
        MDGMPMaterialGroupPos _SelectedGMPMaterialGroupPos;
        /// <summary>
        /// Gets or sets the selected GMP material group pos.
        /// </summary>
        /// <value>The selected GMP material group pos.</value>
        [ACPropertySelected(9999, "MDGMPMaterialGroupPos")]
        public MDGMPMaterialGroupPos SelectedGMPMaterialGroupPos
        {
            get
            {
                return _SelectedGMPMaterialGroupPos;
            }
            set
            {
                _SelectedGMPMaterialGroupPos = value;
                OnPropertyChanged("SelectedGMPMaterialGroupPos");
            }
        }

        /// <summary>
        /// The _ current GMP material group pos
        /// </summary>
        MDGMPMaterialGroupPos _CurrentGMPMaterialGroupPos;
        /// <summary>
        /// Gets or sets the current GMP material group pos.
        /// </summary>
        /// <value>The current GMP material group pos.</value>
        [ACPropertyCurrent(9999, "MDGMPMaterialGroupPos")]
        public MDGMPMaterialGroupPos CurrentGMPMaterialGroupPos
        {
            get
            {
                return _CurrentGMPMaterialGroupPos;
            }
            set
            {
                _CurrentGMPMaterialGroupPos = value;
                OnPropertyChanged("CurrentGMPMaterialGroupPos");
            }
        }

        /// <summary>
        /// Gets the GMP material group pos list.
        /// </summary>
        /// <value>The GMP material group pos list.</value>
        [ACPropertyList(9999, "MDGMPMaterialGroupPos")]
        public IEnumerable<MDGMPMaterialGroupPos> GMPMaterialGroupPosList
        {
            get
            {
                if (CurrentGMPMaterialGroup == null)
                    return null;
                return CurrentGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup;
            }
        }

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(MDGMPMaterialGroup.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(MDGMPMaterialGroup.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(MDGMPMaterialGroup.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedGMPMaterialGroup", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDGMPMaterialGroup>(requery, () => SelectedGMPMaterialGroup, () => CurrentGMPMaterialGroup, c => CurrentGMPMaterialGroup = c,
                        DatabaseApp.MDGMPMaterialGroup
                        .Where(c => c.MDGMPMaterialGroupID == SelectedGMPMaterialGroup.MDGMPMaterialGroupID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedGMPMaterialGroup != null;
        }


        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDGMPMaterialGroup.ClassName, Const.New, (short)MISort.New, true, "SelectedGMPMaterialGroup", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentGMPMaterialGroup = MDGMPMaterialGroup.NewACObject(DatabaseApp, null);
            DatabaseApp.MDGMPMaterialGroup.AddObject(CurrentGMPMaterialGroup);
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
        [ACMethodInteraction(MDGMPMaterialGroup.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentGMPMaterialGroup", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentGMPMaterialGroup.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentGMPMaterialGroup);
            SelectedGMPMaterialGroup = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentGMPMaterialGroup != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDGMPMaterialGroup.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("GMPMaterialGroupList");
        }

        /// <summary>
        /// Loads the GMP material group pos.
        /// </summary>
        [ACMethodInteraction("MDGMPMaterialGroupPos", "en{'Load Ingredient'}de{'Inhaltsstoff laden'}", 9999, false, "SelectedGMPMaterialGroupPos", Global.ACKinds.MSMethodPrePost)]
        public void LoadGMPMaterialGroupPos()
        {
            if (!IsEnabledLoadGMPMaterialGroupPos())
            {
                if (!PreExecute("LoadGMPMaterialGroupPos")) return;
                CurrentGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup.Where(c => c.MDGMPMaterialGroupPosID == SelectedGMPMaterialGroupPos.MDGMPMaterialGroupPosID).FirstOrDefault();
                PostExecute("LoadGMPMaterialGroupPos");

            }
        }

        /// <summary>
        /// Determines whether [is enabled load GMP material group pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load GMP material group pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadGMPMaterialGroupPos()
        {
            return SelectedGMPMaterialGroupPos != null && CurrentGMPMaterialGroup != null;
        }


        /// <summary>
        /// Deletes the GMP material group pos.
        /// </summary>
        [ACMethodInteraction("MDGMPMaterialGroupPos", "en{'Delete Ingredient'}de{'Inhaltsstoff löschen'}", (short)MISort.Delete, true, "CurrentGMPMaterialGroupPos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteGMPMaterialGroupPos()
        {
            if (CurrentGMPMaterialGroupPos != null)
            {
                if (!PreExecute("DeleteGMPMaterialGroupPos")) return;
                Msg msg = CurrentGMPMaterialGroupPos.DeleteACObject(DatabaseApp, true);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }

                PostExecute("DeleteGMPMaterialGroupPos");
            }
        }

        /// <summary>
        /// Determines whether [is enabled delete GMP material group pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete GMP material group pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteGMPMaterialGroupPos()
        {
            return CurrentGMPMaterialGroup != null && CurrentGMPMaterialGroupPos != null;
        }

        /// <summary>
        /// News the GMP material group pos.
        /// </summary>
        [ACMethodInteraction("MDGMPMaterialGroupPos", "en{'New Ingredient'}de{'Neuer Inhaltsstoff'}", (short)MISort.New, true, "SelectedGMPMaterialGroupPos", Global.ACKinds.MSMethodPrePost)]
        public void NewGMPMaterialGroupPos()
        {
            if (!PreExecute("NewGMPMaterialGroupPos")) return;
            CurrentGMPMaterialGroupPos = MDGMPMaterialGroupPos.NewACObject(DatabaseApp, CurrentGMPMaterialGroup);
            CurrentGMPMaterialGroupPos.MDGMPMaterialGroup = CurrentGMPMaterialGroup;
            CurrentGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup.Add(CurrentGMPMaterialGroupPos);
            PostExecute("NewGMPMaterialGroupPos");
        }

        /// <summary>
        /// Determines whether [is enabled new GMP material group pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new GMP material group pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewGMPMaterialGroupPos()
        {
            return CurrentGMPMaterialGroup != null;
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
                case nameof(LoadGMPMaterialGroupPos):
                    LoadGMPMaterialGroupPos();
                    return true;
                case nameof(IsEnabledLoadGMPMaterialGroupPos):
                    result = IsEnabledLoadGMPMaterialGroupPos();
                    return true;
                case nameof(DeleteGMPMaterialGroupPos):
                    DeleteGMPMaterialGroupPos();
                    return true;
                case nameof(IsEnabledDeleteGMPMaterialGroupPos):
                    result = IsEnabledDeleteGMPMaterialGroupPos();
                    return true;
                case nameof(NewGMPMaterialGroupPos):
                    NewGMPMaterialGroupPos();
                    return true;
                case nameof(IsEnabledNewGMPMaterialGroupPos):
                    result = IsEnabledNewGMPMaterialGroupPos();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}