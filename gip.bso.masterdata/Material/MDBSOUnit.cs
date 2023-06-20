// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 10.01.2018
// ***********************************************************************
// <copyright file="MDBSOUnit.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using System.Data;
using gip.mes.autocomponent;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Allgemeine Stammdatenmaske für MDUnit
    /// Bei den einfachen MD-Tabellen wird bewußt auf die Managerklassen verzichtet.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Units of Measurement'}de{'Maßeinheiten'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDUnit.ClassName)]
    public class MDBSOUnit : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOUnit"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOUnit(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            this._AccessUnitConversion = null;
            this._CurrentNewUnitConversion = null;
            this._CurrentUnitConversion = null;
            this._CurrentUnitConvertTest = null;
            this._SelectedUnitConvertTest = null;
            this._UnitConversionList = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessUnitConversion != null)
            {
                _AccessUnitConversion.ACDeInit(false);
                _AccessUnitConversion = null;
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
        #region 1. MDUnit
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<MDUnit> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDUnit.ClassName)]
        public ACAccessNav<MDUnit> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDUnit>(MDUnit.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected unit.
        /// </summary>
        /// <value>The selected unit.</value>
        [ACPropertySelected(9999, MDUnit.ClassName)]
        public MDUnit SelectedUnit
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedUnit");
            }
        }

        /// <summary>
        /// Gets or sets the current unit.
        /// </summary>
        /// <value>The current unit.</value>
        [ACPropertyCurrent(9999, MDUnit.ClassName)]
        public MDUnit CurrentUnit
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (CurrentUnit == value)
                    return;

                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentUnit");
                _UnitConversionList = null;
                OnPropertyChanged("UnitConversionList");
                OnPropertyChanged("ConvertableUnits");
                OnPropertyChanged("UnitConvertTestList");
                SelectedUnitConvertTest = null;
            }
        }

        /// <summary>
        /// Gets the unit list.
        /// </summary>
        /// <value>The unit list.</value>
        [ACPropertyList(9999, MDUnit.ClassName)]
        public IEnumerable<MDUnit> UnitList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region 1.1 MDUnitConversion
        /// <summary>
        /// The _ access unit conversion
        /// </summary>
        ACAccess<MDUnitConversion> _AccessUnitConversion;
        /// <summary>
        /// Gets the access unit conversion.
        /// </summary>
        /// <value>The access unit conversion.</value>
        [ACPropertyAccess(9999, "MDUnitConversion")]
        public ACAccess<MDUnitConversion> AccessUnitConversion
        {
            get
            {
                if (_AccessUnitConversion == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + MDUnitConversion.ClassName) as ACQueryDefinition;
                    _AccessUnitConversion = acQueryDefinition.NewAccess<MDUnitConversion>("MDUnitConversion", this);
                }
                return _AccessUnitConversion;
            }
        }

        /// <summary>
        /// The _ current unit conversion
        /// </summary>
        MDUnitConversion _CurrentUnitConversion;
        /// <summary>
        /// Gets or sets the current unit conversion.
        /// </summary>
        /// <value>The current unit conversion.</value>
        [ACPropertyCurrent(9999, "MDUnitConversion")]
        public MDUnitConversion CurrentUnitConversion
        {
            get
            {
                return _CurrentUnitConversion;
            }
            set
            {
                _CurrentUnitConversion = value;
                OnPropertyChanged("CurrentUnitConversion");
            }
        }

        /// <summary>
        /// The _ unit conversion list
        /// </summary>
        List<MDUnitConversion> _UnitConversionList = null;
        /// <summary>
        /// Gets the unit conversion list.
        /// </summary>
        /// <value>The unit conversion list.</value>
        [ACPropertyList(9999, "MDUnitConversion")]
        public IEnumerable<MDUnitConversion> UnitConversionList
        {
            get
            {
                if (CurrentUnit == null)
                    return null;
                UpdateLocalConversionList();
                return _UnitConversionList;
            }
        }

        private void UpdateLocalConversionList()
        {
            if (CurrentUnit != null && CurrentUnit.MDUnitConversion_MDUnit != null)
                _UnitConversionList = CurrentUnit.MDUnitConversion_MDUnit.ToList();
            else
                _UnitConversionList = null;
        }


        /// <summary>
        /// The _ current new unit conversion
        /// </summary>
        MDUnitConversion _CurrentNewUnitConversion;
        /// <summary>
        /// Gets or sets the current new unit conversion.
        /// </summary>
        /// <value>The current new unit conversion.</value>
        [ACPropertyCurrent(9999, "NewMDUnitConversion")]
        public MDUnitConversion CurrentNewUnitConversion
        {
            get
            {
                return _CurrentNewUnitConversion;
            }
            set
            {
                _CurrentNewUnitConversion = value;
                OnPropertyChanged("CurrentNewUnitConversion");
            }
        }

        [ACPropertyList(9999, "ConvertableUnits")]
        public IEnumerable<MDUnit> ConvertableUnits
        {
            get
            {
                if (CurrentUnit == null)
                    return null;
                List<MDUnit> convertableUnits = new List<MDUnit>();
                UpdateLocalConversionList();
                List<Guid> existingConversions = null;
                if (_UnitConversionList != null)
                    existingConversions = _UnitConversionList.Where(c => c.ToMDUnit != null).Select(c => c.ToMDUnit.MDUnitID).ToList();
                if (CurrentUnit.SIDimension == GlobalApp.SIDimensions.None)
                {
                    if (existingConversions == null)
                        return DatabaseApp.MDUnit.Where(c => (c.MDUnitID != CurrentUnit.MDUnitID) && (c.SIDimensionIndex == (short)GlobalApp.SIDimensions.None || c.IsSIUnit == true));
                    else
                        return DatabaseApp.MDUnit.Where(c => (c.MDUnitID != CurrentUnit.MDUnitID) && (c.SIDimensionIndex == (short)GlobalApp.SIDimensions.None || c.IsSIUnit == true) && !existingConversions.Contains(c.MDUnitID)).ToList();
                }
                else if (CurrentUnit.IsSIUnit)
                {
                    if (existingConversions == null)
                        return DatabaseApp.MDUnit.Where(c => c.MDUnitID != CurrentUnit.MDUnitID && c.SIDimensionIndex == CurrentUnit.SIDimensionIndex);
                    else
                        return DatabaseApp.MDUnit.Where(c => c.MDUnitID != CurrentUnit.MDUnitID && c.SIDimensionIndex == CurrentUnit.SIDimensionIndex && !existingConversions.Contains(c.MDUnitID)).ToList();
                }
                return null;
            }
        }

        #endregion

        #region Conversion-Test
        private double _ConvertTestInput;
        [ACPropertyInfo(9999, "Conversiontest", "en{'Input'}de{'Eingabe'}")]
        public double ConvertTestInput
        {
            get
            {
                return _ConvertTestInput;
            }
            set
            {
                _ConvertTestInput = value;
                OnPropertyChanged("ConvertTestInput");
            }
        }

        private double _ConvertTestOutput;
        [ACPropertyInfo(9999, "Conversiontest", "en{'Output'}de{'Ausgabe'}")]
        public double ConvertTestOutput
        {
            get
            {
                return _ConvertTestOutput;
            }
            set
            {
                _ConvertTestOutput = value;
                OnPropertyChanged("ConvertTestOutput");
            }
        }

        MDUnit _SelectedUnitConvertTest;
        [ACPropertySelected(9999, "Conversiontest", "en{'To Unit'}de{'Nach Einheit'}")]
        public MDUnit SelectedUnitConvertTest
        {
            get
            {
                return _SelectedUnitConvertTest;
            }
            set
            {
                _SelectedUnitConvertTest = value;
                OnPropertyChanged("SelectedUnitConvertTest");
            }
        }

        MDUnit _CurrentUnitConvertTest;
        [ACPropertyCurrent(9999, "Conversiontest", "en{'To Unit'}de{'Nach Einheit'}")]
        public MDUnit CurrentUnitConvertTest
        {
            get
            {
                return _CurrentUnitConvertTest;
            }
            set
            {
                _CurrentUnitConvertTest = value;
                OnPropertyChanged("CurrentUnitConvertTest");
            }
        }

        [ACPropertyList(9999, "Conversiontest")]
        public IEnumerable<MDUnit> UnitConvertTestList
        {
            get
            {
                if (CurrentUnit == null || CurrentUnit.ConvertableUnits == null)
                    return null;
                return CurrentUnit.ConvertableUnits;
            }
        }


        #endregion

        #endregion

        #region BSO->ACMethod

        #region 1. MDUnit
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(MDUnit.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(MDUnit.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(MDUnit.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedUnit", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDUnit>(requery, () => SelectedUnit, () => CurrentUnit, c => CurrentUnit = c,
                        DatabaseApp.MDUnit
                        .Where(c => c.MDUnitID == SelectedUnit.MDUnitID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedUnit != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDUnit.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedUnit", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentUnit = MDUnit.NewACObject(DatabaseApp, null);
            DatabaseApp.MDUnit.Add(CurrentUnit);
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
        [ACMethodInteraction(MDUnit.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentUnit", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentUnit.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentUnit);
            SelectedUnit = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(UnitList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentUnit != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDUnit.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(UnitList));
        }
        #endregion

        #region 1.1 MDUnitConversion
        /// <summary>
        /// News the unit conversion.
        /// </summary>
        [ACMethodInteraction("MDUnitConversion", "en{'New Unit Conversion'}de{'Neue Einheitenumrechnung'}", (short)MISort.New, true, "CurrentUnitConversion", Global.ACKinds.MSMethodPrePost)]
        public void NewUnitConversion()
        {
            if (!PreExecute("NewUnitConversion")) return;
            CurrentNewUnitConversion = MDUnitConversion.NewACObject(DatabaseApp, CurrentUnit);
            ShowDialog(this, "UnitConversionNew");

            PostExecute("NewUnitConversion");
        }

        /// <summary>
        /// Determines whether [is enabled new unit conversion].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new unit conversion]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewUnitConversion()
        {
            if (CurrentUnit.IsSIUnit || (CurrentUnit.SIDimension == GlobalApp.SIDimensions.None))
                return true;
            return false;
        }

        /// <summary>
        /// Deletes the unit conversion.
        /// </summary>
        [ACMethodInteraction("MDUnitConversion", "en{'Delete Unit Conversion'}de{'Einheitenumrechnung löschen'}", (short)MISort.Delete, true, "CurrentUnitConversion", Global.ACKinds.MSMethodPrePost)]
        public void DeleteUnitConversion()
        {
            if (!PreExecute("DeleteUnitConversion")) return;
            Msg msg = CurrentUnitConversion.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            PostExecute("DeleteUnitConversion");
            OnPropertyChanged("UnitConversionList");
        }

        /// <summary>
        /// Determines whether [is enabled delete unit conversion].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete unit conversion]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteUnitConversion()
        {
            return CurrentUnit != null && CurrentUnitConversion != null;
        }

        /// <summary>
        /// News the unit conversion OK.
        /// </summary>
        [ACMethodCommand("NewMDUnitConversion", "en{'OK'}de{'OK'}", (short)MISort.Okay)]
        public void NewUnitConversionOK()
        {
            CloseTopDialog();
            CurrentUnitConversion = CurrentNewUnitConversion;
            MDUnit toUnit = CurrentNewUnitConversion.ToMDUnit;
            CurrentUnit.MDUnitConversion_MDUnit.Add(CurrentUnitConversion);
            OnPropertyChanged("UnitConversionList");
            OnPropertyChanged("ConvertableUnits");
            CurrentUnitConversion.ToMDUnit = toUnit;
            OnPropertyChanged("UnitConvertTestList");
            SelectedUnitConvertTest = null;
        }

        /// <summary>
        /// Determines whether [is enabled new unit conversion OK].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new unit conversion OK]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewUnitConversionOK()
        {
            if ((CurrentNewUnitConversion == null)
                || (CurrentNewUnitConversion.ToMDUnit == null)
                || (Math.Abs(CurrentNewUnitConversion.Multiplier - 0) <= Double.Epsilon)
                || (Math.Abs(CurrentNewUnitConversion.Divisor - 0) <= Double.Epsilon))
                return false;
            return true;
        }

        /// <summary>
        /// News the unit conversion cancel.
        /// </summary>
        [ACMethodCommand("NewMDUnitConversion", "en{'Cancel'}de{'Abbrechen'}", (short)MISort.Cancel)]
        public void NewUnitConversionCancel()
        {
            CloseTopDialog();
            if (CurrentNewUnitConversion != null)
            {
                Msg msg = CurrentNewUnitConversion.DeleteACObject(DatabaseApp, true);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }

            }
            CurrentNewUnitConversion = null;
        }
        #endregion

        #region 1.2 Conversion-Test

        [ACMethodCommand("Conversiontest", "en{'Convert'}de{'Umrechnen'}", (short)MISort.Okay)]
        public void ConvertTest()
        {
            if (!IsEnabledConvertTest())
                return;
            try
            {
                ConvertTestOutput = CurrentUnit.ConvertToUnit(ConvertTestInput, SelectedUnitConvertTest);
            }
            catch (Exception ec)
            {
                ConvertTestOutput = 0;

                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("MDBSOUnit", "ConvertTest", msg);
            }
        }

        public bool IsEnabledConvertTest()
        {
            if (SelectedUnitConvertTest != null)
                return true;
            return false;
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
                case"NewUnitConversion":
                    NewUnitConversion();
                    return true;
                case"IsEnabledNewUnitConversion":
                    result = IsEnabledNewUnitConversion();
                    return true;
                case"DeleteUnitConversion":
                    DeleteUnitConversion();
                    return true;
                case"IsEnabledDeleteUnitConversion":
                    result = IsEnabledDeleteUnitConversion();
                    return true;
                case"NewUnitConversionOK":
                    NewUnitConversionOK();
                    return true;
                case"IsEnabledNewUnitConversionOK":
                    result = IsEnabledNewUnitConversionOK();
                    return true;
                case"NewUnitConversionCancel":
                    NewUnitConversionCancel();
                    return true;
                case"ConvertTest":
                    ConvertTest();
                    return true;
                case"IsEnabledConvertTest":
                    result = IsEnabledConvertTest();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
