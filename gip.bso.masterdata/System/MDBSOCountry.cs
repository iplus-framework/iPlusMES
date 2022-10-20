using System;
using gip.core.autocomponent;
// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="MDBSOCountry.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.mes.autocomponent;

namespace gip.bso.masterdata
{
    /*
     * Onlinehelp: Businessobjekte ohne Managerklasse (Nur für einfache MD-Tabellen)
     * 
     * Überlicherweise arbeiten Businessobjekte immer nur mit Managerklassen zusammen,
     * um über die Managerklassen ein einheitliches Handling und eine Validierung der
     * Daten vor dem Speichern zu ermöglichen. 
     * 
     * Bei den meisten MD-Tabellen handelt es sich aber um sehr einfache Hilfstabellen,
     * welche nur über die hierfür definierten BSOs gepflegt werden dürfen und die auch 
     * nicht über Schnittstellen/Webservices für schreibende Zugriffe bereitgestellt 
     * werden dürfen.
     * 
     * Aus diesem Grund kann bei diesen zahlreich vorhanden MD-Tabellen auf die Managerklassen
     * verzichtet werden. !!! Aber auch nur in diesen Fällen !!!
     * 
     * Im diesen BSOs erfolgen also alle Linq-Zugriffe direkt auf dem Linq-Modell,
     * ohne Umweg über die Managerklasse.
     */

    /// <summary>
    /// Allgemeine Stammdatenmaske für MDCountry
    /// Bei den einfachen MD-Tabellen wird bewußt auf die Managerklassen verzichtet.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Country'}de{'Land'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDCountry.ClassName)]
    public class MDBSOCountry : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOCountry"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOCountry(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            this._AccessCountryLand = null;
            this._CurrentCountryLand = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessCountryLand != null)
            {
                _AccessCountryLand.ACDeInit(false);
                _AccessCountryLand = null;
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
        #region 1. MDCountry
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<MDCountry> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDCountry.ClassName)]
        public ACAccessNav<MDCountry> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    // Instanz anhand der ACQRY-Klasse erzeugen
                    // Parameter: acComponentParent (optional), qryACClass, acKey
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);

                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDCountry>(MDCountry.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected country.
        /// </summary>
        /// <value>The selected country.</value>
        [ACPropertySelected(9999, MDCountry.ClassName)]
        public MDCountry SelectedCountry
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedCountry");
            }
        }

        /// <summary>
        /// Gets or sets the current country.
        /// </summary>
        /// <value>The current country.</value>
        [ACPropertyCurrent(9999, MDCountry.ClassName)]
        public MDCountry CurrentCountry
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current as MDCountry;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentCountry");
                OnPropertyChanged("CountryLandList");
            }
        }

        /// <summary>
        /// Gets the country list.
        /// </summary>
        /// <value>The country list.</value>
        [ACPropertyList(9999, MDCountry.ClassName)]
        public IEnumerable<MDCountry> CountryList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region 1.1 MDCountryLand
        /// <summary>
        /// The _ access country land
        /// </summary>
        ACAccess<MDCountryLand> _AccessCountryLand;
        /// <summary>
        /// Gets the access country land.
        /// </summary>
        /// <value>The access country land.</value>
        [ACPropertyAccessPrimary(9999, MDCountryLand.ClassName)]
        public ACAccess<MDCountryLand> AccessCountryLand
        {
            get
            {
                if (_AccessCountryLand == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + MDCountryLand.ClassName) as ACQueryDefinition;
                    _AccessCountryLand = acQueryDefinition.NewAccess<MDCountryLand>(MDCountryLand.ClassName, this);
                }
                return _AccessCountryLand;
            }
        }

        /// <summary>
        /// The _ current country land
        /// </summary>
        MDCountryLand _CurrentCountryLand;
        /// <summary>
        /// Gets or sets the current country land.
        /// </summary>
        /// <value>The current country land.</value>
        [ACPropertyCurrent(9999, MDCountryLand.ClassName)]
        public MDCountryLand CurrentCountryLand
        {
            get
            {
                return _CurrentCountryLand;
            }
            set
            {
                if (_CurrentCountryLand != value)
                {
                    _CurrentCountryLand = value;
                    OnPropertyChanged("CurrentCountryLand");
                }
            }
        }

        /// <summary>
        /// Gets the country land list.
        /// </summary>
        /// <value>The country land list.</value>
        [ACPropertyList(9999, MDCountryLand.ClassName)]
        public IEnumerable<MDCountryLand> CountryLandList
        {
            get
            {
                if (CurrentCountry == null)
                    return null;
                return CurrentCountry.MDCountryLand_MDCountry; //.OrderBy(c => c.MDCountryLandName);
            }
        }
        #endregion
        #endregion

        #region BSO->ACMethod
        #region 1. MDCountry
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(MDCountry.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(MDCountry.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(MDCountry.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedCountry", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDCountry>(requery, () => SelectedCountry, () => CurrentCountry, c => CurrentCountry = c,
                        DatabaseApp.MDCountry
                        .Where(c => c.MDCountryID == SelectedCountry.MDCountryID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedCountry != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDCountry.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedCountry", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentCountry = MDCountry.NewACObject(DatabaseApp, null);
            DatabaseApp.MDCountry.AddObject(CurrentCountry);
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
        [ACMethodInteraction(MDCountry.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentCountry", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentCountry.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentCountry);
            SelectedCountry = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentCountry != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDCountry.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("CountryList");
        }
        #endregion

        #region 1.1 MDCountryLand
        /// <summary>
        /// News the country land.
        /// </summary>
        [ACMethodInteraction(MDCountryLand.ClassName, "en{'New Federal State'}de{'Neues Bundesland'}", (short)MISort.New, true, "CurrentCountryLand", Global.ACKinds.MSMethodPrePost)]
        public void NewCountryLand()
        {
            if (!PreExecute("NewCountryLand")) return;
            CurrentCountryLand = MDCountryLand.NewACObject(DatabaseApp, CurrentCountry);
            CurrentCountryLand.MDCountry = CurrentCountry;
            PostExecute("NewCountryLand");
            OnPropertyChanged("CountryLandList");
        }

        /// <summary>
        /// Determines whether [is enabled new country land].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new country land]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewCountryLand()
        {
            return CurrentCountry != null;
        }

        /// <summary>
        /// Deletes the country land.
        /// </summary>
        [ACMethodInteraction(MDCountryLand.ClassName, "en{'Delete Federal State'}de{'Bundesland löschen'}", (short)MISort.Delete, true, "CurrentCountryLand", Global.ACKinds.MSMethodPrePost)]
        public void DeleteCountryLand()
        {
            if (CurrentCountryLand != null)
            {
                if (!PreExecute("DeleteCountryLand")) return;
                CurrentCountryLand.DeleteACObject(DatabaseApp, true);
                PostExecute("DeleteCountryLand");
            }
        }

        /// <summary>
        /// Determines whether [is enabled delete country land].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete country land]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteCountryLand()
        {
            return CurrentCountry != null && CurrentCountryLand != null;
        }
        #endregion

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
                case"NewCountryLand":
                    NewCountryLand();
                    return true;
                case"IsEnabledNewCountryLand":
                    result = IsEnabledNewCountryLand();
                    return true;
                case"DeleteCountryLand":
                    DeleteCountryLand();
                    return true;
                case"IsEnabledDeleteCountryLand":
                    result = IsEnabledDeleteCountryLand();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
