// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 10.01.2018
// ***********************************************************************
// <copyright file="MDBSOMaterialGroup.cs" company="gip mbh, Oftersheim, Germany">
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
using System.IO;
using System;
using gip.core.media;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Allgemeine Stammdatenmaske für MDMaterialGroup
    /// Bei den einfachen MD-Tabellen wird bewußt auf die Managerklassen verzichtet.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Material Group'}de{'Materialgruppe'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDMaterialGroup.ClassName)]
    public class MDBSOMaterialGroup : ACBSOvbNav
    {
        
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOMaterialGroup"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOMaterialGroup(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
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
            
            MediaController = ACMediaController.GetServiceInstance(this);

            if (BSOMedia_Child != null && BSOMedia_Child.Value != null)
                BSOMedia_Child.Value.OnDefaultImageDelete += Value_OnDefaultImageDelete;

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
           
            if (BSOMedia_Child != null && BSOMedia_Child.Value != null)
                BSOMedia_Child.Value.OnDefaultImageDelete -= Value_OnDefaultImageDelete;

            MediaController = null;
            return b;
        }

        private void Value_OnDefaultImageDelete(object sender, EventArgs e)
        {
            OnPropertyChanged("MaterialGroupList");
        }


        #endregion

        #region Managers
        
        public ACMediaController MediaController { get; set; }

        #endregion

        #region Child BSO
        ACChildItem<BSOMedia> _BSOMedia_Child;
        [ACPropertyInfo(9999)]
        [ACChildInfo("BSOMedia_Child", typeof(BSOMedia))]
        public ACChildItem<BSOMedia> BSOMedia_Child
        {
            get
            {
                if (_BSOMedia_Child == null)
                    _BSOMedia_Child = new ACChildItem<BSOMedia>(this, "BSOMedia_Child");
                return _BSOMedia_Child;
            }
        }
        #endregion

        #region Configuration
        private ACPropertyConfigValue<bool> _ShowImages;
        [ACPropertyConfig("en{'Show images'}de{'Bilder anzeigen'}")]
        public bool ShowImages
        {
            get
            {
                return _ShowImages.ValueT;
            }
            set
            {
                _ShowImages.ValueT = value;
            }
        }
        #endregion

        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<MDMaterialGroup> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDMaterialGroup.ClassName)]
        public ACAccessNav<MDMaterialGroup> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDMaterialGroup>(MDMaterialGroup.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected material group.
        /// </summary>
        /// <value>The selected material group.</value>
        [ACPropertySelected(9999, MDMaterialGroup.ClassName)]
        public MDMaterialGroup SelectedMaterialGroup
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                CurrentMaterialGroup = SelectedMaterialGroup;
                OnPropertyChanged("SelectedMaterialGroup");
            }
        }

        /// <summary>
        /// Gets or sets the current material group.
        /// </summary>
        /// <value>The current material group.</value>
        [ACPropertyCurrent(9999, MDMaterialGroup.ClassName)]
        public MDMaterialGroup CurrentMaterialGroup
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return;
                if (AccessPrimary.Current != value)
                {
                    if (BSOMedia_Child != null && BSOMedia_Child.Value != null)
                    {
                        BSOMedia_Child.Value.LoadMedia(value);
                    }

                    AccessPrimary.Current = value;
                    OnPropertyChanged("CurrentMaterialGroup");
                }
            }
        }

        /// <summary>
        /// Gets the material group list.
        /// </summary>
        /// <value>The material group list.</value>
        [ACPropertyList(9999, MDMaterialGroup.ClassName)]
        public List<MDMaterialGroup> MaterialGroupList
        {
            get
            {
                var groupList = AccessPrimary.NavList.ToList();
                if (ShowImages)
                {
                    foreach (var item in groupList)
                    {
                        MediaController.LoadIImageInfo(item);
                    }
                }
                return groupList;
            }
        }

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(MDMaterialGroup.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(MDMaterialGroup.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(MDMaterialGroup.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMaterialGroup", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDMaterialGroup>(requery, () => SelectedMaterialGroup, () => CurrentMaterialGroup, c => CurrentMaterialGroup = c,
                        DatabaseApp.MDMaterialGroup
                        .Where(c => c.MDMaterialGroupID == SelectedMaterialGroup.MDMaterialGroupID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedMaterialGroup != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDMaterialGroup.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedMaterialGroup", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            SelectedMaterialGroup = MDMaterialGroup.NewACObject(DatabaseApp, null);
            DatabaseApp.MDMaterialGroup.AddObject(SelectedMaterialGroup);
            AccessPrimary.NavList.Add(SelectedMaterialGroup);
            ACState = Const.SMNew;
            OnPropertyChanged("MaterialGroupList");
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
        [ACMethodInteraction(MDMaterialGroup.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentMaterialGroup", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentMaterialGroup.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentMaterialGroup);
            SelectedMaterialGroup = AccessPrimary.NavList.FirstOrDefault();
            Load();
            OnPropertyChanged("MaterialGroupList");
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentMaterialGroup != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDMaterialGroup.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("MaterialGroupList");
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
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}