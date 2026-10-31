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
// <copyright file="MDBSOUnit.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.core.datamodel;
using System.Data;
using gip.mes.autocomponent;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Value list for units of measurement (kg, t, g, piece, litre, metre, hour, ...) used wherever quantities are entered or displayed; includes SI base units and packaging units.
    /// Each unit can have conversion rules (MDUnitConversion) to other units, defined by multiplier and divisor.
    /// To search for records, enter the search string in the SearchWord property.
    /// The database result is copied to the UnitList property.
    /// Then call the NavigateFirst() method to set CurrentUnit with the first record in the list.
    /// CurrentUnit is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentUnit and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns the new entity object to the CurrentUnit property. Fill in all required fields before saving.
    /// Use the Delete() method to delete the unit provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load() method updates the CurrentUnit object with fresh database data if another user has made changes in the background.
    /// Special functions: New Unit Conversion / Delete Unit Conversion maintain the conversion rules of the current unit; Convert tests a conversion with a sample value.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOUnit&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Units of Measurement'}de{'Maßeinheiten'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDUnit.ClassName,
        Description = @"Value list for units of measurement (kg, t, g, piece, litre, metre, hour, ...) used wherever quantities are entered or displayed; includes SI base units and packaging units.
        Each unit can have conversion rules (MDUnitConversion) to other units, defined by multiplier and divisor.
        To search for records, enter the search string in the SearchWord property.
        The database result is copied to the UnitList property.
        Then call the NavigateFirst() method to set CurrentUnit with the first record in the list.
        CurrentUnit is used to display and edit the currently selected record.
        Property changes should always be made to CurrentUnit and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns the new entity object to the CurrentUnit property. Fill in all required fields before saving.
        Use the Delete() method to delete the unit provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load() method updates the CurrentUnit object with fresh database data if another user has made changes in the background.
        Special functions: New Unit Conversion / Delete Unit Conversion maintain the conversion rules of the current unit; Convert tests a conversion with a sample value.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOUnit&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
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

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._AccessUnitConversion = null;
            this._CurrentNewUnitConversion = null;
            this._CurrentUnitConversion = null;
            this._CurrentUnitConvertTest = null;
            this._SelectedUnitConvertTest = null;
            this._UnitConversionList = null;
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessUnitConversion != null)
            {
                await _AccessUnitConversion.ACDeInit(false);
                _AccessUnitConversion = null;
            }
            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
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
        [ACPropertyAccessPrimary(9999, MDUnit.ClassName, Description = "Gets the primary navigation access to the unit entities.")]
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
        [ACPropertySelected(9999, MDUnit.ClassName, Description = "Gets or sets the unit currently selected in the list.")]
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
        [ACPropertyCurrent(9999, MDUnit.ClassName, Description = "Gets or sets the current unit. Property changes are made on this record before calling Save().")]
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
        [ACPropertyList(9999, MDUnit.ClassName, Description = "Gets the list of units resulting from the search.")]
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
        [ACPropertyAccess(9999, "MDUnitConversion", Description = "Gets the access to the unit conversion rules (MDUnitConversion) of the current unit.")]
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
        [ACPropertyCurrent(9999, "MDUnitConversion", Description = "Gets or sets the current unit conversion rule of the current unit (target unit, multiplier, divisor).")]
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
        [ACPropertyList(9999, "MDUnitConversion", Description = "Gets the list of unit conversion rules defined for the current unit.")]
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
        [ACPropertyCurrent(9999, "NewMDUnitConversion", Description = "Gets or sets the unit conversion rule currently being created in the New Unit Conversion dialog.")]
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

        [ACPropertyList(9999, "ConvertableUnits", Description = "Gets the list of units the current unit can be converted to (via its conversion rules).")]
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
        [ACPropertyInfo(9999, "Conversiontest", "en{'Input'}de{'Eingabe'}", Description = "Gets or sets the sample input value for the conversion test.")]
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
        [ACPropertyInfo(9999, "Conversiontest", "en{'Output'}de{'Ausgabe'}", Description = "Gets the result of the conversion test after Convert was executed.")]
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
        [ACPropertySelected(9999, "Conversiontest", "en{'To Unit'}de{'Nach Einheit'}", Description = "Gets or sets the target unit selected for the conversion test.")]
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
        [ACPropertyCurrent(9999, "Conversiontest", "en{'To Unit'}de{'Nach Einheit'}", Description = "Gets or sets the current target unit for the conversion test.")]
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

        [ACPropertyList(9999, "Conversiontest", Description = "Gets the list of target units available for the conversion test.")]
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
        [ACMethodCommand(MDUnit.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Saves the changes made to the current unit and its conversion rules to the database and refreshes the list.
                           Always call Save() before navigating to another record or creating a new one.")]
        public async void Save()
        {
            if (await OnSave())
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
        [ACMethodCommand(MDUnit.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Discards all unsaved changes made to the current unit and restores the database state.")]
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
        [ACMethodInteraction(MDUnit.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedUnit", Global.ACKinds.MSMethodPrePost, Description =
                         @"Reloads the selected unit from the database with fresh data, e.g. if another user has changed it in the background.")]
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
        [ACMethodInteraction(MDUnit.ClassName, Const.New, (short)MISort.New, true, "SelectedUnit", Global.ACKinds.MSMethodPrePost, Description =
                         @"Creates a new unit and assigns it to the CurrentUnit property. Fill in all required fields and call Save().")]
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
        [ACMethodInteraction(MDUnit.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentUnit", Global.ACKinds.MSMethodPrePost, Description =
                         @"Deletes the current unit, provided no other tables reference it (foreign key check). Confirm the deletion with Save().")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentUnit.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
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
        [ACMethodCommand(MDUnit.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description =
                         @"Executes the search and refreshes the UnitList with the database result.")]
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
        [ACMethodInteraction("MDUnitConversion", "en{'New Unit Conversion'}de{'Neue Einheitenumrechnung'}", (short)MISort.New, true, "CurrentUnitConversion", Global.ACKinds.MSMethodPrePost, Description =
                         @"Opens the dialog to create a new conversion rule for the current unit. Choose the target unit, multiplier and divisor, then confirm with OK.")]
        public async Task NewUnitConversion()
        {
            if (!PreExecute("NewUnitConversion")) return;
            CurrentNewUnitConversion = MDUnitConversion.NewACObject(DatabaseApp, CurrentUnit);
            await ShowDialogAsync(this, "UnitConversionNew");

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
        [ACMethodInteraction("MDUnitConversion", "en{'Delete Unit Conversion'}de{'Einheitenumrechnung löschen'}", (short)MISort.Delete, true, "CurrentUnitConversion", Global.ACKinds.MSMethodPrePost, Description =
                         @"Deletes the current conversion rule of the current unit. Confirm the deletion with Save().")]
        public void DeleteUnitConversion()
        {
            if (!PreExecute("DeleteUnitConversion")) return;
            Msg msg = CurrentUnitConversion.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
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
        [ACMethodCommand("NewMDUnitConversion", Const.Ok, (short)MISort.Okay, Description =
                         @"Confirms the New Unit Conversion dialog: adds the new conversion rule to the current unit. Confirm afterwards with Save().")]
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
        [ACMethodCommand("NewMDUnitConversion", Const.Cancel, (short)MISort.Cancel, Description =
                         @"Cancels the New Unit Conversion dialog and discards the new conversion rule.")]
        public void NewUnitConversionCancel()
        {
            CloseTopDialog();
            if (CurrentNewUnitConversion != null)
            {
                Msg msg = CurrentNewUnitConversion.DeleteACObject(DatabaseApp, true);
                if (msg != null)
                {
                    Messages.MsgAsync(msg);
                    return;
                }

            }
            CurrentNewUnitConversion = null;
        }
        #endregion

        #region 1.2 Conversion-Test

        [ACMethodCommand("Conversiontest", "en{'Convert'}de{'Umrechnen'}", (short)MISort.Okay, Description =
                         @"Converts the sample input value from the current unit to the selected target unit and displays the result, using the defined conversion rules.")]
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

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string methodName)
        {
            switch (methodName)
            {
                // Search is always enabled
                case nameof(Search):
                case nameof(NewUnitConversionCancel):
                    return new string[] { nameof(InitState) };
                // Load depends on ACState and SelectedUnit
                case nameof(Load):
                    return new string[] { nameof(ACState), nameof(SelectedUnit) };
                // New depends on ACState
                case nameof(New):
                    return new string[] { nameof(ACState) };
                // Delete depends on CurrentUnit
                case nameof(Delete):
                    return new string[] { nameof(CurrentUnit) };
                // Unit conversion methods
                case nameof(NewUnitConversion):
                    return new string[] { nameof(CurrentUnit) };
                case nameof(DeleteUnitConversion):
                    return new string[] { nameof(CurrentUnit), nameof(CurrentUnitConversion) };
                case nameof(NewUnitConversionOK):
                    return new string[] { nameof(CurrentNewUnitConversion) };
                case nameof(ConvertTest):
                    return new string[] { nameof(SelectedUnitConvertTest) };
                default:
                    return base.GetPropsToObserveForIsEnabled(methodName);
            }
        }

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
                case nameof(NewUnitConversion):
                    _ = NewUnitConversion();
                    return true;
                case nameof(IsEnabledNewUnitConversion):
                    result = IsEnabledNewUnitConversion();
                    return true;
                case nameof(DeleteUnitConversion):
                    DeleteUnitConversion();
                    return true;
                case nameof(IsEnabledDeleteUnitConversion):
                    result = IsEnabledDeleteUnitConversion();
                    return true;
                case nameof(NewUnitConversionOK):
                    NewUnitConversionOK();
                    return true;
                case nameof(IsEnabledNewUnitConversionOK):
                    result = IsEnabledNewUnitConversionOK();
                    return true;
                case nameof(NewUnitConversionCancel):
                    NewUnitConversionCancel();
                    return true;
                case nameof(ConvertTest):
                    ConvertTest();
                    return true;
                case nameof(IsEnabledConvertTest):
                    result = IsEnabledConvertTest();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}